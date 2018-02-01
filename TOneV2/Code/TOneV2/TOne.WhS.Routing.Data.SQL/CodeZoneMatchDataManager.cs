using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
	public class CodeZoneMatchDataManager : BaseTOneDataManager, ICodeZoneMatchDataManager
	{
		

		#region Constructors

		public CodeZoneMatchDataManager()
			: base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
		{

		}

		#endregion

        #region Fields

        public RoutingDatabase RPRouteDatabase { get; set; }

        #endregion

		public IEnumerable<CodeSaleZoneMatch> GetSaleZonesMatchedToSupplierZones(IEnumerable<long> supplierZoneIds)
		{
			return base.GetItemsText<CodeSaleZoneMatch>(query_GetSaleZonesMatchedToSupplierZones, CodeSaleZoneMatchMapper, (cmd) =>
			{
				cmd.Parameters.Add(new SqlParameter()
				{
					ParameterName = "@SupplierZoneIds",
					SqlDbType = System.Data.SqlDbType.NVarChar,
					Value = string.Join(",", supplierZoneIds)
				});
			});
		}

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBysupplierIds(IEnumerable<long> supplierIds, string codeStartWith)
        {
            return base.GetItemsText<CodeSupplierZoneMatch>(query_GetCodeSupplierZoneMatchBySupplierIds, CodeSupplierZoneMatchMapper, (cmd) =>
            {
                var codeStartWithParameter = new SqlParameter()
                {
                    ParameterName = "@CodeStartWith",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    IsNullable = true,
                    Value = DBNull.Value
                };
                if (codeStartWith != null)
                    codeStartWithParameter.Value = codeStartWith;
                cmd.Parameters.Add(codeStartWithParameter);
        
                var supplierIdsParameter = new SqlParameter()
                {
                    ParameterName = "@SupplierIds",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    Value = DBNull.Value
                };
                if (supplierIds != null)
                    supplierIdsParameter.Value = string.Join(",", supplierIds);
                cmd.Parameters.Add(supplierIdsParameter);
            });
        }

        public IEnumerable<CodeSupplierZoneMatch> GetSupplierZoneMatchBysupplierIdsAndSellingNumberPanId(int sellingNumberPlanId ,IEnumerable<long> supplierIds, string codeStartWith)
        {
            return base.GetItemsText<CodeSupplierZoneMatch>(query_GetCodeSupplierZoneMatchBySupplierIdsAndSellingNumberPlanId, CodeSupplierZoneMatchMapper, (cmd) =>
            {
                var codeStartWithParameter = new SqlParameter()
                {
                    ParameterName = "@CodeStartWith",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    IsNullable = true,
                    Value = DBNull.Value
                };
                if (codeStartWith != null)
                    codeStartWithParameter.Value = codeStartWith;
                cmd.Parameters.Add(codeStartWithParameter);

                var sNPId = new SqlParameter()
                {
                    ParameterName = "@SellingNumberPlanId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = sellingNumberPlanId
                };

                cmd.Parameters.Add(sNPId);
                var supplierIdsParameter = new SqlParameter()
                {
                    ParameterName = "@SupplierIds",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    Value = DBNull.Value
                };
                if (supplierIds != null)
                    supplierIdsParameter.Value = string.Join(",", supplierIds);
                cmd.Parameters.Add(supplierIdsParameter);
            });
        }
       public IEnumerable<CodeSaleZoneMatch> GetSaleZoneMatchBySellingNumberPlanId(int sellingNumberPlanId, string codeStartWith)
        {
            return base.GetItemsText<CodeSaleZoneMatch>(query_GetSaleZoneMatchBySellingNumberPlanId, CodeSaleZoneMatchMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@SellingNumberPlanId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = sellingNumberPlanId
                });

                var codeStartWithParameter = new SqlParameter()
                {
                    ParameterName = "@CodeStartWith",
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    IsNullable = true,
                    Value = DBNull.Value
                };
                if (codeStartWith != null)
                    codeStartWithParameter.Value = codeStartWith;
                cmd.Parameters.Add(codeStartWithParameter);
            });
        }
		public IEnumerable<CodeSupplierZoneMatch> GetSupplierZonesMatchedToSaleZones(IEnumerable<long> saleZoneIds, IEnumerable<int> supplierIds)
		{
			return base.GetItemsText<CodeSupplierZoneMatch>(query_GetSupplierZonesMatchedToSaleZones, CodeSupplierZoneMatchMapper, (cmd) =>
			{
				cmd.Parameters.Add(new SqlParameter()
				{
					ParameterName = "@SaleZoneIds",
					SqlDbType = System.Data.SqlDbType.NVarChar,
					Value = string.Join(",", saleZoneIds)
				});
				var supplierIdsParameter = new SqlParameter()
				{
					ParameterName = "@SupplierIds",
					SqlDbType = System.Data.SqlDbType.NVarChar,
					IsNullable = true,
					Value = DBNull.Value
				};
				if (supplierIds != null)
					supplierIdsParameter.Value = string.Join(",", supplierIds);
				cmd.Parameters.Add(supplierIdsParameter);
			});
		}

		public IEnumerable<CodeSupplierZoneMatch> GetOtherSupplierZonesMatchedToSupplierZones(int supplierId, IEnumerable<long> supplierZoneIds, IEnumerable<int> otherSupplierIds)
		{
			return base.GetItemsText<CodeSupplierZoneMatch>(query_GetOtherSupplierZonesMatchedToSupplierZones, CodeSupplierZoneMatchMapper, (cmd) =>
			{
				cmd.Parameters.Add(new SqlParameter()
				{
					ParameterName = "@SupplierId",
					SqlDbType = SqlDbType.Int,
					Value = supplierId
				});
				cmd.Parameters.Add(new SqlParameter()
				{
					ParameterName = "@SupplierZoneIds",
					SqlDbType = SqlDbType.NVarChar,
					Value = string.Join(",", supplierZoneIds)
				});
				var otherSupplierIdsParameter = new SqlParameter()
				{
					ParameterName = "@OtherSupplierIds",
					SqlDbType = System.Data.SqlDbType.NVarChar,
					IsNullable = true,
					Value = DBNull.Value
				};
				if (otherSupplierIds != null)
					otherSupplierIdsParameter.Value = string.Join(",", otherSupplierIds);
				cmd.Parameters.Add(otherSupplierIdsParameter);
			});
		}

		#region Protected Methods

		protected override string GetConnectionString()
		{
			if (RPRouteDatabase == null)
				throw new NullReferenceException("RoutingDatabase");
			if (RPRouteDatabase.Settings == null)
				throw new NullReferenceException("RoutingDatabase.Settings");
			if (string.IsNullOrEmpty(RPRouteDatabase.Settings.DatabaseName))
				throw new NullReferenceException("RoutingDatabase.Settings.DatabaseName");
			var builder = new SqlConnectionStringBuilder(base.GetConnectionString());
			builder.InitialCatalog = RPRouteDatabase.Settings.DatabaseName;
			return builder.ToString();
		}

		#endregion

		#region Queries

		private const string query_GetSaleZonesMatchedToSupplierZones =
		@"
			declare @SupplierZoneIdTable table (SupplierZoneId bigint not null)
			insert into @SupplierZoneIdTable (SupplierZoneId)
			select ParsedString from dbo.ParseStringList(@SupplierZoneIds);
			
			with CodeCTE as (select Code from dbo.CodeSupplierZoneMatch where SupplierZoneId in (select SupplierZoneId from @SupplierZoneIdTable))
			
			select Code, SellingNumberPlanId, SaleZoneId
			from dbo.CodeSaleZoneMatch
			where Code in (select Code from CodeCTE)
		";


        private const string query_GetSaleZoneMatchBySellingNumberPlanId =
        @"
			select Code,SellingNumberPlanID,SaleZoneID,CodeMatch
			from dbo.CodeSaleZoneMatch
			where SellingNumberPlanID = @SellingNumberPlanId 
                And( @CodeStartWith is null OR Code like @CodeStartWith+'%')
		";

        private const string query_GetCodeSupplierZoneMatchBySupplierIds =
        @"
			declare @SupplierIdsTable table (SupplierId bigint not null)
			insert into @SupplierIdsTable (SupplierId)
			select ParsedString from dbo.ParseStringList(@SupplierIds);
		
			select Code,SupplierID,SupplierZoneID,CodeMatch
			from dbo.CodeSupplierZoneMatch
			where SupplierID in (select SupplierId from @SupplierIdsTable)
                And( @CodeStartWith is null OR Code like @CodeStartWith+'%')
		";

        private const string query_GetCodeSupplierZoneMatchBySupplierIdsAndSellingNumberPlanId =
       @"
			declare @SupplierIdsTable table (SupplierId bigint not null)
			insert into @SupplierIdsTable (SupplierId)
			select ParsedString from dbo.ParseStringList(@SupplierIds);
		    
           

                         select cs.Code,cs.SupplierID,cs.SupplierZoneID,cs.CodeMatch from [dbo].[CodeSupplierZoneMatch] as cs JOIN @SupplierIdsTable sIds ON cs.SupplierID = sIds.SupplierId
                            WHERE( @CodeStartWith is null OR cs.Code like @CodeStartWith+'%')";

		private const string query_GetSupplierZonesMatchedToSaleZones =
		@"
			declare @SaleZoneIdTable table (SaleZoneId bigint not null)
			insert into @SaleZoneIdTable (SaleZoneId)
			select ParsedString from dbo.ParseStringList(@SaleZoneIds)
			
			declare @SupplierIdTable table (SupplierId int not null)
			if (@SupplierIds is not null)
			begin
				insert into @SupplierIdTable (SupplierId)
				select ParsedString from dbo.ParseStringList(@SupplierIds)
			end;
			
			with CodeCTE as (select Code from dbo.CodeSaleZoneMatch where SaleZoneID in (select SaleZoneId from @SaleZoneIdTable))
			
			select Code, SupplierId, SupplierZoneId, CodeMatch
			from dbo.CodeSupplierZoneMatch
			where Code in (select Code from CodeCTE) and (@SupplierIds is null or SupplierID in (select SupplierId from @SupplierIdTable))
		";

		private const string query_GetOtherSupplierZonesMatchedToSupplierZones =
		@"
			declare @SupplierZoneIdTable table (SupplierZoneId bigint not null)
			insert into @SupplierZoneIdTable (SupplierZoneId)
			select ParsedString from dbo.ParseStringList(@SupplierZoneIds)
			
			declare @OtherSupplierIdTable table (OtherSupplierId int not null)
			if (@OtherSupplierIds is not null)
			begin
				insert into @OtherSupplierIdTable (OtherSupplierId)
				select ParsedString from dbo.ParseStringList(@OtherSupplierIds)
			end;
			
			with CodeCTE as (select Code from dbo.CodeSupplierZoneMatch where SupplierZoneID in (select SupplierZoneId from @SupplierZoneIdTable))
			
			select Code, SupplierId, SupplierZoneId
			from dbo.CodeSupplierZoneMatch
			where SupplierID != @SupplierId and Code in (select Code from CodeCTE) and (@OtherSupplierIds is null or SupplierID in (select OtherSupplierId from @OtherSupplierIdTable))
		";

		#endregion

		#region Mappers

		private CodeSaleZoneMatch CodeSaleZoneMatchMapper(IDataReader reader)
		{
			return new CodeSaleZoneMatch()
			{
				Code = reader["Code"] as string,
				SellingNumberPlanId = (int)reader["SellingNumberPlanId"],
				SaleZoneId = (long)reader["SaleZoneId"],
                CodeMatch = reader["CodeMatch"] as string
			};
		}

		private CodeSupplierZoneMatch CodeSupplierZoneMatchMapper(IDataReader reader)
		{
			return new CodeSupplierZoneMatch()
			{
				Code = reader["Code"] as string,
				SupplierId = (int)reader["SupplierId"],
				SupplierZoneId = (long)reader["SupplierZoneId"],
                CodeMatch = reader["CodeMatch"] as string
			};
		}

		#endregion

        //RoutingDatabase ICodeZoneMatchDataManager.RPRouteDatabase
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //       throw new NotImplementedException();
        //    }
        //}

        //IEnumerable<CodeSaleZoneMatch> ICodeZoneMatchDataManager.GetSaleZonesMatchedToSupplierZones(IEnumerable<long> supplierZoneIds)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<CodeSupplierZoneMatch> ICodeZoneMatchDataManager.GetSupplierZonesMatchedToSaleZones(IEnumerable<long> saleZoneIds, IEnumerable<int> supplierIds)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<CodeSupplierZoneMatch> ICodeZoneMatchDataManager.GetOtherSupplierZonesMatchedToSupplierZones(int supplierId, IEnumerable<long> supplierZoneIds, IEnumerable<int> otherSupplierIds)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<CodeSupplierZoneMatch> ICodeZoneMatchDataManager.GetSupplierZoneMatchBysupplierIds(IEnumerable<long> supplierIds, string codeStartWith)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<CodeSaleZoneMatch> ICodeZoneMatchDataManager.GetSaleZoneMatchBySellingNumberPlanId(int sellingNumberPlanId, string codeStartWith)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
