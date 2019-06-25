using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PhoneNumberStatus
    {
        public static Guid Used = new Guid("1c67115b-2347-4f69-a04c-ab3e20a9b4cf");

        public static Guid Error = new Guid("571844c1-83c2-4e51-9873-df08a736ea68");

        public static Guid Free = new Guid("f6cab3e1-3cfb-4efd-80d1-9feea7c38da4");

        public static Guid Reserved = new Guid("01014c67-49f8-4959-b56e-d10aaaa21319");
    }
}