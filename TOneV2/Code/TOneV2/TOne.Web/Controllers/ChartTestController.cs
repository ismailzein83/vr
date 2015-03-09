using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TOne.Web.Controllers
{
    public class ChartTestController : ApiController
    {
        static Random random = new Random();
        static DateTime recentDate = DateTime.Now.AddMinutes(-1);

        [HttpGet]
        public List<List<ChartPoint>> GetLatestValues()
        {
            List<List<ChartPoint>> points = new List<List<ChartPoint>>();
            for (int i = 0; i < 3; i++)
                points.Add(new List<ChartPoint>());
            DateTime now = DateTime.Now;
            for (DateTime d = recentDate.AddSeconds(1); d <= now; d = d.AddSeconds(1))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    var series = points[i];
                    series.Add(new ChartPoint
                        {
                            Value = random.Next((i + 1) * 200),
                            Time = d
                        });
                }
            }
            recentDate = now;
            return points;
        }
    }

    public class ChartPoint
    {
        public DateTime Time { get; set; }

        public int Value { get; set; }
    }
}