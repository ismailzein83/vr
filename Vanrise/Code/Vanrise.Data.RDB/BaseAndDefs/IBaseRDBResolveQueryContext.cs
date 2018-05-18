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

        Dictionary<string, Object> ParameterValues { get; }

        Dictionary<string, RDBDataType> OutputParameters { get; }

        Dictionary<string, RDBParameter> Parameters { get; }

        RDBParameter GetParameterWithValidate(string parameterName);

        int PrmIndex { get; set; }

        void AddParameterValue(string parameterName, Object value);

        string GenerateUniqueDBParameterName();


        void AddParameter(RDBParameter parameter);

        void AddOutputParameter(string parameterName, RDBDataType dataType);
    }

    public abstract class BaseRDBResolveQueryContext : IBaseRDBResolveQueryContext
    {
        BaseRDBDataProvider _dataProvider;
        Dictionary<string, Object> _parameterValues;
        IBaseRDBResolveQueryContext _parentContext;
        Dictionary<string, RDBDataType> _outputParameters;

        public BaseRDBResolveQueryContext(BaseRDBDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _outputParameters = new Dictionary<string, RDBDataType>();
            _parameterValues = new Dictionary<string,object>();
            this.Parameters = new Dictionary<string, RDBParameter>();
        }

        public BaseRDBResolveQueryContext(IBaseRDBResolveQueryContext parentContext)
        {
            _parentContext = parentContext;
            this._dataProvider = parentContext.DataProvider;
            this._outputParameters = parentContext.OutputParameters;
            this._parameterValues = parentContext.ParameterValues;
            this.Parameters = parentContext.Parameters;
        }

        public BaseRDBDataProvider DataProvider
        {
            get { return _dataProvider; }
        }

        public Dictionary<string, Object> ParameterValues
        {
            get { return _parameterValues; }
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

        public Dictionary<string, RDBDataType> OutputParameters
        {
            get { return _outputParameters; }
        }

        public void AddParameterValue(string parameterName, object value)
        {
            if (_parameterValues.ContainsKey(parameterName))
                throw new Exception(String.Format("Parameter '{0}' already exists", parameterName));
            _parameterValues.Add(parameterName, value);
        }

        int _prmIndex;

        public string GenerateUniqueDBParameterName()
        {
            return this.DataProvider.ConvertToDBParameterName(String.Concat("Prm_", this.PrmIndex++));
        }

        public void AddOutputParameter(string parameterName, RDBDataType dataType)
        {
            _outputParameters.Add(this.DataProvider.ConvertToDBParameterName(parameterName), dataType);
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
