using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public enum MultipleSelectionOption { OnlyItems = 1, AllExceptItems = 2}

    public class MultipleSelection <T>
    {
        public MultipleSelectionOption SelectionOption { get; set; }

        public List<T> SelectedValues { get; set; }

        public bool Contains(T value)
        {
            switch(this.SelectionOption)
            {
                case MultipleSelectionOption.OnlyItems: return (SelectedValues != null && SelectedValues.Contains(value));
                case MultipleSelectionOption.AllExceptItems: return (SelectedValues == null || !SelectedValues.Contains(value));
            }
            return false;
        }
    }
}
