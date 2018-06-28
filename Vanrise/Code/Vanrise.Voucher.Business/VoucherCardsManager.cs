using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;
using Vanrise.Security.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Entities;
using Vanrise.Voucher.Entities;
namespace Vanrise.Voucher.Business
{
    public class VoucherCardsManager
    {
        static Guid _definitionId = new Guid("6761d9be-baff-4d80-a903-16947b705395");

        public VoucherCardResult CheckAvailablePinCode(string pinCode)
        {
            throw new NotImplementedException();
        }

        public VoucherCardResult SetVoucherUsed(string pinCode, string usedBy)
        {
            throw new NotImplementedException();
        }


        public void GenerateVoucherCards(long generationVoucherId, DateTime expiryDate, long voucherTypeId, int numberOfCards)
        {
            var dataRecordStorageId = new GenericBusinessEntityDefinitionManager().GetGenericBEDataRecordStorageId(_definitionId);
            var dataRcordStorageManager = new DataRecordStorageManager();
           
            var dataRecordStorage = dataRcordStorageManager.GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            var storageDataManager = dataRcordStorageManager.GetStorageDataManager(dataRecordStorageId);
            storageDataManager.ThrowIfNull("recordStorageDataManager", dataRecordStorageId);
           
            Type recordRuntimeType = GetRecordRuntimeTypeWithValidate(dataRecordStorage.DataRecordTypeId);

            List<dynamic> voucherRecords = new List<dynamic>();
            var userId = SecurityContext.Current.GetLoggedInUserId();

            var voucherType = new VoucherTypeManager().GetVoucherType(voucherTypeId);
            voucherType.ThrowIfNull("voucherType", voucherTypeId);

            var currentPinCodes = GetAllVoucherCardsPinCodes();

            List<string> pinCodesToAdd = new List<string>();


            for (var i = 0; i < numberOfCards; i++)
            {
                string activationCode = null;
                string pinCode = null;
                while (pinCode == null)
                {
                    string newActivationCode;
                    string newPinCode = GetPinCode(out  newActivationCode);
                    if ((currentPinCodes == null || !currentPinCodes.Contains(newPinCode)) && !pinCodesToAdd.Contains(newPinCode))
                    {
                        activationCode = newActivationCode;
                        pinCode = newPinCode;
                        pinCodesToAdd.Add(newPinCode);
                        break;
                    }
                }
                
                 dynamic _object = Activator.CreateInstance(recordRuntimeType);
                _object.VoucherTypeId = voucherTypeId;
                _object.GenerationVoucherId = generationVoucherId;
               // _object.SerialNumber = GetSerialNumber();
                _object.Amount = voucherType.Amount;
                _object.CurrencyId = voucherType.CurrencyId;
                _object.ActivationCode = activationCode;
                _object.PinCode = pinCode;
                _object.ExpiryDate = expiryDate;
                _object.CreatedBy = userId;
                _object.LastModifiedTime = DateTime.Now;
                _object.LastModifiedBy = userId;
                voucherRecords.Add(_object);
            }
            if (voucherRecords != null && voucherRecords.Count > 0)
            {
                var dbApplyStream = storageDataManager.InitialiazeStreamForDBApply();
                foreach (var record in voucherRecords)
                {
                    storageDataManager.WriteRecordToStream(record as Object, dbApplyStream);
                }
                var streamReadyToApply = storageDataManager.FinishDBApplyStream(dbApplyStream);
                storageDataManager.ApplyStreamToDB(streamReadyToApply);
            }
        }

        public bool SetVoucherCardsActive(long generationVoucherId, int? numberOfCards)
        {
            //List<long> voucherCardsIds
            throw new NotImplementedException();
        }

        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();
        Type _recordRuntimeType;
        private Type GetRecordRuntimeTypeWithValidate(Guid dataRecordTypeId)
        {
            if (_recordRuntimeType == null)
            {
                _recordRuntimeType = s_dataRecordTypeManager.GetDataRecordRuntimeType(dataRecordTypeId);
                _recordRuntimeType.ThrowIfNull("recordRuntimeType", dataRecordTypeId);
            }
            return _recordRuntimeType;
        }

        private string GetPinCode(out string activationCode)
        {
           /// UInt32 pinNum = 0;
            Guid pinGuid;
            byte[] arr;
            pinGuid = Guid.NewGuid();
            arr = pinGuid.ToByteArray();
            var num = BitConverter.ToUInt64(arr, 0);
            var pinNum = num % 100000000000000; // 14 numbers
            activationCode = Encrypt(pinGuid.ToString());
            return Encrypt(pinNum.ToString());
        }
        private string GetSerialNumber()
        {
            ConfigManager configManager = new ConfigManager();
            var serialNumberPattern = configManager.GetSerialNumberPattern();
            serialNumberPattern.ThrowIfNull("serialNumberPattern");

            var genericBusinessEntitySettings = new GenericBusinessEntityDefinitionManager().GetGenericBEDefinitionSettings(_definitionId);
            genericBusinessEntitySettings.ThrowIfNull("genericBusinessEntityExtendedSettings", _definitionId);

            var genericBusinessEntityExtendedSettings = genericBusinessEntitySettings.ExtendedSettings.CastWithValidate<VoucharCardsExtendedSettings>("VoucharCardsExtendedSettings");
            genericBusinessEntityExtendedSettings.SerialNumberParts.ThrowIfNull("genericBusinessEntityExtendedSettings.SerialNumberParts");

            string serialNumber = serialNumberPattern;
            var serialNumberContext = new VoucharCardSerialNumberPartConcatenatedPartContext
            {
                VoucherCardBEDefinitionId = _definitionId,
            };
            foreach (var serialNumberPart in genericBusinessEntityExtendedSettings.SerialNumberParts)
            {
                if (serialNumber != null && serialNumber.Contains(string.Format("#{0}#", serialNumberPart.VariableName)))
                {
                    serialNumber = serialNumber.Replace(string.Format("#{0}#", serialNumberPart.VariableName), serialNumberPart.Settings.GetPartText(serialNumberContext));
                }
            }
            return serialNumber;
        }

        static byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
        private static string Encrypt(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(code);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }
        private static string Decrypt(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(code));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }




        public List<string> GetAllVoucherCardsPinCodes()
        {
            var genericBusinessEntityManager = new GenericBusinessEntityManager();
            int totalCount;
            var genericBusinessEntities = genericBusinessEntityManager.GetGenericBusinessEntities(null, false, null, null, false, null, false, DataRetrievalResultType.Normal, _definitionId, new List<string> { "PinCode" }, null, null, null, DateTime.MinValue, DateTime.MaxValue, null, out totalCount);
            List<string> pinCodes = new List<string>();
            if (genericBusinessEntities != null && genericBusinessEntities.Count > 0)
            {
                foreach (var genericBusinessEntity in genericBusinessEntities)
                {
                    string pinCode = genericBusinessEntity.FieldValues.GetRecord("PinCode") as string;
                    pinCodes.Add(pinCode);
                }
            }
            return pinCodes;
        }

    }
}
