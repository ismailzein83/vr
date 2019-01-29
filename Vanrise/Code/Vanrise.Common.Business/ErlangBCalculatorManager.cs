//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Vanrise.Caching;
//using Vanrise.Common.Data;
//using Vanrise.Entities;

//namespace Vanrise.Common.Business
//{
//    public class ErlangBCalculatorManager
//    {
//        public double CalculateErlangBHT(int numberOfDevices, float blockingPercentage)
//        {
//            //var erlangBValues = GetCachedErlangBValues();
//            //var erlangBEntityIdentifier = new ErlangBEntityIdentifier() { NumberOfDevices = numberOfDevices, BlockingPercentage = blockingPercentage };
//            //var erlangBEntity = erlangBValues.GetRecord(erlangBEntityIdentifier);

//            //if (erlangBEntity != null)
//            //    return erlangBEntity.BHT;

//            var erlangBEntity = new ErlangBEntity();
//            erlangBEntity.BlockingPercentage = blockingPercentage;
//            erlangBEntity.NumberOfDevices = numberOfDevices;
//            erlangBEntity.BHT = CalculateBHT(numberOfDevices, blockingPercentage);


//            tryAddErlangBValueEntity(erlangBEntity);
//            //erlangBValues.Add(erlangBEntityIdentifier, erlangBEntity);

//            return erlangBEntity.BHT;
//        }

//        private static void tryAddErlangBValueEntity(ErlangBEntity erlangBEntity)
//        {
//            IErlangBCalculatorDataManager dataManager = CommonDataManagerFactory.GetDataManager<IErlangBCalculatorDataManager>();
//            dataManager.Insert(erlangBEntity);
//        }

//        private Dictionary<ErlangBEntityIdentifier, ErlangBEntity> GetCachedErlangBValues()
//        {
//            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetErlangBValues",
//               () =>
//               {
//                   IErlangBCalculatorDataManager dataManager = CommonDataManagerFactory.GetDataManager<IErlangBCalculatorDataManager>();
//                   IEnumerable<ErlangBEntity> erlangBValues = dataManager.GetErlangBValues();
//                   return erlangBValues.ToDictionary(erlangBValue => new ErlangBEntityIdentifier() { BlockingPercentage = erlangBValue.BlockingPercentage, NumberOfDevices = erlangBValue.NumberOfDevices }, erlangBValue => erlangBValue);
//               });
//        }

//        private class CacheManager : Vanrise.Caching.BaseCacheManager
//        {
//            IErlangBCalculatorDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IErlangBCalculatorDataManager>();
//            object _lastReceivedDataInfo;

//            protected override bool ShouldSetCacheExpired(object parameter)
//            {
//                return _dataManager.AreGetErlangBValuesUpdated(ref _lastReceivedDataInfo);
//            }
//        }

//        #region Private Methods

//        public static double CalculateBHT(int numberOfDevices, float blockingPercentage)
//        {
//            if (numberOfDevices <= 0)
//                throw new Exception($"Invalid value of number of devices {numberOfDevices}");

//            if (blockingPercentage <= 0 || blockingPercentage > 100)
//                throw new Exception($"Invalid value of blocking percentage {blockingPercentage}");

//            return erlangbOfferedLoad(blockingPercentage, numberOfDevices);
//        }

//        private static double erlangbOfferedLoad(float blockingPercentage, int numberOfDevices)
//        {
//            float blockingPercentageProbability = blockingPercentage / 100;

//            double a = numberOfDevices;
//            var check = probBlock(a, numberOfDevices);
//            while (check > blockingPercentageProbability)
//            {
//                a = a / 2;
//                check = probBlock(a, numberOfDevices);
//            }
//            while (check < blockingPercentageProbability)
//            {
//                a = a * 2;
//                check = probBlock(a, numberOfDevices);
//            }
//            var fraction = 1e-7;
//            double step = a;
//            var gosCalc = probBlock(a, numberOfDevices);
//            var diffError = blockingPercentageProbability - gosCalc;
//            if (diffError < 0)
//            {
//                step = a / 2;
//                a = a / 2;
//            }
//            while (Math.Abs(diffError) > (blockingPercentageProbability * fraction))
//            {
//                step = step * 0.75;
//                var prevA = a;
//                a = a + (step * ((diffError > 0) ? 1 : 0) - (step * ((diffError < 0) ? 1 : 0)));
//                if (a < 0)
//                {
//                    a = prevA / 2;
//                }
//                gosCalc = probBlock(a, numberOfDevices);
//                diffError = blockingPercentageProbability - gosCalc;
//            }
//            return a;

//        }

//        private static double probBlock(double a, double c)
//        {
//            var invB = 1.0;
//            c = Math.Floor(c);
//            var count = 0.0;

//            for (int k = 1; k <= c; k++)
//            {
//                count++;
//                invB = 1.0 + (count * invB) / a;
//            }

//            var blockProb = 1.0 / invB;
//            return blockProb;
//        }

//        //private static double ErlangBBHT(float blockingPercentage, int numberOfDevices)
//        //{
//        //    float blockingProbability = blockingPercentage / 100;

//        //    double BHTCount = 0.00005;

//        //    while (ErlangB(BHTCount, numberOfDevices) < blockingProbability)
//        //        BHTCount = BHTCount + 0.00005;

//        //    return BHTCount - 0.00005;
//        //}

//        //private static double ErlangB(double blockingProbability, int numberOfDevices)
//        //{
//        //    double pbr, index;

//        //    if (blockingProbability > 0)
//        //    {
//        //        pbr = (1 + blockingProbability) / blockingProbability;
//        //        for (index = 2; index != numberOfDevices + 1; index++)
//        //        {
//        //            pbr = index / blockingProbability * pbr + 1;
//        //            if (pbr > 10000)
//        //                return 0;
//        //        }

//        //        return 1 / pbr;
//        //    }

//        //    return 0;
//        //}

//        //private static double RoundNumber(double result)
//        //{
//        //    int length = result < 1 ? 5 : 4;
//        //    string[] splitted = result.ToString().Split('.');
//        //    int remainingLength = length - splitted[0].Length;

//        //    for (int index = splitted[1].Length; index > remainingLength; index--)
//        //    {
//        //        long factor2 = long.Parse(1.ToString().PadRight(index, '0'));
//        //        result = (double)Math.Round((decimal)result * factor2, MidpointRounding.AwayFromZero) / factor2;
//        //    }

//        //    return result;
//        //}



//        #endregion
//    }
//}
