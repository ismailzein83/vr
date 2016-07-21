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
        public List<StyleDefinition> GetStyleDefinition()
        {
            return GetItemsSP("common.sp_StyleDefinition_GetAll", StyleDefinitionMapper);
        }

        public bool AreStyleDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.StyleDefinition", ref updateHandle);
        }

        public bool Insert(StyleDefinition StyleDefinitionItem)
        {
            string serializedSettings = StyleDefinitionItem.StyleDefinitionSettings != null ? Vanrise.Common.Serializer.Serialize(StyleDefinitionItem.StyleDefinitionSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_StyleDefinition_Insert", StyleDefinitionItem.StyleDefinitionId, StyleDefinitionItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(StyleDefinition StyleDefinitionItem)
        {
            string serializedSettings = StyleDefinitionItem.StyleDefinitionSettings != null ? Vanrise.Common.Serializer.Serialize(StyleDefinitionItem.StyleDefinitionSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_StyleDefinition_Update", StyleDefinitionItem.StyleDefinitionId, StyleDefinitionItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion


        #region Mappers

        StyleDefinition StyleDefinitionMapper(IDataReader reader)
        {
            StyleDefinition StyleDefinition = new StyleDefinition
            {
                StyleDefinitionId = (Guid) reader["ID"],
                Name = reader["Name"] as string,
                StyleDefinitionSettings = reader["Settings"] as string != null ? Vanrise.Common.Serializer.Deserialize<StyleDefinitionSettings>(reader["Settings"] as string) :null
            }; 
            return StyleDefinition;
        }

        #endregion
    }
}
