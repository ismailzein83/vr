using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class TextManipulationActionSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetDescription();

        public abstract void Execute(ITextManipulationActionContext context, TextManipulationTarget target);
    }
}
