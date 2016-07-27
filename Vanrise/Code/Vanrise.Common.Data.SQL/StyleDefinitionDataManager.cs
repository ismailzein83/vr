using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class StyleDefinitionDataManager : BaseSQLDataManager, IStyleDefinitionDataManager
    {
        #region ctor/Local Variables
        public StyleDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion


        #region Public Methods

        public List<StyleDefinition> GetStyleDefinitions()
        {
            return GetItemsSP("common.sp_StyleDefinition_GetAll", StyleDefinitionMapper);
        }

        public bool AreStyleDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.StyleDefinition", ref updateHandle);
        }

        public bool Insert(StyleDefinition styleDefinitionItem)
        {
            string serializedSettings = styleDefinitionItem.StyleDefinitionSettings != null ? Vanrise.Common.Serializer.Serialize(styleDefinitionItem.StyleDefinitionSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_StyleDefinition_Insert", styleDefinitionItem.StyleDefinitionId, styleDefinitionItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(StyleDefinition styleDefinitionItem)
        {
            string serializedSettings = styleDefinitionItem.StyleDefinitionSettings != null ? Vanrise.Common.Serializer.Serialize(styleDefinitionItem.StyleDefinitionSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_StyleDefinition_Update", styleDefinitionItem.StyleDefinitionId, styleDefinitionItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion


        #region Mappers

        StyleDefinition StyleDefinitionMapper(IDataReader reader)
        {
            StyleDefinition styleDefinition = new StyleDefinition
            {
                StyleDefinitionId = (Guid) reader["ID"],
                Name = reader["Name"] as string,
                StyleDefinitionSettings = Vanrise.Common.Serializer.Deserialize<StyleDefinitionSettings>(reader["Settings"] as string) 
            };
            return styleDefinition;
        }

        #endregion
    }
}
