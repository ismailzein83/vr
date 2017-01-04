using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
namespace TONEAPI.ClassCode
{

  
    public class SupplierRates
    {

        public class Entity
        {
            public int SupplierRateId { get; set; }
            public int ZoneId { get; set; }
            public int PriceListId { get; set; }
            public int CurrencyId { get; set; }
            public double NormalRate { get; set; }
            public object OtherRates { get; set; }
            public DateTime BED { get; set; }
            public object EED { get; set; }
            public object SourceId { get; set; }
        }

        public class Datum
        {
            public Entity Entity { get; set; }
            public string SupplierZoneName { get; set; }
            public string CurrencyName { get; set; }
        }

        public class rate
        {
            public string ResultKey { get; set; }
            public IList<Datum> Data { get; set; }
            public int TotalCount { get; set; }
        }

        public string getsupplierrate(RestClient rc, Uri ur, string token, string parameter, string connections)
        {
         
            string result = rc.MakeRequested(parameter, token);

            var obj = JsonConvert.DeserializeObject<rate>(result);


            List<Entity> LC = obj.Data.AsEnumerable().Select(row => new Entity
            {
                ZoneId = row.Entity.ZoneId,
                BED = row.Entity.BED,
                CurrencyId = row.Entity.CurrencyId,
                NormalRate = row.Entity.NormalRate

            }).ToList();


            connect con = new connect();

            DataSet dsk = con.getdata2("SELECT TOP 1000 [ID]      ,[PriceListID]      ,[ZoneID]      ,[CurrencyID]      ,[NormalRate]      ,[OtherRates]      ,[BED]      ,[EED]      ,[timestamp]      ,[SourceID]  FROM [TOneV2_QA].[TOneWhS_BE].[SupplierRate] WHERE PriceListID IN ( SELECT TOP 1000 [ID]   FROM [TOneV2_QA].[TOneWhS_BE].[SupplierPriceList] WHERE SupplierID=55)", connections);

            List<Entity> fromdatabase = new List<Entity>();

            foreach( DataRow row in dsk.Tables[0].Rows)
            {
                  

                try
                {
                    Entity et = new Entity();
                    et.BED = row.Field<DateTime>("BED");
                    et.CurrencyId = row.Field<int>("CurrencyId");
                 //   et.ZoneId = row.Field<int>("Zoneid");
                  //  et.NormalRate = row.Field<double>("NormalRate");

                    fromdatabase.Add(et);
                }
                catch(Exception exc)
                {

                }
            }



            bool Supplierrate = false;
            foreach (Entity c in LC)
            {
                foreach ( Entity D in fromdatabase)
                {
                    if (c.BED == D.BED && c.CurrencyId == D.CurrencyId)
                        Supplierrate = true;
                    else
                        Supplierrate = false;
                }
            }
            if (Supplierrate)
                result = result + " Success : Supplier rate Page rate are retrieved correctly \n|";
          

            return result;


        }
    }
}