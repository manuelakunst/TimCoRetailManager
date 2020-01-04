﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            var data = new SaleData();
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

            var data = new SaleData();

            return data.GetSaleReport();
        }

    }
}