using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.RDBTests.Common
{
    public static class UTAssert
    {
        public static void ObjectsAreEqual<T>(T obj1, T obj2)
        {
            Assert.AreEqual(obj1, obj2);
        }

        public static void ObjectsAreSimilar<T>(T obj1, T obj2)
        {
            string serializedObj1 = Serializer.Serialize(obj1);
            string serializedObj2 = Serializer.Serialize(obj2);
            Assert.AreEqual(serializedObj1, serializedObj2);
        }

        public static void NotNullObject(Object obj)
        {
            Assert.IsNotNull(obj);
        }
    }
}
