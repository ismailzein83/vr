using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class MainProxy
    {
        public void InvokeMethod(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has occurred. Exception details {0}", ex.Message);
                throw ex;
            }
        }

        public T InvokeMehtod<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has occurred. Exception details {0}", ex.Message);
                throw ex;
            }
        }

        public void Divide(int x, int y)
        {
            int z = x / y;
        }

        public int GetTemprature(string date)
        {
            DateTime.Parse(date);
            return 30;
        }
    }
}
