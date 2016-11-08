using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;
using Npgsql;
 
namespace NP.IVSwitch.Data.Postgres
{
    class TranslationRuleDataManager : BasePostgresDataManager, ITranslationRuleDataManager
    {
        public TranslationRuleDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private TranslationRule TranslationRuleMapper(IDataReader reader)
        {
            TranslationRule translationRule = new TranslationRule
            {
                TranslationRuleId = (int)reader["trans_rule_id"],
                Name = reader["trans_rule_name"] as string,
                DNIS_Pattern = reader["dnis_pattern"] as string,
                CLI_Pattern = reader["cli_pattern"] as string
            };

            return translationRule;
        }


        public List<TranslationRule> GetTranslationRules()
        {
            String cmdText = "SELECT trans_rule_id, trans_rule_name, dnis_pattern, cli_pattern  FROM trans_rules;";
            return GetItemsText(cmdText, TranslationRuleMapper,(cmd) =>
            {
            });
        }
          public bool Update(TranslationRule translationRule)
           {
              String cmdText = @"UPDATE trans_rules
	                             SET trans_rule_name = @psgname, dnis_pattern = @psgdnis ,  cli_pattern = @psgcli
                                 WHERE  trans_rule_id = @psgid and  NOT EXISTS(SELECT 1 FROM  trans_rules WHERE trans_rule_id != @psgid and trans_rule_name = @psgname);";
                                	 
               int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@psgid", translationRule.TranslationRuleId);
                    cmd.Parameters.AddWithValue("@psgname", translationRule.Name);
                    cmd.Parameters.AddWithValue("@psgdnis", translationRule.DNIS_Pattern);
                    cmd.Parameters.AddWithValue("@psgcli", translationRule.CLI_Pattern);

                }
              );
               return (recordsEffected > 0);
            }

           public bool Insert(TranslationRule translationRule, out int insertedId)
           {
               object translationRuleId;

               String cmdText = @"INSERT INTO trans_rules(trans_rule_name,dnis_pattern,cli_pattern)
	                             SELECT @psgname,@psgdnis,@psgcli 
	                             WHERE (NOT EXISTS(SELECT 1 FROM trans_rules WHERE  trans_rule_name = @psgname))
	                             returning  trans_rule_id;";

               translationRuleId = ExecuteScalarText(cmdText, (cmd) =>
                 {
                     cmd.Parameters.AddWithValue("@psgname", translationRule.Name);
                     cmd.Parameters.AddWithValue("@psgdnis", translationRule.DNIS_Pattern);
                     cmd.Parameters.AddWithValue("@psgcli ", translationRule.CLI_Pattern);

                 }
               );

               insertedId = -1;
               if (translationRuleId != null)
               {
                   insertedId = Convert.ToInt32(translationRuleId);
                   return true;
               }
               else
                   return false;              

           }

    }
}
