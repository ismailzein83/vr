using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class FreeFTTHPathManager
    {
        public static Guid s_beDefinitionId = new Guid("a37ccee4-e846-42cc-be54-35881681185f");

        public FreeFTTHPath GetFreeFTTHPath(string fdbNumber)
        {
            Dictionary<string, FreeFTTHPath> freeFTTHPathsByNumber = GetFreeFTTHPathsByNumber();
            return freeFTTHPathsByNumber.GetRecord(fdbNumber);
        }

        private Dictionary<string, FreeFTTHPath> GetFreeFTTHPathsByNumber()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_beDefinitionId);

            Dictionary<string, FreeFTTHPath> results = new Dictionary<string, FreeFTTHPath>();

            if (genericBusinessEntities != null)
            {
                foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                {
                    if (genericBusinessEntity.FieldValues == null)
                        continue;

                    FreeFTTHPath freeFTTHPath = new FreeFTTHPath()
                    {
                        FDBNumber = (string)genericBusinessEntity.FieldValues.GetRecord("FDBNumber"),
                        FDB = (long)genericBusinessEntity.FieldValues.GetRecord("FDB"),
                        FDBPort = (long)genericBusinessEntity.FieldValues.GetRecord("FDBPort"),
                        Splitter = (long)genericBusinessEntity.FieldValues.GetRecord("Splitter"),
                        SplitterOutPort = (long)genericBusinessEntity.FieldValues.GetRecord("SplitterOutPort"),
                        SplitterInPort = (long)genericBusinessEntity.FieldValues.GetRecord("SplitterInPort"),
                        OLT = (long)genericBusinessEntity.FieldValues.GetRecord("OLT"),
                        OLTVerticalPort = (long)genericBusinessEntity.FieldValues.GetRecord("OLTVerticalPort"),
                        OLTHorizontalPort = (long)genericBusinessEntity.FieldValues.GetRecord("OLTHorizontalPort"),
                        IMS = (long)genericBusinessEntity.FieldValues.GetRecord("IMS"),
                        TID = (long)genericBusinessEntity.FieldValues.GetRecord("TID"),
                        CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                    };
                    results.Add(freeFTTHPath.FDBNumber, freeFTTHPath);
                }
            }

            return results;
        }
    }
}