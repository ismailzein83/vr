using System;
using System.Activities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class CombineLists<T> : CodeActivity
    {

        [RequiredArgument]
        public InArgument<List<IEnumerable>> InputLists { get; set; }


        [RequiredArgument]
        public OutArgument<IEnumerable<T>> OutputList { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<IEnumerable> inputLists = this.InputLists.Get(context);

            List<T> outputList = new List<T>();

            foreach (IEnumerable list in inputLists)
            {
                foreach (T item in list)
                {
                    outputList.Add(item);
                }
            }
                

            this.OutputList.Set(context, outputList);
        }
    }
}
