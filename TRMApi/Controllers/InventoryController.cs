using System;
using System.Collections.Generic;
using System.Linq;
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
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            this._config = config;
        }

        [Authorize(Roles = "Manager,Admin")] // =OR
        [HttpGet]
        public List<InventoryModel> Get()
        {
            var data = new InventoryData(_config);

            return data.GetInventory();
        }

        //[Authorize(Roles = "Warehouse")] // =AND 
        //[Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(InventoryModel item)
        {
            var data = new InventoryData(_config);

            data.SaveInventoryRecord(item);
        }
    }
}