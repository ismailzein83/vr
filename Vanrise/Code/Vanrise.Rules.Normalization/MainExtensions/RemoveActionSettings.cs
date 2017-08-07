using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public enum RemoveDirection { Before, After };
    public enum TextOccurrence { LastOccurence, FirstOccurence };

    public class RemoveActionSettings : NormalizeNumberActionSettings
    {
        public override Guid ConfigId { get { return new Guid("6DD13404-F488-4D59-A2A0-2135D3826B28"); } }
        public RemoveDirection RemoveDirection { get; set; }
        public string TextToRemove { get; set; }
        public TextOccurrence TextOccurrence { get; set; }
        public bool IncludingText { get; set; }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
