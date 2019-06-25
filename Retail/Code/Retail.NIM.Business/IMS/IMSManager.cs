using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class IMSManager
    {
        public static Guid s_beDefinitionId = new Guid("37cee388-9fe0-464b-bdbc-576c4f14c3fd");

        public IMS GetIMS(string imsNumber)
        {
            Dictionary<string, IMS> imsByNumber = GetCachedIMSsByNumber();
            return imsByNumber.GetRecord(imsNumber);
        }

        private Dictionary<string, IMS> GetCachedIMSsByNumber()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedIMSsByNumber", s_beDefinitionId, () =>
            {
                List<IMS> imsList = this.GetCachedIMSs();
                return imsList.ToDictionary(itm => itm.Number, itm => itm);
            });
        }

        private List<IMS> GetCachedIMSs()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedIMSs", s_beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_beDefinitionId);

                List<IMS> results = new List<IMS>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        IMS ims = new IMS()
                        {
                            Id = (long)genericBusinessEntity.FieldValues.GetRecord("Id"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Number = (string)genericBusinessEntity.FieldValues.GetRecord("Number"),
                            Area = (long)genericBusinessEntity.FieldValues.GetRecord("Area"),
                            Site = (long)genericBusinessEntity.FieldValues.GetRecord("Site"),
                            Vendor = (long)genericBusinessEntity.FieldValues.GetRecord("Vendor"),
                            Region = (int)genericBusinessEntity.FieldValues.GetRecord("Region"),
                            City = (int)genericBusinessEntity.FieldValues.GetRecord("City"),
                            Town = (int)genericBusinessEntity.FieldValues.GetRecord("Town"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        results.Add(ims);
                    }
                }

                return results;
            });
        }
    }
}
