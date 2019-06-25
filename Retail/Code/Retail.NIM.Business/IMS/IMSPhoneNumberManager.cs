using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class IMSPhoneNumberManager
    {
        public static Guid s_beDefinitionId = new Guid("fadc30a2-04bd-4673-bedc-d42284f40d1b");

        #region Public Methods

        public GetFreeIMSPhoneNumbersOutput GetFreeIMSPhoneNumbers(GetFreeIMSPhoneNumbersInput input)
        {
            input.ThrowIfNull("GetFreeIMSPhoneNumbersInput");

            IMS ims = new IMSManager().GetIMS(input.IMSNumber);
            if (ims == null)
                throw new NullReferenceException($"IMS of Number: {input.IMSNumber}");

            List<IMSPhoneNumber> imsPhoneNumbers = GetIMSPhoneNumbers(ims.Id, PhoneNumberStatus.Free, input.Category);
            return new GetFreeIMSPhoneNumbersOutput() { IMSPhoneNumbers = imsPhoneNumbers };
        }

        public ReserveIMSPhoneNumberOutput ReserveIMSPhoneNumber(ReserveIMSPhoneNumberInput input)
        {
            UpdateGenericBEPhoneNumberStatus(input.IMSPhoneNumberId, PhoneNumberStatus.Reserved);
            return new ReserveIMSPhoneNumberOutput() { OperationSucceeded = true };
        }

        public SetUsedIMSPhoneNumberOutput SetUsedIMSPhoneNumber(SetUsedIMSPhoneNumberInput input)
        {
            UpdateGenericBEPhoneNumberStatus(input.IMSPhoneNumberId, PhoneNumberStatus.Used);
            return new SetUsedIMSPhoneNumberOutput() { OperationSucceeded = true };
        }

        #endregion

        #region Private Methods

        private void UpdateGenericBEPhoneNumberStatus(long imsPhoneNumberId, Guid statusId)
        {
            GenericBusinessEntityToUpdate genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate()
            {
                BusinessEntityDefinitionId = s_beDefinitionId,
                GenericBusinessEntityId = imsPhoneNumberId,
                FieldValues = new Dictionary<string, object>()
            };
            genericBusinessEntityToUpdate.FieldValues.Add("Status", statusId);
            new GenericBusinessEntityManager().UpdateGenericBusinessEntity(genericBusinessEntityToUpdate, null);
        }


        private List<IMSPhoneNumber> GetIMSPhoneNumbers(long imsId, Guid? statusId = null, int? category = null)
        {
            List<IMSPhoneNumber> imsPhoneNumbers = GetCachedIMSPhoneNumbers();

            Func<IMSPhoneNumber, bool> filterExpression = (imsPhoneNumber) =>
            {
                if (imsPhoneNumber.IMS != imsId)
                    return false;

                if (statusId.HasValue && statusId.Value != imsPhoneNumber.Status)
                    return false;

                if (category.HasValue && category.Value != imsPhoneNumber.Category)
                    return false;

                return true;
            };
            return imsPhoneNumbers.FindAllRecords(filterExpression).ToList();
        }

        private List<IMSPhoneNumber> GetCachedIMSPhoneNumbers()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedIMSPhoneNumbers", s_beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_beDefinitionId);

                List<IMSPhoneNumber> results = new List<IMSPhoneNumber>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        IMSPhoneNumber imsPhoneNumber = new IMSPhoneNumber()
                        {
                            Id = (long)genericBusinessEntity.FieldValues.GetRecord("Id"),
                            PhoneNumber = (string)genericBusinessEntity.FieldValues.GetRecord("PhoneNumber"),
                            Area = (long)genericBusinessEntity.FieldValues.GetRecord("Area"),
                            Site = (long)genericBusinessEntity.FieldValues.GetRecord("Site"),
                            IMS = (long)genericBusinessEntity.FieldValues.GetRecord("IMS"),
                            LocalAreaCode = (long)genericBusinessEntity.FieldValues.GetRecord("LocalAreaCode"),
                            IMSLAC = (long)genericBusinessEntity.FieldValues.GetRecord("IMSLAC"),
                            Category = (int)genericBusinessEntity.FieldValues.GetRecord("Category"),
                            Status = (Guid)genericBusinessEntity.FieldValues.GetRecord("Status"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        results.Add(imsPhoneNumber);
                    }
                }

                return results;
            });
        }

        #endregion
    }

    public class GetFreeIMSPhoneNumbersInput
    {
        public string IMSNumber { get; set; }

        public int? Category { get; set; }
    }
    public class GetFreeIMSPhoneNumbersOutput
    {
        public List<IMSPhoneNumber> IMSPhoneNumbers { get; set; }
    }

    public class ReserveIMSPhoneNumberInput
    {
        public long IMSPhoneNumberId { get; set; }
    }
    public class ReserveIMSPhoneNumberOutput
    {
        public bool OperationSucceeded { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class SetUsedIMSPhoneNumberInput
    {
        public long IMSPhoneNumberId { get; set; }
    }
    public class SetUsedIMSPhoneNumberOutput
    {
        public bool OperationSucceeded { get; set; }

        public string ErrorMessage { get; set; }
    }
}