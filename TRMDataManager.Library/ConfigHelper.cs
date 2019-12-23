using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library
{
    public static class ConfigHelper
    {
        public static decimal GetTaxRate()
        {
            var taxRateText = ConfigurationManager.AppSettings["taxRate"];
            if (Decimal.TryParse(taxRateText, out decimal output))
                return output / 100;

            throw new ConfigurationErrorsException("The tax rate is not set up properly");
        }
    }
}
