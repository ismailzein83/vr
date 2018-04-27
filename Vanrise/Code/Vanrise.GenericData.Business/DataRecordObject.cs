using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class DataRecordObject
    {
        #region Local Variables

        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();
        Guid? _dataRecordTypeId;
        string _dataRecordTypeName;
        Type _recordRuntimeType;

        Dictionary<string, dynamic> _fieldValues;

        dynamic _object;
        IDataRecordFiller _objectAsFiller;

        #endregion

        #region ctor

        public DataRecordObject(Guid dataRecordTypeId, dynamic obj)
        {
            _dataRecordTypeId = dataRecordTypeId;
            SetObjectFromDynamic(obj);
        }

        public DataRecordObject(Guid dataRecordTypeId, Dictionary<string, dynamic> fieldValues)
        {
            _dataRecordTypeId = dataRecordTypeId;
            this._fieldValues = fieldValues;
        }

        public DataRecordObject(string dataRecordTypeName, dynamic obj)
        {
            _dataRecordTypeName = dataRecordTypeName;
            SetObjectFromDynamic(obj);
        }

        public DataRecordObject(string dataRecordTypeName, Dictionary<string, dynamic> fieldValues)
        {
            _dataRecordTypeName = dataRecordTypeName;
            this._fieldValues = fieldValues;
        }

        #endregion

        #region Public

        public dynamic Object
        {
            get
            {
                if (_object == null)
                {
                    _fieldValues.ThrowIfNull("_fieldValues");
                    Type recordRuntimeType = GetRecordRuntimeTypeWithValidate();
                    _object = Activator.CreateInstance(recordRuntimeType, _fieldValues);
                }
                return _object;
            }
        }

        public IDataRecordFiller ObjectAsFiller
        {
            get
            {
                if (_objectAsFiller == null)
                    _objectAsFiller = ExtensionMethods.CastWithValidate<IDataRecordFiller>(this.Object, "this.Object");
                return _objectAsFiller;
            }
        }

        public dynamic GetFieldValue(string fieldName)
        {
            dynamic fieldValue;
            if (_fieldValues != null && _fieldValues.TryGetValue(fieldName, out fieldValue))//if the field is a formula, it might not be available in the Dictionary. therefore we should get the value from the compiled object
                return fieldValue;
            else
                return this.ObjectAsFiller.GetFieldValue(fieldName);
        }

        public Guid DataRecordTypeId
        {
            get
            {
                if (!_dataRecordTypeId.HasValue)
                {
                    if (!String.IsNullOrEmpty(_dataRecordTypeName))
                    {
                        var dataRecordType = s_dataRecordTypeManager.GetDataRecordType(_dataRecordTypeName);
                        dataRecordType.ThrowIfNull("dataRecordType", _dataRecordTypeName);
                        _dataRecordTypeId = dataRecordType.DataRecordTypeId;
                    }
                    else
                    {
                        throw new NullReferenceException("_dataRecordTypeId");
                    }

                }
                return _dataRecordTypeId.Value;
            }
        }

        #endregion

        #region Private Methods

        private void SetObjectFromDynamic(dynamic obj)
        {
            if (obj is IDataRecordFiller)
            {
                this._object = obj;
            }
            else
            {
                this._object = Activator.CreateInstance(GetRecordRuntimeTypeWithValidate(), obj);
            }
        }

        private Type GetRecordRuntimeTypeWithValidate()
        {
            if (_recordRuntimeType == null)
            {
                if (_dataRecordTypeId.HasValue)
                {
                    _recordRuntimeType = s_dataRecordTypeManager.GetDataRecordRuntimeType(_dataRecordTypeId.Value);
                    _recordRuntimeType.ThrowIfNull("recordRuntimeType", _dataRecordTypeId.Value);
                }
                else if (!String.IsNullOrEmpty(_dataRecordTypeName))
                {
                    _recordRuntimeType = s_dataRecordTypeManager.GetDataRecordRuntimeType(_dataRecordTypeName);
                    _recordRuntimeType.ThrowIfNull("recordRuntimeType", _dataRecordTypeName);
                }
                else
                {
                    throw new NullReferenceException("_dataRecordTypeId");
                }
            }
            return _recordRuntimeType;
        }

        #endregion
    }
}
