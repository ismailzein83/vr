using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Mappers
{
    public class Utils
    {
        public static T GetReaderValue<T>(IDataReader reader, string fieldName)
        {
            return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
        }
    }
}
