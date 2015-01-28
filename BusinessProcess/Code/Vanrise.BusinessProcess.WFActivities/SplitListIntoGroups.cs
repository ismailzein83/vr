using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class SplitListIntoGroups<T> : CodeActivity
    {
        
        [RequiredArgument]
        public InArgument<List<T>> List { get; set; }

        [RequiredArgument]
        public InArgument<int> GroupSize { get; set; }

        [RequiredArgument]
        public OutArgument<List<List<T>>> Groups { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<T> list = this.List.Get(context);
            int groupSize = this.GroupSize.Get(context);
            int addedItems = 0;
            List<List<T>> groups = new List<List<T>>();
            while (addedItems < list.Count)
            {
                List<T> group = list.GetRange(addedItems, Math.Min(groupSize, list.Count - addedItems));
                addedItems += group.Count;
                groups.Add(group);
            }
            this.Groups.Set(context, groups);
        }
    }
}
