using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public abstract class VRCodeIterator<T>
    {
        protected abstract List<string> GetCodes(T codeObject);

        #region ctor/Local Variables

        VRDictionary<string, T> _codesByValue = new VRDictionary<string, T>();
        int _minLength = int.MaxValue;
        int _maxLength = 0;

        #endregion

        #region Public Methods
        public VRCodeIterator(IEnumerable<T> codeObjects)
        {
            foreach (var codeObj in codeObjects)
            {
                var codes = GetCodes(codeObj);
                if(codes != null)
                {
                    foreach(var code in codes)
                    {
                        if (!_codesByValue.ContainsKey(code))
                        {
                            int codeLength = code.Length;
                            if (codeLength < _minLength)
                                _minLength = codeLength;
                            if (codeLength > _maxLength)
                                _maxLength = codeLength;
                            _codesByValue.Add(code, codeObj);
                        }
                    }
                }               
            }
        }
        public T GetLongestMatch(string phoneNumber)
        {
            if (phoneNumber == null)
                return default(T);

            string prefix = phoneNumber.Substring(0, Math.Min(_maxLength, phoneNumber.Length));
            while (prefix.Length >= _minLength)
            {
                T matchCode;
                if (_codesByValue.TryGetValue(prefix, out matchCode))
                    return matchCode;
                prefix = prefix.Substring(0, prefix.Length - 1);
            }
            return default(T);
        }
        public T GetExactMatch(string phoneNumber)
        {
            if (phoneNumber == null)
                return default(T);
            T matchCode;
            if (_codesByValue.TryGetValue(phoneNumber, out matchCode))
                return matchCode;
            return default(T);
        }
        #endregion     
    }
}
