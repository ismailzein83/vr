using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBParameterDeclarationQuery<T> : BaseRDBQuery, IRDBParameterDeclarationQuery<T>, IRDBParameterDeclarationQueryReady<T>
    {
         T _parent;
        RDBQueryBuilderContext _queryBuilderContext;
        
        public RDBParameterDeclarationQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        List<RDBParameter> _parameterDeclarations = new List<RDBParameter>();

        public IRDBParameterDeclarationQueryReady<T> AddParameter(string name, RDBDataType type)
        {
            _parameterDeclarations.Add(new RDBParameter
                {
                    Name = name,
                    DBParameterName = _queryBuilderContext.DataProvider.ConvertToDBParameterName(name),
                    Type = type,
                    Direction = RDBParameterDirection.Declared
                });
            return this;
        }

        public T EndParameterDeclaration()
        {
            return _parent;
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            foreach(var prm in _parameterDeclarations)
            {
                context.AddParameter(prm);
            }
            return new RDBResolvedQuery { QueryText = String.Empty };
        }
    }

    public interface IRDBParameterDeclarationQuery<T> : IRDBParameterDeclarationQueryCanAddParameter<T>
    {

    }

    public interface IRDBParameterDeclarationQueryReady<T> : IRDBParameterDeclarationQueryCanAddParameter<T>
    {
        T EndParameterDeclaration();
    }

    public interface IRDBParameterDeclarationQueryCanAddParameter<T>
    {
        IRDBParameterDeclarationQueryReady<T> AddParameter(string name, RDBDataType type);
    }
}
