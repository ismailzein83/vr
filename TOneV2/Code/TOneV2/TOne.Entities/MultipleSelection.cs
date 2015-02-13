using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public enum MultipleSelectionOption { All, AllExceptItems, OnlyItems }

    public class MultipleSelection <T>
    {
        public MultipleSelectionOption SelectionOption { get; set; }

        public List<T> SelectedValues { get; set; }
    }
}
