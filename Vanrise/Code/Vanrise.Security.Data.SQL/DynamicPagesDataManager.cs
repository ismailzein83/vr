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

            return GetItemsSP("sec.sp_View_GetByType", DynamicPageMapper, ViewType.Dynamic);
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
        public List<WidgetDefinition> GetWidgets()
        {

            return GetItemsSP("sec.sp_WidgetDefinition_GetWidgets", WidgetMapper);
        }
        private WidgetDefinition WidgetMapper(IDataReader reader)
        {
            WidgetDefinition instance = new WidgetDefinition
            {
                ID = (int)reader["ID"],
                Name = reader["Name"] as string,
                directiveName = reader["DirectiveName"] as string,
                Setting=Vanrise.Common.Serializer.Deserialize<WidgetDefinitionSetting>(reader["Setting"] as string)
        };
            return instance;
        }
        public bool SaveView(View view, out int insertedId)
        {string serialziedContent=null;
        if (view.Content != null)
            serialziedContent = Common.Serializer.Serialize(view.Content);
            string serialziedAudience =null;
            if (view.Audience != null)
                serialziedAudience = Common.Serializer.Serialize(view.Audience,true);
            object viewId;
            string URL = "#/viewwithparams/Security/Views/DynamicPages/DynamicPagePreview";
            int recordesEffected = ExecuteNonQuerySP("sec.sp_View_InsertView", out viewId, view.Name, URL, view.ModuleId, null,
               serialziedAudience, serialziedContent, ViewType.Dynamic);
            insertedId = (int)viewId;
            return (recordesEffected > 0);
          //  return false;
        }
      
        public View GetView(int viewId)
        {
            return GetItemSP("sec.sp_View_GetById ", GetPageMapper, viewId); 
        }
        private View GetPageMapper(IDataReader reader)
        {

            View instance=new View();
            instance.Content= Common.Serializer.Deserialize<List<VisualElement>>(reader["Content"] as string);
           
            return instance;
        }
    }
}
