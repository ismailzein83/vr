using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public interface IBaseRDBResolveQueryContext
    {
        BaseRDBQueryContext QueryContext { get; }

        BaseRDBDataProvider DataProvider { get; }

        Dictionary<string, string> TableAliases { get; }

        Dictionary<string, Object> ParameterValues { get; }

        Dictionary<string, RDBDataType> OutputParameters { get; }

        int PrmIndex { get; set; }

        int TableAliasIndex { get; set; }

        string GetTableAlias(IRDBTableQuerySource table);

        string GenerateTableAliasIfNotExists(IRDBTableQuerySource table);

        void AddParameterValue(string parameterName, Object value);

        string GenerateParameterName();

        void AddOutputParameter(string parameterName, RDBDataType dataType);
    }

    public abstract class BaseRDBResolveQueryContext : IBaseRDBResolveQueryContext
    {
        BaseRDBDataProvider _dataProvider;
        Dictionary<string, string> _tableAliases;
        Dictionary<string, Object> _parameterValues;
        IBaseRDBResolveQueryContext _parentContext;
        Dictionary<string, RDBDataType> _outputParameters;

        public BaseRDBResolveQueryContext(BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
        {
            this.QueryContext = queryContext;
            _dataProvider = dataProvider;
            _tableAliases = new Dictionary<string,string>();
            _outputParameters = new Dictionary<string, RDBDataType>();
            _parameterValues = parameterValues;
        }

        public BaseRDBResolveQueryContext(IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : this(parentContext.QueryContext, parentContext.DataProvider, parentContext.ParameterValues)
        {
            _parentContext = parentContext;
            this._outputParameters = parentContext.OutputParameters;
            if (newQueryScope)
                this._tableAliases = new Dictionary<string, string>();
            else
                this._tableAliases = parentContext.TableAliases;
        }

        public BaseRDBDataProvider DataProvider
        {
            get { return _dataProvider; }
        }

        public Dictionary<string, string> TableAliases
        {
            get { return _tableAliases; }
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

        public int TableAliasIndex
        {
            get { return this._parentContext != null ? this._parentContext.TableAliasIndex : _tableAliasIndex; }
            set
            {
                if (this._parentContext != null)
                    this._parentContext.TableAliasIndex++;
                else
                    _tableAliasIndex++;
            }
        }

        public Dictionary<string, RDBDataType> OutputParameters
        {
            get { return _outputParameters; }
        }

        public string GetTableAlias(IRDBTableQuerySource table)
        {
            string tableAlias;
            _tableAliases.TryGetValue(table.GetUniqueName(), out tableAlias);
            return tableAlias;
        }

        public void AddParameterValue(string parameterName, object value)
        {
            if (_parameterValues.ContainsKey(parameterName))
                throw new Exception(String.Format("Parameter '{0}' already exists", parameterName));
            _parameterValues.Add(parameterName, value);
        }

        int _prmIndex;

        public string GenerateParameterName()
        {
            return String.Concat(_dataProvider.ParameterNamePrefix, "Prm_", this.PrmIndex++);
        }

        int _tableAliasIndex;
        public string GenerateTableAliasIfNotExists(IRDBTableQuerySource table)
        {
            string tableQuerySourceUniqueName = table.GetUniqueName();
            string tableAlias;
            if (!_tableAliases.TryGetValue(tableQuerySourceUniqueName, out tableAlias))
            {
                tableAlias = string.Concat("Table_", this.TableAliasIndex++);
                _tableAliases.Add(tableQuerySourceUniqueName, tableAlias);
            }
            return tableAlias;
        }

        public BaseRDBQueryContext QueryContext
        {
            get;
            private set;
        }


        public void AddOutputParameter(string parameterName, RDBDataType dataType)
        {
            _outputParameters.Add(String.Concat(this.DataProvider.ParameterNamePrefix, parameterName), dataType);
        }
    }
}
