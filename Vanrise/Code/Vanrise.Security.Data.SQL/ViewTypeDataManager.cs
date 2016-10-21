using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class ViewTypeDataManager : BaseSQLDataManager, IViewTypeDataManager
    {
        //#region ctor
        //public ViewTypeDataManager()
        //    : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        //{

        //}
        //#endregion

        //#region Public Methods
        //public List<ViewType> GetViewTypes()
        //{
        //    return GetItemsSP("sec.sp_ViewType_GetAll", ViewTypeMapper);
        //}

        //public bool AreViewTypesUpdated(ref object updateHandle)
        //{
        //    return base.IsDataUpdated("[sec].[ViewType]", ref updateHandle);
        //}

        //#endregion
      
        //#region Mappers
        //private ViewType ViewTypeMapper(IDataReader reader)
        //{
        //    ViewType instance = Vanrise.Common.Serializer.Deserialize<ViewType>(reader["Details"] as string);
        //    instance.ViewTypeId =GetReaderValue<Guid>(reader,"ID");
        //    instance.Name = reader["Name"] as string;
        //    instance.Title = reader["Title"] as string;
        //    return instance;
        //}
        //#endregion
    }
}
