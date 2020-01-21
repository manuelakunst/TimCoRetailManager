using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class InventoryData : IInventoryData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _sqlAccess;

        public InventoryData(IConfiguration config, ISqlDataAccess sqlAccess)
        {
            _config = config;
            _sqlAccess = sqlAccess;
        }

        public List<InventoryModel> GetInventory()
        {
            var output = _sqlAccess.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", 
                new { }, "TRMData");

            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            _sqlAccess.SaveData("dbo.spInventory_Insert", item, "TRMData");
        }
    }
}
