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
    public class NumberPrefixesManager : GenericConfigurationManager<NumberPrefixes>
    {
        public Vanrise.Entities.IDataRetrievalResult<NumberPrefixDetail> GetFilteredNumberPrefixes(Vanrise.Entities.DataRetrievalInput<NumberPrefixesQuery> input)
        {
            var config = GetCachedNumberPrefixes();

            Func<NumberPrefix, bool> filterExpression = (prod) =>
                (input.Query.Name == null || prod.Prefix.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, config.Prefixes.ToBigResult(input, filterExpression, MapToDetails));
        }
        public Vanrise.Entities.UpdateOperationOutput<NumberPrefixDetail> UpdateNumberPrefix(NumberPrefix numberPrefix)
        {
            Vanrise.Entities.UpdateOperationOutput<NumberPrefixDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<NumberPrefixDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var numberPrefixes = GetCachedNumberPrefixes();
            bool updateActionSucc=false;
            if (numberPrefixes.Prefixes.Any(x => x.ID == numberPrefix.ID))
            {
                if (numberPrefixes.Prefixes.FindRecord(x => x.ID == numberPrefix.ID).Prefix != numberPrefix.Prefix && numberPrefixes.Prefixes.Exists(x => x.Prefix == numberPrefix.Prefix))
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
                else
                {
                    numberPrefixes.Prefixes.FindRecord(x => x.ID == numberPrefix.ID).Prefix = numberPrefix.Prefix;
                    updateActionSucc = base.UpdateConfiguration(numberPrefixes);
                }
                
            }

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MapToDetails(numberPrefix);
            }

            return updateOperationOutput;

        }

        public Vanrise.Entities.DeleteOperationOutput<NumberPrefixDetail> DeleteNumberPrefix(int numberPrefixId)
        {
            Vanrise.Entities.DeleteOperationOutput<NumberPrefixDetail> deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<NumberPrefixDetail>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;
            NumberPrefixesManager manager = new NumberPrefixesManager();
            var numberPrefixes = GetCachedNumberPrefixes();
            bool deleteActionSucc=false;
            if (numberPrefixes.Prefixes.Any(x => x.ID == numberPrefixId))
            {
                numberPrefixes.Prefixes.Remove(numberPrefixes.Prefixes.FindRecord(x => x.ID == numberPrefixId));
                deleteActionSucc = base.UpdateConfiguration(numberPrefixes);
            }
            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }

        public Vanrise.Entities.InsertOperationOutput<NumberPrefixDetail> AddNumberPrefix(NumberPrefix numberPrefix)
        {

            Vanrise.Entities.InsertOperationOutput<NumberPrefixDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<NumberPrefixDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            var numberPrefixes = GetCachedNumberPrefixes();
             bool insertActionSucc=false;

             if (!numberPrefixes.Prefixes.Exists(x => x.Prefix == numberPrefix.Prefix))
            {
                numberPrefix.ID = numberPrefixes.Prefixes.Count() + 1;
                numberPrefixes.Prefixes.Add(numberPrefix);
                 insertActionSucc = base.UpdateConfiguration(numberPrefixes);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }
                
 
            if (insertActionSucc)
            {
                
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = MapToDetails(numberPrefix);
            }

            return insertOperationOutput;
        }

        public NumberPrefix GetNumberPrefix(int numberPrefixId)
        {
            var numberPrefixes = GetCachedNumberPrefixes();
            return numberPrefixes.Prefixes.FindRecord(x=>x.ID==numberPrefixId);
        }
        private NumberPrefixDetail MapToDetails(NumberPrefix numberPrefix)
        {
            return new NumberPrefixDetail
            {
                Entity = numberPrefix,
            };
        }

        private NumberPrefixInfo NumberPrefixInfoMapper(NumberPrefix numberPrefix)
        {
            return new NumberPrefixInfo()
            {
                ID = numberPrefix.ID,
                Prefix = numberPrefix.Prefix,
            };
        }

        private NumberPrefixes GetCachedNumberPrefixes()
        {
            var numberPrefixes = base.GetConfiguration(null);
            if (numberPrefixes.Prefixes == null)
            {
                numberPrefixes.Prefixes = new List<NumberPrefix>();
            }
            return numberPrefixes;
        }


        public IEnumerable<NumberPrefixInfo> GetPrefixesInfo()
        {
            var cachedPrefixes = GetCachedNumberPrefixes();
            return cachedPrefixes.Prefixes.MapRecords(NumberPrefixInfoMapper);
        }
    }
}
