﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData
    {
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: make it SOLID/DRY/better

            // create the Sale Details Model
            var details = new List<SaleDetailDBModel>();
            var productData = new ProductData();
            var taxRate = ConfigHelper.GetTaxRate();

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                var productInfo = productData.GetProductById(detail.ProductId);

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
            using (var sql = new SqlDataAccess())
            {
                try
                {
                    sql.StartTransaction("TRMData");

                    sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                    // get the Sale Id 
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { sale.CashierId, sale.SaleDate })
                        .FirstOrDefault();

                    // finish saving the sale details
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }

                    //sql.CommitTransaction();
                }
                catch ()
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }
        }

    }
}
