using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DefineFixedPrefixesManager : GenericConfigurationManager<FixedPrefixes>
    {
        public Vanrise.Entities.IDataRetrievalResult<FixedPrefixDetail> GetFilteredFixedPrefixes(Vanrise.Entities.DataRetrievalInput<FixedPrefixesQuery> input)
        {
            var config = GetCachedFixedPrefixes();

            Func<FixedPrefix, bool> filterExpression = (prod) =>
                (input.Query.Name == null || prod.Prefix.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, config.Prefixes.ToBigResult(input, filterExpression, MapToDetails));
        }
        public Vanrise.Entities.UpdateOperationOutput<FixedPrefixDetail> UpdateFixedPrefix(FixedPrefix fixedPrefix)
        {
            Vanrise.Entities.UpdateOperationOutput<FixedPrefixDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<FixedPrefixDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var fixedPrefixes = GetCachedFixedPrefixes();
            bool updateActionSucc=false;
            if (fixedPrefixes.Prefixes.Any(x => x.ID == fixedPrefix.ID))
            {
                if (fixedPrefixes.Prefixes.FindRecord(x => x.ID == fixedPrefix.ID).Prefix != fixedPrefix.Prefix && fixedPrefixes.Prefixes.Exists(x => x.Prefix == fixedPrefix.Prefix))
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
                else
                {
                    fixedPrefixes.Prefixes.FindRecord(x => x.ID == fixedPrefix.ID).Prefix = fixedPrefix.Prefix;
                    updateActionSucc = base.UpdateConfiguration(fixedPrefixes);
                }
                
            }

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MapToDetails(fixedPrefix);
            }

            return updateOperationOutput;

        }

        public Vanrise.Entities.DeleteOperationOutput<FixedPrefixDetail> DeleteFixedPrefix(int fixedPrefixId)
        {
            Vanrise.Entities.DeleteOperationOutput<FixedPrefixDetail> deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<FixedPrefixDetail>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
            DefineFixedPrefixesManager manager = new DefineFixedPrefixesManager();
            var fixedPrefixes = GetCachedFixedPrefixes();
            bool deleteActionSucc=false;
            if (fixedPrefixes.Prefixes.Any(x => x.ID == fixedPrefixId))
            {
                fixedPrefixes.Prefixes.Remove(fixedPrefixes.Prefixes.FindRecord(x => x.ID == fixedPrefixId));
                deleteActionSucc = base.UpdateConfiguration(fixedPrefixes);
            }
            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }

        public Vanrise.Entities.InsertOperationOutput<FixedPrefixDetail> AddFixedPrefix(FixedPrefix fixedPrefix)
        {

            Vanrise.Entities.InsertOperationOutput<FixedPrefixDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<FixedPrefixDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            var fixedPrefixes = GetCachedFixedPrefixes();
             bool insertActionSucc=false;

             if (!fixedPrefixes.Prefixes.Exists(x => x.Prefix == fixedPrefix.Prefix))
            {
                fixedPrefix.ID = fixedPrefixes.Prefixes.Count() + 1;
                fixedPrefixes.Prefixes.Add(fixedPrefix);
                 insertActionSucc = base.UpdateConfiguration(fixedPrefixes);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }
                
 
            if (insertActionSucc)
            {
                
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = MapToDetails(fixedPrefix);
            }

            return insertOperationOutput;
        }

        public FixedPrefix GetFixedPrefix(int fixedPrefixId)
        {
            var fixedPrefixes = GetCachedFixedPrefixes();
            return fixedPrefixes.Prefixes.FindRecord(x=>x.ID==fixedPrefixId);
        }
        private FixedPrefixDetail MapToDetails(FixedPrefix fixedPrefix)
        {
            return new FixedPrefixDetail
            {
                Entity = fixedPrefix,
            };
        }

        private FixedPrefixInfo FixedPrefixInfoMapper(FixedPrefix fixedPrefix)
        {
            return new FixedPrefixInfo()
            {
                ID = fixedPrefix.ID,
                Prefix = fixedPrefix.Prefix,
            };
        }

        private FixedPrefixes GetCachedFixedPrefixes()
        {
            var fixedPrefixes = base.GetConfiguration(null);
            if(fixedPrefixes.Prefixes==null)
            {
                fixedPrefixes.Prefixes = new List<FixedPrefix>();
            }
            return fixedPrefixes;
        }


        public IEnumerable<FixedPrefixInfo> GetPrefixesInfo()
        {
            var cachedPrefixes = GetCachedFixedPrefixes();
            return cachedPrefixes.Prefixes.MapRecords(FixedPrefixInfoMapper);
        }
    }
}
