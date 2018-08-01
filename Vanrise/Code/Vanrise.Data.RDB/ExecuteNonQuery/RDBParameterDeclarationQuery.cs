using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBParameterDeclarationQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;

        internal RDBParameterDeclarationQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        List<RDBParameter> _parameterDeclarations = new List<RDBParameter>();

        public void AddParameter(string name, RDBDataType type)
        {
            _parameterDeclarations.Add(new RDBParameter
                {
                    Name = name,
                    DBParameterName = _queryBuilderContext.DataProvider.ConvertToDBParameterName(name),
                    Type = type,
                    Direction = RDBParameterDirection.Declared
                });
        }
        
        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            foreach(var prm in _parameterDeclarations)
            {
                context.AddParameter(prm);
            }
            return new RDBResolvedQuery { QueryText = String.Empty };
        }
    }
}
