using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.RDBTests.Common;
using Vanrise.Rules.Data;
using Vanrise.Rules.Entities;

namespace Vanrise.RDB.Tests.Rule
{
    [TestClass]
    public class RuleDataManagerTests
    {
        const string DBTABLE_NAME_Rule = "[Rule]";
        const string DBTABLE_NAME_RuleChanged = "[RuleChanged]";
        const string DBTABLE_NAME_RuleChangedForProcessing = "[RuleChangedForProcessing]";

        const int ruleTypeId = 2;
        IRuleDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IRuleDataManager>();
        IRuleDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IRuleDataManager>();

        [TestMethod]
        public void AddRule()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_Rule);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_RuleChanged);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_RuleChangedForProcessing);


            var rule = new Rules.Entities.Rule
            {
                BED = DateTime.Now,
                IsDeleted = true,
                TypeId = ruleTypeId,
                CreatedBy = 1,
                LastModifiedBy = 1,
                RuleDetails = "",
                EED = DateTime.Now,
                SourceId = "s"
            };
            int ruleid;
            _sqlDataManager.AddRule(rule, out ruleid);
            UTUtilities.AssertValuesAreEqual(true, _rdbDataManager.AddRule(rule, out ruleid));
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_Rule);
        }
        [TestMethod]
        public void AddRuleAndRuleChanged()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_Rule);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_RuleChanged);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_RuleChangedForProcessing);
            var rule = new Rules.Entities.Rule
            {
                BED = DateTime.Now,
                IsDeleted = false,
                TypeId = ruleTypeId,
                CreatedBy = 1,
                LastModifiedBy = 1,
                RuleDetails = "",
                EED = DateTime.Now,
                SourceId = "s"
            };
            int ruleid;
            _sqlDataManager.AddRuleAndRuleChanged(rule, ActionType.AddedRule, out ruleid);
            UTUtilities.AssertValuesAreEqual(true, _rdbDataManager.AddRuleAndRuleChanged(rule,ActionType.AddedRule, out ruleid));
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_Rule);
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_RuleChanged);
        }
        [TestMethod]
        public void GetRuleChanged()
        {
            var rule = new Rules.Entities.Rule
            {
                BED = DateTime.Now,
                IsDeleted = false,
                TypeId = ruleTypeId,
                CreatedBy = 1,
                LastModifiedBy = 1,
                RuleDetails = "",
                EED = DateTime.Now,
                SourceId = "s"
            };
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var ruleChanged1 = _sqlDataManager.GetRuleChanged(ruleid, ruleTypeId);
            ResolveDateIssue(ruleChanged1, "CreatedTime");
            var ruleChanged2 = _rdbDataManager.GetRuleChanged(ruleid, ruleTypeId);
            ResolveDateIssue(ruleChanged2, "CreatedTime");
            UTUtilities.AssertObjectsAreSimilar(ruleChanged1, ruleChanged2);
        }

        [TestMethod]
        public void DeleteRuleAndRuleChanged()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            _sqlDataManager.DeleteRuleAndRuleChanged(ruleid, ruleTypeId, 1, ActionType.DeletedRule, null);
            UTUtilities.AssertValuesAreEqual(true, _rdbDataManager.DeleteRuleAndRuleChanged(ruleid, ruleTypeId, 1, ActionType.DeletedRule, null));
        }
        [TestMethod]
        public void DeleteRuleChangedForProcessing()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            _sqlDataManager.DeleteRuleChangedForProcessing(ruleid, ruleTypeId);
            _rdbDataManager.DeleteRuleChangedForProcessing(ruleid, ruleTypeId);
        }
        [TestMethod]
        public void FillAndGetRuleChangedForProcessing()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var ruleChanged1 = _sqlDataManager.FillAndGetRuleChangedForProcessing(ruleid, ruleTypeId);
            ResolveDateIssue(ruleChanged1, "CreatedTime");
            var ruleChanged2 = _rdbDataManager.FillAndGetRuleChangedForProcessing(ruleid, ruleTypeId);
            ResolveDateIssue(ruleChanged2, "CreatedTime");
            UTUtilities.AssertObjectsAreSimilar(ruleChanged1, ruleChanged2);
        }
        [TestMethod]
        public void GetRuleChangedForProcessing()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var ruleChanged1 = _sqlDataManager.GetRuleChangedForProcessing(ruleid, ruleTypeId);
            ResolveDateIssue(ruleChanged1, "CreatedTime");
            var ruleChanged2 = _rdbDataManager.GetRuleChangedForProcessing(ruleid, ruleTypeId);
            ResolveDateIssue(ruleChanged2, "CreatedTime");
            UTUtilities.AssertObjectsAreSimilar(ruleChanged1, ruleChanged2);
        }
        [TestMethod]
        public void GetRuleTypeId()
        {
            var ruleChanged1 = _sqlDataManager.GetRuleTypeId("TOne.WhS.BusinessEntity.Entities.RouteOptionRule");
            var ruleChanged2 = _rdbDataManager.GetRuleTypeId("TOne.WhS.BusinessEntity.Entities.RouteOptionRule");
            UTUtilities.AssertValuesAreEqual(ruleChanged1, ruleChanged2);
        }
        [TestMethod]
        public void GetRulesChanged()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var ruleChanged1 = _sqlDataManager.GetRulesChanged(ruleTypeId);
            ResolveDateIssues(ruleChanged1, "CreatedTime");
            var ruleChanged2 = _rdbDataManager.GetRulesChanged(ruleTypeId);
            ResolveDateIssues(ruleChanged2, "CreatedTime");
            UTUtilities.AssertObjectsAreSimilar(ruleChanged1, ruleChanged2);
        }
        [TestMethod]
        public void DeleteRulesChangedForProcessing()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            _sqlDataManager.DeleteRulesChangedForProcessing(ruleTypeId);
            _rdbDataManager.DeleteRulesChangedForProcessing(ruleTypeId);
        }

        [TestMethod]
        public void FillAndGetRulesChangedForProcessing()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var ruleChanged1 = _sqlDataManager.FillAndGetRulesChangedForProcessing(ruleTypeId);
            ResolveDateIssues(ruleChanged1, "CreatedTime");
            var ruleChanged2 = _rdbDataManager.FillAndGetRulesChangedForProcessing(ruleTypeId);
            ResolveDateIssues(ruleChanged2, "CreatedTime");

            UTUtilities.AssertObjectsAreSimilar(ruleChanged1, ruleChanged2);
        }
       
        [TestMethod]
        public void GetRulesByType()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var ruleChanged1 = _sqlDataManager.GetRulesByType(ruleTypeId);
            ResolveDateIssues(ruleChanged1, "CreatedTime");
            ResolveDateIssues(ruleChanged1, "LastModifiedTime");
            var ruleChanged2 = _rdbDataManager.GetRulesByType(ruleTypeId);
            ResolveDateIssues(ruleChanged2, "CreatedTime");
            ResolveDateIssues(ruleChanged2, "LastModifiedTime");

            UTUtilities.AssertObjectsAreSimilar(ruleChanged1, ruleChanged2);
        }

        [TestMethod]
        public void GetRulesChangedForProcessing()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var ruleChanged1 = _sqlDataManager.GetRulesChangedForProcessing(ruleTypeId);
            ResolveDateIssues(ruleChanged1, "CreatedTime");
            var ruleChanged2 = _rdbDataManager.GetRulesChangedForProcessing(ruleTypeId);
            ResolveDateIssues(ruleChanged2, "CreatedTime");
            UTUtilities.AssertObjectsAreSimilar(ruleChanged1, ruleChanged2);
        }

        private void ResolveDateIssue(dynamic obj, string dateTimeField)
        {

            switch (dateTimeField)
            {
                case "CreatedTime":
                    obj.CreatedTime = obj.CreatedTime.Date;
                    break;
                case "LastModifiedTime":
                    obj.LastModifiedTime = obj.LastModifiedTime.Date;
                    break;
            }
        }
        private void ResolveDateIssues(dynamic objs, string dateTimeField)
        {
            foreach(var obj in objs)
            {
                switch (dateTimeField)
                {
                    case "CreatedTime":
                        obj.CreatedTime = obj.CreatedTime.Date;
                        break;
                    case "LastModifiedTime":
                        obj.LastModifiedTime = obj.LastModifiedTime.Date;
                        break;
                }
            }
        }
        [TestMethod]
        public void SetDeleted()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            _sqlDataManager.SetDeleted(new List<int> { ruleid }, 1);
            _rdbDataManager.SetDeleted(new List<int> { ruleid },1);
        }
        [TestMethod]
        public void UpdateRule()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var rule = new Rules.Entities.Rule
            {
                RuleId = ruleid,
                BED = DateTime.Now,
                IsDeleted = true,
                TypeId = ruleTypeId,
                CreatedBy = 1,
                LastModifiedBy = 1,
                RuleDetails = "Updated",
                EED = DateTime.Now,
                SourceId = "test"
            };
            _sqlDataManager.UpdateRule(rule);
            UTUtilities.AssertValuesAreEqual(true, _rdbDataManager.UpdateRule(rule));
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_Rule);
        }
        [TestMethod]
        public void UpdateRuleAndRuleChanged()
        {
            int ruleid;
            AddRuleAndRuleChanged(out ruleid);
            var rule = new Rules.Entities.Rule
            {
                RuleId = ruleid,
                BED = DateTime.Now,
                IsDeleted = true,
                TypeId = ruleTypeId,
                CreatedBy = 1,
                LastModifiedBy = 1,
                RuleDetails = "Updated1",
                EED = DateTime.Now,
                SourceId = "Updated"
            };
            _sqlDataManager.UpdateRuleAndRuleChanged(rule, ActionType.UpdatedRule,"","");
            UTUtilities.AssertValuesAreEqual(true, _rdbDataManager.UpdateRuleAndRuleChanged(rule,ActionType.UpdatedRule, "", ""));
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_Rule);
        }

        private void AddRuleAndRuleChanged(out int ruleid)
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_Rule);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_RuleChanged);
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_Rule, DBTABLE_NAME_RuleChangedForProcessing);
            var rule = new Rules.Entities.Rule
            {
                BED = DateTime.Now,
                IsDeleted = false,
                TypeId = ruleTypeId,
                CreatedBy = 1,
                LastModifiedBy = 1,
                RuleDetails = "",
                EED = DateTime.Now,
                SourceId = "s"
            };
            _sqlDataManager.AddRuleAndRuleChanged(rule, ActionType.AddedRule, out ruleid);
            _rdbDataManager.AddRuleAndRuleChanged(rule, ActionType.AddedRule, out ruleid);
        }
    }
}
