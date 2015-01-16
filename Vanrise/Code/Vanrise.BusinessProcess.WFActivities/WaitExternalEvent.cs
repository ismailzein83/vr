using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Vanrise.BusinessProcess.WFActivities
{

    public sealed class WaitExternalEvent<T> : NativeActivity
    {
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }

        public OutArgument<T> EventData { get; set; }

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }
               
        
        protected override void Execute(NativeActivityContext context)
        {
            context.CreateBookmark(this.BookmarkName.Get(context), BookmarkResumed);
        }

        private void BookmarkResumed(NativeActivityContext context,
              Bookmark bookmark,
              object value)
        {
            //Console.WriteLine("Bookmark resumed with '{0}'.", value);
            context.SetValue(this.EventData, value != null ? (T)value : default(T));
        }
    }
}
