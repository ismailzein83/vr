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
        {

        }
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
        protected override string GetConnectionString()
        {
            return IvSwitchSync.MasterConnectionString;
        }
        private TranslationRule TranslationRuleMapper(IDataReader reader)
        {
            TranslationRule translationRule = new TranslationRule
            {
                TranslationRuleId = (int)reader["trans_rule_id"],
                Name = reader["trans_rule_name"] as string,
                DNISPattern = reader["dnis_pattern"] as string,
                CLIPattern = reader["cli_pattern"] as string,
                CreationDate = (DateTime)reader["create_date"]
            };

            return translationRule;
        }


        public List<TranslationRule> GetTranslationRules()
        {
            String cmdText = "SELECT  trans_rule_id,trans_rule_name, dnis_pattern, cli_pattern, create_date  FROM trans_rules;";
            return GetItemsText(cmdText, TranslationRuleMapper, (cmd) =>
            {
            });
        }
        public bool Update(TranslationRule translationRule)
        {
            String cmdText = String.Format(@"UPDATE trans_rules
	                             SET trans_rule_name = @psgname
                                 {0}
                                 {1}
                                 WHERE  trans_rule_id = @psgid and  NOT EXISTS(SELECT 1 FROM  trans_rules WHERE trans_rule_id != @psgid and trans_rule_name = @psgname);"
                , translationRule.DNISPattern != null ? " , dnis_pattern = @psgdnis" : " ,dnis_pattern = DEFAULT"
                , translationRule.CLIPattern != null ? ",cli_pattern = @psgcli" : ",cli_pattern = DEFAULT");

            int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
             {
                 cmd.Parameters.AddWithValue("@psgid", translationRule.TranslationRuleId);
                 cmd.Parameters.AddWithValue("@psgname", translationRule.Name);
                 if (translationRule.DNISPattern != null) cmd.Parameters.AddWithValue("@psgdnis", translationRule.DNISPattern);
                 if (translationRule.CLIPattern != null) cmd.Parameters.AddWithValue("@psgcli", translationRule.CLIPattern);

             }
           );
            return (recordsEffected > 0);
        }

        public bool Insert(TranslationRule translationRule, out int insertedId)
        {
            String cmdText = string.Format(@"INSERT INTO trans_rules(trans_rule_name {0} {1})
	                             SELECT @psgname {2} {3}
	                             WHERE (NOT EXISTS(SELECT 1 FROM trans_rules WHERE  trans_rule_name = @psgname))
	                             returning  trans_rule_id;", translationRule.DNISPattern != null ? ",dnis_pattern" : ""
                , translationRule.CLIPattern != null ? ",cli_pattern" : "",
                translationRule.DNISPattern != null ? ",@psgdnis" : ""
                , translationRule.CLIPattern != null ? ",@psgcli " : "");

            var translationRuleId = ExecuteScalarText(cmdText, cmd =>
                {
                    cmd.Parameters.AddWithValue("@psgname", translationRule.Name);
                    if (translationRule.DNISPattern != null) cmd.Parameters.AddWithValue("@psgdnis", translationRule.DNISPattern);
                    if (translationRule.CLIPattern != null) cmd.Parameters.AddWithValue("@psgcli ", translationRule.CLIPattern);

                }
              );

            insertedId = -1;
            if (translationRuleId != null)
            {
                insertedId = Convert.ToInt32(translationRuleId);
                return true;
            }
            return false;

        }

    }
}
