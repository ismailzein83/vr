using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;
using Npgsql;
using Vanrise.Common;

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
                CreationDate = (DateTime)reader["create_date"],
            };
            var dnisPattern = reader["dnis_pattern"] as string;
            translationRule.DNISPattern = dnisPattern;
            translationRule.EngineType = EngineType.Regex;
            if (!String.IsNullOrEmpty(dnisPattern))
            {
                if (dnisPattern.Contains("+"))
                {
                    translationRule.DNISPatternSign = PrefixSign.Plus;
                    translationRule.DNISPattern = dnisPattern.Replace("+", "");
                    translationRule.EngineType = EngineType.Fast;
                }
                else if (dnisPattern.Contains("-"))
                {
                    translationRule.DNISPatternSign = PrefixSign.Minus;
                    translationRule.DNISPattern = dnisPattern.Replace("-", "");
                    translationRule.EngineType = EngineType.Fast;
                }
            }
            var cliPattern = reader["cli_pattern"] as string;
            if (!String.IsNullOrEmpty(cliPattern))
            {
                if (cliPattern.Contains("*"))
                {
                    translationRule.CLIType = CLIType.PoolBasedCLI;
                    var poolId = int.Parse(cliPattern.Replace("*", ""));
                    translationRule.PoolBasedCLISettings = GetPoolBasedCLISettings(poolId);
                }
                else
                {
                    translationRule.CLIType = CLIType.FixedCLI;
                    translationRule.FixedCLISettings = new FixedCLISettings();
                    if (cliPattern.Contains("+"))
                    {
                        translationRule.FixedCLISettings.CLIPatternSign = PrefixSign.Plus;
                        translationRule.FixedCLISettings.CLIPattern = cliPattern.Replace("+", "");
                    }
                    else if (cliPattern.Contains("-"))
                    {
                        translationRule.FixedCLISettings.CLIPatternSign = PrefixSign.Minus;
                        translationRule.FixedCLISettings.CLIPattern = cliPattern.Replace("-", "");
                    }
                    else
                    {
                        translationRule.FixedCLISettings.CLIPattern = cliPattern;
                    }
                }
            }
            return translationRule;
        }
        private string CLIPatternMapper(IDataReader reader)
        {
            return reader["cli_pattern"] as string;
        }
        private int PoolBasedCLISettingsIdMapper(IDataReader reader)
        {
            return (int)reader["pool_id"];
        }
        public List<TranslationRule> GetTranslationRules()
        {
            String cmdText = "SELECT  trans_rule_id, trans_rule_name, cli_pattern, dnis_pattern, engine_id, create_date  FROM trans_rules;";
            var translationRules = GetItemsText(cmdText, TranslationRuleMapper, (cmd) =>
            {
            });
            return translationRules;
        }

        public PoolBasedCLISettings PoolBasedCLISettingsMapper(IDataReader reader)
        {
            PoolBasedCLISettings poolBasedCLISettings = new PoolBasedCLISettings()
            {
                Destination = reader["destination"] as string,
                DisplayName = reader["dn"] as string,
                Prefix = reader["prefix"] as string,
                RandMin = (int)reader["rand_min"],
                RandMax = (int)reader["rand_max"]
            };
            return poolBasedCLISettings;
        }
        public int GetPoolBasedCLISettingsID()
        {
            String cmdText = String.Format(@"SELECT  POOL_ID FROM X_CLI_POOLS WHERE POOL_ID = (SELECT Max(POOL_ID) FROM X_CLI_POOLS);");
            var id = GetItemText(cmdText, PoolBasedCLISettingsIdMapper, (cmd) =>
            {

            });
            return id;
        }
        public int GetPoolBasedCLISettingsPreference(int poolId)
        {
            String cmdText = String.Format(@"SELECT preference FROM x_cli_pools_items WHERE pool_id = @poolid AND preference=(SELECT MAX(PREFERENCE) from x_cli_pools_items where pool_id=@poolid);");
            return GetItemText(cmdText, PoolBasedCLISettingsPreferenceMapper, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@poolid", poolId);
            });
        }
        public int GetPoolBasedCLISettingsIDByDescription(string translationRuleName)
        {
            String cmdText = String.Format(@"SELECT POOL_ID FROM X_CLI_POOLS WHERE description = @description;");
            return GetItemText(cmdText, PoolBasedCLISettingsIdMapper, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@description", translationRuleName);
            });
        }

        public int PoolBasedCLISettingsPreferenceMapper(IDataReader reader)
        {
            return (int)reader["preference"];
        }

        public bool Update(TranslationRule translationRule)
        {
            var cliPatternString = translationRule.FixedCLISettings!=null ? ",cli_pattern= @psgcli" : "";
            if (translationRule.FixedCLISettings == null && translationRule.PoolBasedCLISettings == null)
                cliPatternString = ",cli_pattern= DEFAULT";
            String cmdText = String.Format(@"UPDATE trans_rules
	                             SET trans_rule_name = @psgname
                                 {0},
                                 engine_id = @engineid
                                 {1}
                                 WHERE  trans_rule_id = @psgid ;",
            !String.IsNullOrEmpty(translationRule.DNISPattern) ? ",dnis_pattern= @psgdnis" : ",dnis_pattern=DEFAULT", cliPatternString);
            int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
             {
                 cmd.Parameters.AddWithValue("@psgid", translationRule.TranslationRuleId);
                 cmd.Parameters.AddWithValue("@psgname", translationRule.Name);

                 string dnisPattern = translationRule.DNISPattern;

                 if (translationRule.EngineType == EngineType.Fast)
                 {
                     var sign = translationRule.DNISPatternSign.Value == PrefixSign.Plus ? "+" : "-";
                     dnisPattern = String.Concat(sign, dnisPattern);
                     cmd.Parameters.AddWithValue("@engineid", 0);
                 }
                 else
                 {
                     cmd.Parameters.AddWithValue("@engineid", 1);
                 }
                 if (!String.IsNullOrEmpty(translationRule.DNISPattern))
                     cmd.Parameters.AddWithValue("@psgdnis", dnisPattern);
                 if (translationRule.FixedCLISettings != null)
                 {
                     string fixedCLIPattern = translationRule.FixedCLISettings.CLIPattern;
                     if (translationRule.FixedCLISettings.CLIPatternSign.HasValue)
                     {
                         var sign = translationRule.FixedCLISettings.CLIPatternSign.Value == PrefixSign.Plus ? "+" : "-";
                         fixedCLIPattern = String.Concat(sign, fixedCLIPattern);
                     }
                     cmd.Parameters.AddWithValue("@psgcli", fixedCLIPattern);
                 }
             }
           );
            if (translationRule.PoolBasedCLISettings != null)
            {
                var poolId = GetPoolBasedCLISettingsIDByDescription(translationRule.Name);
                var existingPoolBasedCLISettings = GetPoolBasedCLISettings(poolId);
                if (existingPoolBasedCLISettings == null)
                {
                    return InsertPoolBasedCLISettings(translationRule, translationRule.TranslationRuleId);
                }
                else
                {
                    return UpdatePoolBasedCLISettings(translationRule, poolId, existingPoolBasedCLISettings);
                }
            }
            else
            {
                var poolId = GetPoolBasedCLISettingsIDByDescription(translationRule.Name);
                if (poolId > 0)
                {
                    var deleteCMDText = String.Format(@"DELETE FROM X_CLI_POOLS WHERE pool_id=@poolid;");
                    int deletePoolItemCMDRecordsAffected = ExecuteNonQueryText(deleteCMDText, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@poolid", poolId);
                    });

                    var deleteCMDItemsText = String.Format(@"DELETE FROM X_CLI_POOLS_ITEMS WHERE pool_id=@poolid;");
                    int deletePoolItemsCMDRecordsAffected = ExecuteNonQueryText(deleteCMDItemsText, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@poolid", poolId);
                    });
                    return deletePoolItemCMDRecordsAffected > 0 && deletePoolItemsCMDRecordsAffected > 0;
                }
                return recordsEffected > 0;
            }
        }

        public bool Delete(int translationRuleId)
        {
            String cliPatternCMDText = @"Select cli_pattern from trans_rules where trans_rule_id=@psgid;";
            var cliPattern = GetItemText(cliPatternCMDText, CLIPatternMapper, cmd =>
            {
                cmd.Parameters.AddWithValue("@psgid", translationRuleId);
            });
            if (cliPattern!=null && cliPattern.Contains("*"))
            {
                var poolId = int.Parse(cliPattern.Replace("*", ""));
                String deleteCLIPoolsCMDText = @"Delete from x_cli_pools where pool_id=@poolid;";
                int cliPoolsRecordsAffected = ExecuteNonQueryText(deleteCLIPoolsCMDText, cmd =>
                {
                    cmd.Parameters.AddWithValue("@poolid", poolId);
                });
                String deleteCLIPoolsItemsCMDText = @"Delete from x_cli_pools_items where pool_id=@poolid;";
                int cliPoolsItemsRecordsAffected = ExecuteNonQueryText(deleteCLIPoolsItemsCMDText, cmd =>
                {
                    cmd.Parameters.AddWithValue("@poolid", poolId);
                });
                if (cliPoolsRecordsAffected == 0 || cliPoolsItemsRecordsAffected == 0)
                    return false;
            }
            String deleteCMDText = String.Format(@"Delete from trans_rules
                                 WHERE  trans_rule_id = @psgid;");
            int recordsAffected = ExecuteNonQueryText(deleteCMDText, cmd =>
            {
                cmd.Parameters.AddWithValue("@psgid", translationRuleId);
            });
            return recordsAffected > 0;
        }

        public string PoolBasedCLISettingsCLIPatternsMapper(IDataReader reader)
        {
            return reader["cli"] as string;
        }
        public void SetCLIPatterns(PoolBasedCLISettings existingPoolBasedCLISettings, int poolId)
        {
            String cliPatternsCMDText = String.Format(@"SELECT cli FROM X_CLI_POOLS_items WHERE pool_id =@poolid;");
            existingPoolBasedCLISettings.CLIPatterns = GetItemsText(cliPatternsCMDText, PoolBasedCLISettingsCLIPatternsMapper, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@poolid", poolId);
            });
        }
        public PoolBasedCLISettings GetPoolBasedCLISettings(int poolId)
        {
            PoolBasedCLISettings poolBasedCLISettings = new PoolBasedCLISettings()
            {
                CLIPatterns = new List<string>()
            };
            String settingsCMDText = string.Format("SELECT prefix, destination, rand_min, rand_max,dn FROM x_cli_pools_items where pool_id=@poolid;");
            poolBasedCLISettings =  GetItemText(settingsCMDText, PoolBasedCLISettingsMapper, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@poolid", poolId);
            });
            if (poolBasedCLISettings != null)
            {
                String cliPatternsCMDText = String.Format(@"SELECT cli FROM X_CLI_POOLS_items WHERE pool_id =@poolid;");
                poolBasedCLISettings.CLIPatterns = GetItemsText(cliPatternsCMDText, PoolBasedCLISettingsCLIPatternsMapper, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@poolid", poolId);
                });
            }
            return poolBasedCLISettings;
        }
        public bool Insert(TranslationRule translationRule, out int insertedId)
        {
            String transRulesTableCMD = string.Format(@"INSERT INTO trans_rules(trans_rule_name {0},engine_id {1})
	                             SELECT @psgname {2}, @engineid {3}
	                             returning  trans_rule_id;",
                !String.IsNullOrEmpty(translationRule.DNISPattern)? ",dnis_pattern" : "",
                translationRule.FixedCLISettings != null ? ",cli_pattern" : "",
                !String.IsNullOrEmpty(translationRule.DNISPattern) ? ",@psgdnis" : "",
                translationRule.FixedCLISettings != null ? ",@psgcli " : "");
            var translationRuleId = ExecuteScalarText(transRulesTableCMD, cmd =>
                {
                    cmd.Parameters.AddWithValue("@psgname", translationRule.Name);

                    string dnisPattern = translationRule.DNISPattern;
                    if (translationRule.EngineType == EngineType.Fast )
                    {
                        if(!String.IsNullOrEmpty(dnisPattern))
                        {
                            var sign = translationRule.DNISPatternSign.Value == PrefixSign.Plus ? "+" : "-";
                            dnisPattern = dnisPattern!=null ? String.Concat(sign, dnisPattern) : null;
                        }
                        cmd.Parameters.AddWithValue("@engineid", 0);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@engineid", 1);
                    }
                    if(!String.IsNullOrEmpty(dnisPattern))
                    {
                        cmd.Parameters.AddWithValue("@psgdnis",dnisPattern);
                    }
                    if (translationRule.FixedCLISettings != null)
                    {
                        string fixedCLIPattern = translationRule.FixedCLISettings.CLIPattern;
                        if (translationRule.FixedCLISettings.CLIPatternSign.HasValue)
                        {
                            var sign = translationRule.FixedCLISettings.CLIPatternSign.Value == PrefixSign.Plus ? "+" : "-";
                            fixedCLIPattern = String.Concat(sign, fixedCLIPattern);
                        }
                        cmd.Parameters.AddWithValue("@psgcli ", fixedCLIPattern);
                    }
                }
              );
            insertedId = -1;
            if (translationRuleId != null)
            {
                insertedId = Convert.ToInt32(translationRuleId);
                if (translationRule.PoolBasedCLISettings != null)
                {
                    return InsertPoolBasedCLISettings(translationRule, Convert.ToInt32(translationRuleId));
                }
                return true;
            }
            return false;

        }
        public bool InsertPoolBasedCLISettings(TranslationRule translationRule, int translationRuleId)
        {
            var poolId = GetPoolBasedCLISettingsID();
            String CLIPoolsTableCMDText = string.Format(@"INSERT INTO x_cli_pools(pool_id, total_items, description, match_cli_dst)
	                             SELECT @poolid, @totalitems, @description, @matchclidst
	                         
                                 returning pool_id;");
            var newPoolId = ExecuteScalarText(CLIPoolsTableCMDText, cmd =>
            {
                cmd.Parameters.AddWithValue("@poolid", poolId + 1);
                cmd.Parameters.AddWithValue("@totalitems", translationRule.PoolBasedCLISettings.CLIPatterns.Count);
                cmd.Parameters.AddWithValue("@description", translationRule.Name);
                cmd.Parameters.AddWithValue("@matchclidst", 1);
            }
             );

            if (newPoolId != null)
            {
                String cmdText = String.Format(@"UPDATE trans_rules
	                             SET cli_pattern = @psgcli
                                 WHERE  trans_rule_id = @psgid;");
                int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
                {
                    cmd.Parameters.AddWithValue("@psgid", translationRuleId);
                    cmd.Parameters.AddWithValue("@psgname", translationRule.Name);
                    cmd.Parameters.AddWithValue("@psgcli", string.Format("*{0}", newPoolId));
                }
               );
                List<int> poolItemIds = new List<int>();
                for (int i = 0; i < translationRule.PoolBasedCLISettings.CLIPatterns.Count; i++)
                {
                    var poolItemId = InsertPoolItem((int)newPoolId, i+1, translationRule.PoolBasedCLISettings.Prefix, translationRule.PoolBasedCLISettings.CLIPatterns[i], translationRule.PoolBasedCLISettings.Destination, translationRule.PoolBasedCLISettings.RandMin, translationRule.PoolBasedCLISettings.RandMax, translationRule.PoolBasedCLISettings.DisplayName);
                    if(poolItemId!=null)
                        poolItemIds.Add((int)poolItemId);
                }
                return recordsEffected > 0 && poolItemIds.Count == translationRule.PoolBasedCLISettings.CLIPatterns.Count;
            }
            return false;
        }
        public bool UpdatePoolBasedCLISettings(TranslationRule translationRule, int poolId, PoolBasedCLISettings existingPoolBasedCLISettings)
        {
            String cmdText = String.Format(@"UPDATE x_cli_pools
	                             SET total_items = @totalitems,
                                 description = @description
                                 WHERE  pool_id = @poolid; ");
            int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
            {
                cmd.Parameters.AddWithValue("@totalitems", translationRule.PoolBasedCLISettings.CLIPatterns.Count);
                cmd.Parameters.AddWithValue("@description", translationRule.Name);
                cmd.Parameters.AddWithValue("@poolid", poolId);
            }
           );
            if (recordsEffected > 0)
            {
                List<string> itemsToAdd = new List<string>();
                List<string> itemsToDelete = new List<string>();
                List<string> itemsToUpdate = new List<string>();
                List<int> recordsAffected = new List<int>();
                var newCLIPatternsCount = translationRule.PoolBasedCLISettings.CLIPatterns.Count;
                var existingCLIPatternsCount = existingPoolBasedCLISettings.CLIPatterns.Count;

                bool areParametersEqual = CheckParameters(translationRule.PoolBasedCLISettings, existingPoolBasedCLISettings);
                if (!areParametersEqual)
                {
                    for (int i = 0; i < existingCLIPatternsCount; i++)
                    {
                        itemsToUpdate.Add(existingPoolBasedCLISettings.CLIPatterns[i]);
                    }
                }
                for (int i = 0; i < newCLIPatternsCount; i++)
                {
                    var item = translationRule.PoolBasedCLISettings.CLIPatterns[i];
                    if (!existingPoolBasedCLISettings.CLIPatterns.Contains(item))
                        itemsToAdd.Add(item);
                }
                for (int i = 0; i < existingCLIPatternsCount; i++)
                {
                    var item = existingPoolBasedCLISettings.CLIPatterns[i];
                    if (!translationRule.PoolBasedCLISettings.CLIPatterns.Contains(item))
                        itemsToDelete.Add(item);
                }
                if (itemsToDelete.Count > 0)
                {
                    foreach (var itemToDelete in itemsToDelete)
                    {
                        String deleteItemCMDText = String.Format(@"DELETE FROM  x_cli_pools_items
                                 WHERE  pool_id = @poolid and cli=@cli;");
                        int deleteItemCMDRecordsAffected = ExecuteNonQueryText(deleteItemCMDText, cmd =>
                        {
                            cmd.Parameters.AddWithValue("@poolid", poolId);
                            cmd.Parameters.AddWithValue("@cli", itemToDelete);
                        });
                        recordsAffected.Add(deleteItemCMDRecordsAffected);
                    }
                }
                if (itemsToAdd.Count > 0)
                {
                    int preference = GetPoolBasedCLISettingsPreference(poolId);
                    for (int i = 0; i < itemsToAdd.Count; i++)
                    {
                        var itemToAdd = itemsToAdd[i];
                        var poolItemId = InsertPoolItem(poolId, preference + 1 + i, translationRule.PoolBasedCLISettings.Prefix, itemToAdd, translationRule.PoolBasedCLISettings.Destination, translationRule.PoolBasedCLISettings.RandMin, translationRule.PoolBasedCLISettings.RandMax, translationRule.PoolBasedCLISettings.DisplayName);
                        if (poolItemId == null)
                            return false;
                        else
                            recordsAffected.Add((int)poolItemId);
                    }
                }
                if (itemsToUpdate.Count > 0)
                {
                    for (int i = 0; i < itemsToUpdate.Count; i++)
                    {
                        String updateItemCMDText = String.Format(@"UPDATE x_cli_pools_items
	                            SET prefix = @prefix
                                {0},
                                rand_min = @randmin, 
                                rand_max = @randmax,
                                dn = @dn
                                WHERE  pool_id = @poolid; ",
                                !String.IsNullOrEmpty(translationRule.PoolBasedCLISettings.Destination) ? ",destination=@destination" : ",destination=DEFAULT");

                        int updateOperationRecordsAffected = ExecuteNonQueryText(updateItemCMDText, cmd =>
                        {
                            cmd.Parameters.AddWithValue("@prefix", translationRule.PoolBasedCLISettings.Prefix);
                            if(!String.IsNullOrEmpty(translationRule.PoolBasedCLISettings.Destination)){
                                cmd.Parameters.AddWithValue("@destination", translationRule.PoolBasedCLISettings.Destination);
                            }
                            cmd.Parameters.AddWithValue("@randmin", translationRule.PoolBasedCLISettings.RandMin!=0 ? translationRule.PoolBasedCLISettings.RandMin : -1);
                            cmd.Parameters.AddWithValue("@randmax", translationRule.PoolBasedCLISettings.RandMax);
                            cmd.Parameters.AddWithValue("@dn", translationRule.PoolBasedCLISettings.DisplayName);
                            cmd.Parameters.AddWithValue("@poolid", poolId);
                            cmd.Parameters.AddWithValue("@cli", itemsToUpdate[i]);
                        }
                );
                        recordsAffected.Add(updateOperationRecordsAffected);
                    }
                }
                foreach (var result in recordsAffected)
                {
                    if (result == 0)
                        return false;
                }
                return true;
            }
            return false;
        }

        public bool CheckParameters(PoolBasedCLISettings newPoolBasedCLISettings, PoolBasedCLISettings oldPoolBasedCLISettings)
        {
            if (newPoolBasedCLISettings.Destination != oldPoolBasedCLISettings.Destination || newPoolBasedCLISettings.DisplayName != oldPoolBasedCLISettings.DisplayName || newPoolBasedCLISettings.Prefix != oldPoolBasedCLISettings.Prefix || newPoolBasedCLISettings.RandMax != oldPoolBasedCLISettings.RandMax || newPoolBasedCLISettings.RandMin != oldPoolBasedCLISettings.RandMin)
                return false;
            return true;
        }

        public object InsertPoolItem(int poolId, int preference, string prefix, string cliPattern, string destination, int randMin, int randMax, string displayName)
        {
            String CLIPoolsItemsTableCMDText = string.Format(@"INSERT INTO x_cli_pools_items(pool_id, preference, prefix, cli {0}, mode, rand_min, rand_max, dn)
	                                    SELECT @poolid, @preference, @prefix, @cli {1}, @mode , @randmin, @randmax, @dn
	                                    returning  pool_id;",
                                       !String.IsNullOrEmpty(destination) ? ",destination" : "",
                                           !String.IsNullOrEmpty(destination) ? ",@destination" : "");
                    return ExecuteScalarText(CLIPoolsItemsTableCMDText, cmd =>
                    {
                        cmd.Parameters.AddWithValue("@poolid", poolId);
                        cmd.Parameters.AddWithValue("@preference", preference);
                        cmd.Parameters.AddWithValue("@prefix", prefix);
                        cmd.Parameters.AddWithValue("@cli", cliPattern);
                        if (!String.IsNullOrEmpty(destination))
                            cmd.Parameters.AddWithValue("@destination", destination);
                        cmd.Parameters.AddWithValue("@mode", 3);
                        if (randMin == 0)
                            randMin = -1;
                        cmd.Parameters.AddWithValue("@randmin", randMin);
                        cmd.Parameters.AddWithValue("@randmax", randMax);
                        cmd.Parameters.AddWithValue("@dn", displayName);

                    });
        }
    }
}
