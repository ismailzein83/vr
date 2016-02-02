﻿using System;
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
        private NumberPrefixes GetCachedNumberPrefixes()
        {
            var numberPrefixes = base.GetConfiguration(null);
            if (numberPrefixes.Prefixes == null)
            {
                numberPrefixes.Prefixes = new List<NumberPrefix>();
            }
            return numberPrefixes;
        }


        public IEnumerable<NumberPrefix> GetPrefixes()
        {
            var cachedPrefixes = GetCachedNumberPrefixes();
            return cachedPrefixes.Prefixes;
        }

        public bool UpdatePrefixes(List<NumberPrefix> prefixes)
        {
            var cachedPrefixes = GetCachedNumberPrefixes();
            cachedPrefixes.Prefixes = prefixes;
            bool updateActionSucc = false;
            updateActionSucc = base.UpdateConfiguration(cachedPrefixes);
            return updateActionSucc;
        }


    }
}
