using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodeIterator<T> where T : ICode
    {
        Dictionary<string, T> _codesByValue = new Dictionary<string,T>();
        int _minLength = int.MaxValue;
        int _maxLength = 0;
        public CodeIterator(IEnumerable<T> saleCodes)
        {
            foreach(var code in saleCodes)
            {
                if(!_codesByValue.ContainsKey(code.Code))
                {
                    int codeLength = code.Code.Length;
                    if (codeLength < _minLength)
                        _minLength = codeLength;
                    if (codeLength > _maxLength)
                        _maxLength = codeLength;
                    _codesByValue.Add(code.Code, code);
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
    }
}
