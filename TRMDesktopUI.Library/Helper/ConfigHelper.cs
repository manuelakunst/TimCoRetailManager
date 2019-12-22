using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Library.Helper
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            var taxRateText = ConfigurationManager.AppSettings["taxRate"];
            if (Decimal.TryParse(taxRateText, out decimal output))
                return output;

            throw new ConfigurationErrorsException("The tax rate is not set up properly");
        }
    }
}
