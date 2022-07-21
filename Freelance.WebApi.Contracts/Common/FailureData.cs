using System;
using System.Collections.Generic;
using System.Linq;

namespace Freelance.WebApi.Contracts.Common
{
    public class FailureData
    {
        public string ErrorMessage { get; set; }
        public ErrorLevel ErrorLevel { get; set; }
        public Dictionary<string, string> Custom { get; set; }

        public FailureData(Exception ex)
        {
            ErrorMessage = ex.Message;
            ErrorLevel = ErrorLevel.Error;
            Custom = new Dictionary<string, string>();
        }

        public FailureData(string errorMessage, ErrorLevel errorLevel = ErrorLevel.Error)
        {
            ErrorMessage = errorMessage;
            ErrorLevel = ErrorLevel.Error;
            Custom = new Dictionary<string, string>(); 
        }

        public void AddCustom(Dictionary<string, string> custom)
        {
            custom.ToList().ForEach(x => Custom.Add(x.Key, x.Value));
        }
    }
}
