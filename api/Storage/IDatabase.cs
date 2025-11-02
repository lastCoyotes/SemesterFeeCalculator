using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Collections;
// API Storage Interface
// Defines functions needed for storage
// Used by the Service Layer to access information for calculations
namespace api.Storage
{
    public interface IDatabase
    {
        abstract void PopulateDepartmentSheet(ArrayList departmentRow);
        // Populate department fees from department sheet
        abstract void PopulateCollegeSheet(ArrayList collegeRow);

        // Populate individual fees from individual fees sheet
        abstract void PopulateIndCourseSheet(ArrayList indCourseRow);
        // Populate group course fees from group course fees sheet
        abstract void PopulateGrpCourseSheet(ArrayList grpCourseRow);
        // Populate semester fees from semester fee sheet
        abstract void PopulateSemesterFeesSheet(ArrayList semesterFeesRow);
        // Populate one time fees from one time fee sheet
        abstract void PopulateOneTimeFeesSheet(ArrayList oneTimeFeesRow);
    }
}