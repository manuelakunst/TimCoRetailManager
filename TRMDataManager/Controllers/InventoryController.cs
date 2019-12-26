using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{ 
    [Authorize]
    public class InventoryController : ApiController
    {
        public List<InventoryModel> Get()
        {
            var data = new InventoryData();

            return data.GetInventory();
        }

        public void Post(InventoryModel item)
        {
            var data = new InventoryData();

            data.SaveInventoryRecord(item);
        }
    }
}
