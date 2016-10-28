using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;


namespace NP.IVSwitch.Data.Postgres
{
    class TranslationRuleDataManager : BaseSQLDataManager, ITranslationRuleDataManager
    {
          public TranslationRuleDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {
 
        }

        private TranslationRule TranslationRuleMapper(IDataReader Reader)
        {
            TranslationRule translationRule = new TranslationRule
            {
                TranslationRuleId = (int)Reader["ID"],
                Name = Reader["Name"] as string,
                DNIS_Pattern = Reader["DNIS_Pattern"] as string,
                CLI_Pattern = Reader["CLI_Pattern"] as string                  
            };

            return translationRule;
        }

        
        public List<TranslationRule> GetTranslationRules()
        {
            return GetItemsSP("NP_IVSwitch.sp_TranslationRule_GetAll", TranslationRuleMapper);
        }
        public bool Update(TranslationRule TranslationRule)
        {
            int recordsEffected = ExecuteNonQuerySP("NP_IVSwitch.sp_TranslationRule_Update", TranslationRule.TranslationRuleId, TranslationRule.Name, TranslationRule.CLI_Pattern, TranslationRule.DNIS_Pattern);
            return (recordsEffected > 0);
        }

        public bool Insert(TranslationRule TranslationRule, out int insertedId)
        {
            object translationRuleId;

            int recordsEffected = ExecuteNonQuerySP("NP_IVSwitch.sp_TranslationRule_Insert", out translationRuleId, TranslationRule.Name, TranslationRule.CLI_Pattern, TranslationRule.DNIS_Pattern);

            insertedId = -1;
            if (recordsEffected > 0)
            {
                insertedId = (int)translationRuleId;
            }

            return (recordsEffected > 0);

        }

        public bool AreTranslationRuleUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("NP_IVSwitch.TranslationRule", ref updateHandle);
        }
    }
}
