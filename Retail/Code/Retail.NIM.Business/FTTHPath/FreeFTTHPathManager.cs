using Retail.NIM.Data;
using Retail.NIM.Entities;
using System;

namespace Retail.NIM.Business
{
    public class FreeFTTHPathManager 
    {
        public static Guid s_beDefinitionId = new Guid("a37ccee4-e846-42cc-be54-35881681185f");

        public FreeFTTHPath GetFreeFTTHPath(string fdbNumber)
        {
            IFreeFTTHPathDataManager freeFTTHPathDataManager = Retail.NIM.Data.NIMDataManagerFactory.GetDataManager<IFreeFTTHPathDataManager>();
            return freeFTTHPathDataManager.GetFreeFTTHPath(fdbNumber);
        }
    }
}