using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public static class ReaderFactory
    {
        public static ICountryReader GetCountryReader()
        {
            return Activator.CreateInstance(Type.GetType("QM.BusinessEntity.MainExtensions.CountryReaders.VanriseCountryReader, QM.BusinessEntity.MainExtensions")) as ICountryReader;
        }

        public static IZoneReader GetZoneReader()
        {
            throw new NotImplementedException();
        }
    }
}
