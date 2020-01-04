using System;
using System.Collections.Generic;
using System.Linq;
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
    public class InventoryController : ControllerBase
    {
        [Authorize(Roles = "Manager,Admin")] // =OR
        public List<InventoryModel> Get()
        {
            var data = new InventoryData();

            return data.GetInventory();
        }

        //[Authorize(Roles = "Warehouse")] // =AND 
        //[Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel item)
        {
            var data = new InventoryData();

            data.SaveInventoryRecord(item);
        }
    }
}