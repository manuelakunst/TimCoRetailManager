﻿using System;
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
        private readonly ISaleData _saleData;

        public SaleController(ISaleData saleData)
        {
            _saleData = saleData;
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _saleData.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Manager,Admin")]  // both roles are allowed to this function
        [Route("GetSalesReport")]
        [HttpGet]
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

            return _saleData.GetSaleReport();
        }

    }
}