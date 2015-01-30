using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewCallGenService
{
    public class CallQueue
    {
        private Queue<string> numbers = new Queue<string>();

        public void Enqueue(string number)
        {
            numbers.Enqueue(number);
        }

        public string Dequeue()
        {
            return numbers.Dequeue();
        }

        public bool IsEmpty
        {
            get
            {
                return numbers.Count == 0;
            }
        }
    }
}
