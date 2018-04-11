using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class SMSMessageTypeManager
    {
        #region Ctor/Properties

        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();

        #endregion

        #region Public Methods
       
        public string GetSMSMessageTypeName(Guid smsMessageTypeId)
        {
            var smsMessageType = _vrComponentTypeManager.GetComponentType<SMSMessageTypeSettings, SMSMessageType>(smsMessageTypeId);
            smsMessageType.ThrowIfNull("smsMessageType", smsMessageTypeId);
            return smsMessageType.Name;
        }

        public IEnumerable<SMSMessageTypeInfo> GetSMSMessageTypeIfo(SMSMessageTypeInfoFilter filter)
        {
            Func<SMSMessageType, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (item) => {
                    return true;
                };
            }

            return _vrComponentTypeManager.GetComponentTypes<SMSMessageTypeSettings, SMSMessageType>().MapRecords(SMSMessageTypeInfoMapper, filterExpression);
        }

        public SMSMessageTypeSettings GetSMSMessageTypeSettings(Guid smsMessageTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<SMSMessageTypeSettings>(smsMessageTypeId);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Mappers

        private SMSMessageTypeInfo SMSMessageTypeInfoMapper(SMSMessageType smsMessageType)
        {
            return new SMSMessageTypeInfo
            {
                SMSMessageTypeId = smsMessageType.VRComponentTypeId,
                Name = smsMessageType.Name,
            };
        }

        #endregion
    }
}
