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
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sqlData;

        public UserData(ISqlDataAccess sqlData)
        {
            _sqlData = sqlData;
        }
        public List<UserModel> GetUserById(string Id)
        {
            // anonyme Klasseninstanz, um den Parameter id uebergeben zu koennen
            //var p = new { Id = id };

            var output = _sqlData.LoadData<UserModel, dynamic>("dbo.spUserLookup", new { Id }, "TRMData");
            return output;
        }
    }
}
