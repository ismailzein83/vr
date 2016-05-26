using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{
    public class City
    {

        public int CountryId { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }

    }

    public class CityDetail
    {
        public City Entity { get; set; }
        public string CountryName  { get; set; }
      

    }


    public class jasonresp
    {
        public List<CityDetail> Data { get; set; }
    
    }


}