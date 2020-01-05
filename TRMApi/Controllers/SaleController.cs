using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SaleController(IConfiguration config)
        {
            this._config = config;
        }

        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            var data = new SaleData(_config);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            data.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Manager,Admin")]  // both roles are allowed to this function
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            // Demo for role dependant behaviour
            //
            if (User.IsInRole("Admin"))
            {
                // do admin stuff
            }
            else if (User.IsInRole("Manager"))
            {
                // do manager stuff
            }
            else
            {
                // default
            }

            var data = new SaleData(_config);

            return data.GetSaleReport();
        }

    }
}