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
using Vanrise.Voucher.Data;
using Vanrise.Common.Business;

namespace Vanrise.Voucher.Business
{
    public class VoucherCardsManager
    {
        public static Guid _definitionId = new Guid("6761d9be-baff-4d80-a903-16947b705395");

        public CheckVoucherAvailabilityOutput CheckVoucherAvailability(string pinCode, string lockedBy)
        {
            //encrypt the pin code
            string encryptedPinCode = Cryptography.Encrypt(pinCode, DataEncryptionKeyManager.GetLocalTokenDataDecryptionKey());

            var getGenericBEFilterGroup = new RecordFilterGroup()
            {
                LogicalOperator = RecordQueryLogicalOperator.And,
                Filters = new List<RecordFilter>()
                {
                    new StringRecordFilter(){FieldName = "PinCode", CompareOperator= StringRecordFilterOperator.Equals, Value = encryptedPinCode },
                }
            };
            var genericBusinessEntityManager = new GenericBusinessEntityManager();
            var genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId, null, getGenericBEFilterGroup);

            if (genericBusinessEntities != null && genericBusinessEntities.Count > 0)
            {
                var genericBusinessEntity = genericBusinessEntities.First();
                var voucharCardId = (long)genericBusinessEntity.FieldValues.GetRecord("ID");
                var updateGenericBEFilterGroup = new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.And,
                    Filters = new List<RecordFilter>()
                    {
                        new NonEmptyRecordFilter(){FieldName ="ActivationDate" },
                        new DateTimeRecordFilter(){FieldName = "ActivationDate" , ComparisonPart = DateTimeRecordFilterComparisonPart.DateTime , CompareOperator = DateTimeRecordFilterOperator.Less , Value = System.DateTime.Now  },
                        new DateTimeRecordFilter(){FieldName = "ExpiryDate" , ComparisonPart = DateTimeRecordFilterComparisonPart.DateTime , CompareOperator = DateTimeRecordFilterOperator.Greater , Value = System.DateTime.Now  },
                        new EmptyRecordFilter(){ FieldName = "LockedDate" }
                    }
                };
                var genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate();
                genericBusinessEntityToUpdate.FieldValues = new Dictionary<string,object>();
                genericBusinessEntityToUpdate.FieldValues.Add("LockedBy",lockedBy);
                genericBusinessEntityToUpdate.FieldValues.Add("LockedDate", DateTime.Now);
                genericBusinessEntityToUpdate.GenericBusinessEntityId = voucharCardId;
                genericBusinessEntityToUpdate.BusinessEntityDefinitionId = _definitionId;
                genericBusinessEntityToUpdate.FilterGroup = updateGenericBEFilterGroup;
                
                var updateOutput = genericBusinessEntityManager.UpdateGenericBusinessEntity(genericBusinessEntityToUpdate);
                if (updateOutput.Result == UpdateOperationResult.Succeeded)
                {
                    int currencyId = (int)genericBusinessEntity.FieldValues.GetRecord("CurrencyId");
                    return new CheckVoucherAvailabilityOutput
                    {
                        Amount = (decimal)genericBusinessEntity.FieldValues.GetRecord("Amount"),
                        CurrencySymbol = new CurrencyManager().GetCurrencySymbol(currencyId),
                        IsAvailable = true
                    };
                }
            }

            return new CheckVoucherAvailabilityOutput
            {
                IsAvailable = false
            };
        }
        public bool ActivateVoucherCards(VoucherCardsActivationInput voucherCardsActivationInput)
        {
            var getGenericBEFilterGroup = new RecordFilterGroup()
            {
                LogicalOperator = RecordQueryLogicalOperator.And,
                Filters = new List<RecordFilter>()
                {
                        new ObjectListRecordFilter(){FieldName = "GenerationVoucherId" ,   Values = new List<object>{voucherCardsActivationInput.VoucherCardsGenerationId}  },
                        new EmptyRecordFilter() {FieldName = "ActivationDate" }
                }
            };

            var genericBusinessEntityManager = new GenericBusinessEntityManager();
            var genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId, null, getGenericBEFilterGroup);
            var count = genericBusinessEntities.Count;
            if (genericBusinessEntities != null && count >= voucherCardsActivationInput.Numberofcards) {
                //var updateGenericBEFilterGroup = new RecordFilterGroup()
             //   {
                    //LogicalOperator = RecordQueryLogicalOperator.And,
                   // Filters = new List<RecordFilter>()
                 //   {  
                        //new DateTimeRecordFilter(){FieldName = "ExpiryDate" , ComparisonPart = DateTimeRecordFilterComparisonPart.DateTime , CompareOperator = DateTimeRecordFilterOperator.Greater , Value = System.DateTime.Now  }
                        
                 //   }
               // };                
                for(var i=0;i<voucherCardsActivationInput.Numberofcards;i++)
                {
                    var voucharCardId = (long)genericBusinessEntities[i].FieldValues.GetRecord("ID");                    
                    var genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate();
                    genericBusinessEntityToUpdate.FieldValues = new Dictionary<string, object>();
                    genericBusinessEntityToUpdate.FieldValues.Add("ActivationDate", DateTime.Now);
                    genericBusinessEntityToUpdate.GenericBusinessEntityId = voucharCardId;
                    genericBusinessEntityToUpdate.BusinessEntityDefinitionId = _definitionId;

                    var updateOutput = genericBusinessEntityManager.UpdateGenericBusinessEntity(genericBusinessEntityToUpdate);
                }
                
                var voucherCardsGenerationsManager = new VoucherCardsGenerationsManager();
                var voucherCardsGeneration = voucherCardsGenerationsManager.GetVoucherCardsGeneration(voucherCardsActivationInput.VoucherCardsGenerationId);
                var InactiveCards =  count - voucherCardsActivationInput.Numberofcards;
                var voucherCardsGenerationToUpdate = new GenericBusinessEntityToUpdate();
                voucherCardsGenerationToUpdate.FieldValues = new Dictionary<string, object>();                
                voucherCardsGenerationToUpdate.FieldValues.Add("InactiveCards", InactiveCards);
                voucherCardsGenerationToUpdate.GenericBusinessEntityId = voucherCardsGeneration.VoucherCardsGenerationId;
                voucherCardsGenerationToUpdate.BusinessEntityDefinitionId = VoucherCardsGenerationsManager._definitionId;

                var updateVGenerationOutput = genericBusinessEntityManager.UpdateGenericBusinessEntity(voucherCardsGenerationToUpdate);

                }
            else throw new NullReferenceException("Not Enough Cards");
            
            
            return true; }
        public SetVoucherUsedOutput SetVoucherUsed(SetVoucherUsedInput input)
        {
            var voucherUsedOutput = new SetVoucherUsedOutput
            {
                Result = SetVoucherUsedResult.Failed
            };
            var userId = SecurityContext.Current.GetLoggedInUserId();

            string encryptedPinCode = Cryptography.Encrypt(input.PinCode, DataEncryptionKeyManager.GetLocalTokenDataDecryptionKey());

            var genericBusinessEntityManager = new GenericBusinessEntityManager();

            RecordFilterGroup recordFilterGroup = new RecordFilterGroup()
            {
                LogicalOperator = RecordQueryLogicalOperator.And,
                Filters = new List<RecordFilter>()
                {
                    new EmptyRecordFilter(){FieldName ="UsedBy" },
                    new StringRecordFilter(){FieldName = "PinCode", CompareOperator= StringRecordFilterOperator.Equals, Value = encryptedPinCode },
                    new NonEmptyRecordFilter(){FieldName = "LockedDate"}
                }
            };

            var genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId, null, recordFilterGroup);
            if (genericBusinessEntities != null && genericBusinessEntities.Count > 0)
            {
                var genericBusinessEntity = genericBusinessEntities.First();
                var voucharCardId = (long)genericBusinessEntity.FieldValues.GetRecord("ID");

                var genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate();
                genericBusinessEntityToUpdate.FieldValues = new Dictionary<string, object>();
                genericBusinessEntityToUpdate.FieldValues.Add("UsedBy", input.UsedBy);
                genericBusinessEntityToUpdate.FieldValues.Add("UsedDate", DateTime.Now);
                genericBusinessEntityToUpdate.GenericBusinessEntityId = voucharCardId;
                genericBusinessEntityToUpdate.BusinessEntityDefinitionId = _definitionId;
                var updateOutput = genericBusinessEntityManager.UpdateGenericBusinessEntity(genericBusinessEntityToUpdate);
                if (updateOutput.Result == UpdateOperationResult.Succeeded)
                {
                    voucherUsedOutput.Result = SetVoucherUsedResult.Succeeded;
                }
            }
            return voucherUsedOutput;
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

            HashSet<string> pinCodesToAdd = new HashSet<string>();
           
            ConfigManager configManager = new ConfigManager();
            var serialNumberPattern = configManager.GetSerialNumberPattern();
            serialNumberPattern.ThrowIfNull("serialNumberPattern");

            var genericBusinessEntitySettings = new GenericBusinessEntityDefinitionManager().GetGenericBEDefinitionSettings(_definitionId);
            genericBusinessEntitySettings.ThrowIfNull("genericBusinessEntityExtendedSettings", _definitionId);

            var genericBusinessEntityExtendedSettings = genericBusinessEntitySettings.ExtendedSettings.CastWithValidate<VoucharCardsExtendedSettings>("VoucharCardsExtendedSettings");
            genericBusinessEntityExtendedSettings.SerialNumberParts.ThrowIfNull("genericBusinessEntityExtendedSettings.SerialNumberParts");

            Dictionary<string, ConcatenatedPartInitializeContext> serialNumberContexts = new Dictionary<string, ConcatenatedPartInitializeContext>();
           
            foreach (var serialNumberPart in genericBusinessEntityExtendedSettings.SerialNumberParts)
            {
                string serialNumber = serialNumberPattern;
                if (serialNumber != null && serialNumber.Contains(string.Format("#{0}#", serialNumberPart.VariableName)))
                {
                    var serialNumberContext = new ConcatenatedPartInitializeContext
                    {
                        SequenceDefinitionId = _definitionId,
                        NumberOfItems = numberOfCards
                    };
                    serialNumberPart.Settings.IntializePart(serialNumberContext);
                    serialNumberContexts.Add(serialNumberPart.VariableName, serialNumberContext);
                }
            }


            var voucharCardSerialNumberPartConcatenatedPartContext = new VoucharCardSerialNumberPartConcatenatedPartContext
            {
                VoucherCardBEDefinitionId = _definitionId,
            };

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

                string serialNumber = serialNumberPattern;
                foreach (var serialNumberPart in genericBusinessEntityExtendedSettings.SerialNumberParts)
                {
                    ConcatenatedPartInitializeContext serialNumberInitializeContext;
                    if (serialNumber != null && serialNumberContexts.TryGetValue(serialNumberPart.VariableName, out serialNumberInitializeContext))
                    {
                        voucharCardSerialNumberPartConcatenatedPartContext.CustomData = serialNumberInitializeContext.CustomData;
                        serialNumber = serialNumber.Replace(string.Format("#{0}#", serialNumberPart.VariableName), serialNumberPart.Settings.GetPartText(voucharCardSerialNumberPartConcatenatedPartContext));
                    }
                }

                 dynamic _object = Activator.CreateInstance(recordRuntimeType);
                _object.VoucherTypeId = voucherTypeId;
                _object.GenerationVoucherId = generationVoucherId;
                _object.SerialNumber = serialNumber;
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
            Guid pinGuid;
            byte[] arr;
            pinGuid = Guid.NewGuid();
            arr = pinGuid.ToByteArray();
            var num = BitConverter.ToUInt64(arr, 0);
            var pinNum = num % 100000000000000; // 14 numbers

            string decryptionKey = DataEncryptionKeyManager.GetLocalTokenDataDecryptionKey();

            activationCode = Cryptography.Encrypt(pinGuid.ToString(), decryptionKey);
            return Cryptography.Encrypt(pinNum.ToString(), decryptionKey);
        }
        public HashSet<string> GetAllVoucherCardsPinCodes()
        {
            var genericBusinessEntityManager = new GenericBusinessEntityManager();
            var genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId, new List<string> { "PinCode" });
            HashSet<string> pinCodes = new HashSet<string>();
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
