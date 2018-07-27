using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public interface IBaseRDBResolveQueryContext
    {
        BaseRDBDataProvider DataProvider { get; }

        Dictionary<string, RDBParameter> Parameters { get; }

        RDBParameter GetParameterWithValidate(string parameterName);

        int PrmIndex { get; set; }

        string GenerateUniqueDBParameterName();


        void AddParameter(RDBParameter parameter);
    }

    public abstract class BaseRDBResolveQueryContext : IBaseRDBResolveQueryContext
    {
        BaseRDBDataProvider _dataProvider;
        IBaseRDBResolveQueryContext _parentContext;

        public BaseRDBResolveQueryContext(BaseRDBDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            this.Parameters = new Dictionary<string, RDBParameter>();
        }

        public BaseRDBResolveQueryContext(IBaseRDBResolveQueryContext parentContext)
        {
            _parentContext = parentContext;
            this._dataProvider = parentContext.DataProvider;
            this.Parameters = parentContext.Parameters;
        }

        public BaseRDBDataProvider DataProvider
        {
            get { return _dataProvider; }
        }

        public int PrmIndex
        {
            get { return this._parentContext != null ? this._parentContext.PrmIndex : _prmIndex; }
            set
            {
                if (this._parentContext != null)
                    this._parentContext.PrmIndex++;
                else
                    _prmIndex++;
            }
        }

        int _prmIndex;

        public string GenerateUniqueDBParameterName()
        {
            return this.DataProvider.ConvertToDBParameterName(String.Concat("Prm_", this.PrmIndex++));
        }

        public Dictionary<string, RDBParameter> Parameters
        {
            get;
            private set;
        }

        public void AddParameter(RDBParameter parameter)
        {
            if (this.Parameters.ContainsKey(parameter.Name))
                throw new Exception(String.Format("Parameter Name '{0}' already exists", parameter.Name));
            this.Parameters.Add(parameter.Name, parameter);
        }




        public RDBParameter GetParameterWithValidate(string parameterName)
        {
            RDBParameter parameter;
            if (!this.Parameters.TryGetValue(parameterName, out parameter))
                throw new Exception(String.Format("Parameter '{0}' not found", parameterName));
            return parameter;
        }
    }

    public enum RDBParameterDirection { Declared = 0, In = 1 }

    public class RDBParameter
    {
        public string Name { get; set; }

        public string DBParameterName { get; set; }

        public RDBDataType Type { get; set; }

        public int? Size { get; set; }

        public int? Precision { get; set; }

        public RDBParameterDirection Direction { get; set; }

        public Object Value { get; set; }
    }
}
