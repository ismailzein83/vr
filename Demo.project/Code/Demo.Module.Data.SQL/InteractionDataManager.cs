using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class InteractionDataManager : BaseSQLDataManager, IInteractionDataManager
    {

        #region Constructors
        public InteractionDataManager() :
            base(GetConnectionStringName("DemoProjectCallCenter_DBConnStringKey", "DemoProjectCallCenter_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public List<Interaction> GetMessages()
        {
            return GetItemsSP("[CcEntities].[sp_Interaction_GetInteractions]", InteractionMapper);
        }
        #endregion  


        #region Mappers
        Interaction InteractionMapper(IDataReader reader)
        {
            return new Interaction
            {
                InteractionId = GetReaderValue<long>(reader, "ID"),
                SenderType = GetReaderValue<int>(reader, "SenderType"),
                SenderName = GetReaderValue<string>(reader, "SenderName"),
                Message = GetReaderValue<string>(reader, "Message"),
                Time = GetReaderValue<DateTime>(reader, "Time"),
            };
        }
        #endregion
    }
}
