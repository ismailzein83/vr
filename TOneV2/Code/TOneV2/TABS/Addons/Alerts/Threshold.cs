using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TABS.Addons.Utilities;
using System.Xml.Linq;

namespace TABS.Addons.Alerts
{
    [Serializable]
    public class ScheduleHours : ISerializable, ICloneable, IXmlSerializable
    {

        [NonSerialized]
        private BitVector32 _Hours = new BitVector32(0);
        public List<int> UsedHours
        {
            get
            {
                List<int> toReturn = new List<int>(24);
                for (int i = 0; i < 24; i++)
                {
                    if (_Hours[(int)Math.Pow(2, i)])
                        toReturn.Add(i);
                }
                return toReturn;
            }
        }
        private string GetHourDescription(int hour)
        {
            if (hour > 23 || hour < 0) throw new ArgumentOutOfRangeException("hour");
            if (hour < 12)
            {
                if (hour == 0) return "12 am";
                return string.Format("{0} am", hour);
            }
            return string.Format("{0} pm", hour);
        }
        public List<string> UsedHourDescriptions
        {
            get
            {
                List<string> toReturn = new List<string>(24);
                List<int> usedHours = UsedHours;
                foreach (int hour in usedHours)
                {
                    toReturn.Add(GetHourDescription(hour));
                }
                return toReturn;
            }
        }
        public ScheduleHours()
        {
        }
        #region Serialization / Deserialization
        public ScheduleHours(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : this()
        {
            _Hours = new BitVector32(info.GetInt32("_Hours"));

        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_Hours", this._Hours.Data);
        }
        #endregion
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            _Hours = new BitVector32(reader.ReadElementContentAsInt("Hours", ""));
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Hours");
            writer.WriteValue(_Hours.Data);
            writer.WriteEndElement();
        }

        #endregion

        public bool IsUsed(int hour)
        {
            return _Hours[(int)Math.Pow(2, hour)];
        }
        public void AddHour(int hour)
        {
            _Hours[(int)Math.Pow(2, hour)] = true;
        }
        public void AddHours(IEnumerable<int> hours)
        {
            foreach (int hour in hours)
                AddHour(hour);
        }

        public void RemoveHour(int hour)
        {
            _Hours[(int)Math.Pow(2, hour)] = false;
        }
        public void Clear()
        {
            _Hours = new BitVector32(0);
        }

        #region ICloneable Members

        public object Clone()
        {
            ScheduleHours clone = new ScheduleHours();
            clone._Hours = this._Hours;
            return clone;
        }

        #endregion

        public override bool Equals(object obj)
        {
            ScheduleHours other = obj as ScheduleHours;
            if (other == null) return false;
            return this._Hours.Equals(other._Hours);
        }
        public override int GetHashCode()
        {
            return _Hours.GetHashCode();
        }

    }
    public enum ThresholdType
    {
        Undefined,
        Min_Attempts,
        Max_Attempts,
        Min_ASR,
        Max_ASR,
        Min_ACD,
        Max_ACD,
        Min_Duration,
        Max_Duration,
        PerCall_Duration,
        Complex,
        Min_PDD,
        Max_PDD,
        Max_NER,
        Min_NER
    }
    [Serializable]
    public class Threshold : ICloneable, IXmlSerializable
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(GeneralAlertCriteria));

        public ThresholdType Type { get; set; }
        private object _Value;
        public object Value
        {
            get { return _Value; }
            set
            {
                if (this.Type == ThresholdType.Undefined) throw new Exception("Type is undefined. Set Type first.");
                bool mustBeInt = false, mustBeString = false;
                switch (Type)
                {
                    case ThresholdType.Min_Attempts:
                    case ThresholdType.Max_Attempts:
                        mustBeInt = true;
                        mustBeString = false;
                        break;
                    case ThresholdType.Complex:
                        mustBeInt = false;
                        mustBeString = true;
                        break;
                }
                bool isInt = false;
                try
                {
                    decimal valueInDecimal = Convert.ToDecimal(value);
                    isInt = (valueInDecimal - Convert.ToInt32(value)) == 0;
                }
                catch
                {
                    isInt = false;
                }
                bool isString = (value is string);
                bool invalidValue = (mustBeInt && !isInt) || (mustBeString && !isString);
                if (invalidValue) throw new ArgumentException("Invalid value. Expected a different type.", "Value");
                _Value = value;
            }
        }
        public string ProgressExpression { get; set; }
        public SerializableDictionary<int, decimal?> LastCrossingZoneValues { get; set; }
        /// <summary>
        /// The hours for this threshold to cause an alert
        /// </summary>
        public ScheduleHours Hours { get; set; }
        public string Name { get; set; }

        public Threshold(ThresholdType type, decimal value, string name, ScheduleHours hours)
        {
            Hours = new ScheduleHours();
            LastCrossingZoneValues = new SerializableDictionary<int, decimal?>();
            Type = type;
            Value = value;
            Hours = hours;
            Name = name;
        }
        public Threshold(ThresholdType type, string complexExpression, string progressExpression, string name, ScheduleHours hours)
        {
            Hours = new ScheduleHours();
            LastCrossingZoneValues = new SerializableDictionary<int, decimal?>();
            Type = type;
            Hours = hours;
            Name = name;
            Value = complexExpression;
            ProgressExpression = progressExpression;
        }
        private Threshold()
        {
        }

        private object EvaluateExpression(string expression, Type type)
        {
            try
            {
                RPNParser parser = new RPNParser();
                System.Collections.ArrayList arrExpr = parser.GetPostFixNotation(expression, type, false);
                return parser.EvaluateRPN(arrExpr, type, null);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Expression is not correctly formatted and cannot be evaluated. Expression: {0}", expression), ex);
                return null;
            }
        }

        private bool EvaluateThresholdExpression(State state, DateTime current)
        {
            if (string.IsNullOrEmpty((string)this.Value))
                log.Error("ComplexExpression not set on a Threshold of Type Complex.");
            Regex regex = new Regex("\\w+\\s*\\(\\s*\\d+(?:\\.\\d+)?\\s*\\)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(((string)Value).ToLower());
            StringBuilder sbExpression = new StringBuilder(((string)Value).ToLower());
            sbExpression.Replace(" and ", " && ").Replace(" or ", " || ");
            foreach (Match match in matches)
            {
                try
                {
                    string sThresholdType = match.Value.Substring(0, match.Value.IndexOf('(')).Trim();
                    ThresholdType thresholdType = (ThresholdType)Enum.Parse(typeof(ThresholdType), sThresholdType, true);
                    int indexOfLB = match.Value.IndexOf('(');
                    int indexOfRB = match.Value.IndexOf(')');
                    string sThresholdValue = match.Value.Substring(indexOfLB + 1, indexOfRB - (indexOfLB + 1));
                    decimal thresholdValue = decimal.Parse(sThresholdValue);
                    Threshold foundThreshold = new Threshold(thresholdType, thresholdValue, "", this.Hours);
                    bool crossed = foundThreshold.CheckStateCrossed(state, current);
                    sbExpression.Replace(match.Value, crossed.ToString());
                }
                catch (ArgumentException ex)
                {
                    log.Error(string.Format("Expression is not correctly formatted and cannot be evaluated. Expression: {0}", Value), ex);
                    return false;
                }
            }
            try
            {
                return Convert.ToBoolean(EvaluateExpression(sbExpression.ToString(), typeof(bool)));
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error Parsing Complex Threshold {0}.", this.Name + " " + this.Value), ex);
                return false;
            }
        }

        public decimal? GetProgressValue(State state)
        {
            if (string.IsNullOrEmpty(this.ProgressExpression))
                log.Error(string.Format("ProgressExpression not set. For Threshold {0}{1}",
                    string.IsNullOrEmpty(Name) ? Name + ": " : string.Empty, Value.ToString()));
            Regex regex = new Regex("\\w+", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(this.ProgressExpression.ToLower());
            StringBuilder sbExpression = new StringBuilder(this.ProgressExpression.ToLower());
            foreach (Match match in matches)
            {
                try
                {
                    decimal stateValue = 0;
                    switch (match.Value)
                    {
                        case "attempts": stateValue = state.Attempts; break;
                        case "acd": stateValue = state.Current_ACD; break;
                        case "asr": stateValue = state.Current_ASR; break;
                        case "pdd": stateValue = state.Average_PDD; break;
                        case "durations": stateValue = state.DurationsInSeconds; break;
                        case "max_duration": stateValue = state.MaxDurationInSeconds; break;
                        case "successful": stateValue = state.SuccessfulAttempts; break;
                        default: log.Error(string.Format("Use of not allowed Operand \"{0}\" in ProgressExpression.", match.Value));
                            break;
                    }
                    sbExpression.Replace(match.Value, stateValue.ToString("f4"));
                }
                catch (ArgumentException ex)
                {
                    log.Error(string.Format("Expression is not correctly formatted and cannot be evaluated. Expression: {0}", ProgressExpression), ex);
                    return null;
                }
            }
            return Convert.ToDecimal(EvaluateExpression(sbExpression.ToString(), typeof(decimal)));
        }
        public bool CheckStateCrossed(State state, DateTime current)
        {
            //no state means no crossing
            if (state == null) return false;
            if (Hours.UsedHours.Contains(current.Hour))
            {
                switch (Type)
                {
                    case ThresholdType.Min_Attempts: return state.Attempts <= Convert.ToInt32(Value);
                    case ThresholdType.Max_Attempts: return state.Attempts >= Convert.ToInt32(Value);
                    case ThresholdType.Min_ASR: return state.Current_ASR <= Convert.ToDecimal(Value);
                    case ThresholdType.Max_ASR: return state.Current_ASR >= Convert.ToDecimal(Value);
                    case ThresholdType.Min_ACD: return state.Current_ACD <= Convert.ToDecimal(Value);
                    case ThresholdType.Max_ACD: return state.Current_ACD >= Convert.ToDecimal(Value);
                    case ThresholdType.Min_Duration: return (state.DurationsInSeconds / 60) <= Convert.ToDecimal(Value);
                    case ThresholdType.Max_Duration: return (state.DurationsInSeconds / 60) >= Convert.ToDecimal(Value);
                    case ThresholdType.PerCall_Duration: return (state.MaxDurationInSeconds / 60) >= Convert.ToDecimal(Value);
                    case ThresholdType.Min_PDD: return state.Average_PDD <= Convert.ToDecimal(Value);
                    case ThresholdType.Max_PDD: return state.Average_PDD >= Convert.ToDecimal(Value);
                    case ThresholdType.Complex:
                        return EvaluateThresholdExpression(state, current);
                }
            }
            return false;
        }
        public override string ToString()
        {
            return string.Format("{0} = {1}: {2}", Type.ToString(), Name, Value.ToString());
        }


        public override bool Equals(object obj)
        {
            Threshold other = obj as Threshold;
            if (obj == null) return false;
            return (this.Type.Equals(other.Type) && this.Value.Equals(other.Value) && this.Hours.Equals(other.Hours));

        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() + Value.GetHashCode() + Hours.GetHashCode();
        }

        #region ICloneable Members

        public object Clone()
        {
            Threshold clone = new Threshold();
            clone._Value = this._Value;
            clone.Hours = (ScheduleHours)this.Hours.Clone();
            clone.LastCrossingZoneValues = this.LastCrossingZoneValues;
            clone.Name = this.Name;
            clone.ProgressExpression = this.ProgressExpression;
            clone.Type = this.Type;
            clone.Value = this.Value;
            return clone;
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        static   XmlSerializer dicSerializer = new XmlSerializer(typeof(SerializableDictionary<int, decimal?>), null, new Type[0], new XmlRootAttribute("LastCrossingZoneValues"), string.Empty);
        static  XmlSerializer hoursSerializer = new XmlSerializer(typeof(ScheduleHours), null, new Type[0], new XmlRootAttribute("Hours"), string.Empty);
        public void ReadXml(System.Xml.XmlReader reader)
        {
  
            //XmlSerializer dicSerializer = new XmlSerializer(typeof(SerializableDictionary<int, decimal?>), null, new Type[0], new XmlRootAttribute("LastCrossingZoneValues"), string.Empty);
            //XmlSerializer hoursSerializer = new XmlSerializer(typeof(ScheduleHours), null, new Type[0], new XmlRootAttribute("Hours"), string.Empty);
            XElement thresholdXML = XElement.Load(reader);

            if (!thresholdXML.HasElements || thresholdXML.IsEmpty)
                return;

            Type = (ThresholdType)int.Parse(thresholdXML.Element("Type").Value);
            Value = Type.Equals(ThresholdType.Complex) ? thresholdXML.Element("Value").Value : (object)Decimal.Parse(thresholdXML.Element("Value").Value);
            ProgressExpression = thresholdXML.Element("ProgressExpression").Value;
            Name = thresholdXML.Element("Name").Value;
            LastCrossingZoneValues = (SerializableDictionary<int, decimal?>)dicSerializer
                                                 .Deserialize(thresholdXML.Element("LastCrossingZoneValues").CreateReader());
            Hours = (ScheduleHours)hoursSerializer.Deserialize(thresholdXML.Element("Hours").CreateReader());

            //XmlSerializer dicSerializer = new XmlSerializer(typeof(SerializableDictionary<int, decimal?>), null, new Type[0], new XmlRootAttribute("LastCrossingZoneValues"), string.Empty);
            //XmlSerializer hoursSerializer = new XmlSerializer(typeof(ScheduleHours), null, new Type[0], new XmlRootAttribute("Hours"), string.Empty);

            //bool wasEmpty = reader.IsEmptyElement;
            //reader.Read();
            //if (wasEmpty)
            //    return;
            //Type = (ThresholdType)reader.ReadElementContentAsInt("Type", "");
            //if (Type == ThresholdType.Complex)
            //    Value = reader.ReadElementContentAsString("Value", "");
            //else
            //    Value = reader.ReadElementContentAsDecimal("Value", "");

            //ProgressExpression = reader.ReadElementContentAsString("ProgressExpression", "");
            //LastCrossingZoneValues = (SerializableDictionary<int, decimal?>)dicSerializer.Deserialize(reader);
            //Hours = (ScheduleHours)hoursSerializer.Deserialize(reader);
            //Name = reader.ReadElementContentAsString("Name", "");
            //reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("", "");

            //XmlSerializer dicSerializer = new XmlSerializer(typeof(SerializableDictionary<int, decimal?>), null, new Type[0], new XmlRootAttribute("LastCrossingZoneValues"), string.Empty);
            //XmlSerializer hoursSerializer = new XmlSerializer(typeof(ScheduleHours), null, new Type[0], new XmlRootAttribute("Hours"), string.Empty);

            writer.WriteElementString("Type", ((int)Type).ToString("D"));
            writer.WriteElementString("Value", Value.ToString());
            writer.WriteElementString("ProgressExpression", ProgressExpression);

            dicSerializer.Serialize(writer, LastCrossingZoneValues, xmlnsEmpty);
            hoursSerializer.Serialize(writer, Hours, xmlnsEmpty);
            writer.WriteElementString("Name", Name);
        }

        #endregion
    }
}
