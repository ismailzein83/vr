using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class Alert
    {
        public long ID { get; set; }
        public DateTime Created { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public AlertLevel Level { get; set; }
        public string Tag { get; set; }
        public AlertProgress Progress { get; set; }
        public bool IsVisible { get; set; }
    }

    public class AlertView : Alert
    {
        string[] criteriaSeparator = new string[] { "[Filters]", "[State]", "[Threshold]", "[FirstAttempt]", "[LastAttempt]" };

        Regex fieldParser = new Regex(@"([*]-[*]){0,1}\s*(?<key>\w+)[:]\s*(?<value>[^*,]*)?\s*",
            RegexOptions.ExplicitCapture
            | RegexOptions.Compiled
            | RegexOptions.Singleline);
        public AlertView(Alert alert)
        {
            this.ID = alert.ID;
            this.Created = alert.Created;
            this.IsVisible = alert.IsVisible;
            this.Level = alert.Level.ToString();
            this.Progress = alert.Progress.ToString();
            this.Source = alert.Source;
            this.Tag = alert.Tag;

            try
            {
                string filters = string.Empty;
                string[] parts = alert.Description.Split(criteriaSeparator, StringSplitOptions.RemoveEmptyEntries);
                string tempFilter = parts[0];
                if (!tempFilter.Contains("Zone"))
                {
                    filters = parts[0].Insert(0, "*-* Zone: Unassigned ");
                }
                else filters = parts[0];
                string state = parts[1];
                string threshold = parts[4];

                try
                {
                    //Parse Filters
                    var matches = fieldParser.Matches(filters);
                    System.Text.StringBuilder sbFilters = new System.Text.StringBuilder("<table><tr class='CRITERIA'>");
                    foreach (Match match in matches)
                    {
                        sbFilters.AppendFormat("<th>{0}</th><td>{1}</td>", match.Groups["key"].Value, match.Groups["value"].Value);
                    }
                    sbFilters.Append("</tr></table>");
                    this.Filters = sbFilters.ToString();

                    //Parse State
                    matches = fieldParser.Matches(state);
                    foreach (Match match in matches)
                    {
                        switch (match.Groups["key"].Value)
                        {
                            case "Durations": this.DurationsInMinutes = decimal.Parse(match.Groups["value"].Value); break;
                            case "Attempts": this.Attempts = int.Parse(match.Groups["value"].Value); break;
                            case "Successful": this.SuccessfulAttempts = int.Parse(match.Groups["value"].Value); break;
                            case "ASR": this.ASR = decimal.Parse(match.Groups["value"].Value); break;
                            case "ACD": this.ACD = decimal.Parse(match.Groups["value"].Value); break;
                            case "Zone": this.Zone = match.Groups["value"].Value.ToString(); break;
                        }
                    }

                    //Threshold
                    string[] thresholdParts = threshold.Trim().Split(new string[] { "*-*", "=" }, StringSplitOptions.RemoveEmptyEntries);
                    this.Threshold = thresholdParts[0].Replace("_", " ").Trim();
                    this.ThresholdValue = thresholdParts[1];
                }
                catch
                {
                    this.Description = alert.Description;
                }
            }
            catch
            {
                this.Description = alert.Description;
            }

        }
        public string Filters { get; set; }
        public decimal? ASR { get; protected set; }
        public decimal? ACD { get; protected set; }
        public decimal? DurationsInMinutes { get; protected set; }
        public int? Attempts { get; protected set; }
        public int? SuccessfulAttempts { get; protected set; }
        public string Zone { get; protected set; }
        public int Hour { get; protected set; }
        public string SpecialDescription { get { return Description == null || Description.StartsWith("[Filters]") ? string.Empty : Description; } }
        public string Threshold { get; protected set; }
        public string ThresholdValue { get; protected set; }
        public string Level { get; set; }
        public string Progress { get; set; }
    }
}
