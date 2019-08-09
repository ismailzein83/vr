using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.DevTools.Data;
using Vanrise.DevTools.Entities;
using Vanrise.Entities;

namespace Vanrise.DevTools.Data.SQL
{
    public class DevProjectTemplateDataManager : BaseSQLDataManager, IVRGeneratedScriptDevProjectTemplateDataManager
    {
        public DevProjectTemplateDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public string Connection_String { get; set; }

        protected override string GetConnectionString()
        {
            return Connection_String;
        }

        public List<VRDevProject> GetDevProjects()
        {
            return GetItemsText("select * from common.VRDevProject ", DevProjectsMapper, null);

        }

        VRDevProject DevProjectsMapper(IDataReader reader)
        {
            return new VRDevProject
            {
                VRDevProjectID = GetReaderValue<Guid>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
    }
}
