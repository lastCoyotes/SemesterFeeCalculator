//using Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Collections;


namespace api.Storage
{
    // Storage via Excel Spreadsheet
    public class ExcelDB : IDatabase
    {
        // Singleton instance of Excel Database.
        private static ExcelDB Instance {get;set;} = new ExcelDB("UAHFeeSchedule.xlsx");

        // Private Workbook xlsx.
        private readonly Workbook xlsx;

        // A set of departments with courses that have individual fees
        public HashSet<string> IndividualCourses { get; private set; }

        // A set of departments with courses that have group fees
        public HashSet<string> GroupCourses { get; private set; }

        // A dictionary that maps departments to Department objects
        public Dictionary<string, Department> DepartmentSheet { get; set; }

        // A dictionary that maps colleges to their college fees
        public Dictionary<string, int> CollegeSheet { get; set; }
        // A dictionary that maps a department and course number to individual course fees.

        public Dictionary<string, decimal> IndCourseSheet { get; set; }
        // A dictionary that maps a department and course number to group course fees.

        public Dictionary<string, decimal> GrpCourseFees { get; set; }

        // A dictionary that maps a from certain conditions to semester fees.
        public Dictionary<string, MiscFee> SemesterFees { get; set; }

        // A dictionary that maps a from certain conditions to one time fees.
        public Dictionary<string, MiscFee> OneTimeFees { get; set; }

        // Populate department fees from department sheet
        public void PopulateDepartmentSheet(ArrayList departmentRow)
        {
            DepartmentSheet[(string)departmentRow[0]] = new Department((string)departmentRow[0], (string)departmentRow[1], Decimal.Parse((string)departmentRow[2]));
        }

        // Populate college fees from college sheet
        public void PopulateCollegeSheet(ArrayList collegeRow)
        {
            CollegeSheet[(string)collegeRow[0]] = Int32.Parse((string)collegeRow[1]);

        }

        // Populate individual fees from individual fees sheet
        public void PopulateIndCourseSheet(ArrayList indCourseRow)
        {
            string courseString = (string)indCourseRow[0] + " " + (string)indCourseRow[1];
            IndividualCourses.Add((string)indCourseRow[0]);
            IndCourseSheet[courseString] = Decimal.Parse((string)indCourseRow[2]);
        }

        // Populate group course fees from group course fees sheet
        public void PopulateGrpCourseSheet(ArrayList grpCourseRow)
        {
            for (int i = Int32.Parse((string)grpCourseRow[1]); i <= Int32.Parse((string)grpCourseRow[2]); i++)
            {
                string courseString = (string)grpCourseRow[0] + " " + i;
                GrpCourseFees[courseString] = Decimal.Parse((string)grpCourseRow[3]);
            }
            GroupCourses.Add((string)grpCourseRow[0]);
        }

        // Populate semester fees from semester fee sheet
        public void PopulateSemesterFeesSheet(ArrayList semesterFeesRow)
        {   
            // Pad out list to correct size
            while(semesterFeesRow.Count < 3){
                semesterFeesRow.Add("");
            }
            SemesterFees[(string)semesterFeesRow[0]] = new MiscFee((string)semesterFeesRow[0], Decimal.Parse((string)semesterFeesRow[1]), (string)semesterFeesRow[2]);
        }

        // Populate one time fees from one time fee sheet
        public void PopulateOneTimeFeesSheet(ArrayList oneTimeFeesRow)
        {
            // Pad out list to correct size
            while(oneTimeFeesRow.Count < 3){
                oneTimeFeesRow.Add("");
            }
            OneTimeFees[(string)oneTimeFeesRow[0]] = new MiscFee((string)oneTimeFeesRow[0], Decimal.Parse((string)oneTimeFeesRow[1]), (string)oneTimeFeesRow[2]);;
        }

        // Function to return the cell data from a cell
        private static string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }

        // Function to get row information from a spreadsheet page
        public ArrayList GetRowInformation(SpreadsheetDocument doc, Row r)
        {
            ArrayList currentList = new();
            foreach (Cell c in r.Descendants<Cell>())
            {
                if (c.InnerText != "")
                {
                    currentList.Add(GetValue(doc, c));
                }
            }
            return currentList;
        }

        public static ExcelDB GetDatabase (){ 
           return Instance;
        }

        public ExcelDB(string file)
        {
            IndividualCourses = new HashSet<string>(); // Set of departments with individual courses
            GroupCourses = new HashSet<string>(); // Set of departments with group courses
            DepartmentSheet = new Dictionary<string, Department>(); // Map from department to Department object
            CollegeSheet = new Dictionary<string, int>(); // Map from college to college fee
            IndCourseSheet = new Dictionary<string, decimal>(); // Dictionary from course name and course number 
            // to individual course fee
            GrpCourseFees = new Dictionary<string, decimal>(); // Dictionary from course name and course number to
            // group course fee
            SemesterFees = new Dictionary<string, MiscFee>(); // Dictionary to map semester fees
            OneTimeFees = new Dictionary<string, MiscFee>(); // Dictionary to map one time fees

            WorkbookPart wbPart;
            //DataTable dt = new();
            // Open Excel

            using SpreadsheetDocument doc = SpreadsheetDocument.Open(file, false);

            wbPart = doc.WorkbookPart;
            xlsx = wbPart.Workbook;

            Console.WriteLine(Environment.CurrentDirectory);
            // Open File
            System.IO.File.ReadAllBytes(file);
            //xlsx = app.Workbooks.Open(file);
            //xlsx = app.Workbooks.OpenXML(file);

            // Iterate over spreadsheets

            Sheets sheets = xlsx.GetFirstChild<Sheets>();
            foreach (Sheet sheet in sheets.Cast<Sheet>())
            {
                string sheetName = sheet.Name;
                Worksheet currentSheet = ((WorksheetPart)wbPart.GetPartById(sheet.Id)).Worksheet;
                SheetData sheetData = (SheetData)currentSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = currentSheet.GetFirstChild<SheetData>().Descendants<Row>();
                switch (sheetName)
                {
                    case "Departments":
                        foreach (Row r in rows)
                        {
                            // Skip first row in all pages
                            if (r.RowIndex.Value == 1)
                            {
                                continue;
                            }
                            ArrayList departmentRow = GetRowInformation(doc, r);
                            if ((departmentRow != null) && (departmentRow.Count > 0))
                            {
                                PopulateDepartmentSheet(departmentRow);
                            }
                        }
                        break;
                    case "Colleges":
                        foreach (Row r in rows)
                        {
                            // Skip first row in all pages
                            if (r.RowIndex.Value == 1)
                            {
                                continue;
                            }
                            ArrayList collegeRow = GetRowInformation(doc, r);
                            if ((collegeRow != null) && (collegeRow.Count > 0))
                            {
                                PopulateCollegeSheet(collegeRow);
                            }
                        }
                        break;
                    case "Individual Course Fees":
                        foreach (Row r in rows)
                        {
                            if (r.RowIndex.Value == 1)
                            {
                                continue;
                            }
                            ArrayList indCourseFeesRow = GetRowInformation(doc, r);
                            if ((indCourseFeesRow != null) && (indCourseFeesRow.Count > 0))
                            {
                                PopulateIndCourseSheet(indCourseFeesRow);
                            }
                        }
                        break;
                    case "Group Course Fees":
                        foreach (Row r in rows)
                        {
                            if (r.RowIndex.Value == 1)
                            {
                                continue;
                            }
                            ArrayList grpCourseFeesRow = GetRowInformation(doc, r);

                            if ((grpCourseFeesRow != null) && (grpCourseFeesRow.Count > 0))
                            {
                                PopulateGrpCourseSheet(grpCourseFeesRow);
                            }
                        }
                        break;
                    case "Semester Fees":
                        foreach (Row r in rows)
                        {
                            if (r.RowIndex.Value == 1)
                            {
                                continue;
                            }
                            ArrayList semesterFeesRow = GetRowInformation(doc, r);
                            PopulateSemesterFeesSheet(semesterFeesRow);
                        }
                        break;
                    case "One-time Fees":
                        foreach (Row r in rows)
                        {
                            if (r.RowIndex.Value == 1)
                            {
                                continue;
                            }
                            ArrayList oneTimeFeesRow = GetRowInformation(doc, r);
                            if ((oneTimeFeesRow != null) && (oneTimeFeesRow.Count > 0))
                            {
                                PopulateOneTimeFeesSheet(oneTimeFeesRow);
                            }
                        }
                        break;
                }
            }
        }

    }
    public class Department
    {
        readonly string department;
        readonly string college;
        readonly decimal departmentFee;
        public Department(string dep, string col, decimal depFee)
        {
            department = dep; college = col; departmentFee = depFee;
        }

        public decimal GetDepartmentFee()
        {
            return departmentFee;
        }

        public string GetCollege()
        {
            return college;
        }
    }

    public class MiscFee{
        public string FeeName {get;set;}
        public decimal FeeCost {get;set;}
        public string FeeDescription {get;set;}

        public MiscFee(string name, decimal cost, string description){
            FeeName = name; FeeCost = cost; FeeDescription = description;
        }
    }

        public class CourseNumber {
        public string Department {get;set;}
        public int No {get;set;}
        public char Ext {get;set;}

        public CourseNumber(){}
        public CourseNumber(string course){
            char[] separator = { ' ' };
            //Split the course into department and course number using a space as a separator
            string[] courseSplit = course.Split(separator, 2, StringSplitOptions.None);
            if(courseSplit.Length > 1){
                this.Department = courseSplit[0];
                if(char.IsLetter(courseSplit[1].Last())){
                    this.Ext = courseSplit[1].Last();
                    courseSplit[1] = courseSplit[1].Remove(courseSplit[1].Length-1);
                }
                this.No = Int32.Parse(courseSplit[1]);
            }
            
          
        }
          public bool Equals(CourseNumber courseNumber){
            if (courseNumber.Department == Department && courseNumber.No == No 
            && courseNumber.Ext == Ext){
                return true;
            }
            return false;
         }
    }

}