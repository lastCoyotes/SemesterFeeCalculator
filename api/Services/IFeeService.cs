// Fee Service
// Defines the interface the API uses to calculate Fees
// Most of the calculation Logic Occurs Here
using System.Collections.Generic;
using System;

namespace api.Services
{

    public interface IFeeService
    {
        public Dictionary<string, decimal> GetCourseFees(string course, int hours);
        public Dictionary<string, decimal> GetSemesterFees (Dictionary<string,int> semester);

        public Dictionary<string,decimal> GetOneTimeFees (Dictionary<string,int> oneTime);
        public List<api.Storage.MiscFee> GetOneTimeFeeValues();
        public List<api.Storage.MiscFee> GetSemesterFeeValues();

    }

}