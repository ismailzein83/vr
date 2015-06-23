using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;
using Vanrise.Security.Data;
namespace Vanrise.Security.Data.SQL
{
   public  class DynamicPagesDataManager : BaseSQLDataManager, IDynamicPagesDataManager
    {
       public List<DynamicPage> GetDynamicPages()
        {

            return GetItemsSP("sec.sp_View_getDynamicPages", DynamicPageMapper);
        }
       private DynamicPage DynamicPageMapper(IDataReader reader)
        {
            DynamicPage instance = new DynamicPage
            {
               // ID = (int)reader["ID"],
                PageName = reader["PageName"] as string,
                ModuleName = reader["ModuleName"] as string
            };
            return instance;
        }
        public List<Widget> GetWidgets()
        {

            return GetItemsSP("sec.sp_View_getWidgets", WidgetMapper);
        }
        private Widget WidgetMapper(IDataReader reader)
        {
            Widget instance = new Widget
            {
                ID = (int)reader["ID"],
                Name = reader["Name"] as string,
                directiveName = reader["DirectiveName"] as string,
                Setting=Vanrise.Common.Serializer.Deserialize<WidgetSetting>(reader["Setting"] as string)
        };
            return instance;
        }
        public Boolean SavePage(PageSettings PageSettings)
        {
            string serialziedContent = Common.Serializer.Serialize(PageSettings.visualElements);
            string serialziedAudience = Common.Serializer.Serialize(PageSettings.AudianceIds);

            int InsertPage = ExecuteNonQuerySP("", PageSettings.PageName,
                serialziedContent, serialziedAudience);
            return (InsertPage>0);
        }
    }
}
