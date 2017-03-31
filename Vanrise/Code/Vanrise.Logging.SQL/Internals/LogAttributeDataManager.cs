using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Logging.SQL
{
    public class LogAttributeDataManager : BaseSQLDataManager
    {
        public LogAttributeDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {

        }
        public bool AreLogAttributesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[logging].[LogAttribute]", ref updateHandle);
        }
        LogAttributesByType _logAttributes;
        public List<LogAttribute> GetLogAttributes()
        {
            return GetItemsSP("[logging].[sp_LogAttribute_GetAll]", LogAttributeMapper);
        }
        LogAttribute LogAttributeMapper(IDataReader reader)
        {
            LogAttribute logAttribute = new LogAttribute
            {
                LogAttributeID = (int)reader["ID"],
                AttributeType = (int)reader["AttributeType"],
                Description = reader["Description"] as string
            };
            return logAttribute;
        }
        public void LoadLogAttributesIfNotLoaded()
        {
            if (_logAttributes == null)
            {
                lock (this)
                {
                    if (_logAttributes == null)
                    {
                        var logAttributesByType = new LogAttributesByType();
                        ExecuteReaderSP("[logging].[sp_LogAttribute_GetAll]",
                            (reader) =>
                            {
                                while (reader.Read())
                                {
                                    LogAttributeType attributeType = (LogAttributeType)reader["AttributeType"];
                                    LogAttributesByDescription logAttributesByDescription;
                                    if (!logAttributesByType.TryGetValue(attributeType, out logAttributesByDescription))
                                    {
                                        logAttributesByDescription = new LogAttributesByDescription();
                                        logAttributesByType.TryAdd(attributeType, logAttributesByDescription);
                                    }
                                    string attributeDescription = reader["Description"] as string;
                                    if (!logAttributesByDescription.ContainsKey(attributeDescription))
                                        logAttributesByDescription.TryAdd(attributeDescription, (int)reader["ID"]);
                                }
                            });
                        _logAttributes = logAttributesByType;
                    }
                }
            }
        }
        public int GetAttributeId(LogAttributeType attributeType, string attributeDescription)
        {
            LogAttributesByDescription logAttributesByDescription;
            if (!_logAttributes.TryGetValue(attributeType, out logAttributesByDescription))
            {
                logAttributesByDescription = new LogAttributesByDescription();
                _logAttributes.TryAdd(attributeType, logAttributesByDescription);
            }
            int attributeId;
            if (!logAttributesByDescription.TryGetValue(attributeDescription, out attributeId))
            {
                attributeId = (int)ExecuteScalarSP("[logging].[sp_LogAttribute_InsertIfNeededAndGetID]", (int)attributeType, attributeDescription);
                logAttributesByDescription.TryAdd(attributeDescription, attributeId);
            }
            return attributeId;
        }
    }
}
