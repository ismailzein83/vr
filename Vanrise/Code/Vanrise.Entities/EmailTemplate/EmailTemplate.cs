using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class EmailTemplate
    {
        public int EmailTemplateId { get; set; }

        public string Name { get; set; }

        public string BodyTemplate { get; set; }

        public string SubjectTemplate { get; set; }

        public string Type { get; set; }

        public string GetParsedBodyTemplate(IEmailContext context)
        {
            if (string.IsNullOrEmpty(BodyTemplate))
                return null;

            return RazorEngine.Razor.Parse(BodyTemplate, context);
        }

        public string GetParsedSubjectTemplate(IEmailContext context)
        {
            if (string.IsNullOrEmpty(SubjectTemplate))
                return null;

            return RazorEngine.Razor.Parse(SubjectTemplate, context);
        }
    }
    public interface IEmailContext
    {
    }
}