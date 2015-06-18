using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class MyInterpolation
    {
        protected double a { get; set; }
        protected double b { get; set; }

        public MyInterpolation(List<double> x, List<double> y)
        {
            double x_avg = x.Average();
            double y_avg = y.Average();
            int count = x.Count;

            double sum1 = 0, sum2 = 0;
            for (int i = 0; i < count; i++)
            {
                sum1 += (x[i] - x_avg) * (y[i] - y_avg);
                sum2 += (x[i] - x_avg) * (x[i] - x_avg);
            }
            a = sum1 / sum2;
            b = y_avg - a * x_avg;
        }

        public double Interpolate(double x)
        {
            return a * x + b;
        }
    }
}
