using System;
using System.ComponentModel;

namespace Vanrise.Rules.Normalization.MainExtensions
{
    public enum RemoveDirection
    {
        [Description("After")]
        After = 0,
        [Description("Before")]
        Before = 1,
    };

    public enum TextOccurrence
    {
        [Description("First Occurrence")]
        FirstOccurrence = 0,
        [Description("Last Occurrence")]
        LastOccurrence = 1
    };

    public class RemoveActionSettings : NormalizeNumberActionSettings
    {
        public override Guid ConfigId { get { return new Guid("6DD13404-F488-4D59-A2A0-2135D3826B28"); } }
        public RemoveDirection RemoveDirection { get; set; }
        public string TextToRemove { get; set; }
        public TextOccurrence TextOccurrence { get; set; }
        public bool IncludingText { get; set; }

        public override string GetDescription()
        {
            string removeDirection = Vanrise.Common.Utilities.GetEnumDescription(RemoveDirection);
            string textOccurrence = Vanrise.Common.Utilities.GetEnumDescription(TextOccurrence);
            string includingText = IncludingText ? "including text" : string.Empty;

            return string.Format("Remove {0} '{1}' from {2} {3}", removeDirection, TextToRemove, textOccurrence, includingText);
        }

        public override void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target)
        {
            if (string.IsNullOrEmpty(target.PhoneNumber))
                return;

            if (string.IsNullOrEmpty(TextToRemove))
                return;

            int index = -1;

            switch (TextOccurrence)
            {
                case TextOccurrence.FirstOccurrence:
                    index = target.PhoneNumber.IndexOf(TextToRemove);
                    break;

                case TextOccurrence.LastOccurrence:
                    index = target.PhoneNumber.LastIndexOf(TextToRemove);
                    break;

                default: throw new NotSupportedException(string.Format("TextOccurrence {0} not supported.", TextOccurrence));
            }

            if (index >= 0)
            {
                switch (this.RemoveDirection)
                {
                    case RemoveDirection.Before:
                        int startingIndex = IncludingText ? index + TextToRemove.Length : index;
                        target.PhoneNumber = target.PhoneNumber.Substring(startingIndex);
                        break;

                    case RemoveDirection.After:
                        int length = IncludingText ? index : index + TextToRemove.Length;
                        target.PhoneNumber = target.PhoneNumber.Substring(0, length);
                        break;

                    default: throw new NotSupportedException(string.Format("RemoveDirection {0} not supported.", RemoveDirection));
                }
            }
        }
    }
}