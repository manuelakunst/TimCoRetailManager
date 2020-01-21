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
    public class SaleData : ISaleData
    {
        private readonly IProductData _prodData;
        private readonly ISqlDataAccess _sqlData;

        public SaleData(IProductData prodData, ISqlDataAccess sqlData)
        {
            _prodData = prodData;
            _sqlData = sqlData;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: make it SOLID/DRY/better

            // create the Sale Details Model
            var details = new List<SaleDetailDBModel>();
            var taxRate = ConfigHelper.GetTaxRate();

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                var productInfo = _prodData.GetProductById(detail.ProductId);

                if (productInfo == null)
                {
                    throw new Exception($"The product Id of { detail.ProductId } could not be found in the database.");
                }

                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;
                if (productInfo.IsTaxable)
                    detail.Tax = detail.PurchasePrice * taxRate;

                details.Add(detail);
            }

            // create the Sale Model
            var sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };
            sale.Total = sale.SubTotal + sale.Tax;

            // save Sale to DB
            try
            {
                // ACHTUNG: SQL Transaction in C# sollte nur selten verwendet werden. 
                // Das Offen-Halten der DB-Connection ist immer ein Risiko (Performanz!)

                _sqlData.StartTransaction("TRMData");

                _sqlData.SaveDataInTransaction("dbo.spSale_Insert", sale);

                // get the Sale Id 
                sale.Id = _sqlData.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate })
                    .FirstOrDefault();

                // finish saving the sale details
                foreach (var item in details)
                {
                    item.SaleId = sale.Id;
                    _sqlData.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                _sqlData.CommitTransaction();
            }
            catch
            {
                _sqlData.RollbackTransaction();
                throw;
            }
        }

        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sqlData.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

            return output;
        }

    }
}
