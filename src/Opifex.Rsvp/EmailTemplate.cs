using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Opifex.Rsvp
{
    public class EmailTemplate
    {
        public EmailTemplate(string templatePath)
        {
            using StreamReader reader = new StreamReader(templatePath);
            Template = reader.ReadToEnd();
        }

        public string Template { get; }

        public string MergeWith(string displayName) => Template.Replace("{{DisplayName}}", displayName);
    }
}
