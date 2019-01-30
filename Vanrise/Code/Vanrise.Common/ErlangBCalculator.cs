using System;

namespace Vanrise.Common
{
    public static class ErlangBCalculator
    {
        public static double CalculateBHT(int numberOfDevices, float blockingPercentage)
        {
            if (numberOfDevices <= 0)
                throw new Exception($"Invalid value of number of devices {numberOfDevices}");

            if (blockingPercentage <= 0 || blockingPercentage > 100)
                throw new Exception($"Invalid value of blocking percentage {blockingPercentage}");

            return ErlangBOfferedLoad(blockingPercentage, numberOfDevices);
        }

        private static double ErlangBOfferedLoad(float blockingPercentage, int numberOfDevices)
        {
            float blockingPercentageProbability = blockingPercentage / 100;

            double a = numberOfDevices;
            var check = ProbBlock(a, numberOfDevices);

            while (check > blockingPercentageProbability)
            {
                a = a / 2;
                check = ProbBlock(a, numberOfDevices);
            }

            while (check < blockingPercentageProbability)
            {
                a = a * 2;
                check = ProbBlock(a, numberOfDevices);
            }

            var fraction = 1e-7;
            double step = a;

            var gosCalc = ProbBlock(a, numberOfDevices);
            var diffError = blockingPercentageProbability - gosCalc;
            if (diffError < 0)
            {
                step = a / 2;
                a = a / 2;
            }

            while (Math.Abs(diffError) > (blockingPercentageProbability * fraction))
            {
                step = step * 0.75;
                var prevA = a;
                a = a + (step * ((diffError > 0) ? 1 : 0) - (step * ((diffError < 0) ? 1 : 0)));
                if (a < 0)
                {
                    a = prevA / 2;
                }
                gosCalc = ProbBlock(a, numberOfDevices);
                diffError = blockingPercentageProbability - gosCalc;
            }
            return a;
        }

        private static double ProbBlock(double a, double c)
        {
            var invB = 1.0;
            c = Math.Floor(c);
            var count = 0.0;

            for (int k = 1; k <= c; k++)
            {
                count++;
                invB = 1.0 + (count * invB) / a;
            }

            var blockProb = 1.0 / invB;
            return blockProb;
        }

        //private static double ErlangBBHT(float blockingPercentage, int numberOfDevices)
        //{
        //    float blockingProbability = blockingPercentage / 100;

        //    double BHTCount = 0.00005;

        //    while (ErlangB(BHTCount, numberOfDevices) < blockingProbability)
        //        BHTCount = BHTCount + 0.00005;

        //    return BHTCount - 0.00005;
        //}

        //private static double ErlangB(double blockingProbability, int numberOfDevices)
        //{
        //    double pbr, index;

        //    if (blockingProbability > 0)
        //    {
        //        pbr = (1 + blockingProbability) / blockingProbability;
        //        for (index = 2; index != numberOfDevices + 1; index++)
        //        {
        //            pbr = index / blockingProbability * pbr + 1;
        //            if (pbr > 10000)
        //                return 0;
        //        }

        //        return 1 / pbr;
        //    }

        //    return 0;
        //}

        //private static double RoundNumber(double result)
        //{
        //    int length = result < 1 ? 5 : 4;
        //    string[] splitted = result.ToString().Split('.');
        //    int remainingLength = length - splitted[0].Length;

        //    for (int index = splitted[1].Length; index > remainingLength; index--)
        //    {
        //        long factor2 = long.Parse(1.ToString().PadRight(index, '0'));
        //        result = (double)Math.Round((decimal)result * factor2, MidpointRounding.AwayFromZero) / factor2;
        //    }

        //    return result;
        //}
    }
}