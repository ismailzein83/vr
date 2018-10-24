using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Tools.Data
{
    public interface ISchemaDataManager : IDataManager
    {

        List<Schema> GetSchemas();
        string Connection_String { set; }


    }
}
