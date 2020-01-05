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
    public class UserData
    {
        private readonly IConfiguration _config;

        public UserData(IConfiguration config)
        {
            this._config = config;
        }
        public List<UserModel> GetUserById(string id)
        {
            var sql = new SqlDataAccess(_config);

            // anonyme Klasseninstanz, um den Parameter id uebergeben zu koennen
            var p = new { Id = id };

            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "TRMData");
            return output;
        }
    }
}
