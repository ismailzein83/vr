using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Integration.Entities;
using Vanrise.Common;
using System.Text;

namespace Vanrise.Integration.MainExtensions.FileMissingChecker
{
    public enum FileNamePatternType { DateTime = 0, Sequence = 1, DateTimeSequence = 2 }

    public class SequencialFileMissingChecker : Vanrise.Integration.Entities.FileMissingChecker
    {
        const string PatternSeparatorAsString = "#";
        const string LongYearPatternPart = "YYYY";
        const string ShortYearPatternPart = "YY";
        const string LongMonthPatternPart = "MMM";
        const string ShortMonthPatternPart = "MM";
        const string DayPatternPart = "DD";
        const string HourPatternPart = "hh";
        const string MinutePatternPart = "mm";
        const string SecondPatternPart = "ss";
        const string SequencePatternPart = "Sequence";

        HashSet<string> allPatternParts = new HashSet<string>() { LongYearPatternPart, ShortYearPatternPart, LongMonthPatternPart, ShortMonthPatternPart,
            DayPatternPart, HourPatternPart, MinutePatternPart,  SecondPatternPart , SequencePatternPart};

        HashSet<string> dateTimePatternParts = new HashSet<string>() { LongYearPatternPart, ShortYearPatternPart, LongMonthPatternPart, ShortMonthPatternPart,
            DayPatternPart, HourPatternPart, MinutePatternPart,  SecondPatternPart };

        public override Guid ConfigId { get { return new Guid("FA37168F-25B8-44B2-8D27-CA0DD3E3265E"); } }

        public string FileNamePattern { get; set; }

        private TimeSpan _fileImportTimeInterval = new TimeSpan(0, 15, 0);
        public TimeSpan FileImportTimeInterval
        {
            get
            {
                return _fileImportTimeInterval;
            }
            set
            {
                _fileImportTimeInterval = value;
            }
        }

        public long SequenceSeed { get; set; } //ask fidaa about name 

        public override void CheckMissingFiles(ICheckMissingFilesContext context)
        {
            List<string> missingFileNames = new List<string>();

            List<SequencialFileNamePatternPart> fileNamePatternParts = BuildFileNamePatternParts();
            if (fileNamePatternParts == null)
                return;

            HashSet<string> distinctFileNameParts = Vanrise.Common.ExtensionMethods.ToHashSet(fileNamePatternParts.Select(itm => itm.FileNamePart));

            FileNamePatternType patternType = GetFileNamePatternType(distinctFileNameParts);

            SequencialFileNameParts lastSequencialFileNameParts = BuildFileNameParts(context.LastRetrievedFileName, fileNamePatternParts);
            SequencialFileNameParts newSequencialFileNameParts = BuildFileNameParts(context.CurrentFileName, fileNamePatternParts);

            lastSequencialFileNameParts.ThrowIfNull("lastSequencialFileNameParts");
            newSequencialFileNameParts.ThrowIfNull("newSequencialFileNameParts");

            switch (patternType)
            {
                case FileNamePatternType.DateTimeSequence: context.MissingFileNames = GetDateTimeSequenceMissingFiles(fileNamePatternParts, lastSequencialFileNameParts, newSequencialFileNameParts); return;
                case FileNamePatternType.Sequence: context.MissingFileNames = GetSequenceMissingFiles(fileNamePatternParts, lastSequencialFileNameParts, newSequencialFileNameParts); return;
                case FileNamePatternType.DateTime: context.MissingFileNames = GetDateTimeMissingFiles(fileNamePatternParts, lastSequencialFileNameParts, newSequencialFileNameParts); return;
            }
        }

        #region Private Methods

        private List<SequencialFileNamePatternPart> BuildFileNamePatternParts()
        {
            string fileNamePattern = this.FileNamePattern;
            if (string.IsNullOrEmpty(fileNamePattern))
                return null;

            List<SequencialFileNamePatternPart> fileNamePatternParts = new List<SequencialFileNamePatternPart>();

            int fileNameIndex = 0;

            while (!string.IsNullOrEmpty(fileNamePattern))
            {
                int startSeparatorIndex = fileNamePattern.IndexOf(PatternSeparatorAsString);
                if (startSeparatorIndex == -1)
                    break;

                fileNameIndex += startSeparatorIndex;
                fileNamePattern = fileNamePattern.Substring(startSeparatorIndex + 1);

                int endSeparatorIndex = fileNamePattern.IndexOf(PatternSeparatorAsString);
                if (endSeparatorIndex == -1)
                    break;

                int fileNamePatternPartLength;
                string fileNamePatternPart = fileNamePattern.Substring(0, endSeparatorIndex);
                fileNamePattern = fileNamePattern.Substring(endSeparatorIndex + 1);

                if (fileNamePatternPart.StartsWith("S"))
                {
                    string lengthAsString = fileNamePatternPart.Substring(1);
                    if ((lengthAsString.Length != 1 && lengthAsString.Length != 2) || !Int32.TryParse(lengthAsString, out fileNamePatternPartLength))
                        fileNamePatternPartLength = fileNamePatternPart.Length;
                    else
                        fileNamePatternPart = SequencePatternPart;
                }
                else
                {
                    fileNamePatternPartLength = fileNamePatternPart.Length;
                }

                if (allPatternParts.Contains(fileNamePatternPart))
                {
                    var sequencialFileNamePatternPart = new SequencialFileNamePatternPart() { FileNamePart = fileNamePatternPart, StartIndex = fileNameIndex, Length = fileNamePatternPartLength };
                    fileNamePatternParts.Add(sequencialFileNamePatternPart);
                    fileNameIndex += fileNamePatternPartLength;
                }
                else
                {
                    fileNamePattern = string.Concat(PatternSeparatorAsString, fileNamePattern);
                    fileNameIndex += fileNamePatternPartLength + 1;
                }
            }

            return fileNamePatternParts.Count > 0 ? fileNamePatternParts : null;
        }

        private SequencialFileNameParts BuildFileNameParts(string fileName, List<SequencialFileNamePatternPart> fileNamePatternParts)
        {
            SequencialFileNameParts sequencialFileNameParts = new SequencialFileNameParts();

            foreach (var fileNamePatternPart in fileNamePatternParts)
            {
                string fileNamePart = fileNamePatternPart.FileNamePart;
                string fileNamePartValue = fileName.Substring(fileNamePatternPart.StartIndex, fileNamePatternPart.Length);

                BuildFileNamePart(sequencialFileNameParts, fileNamePart, fileNamePartValue);
            }

            return sequencialFileNameParts;
        }

        private void BuildFileNamePart(SequencialFileNameParts fileNameParts, string fileNamePart, string fileNamePartValue)
        {
            switch (fileNamePart)
            {
                case LongYearPatternPart:
                    fileNameParts.YearPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseLongWithValidate(fileNamePartValue)
                    };
                    break;

                case ShortYearPatternPart:
                    fileNameParts.YearPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseLongWithValidate(fileNamePartValue)
                    };
                    break;

                case ShortMonthPatternPart:
                    fileNameParts.MonthPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseLongWithValidate(fileNamePartValue)
                    };
                    break;

                case LongMonthPatternPart:
                    fileNameParts.MonthPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseMonthWithValidate(fileNamePartValue)
                    };
                    break;

                case DayPatternPart:
                    fileNameParts.DayPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseLongWithValidate(fileNamePartValue)
                    };
                    break;

                case HourPatternPart:
                    fileNameParts.HourPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseLongWithValidate(fileNamePartValue)
                    };
                    break;

                case MinutePatternPart:
                    fileNameParts.MinutePart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseLongWithValidate(fileNamePartValue)
                    };
                    break;

                case SecondPatternPart:
                    fileNameParts.SecondPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseLongWithValidate(fileNamePartValue)
                    };
                    break;

                case SequencePatternPart:
                    fileNameParts.SequencePart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseLongWithValidate(fileNamePartValue)
                    };
                    break;

                default:
                    fileNameParts = null;
                    break;

            }
        }

        private string BuildMissingFileName(List<SequencialFileNamePatternPart> fileNamePatternParts, DateTime? missingFileDateTime, long? missingFileSequence)
        {
            string missingFileName = this.FileNamePattern;

            foreach (SequencialFileNamePatternPart fileNamePatternPart in fileNamePatternParts)
            {
                switch (fileNamePatternPart.FileNamePart)
                {
                    case LongYearPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(LongYearPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Year.ToString(), fileNamePatternPart.Length));
                        break;

                    case ShortYearPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(ShortYearPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Year.ToString(), fileNamePatternPart.Length));
                        break;

                    case ShortMonthPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(ShortMonthPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Month.ToString(), fileNamePatternPart.Length));
                        break;

                    case DayPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(DayPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Day.ToString(), fileNamePatternPart.Length));
                        break;

                    case HourPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(HourPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Hour.ToString(), fileNamePatternPart.Length));
                        break;

                    case MinutePatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(MinutePatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Minute.ToString(), fileNamePatternPart.Length));
                        break;

                    case SecondPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(SecondPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Second.ToString(), fileNamePatternPart.Length));
                        break;

                    case SequencePatternPart:
                        missingFileName = missingFileName.Replace("#S" + fileNamePatternPart.Length + "#", PadLeftZerosIfNecessary(missingFileSequence.Value.ToString(), fileNamePatternPart.Length));
                        break;
                }
            }

            return missingFileName;
        }

        private FileNamePatternType GetFileNamePatternType(HashSet<string> distinctFileNameParts)
        {
            bool isDataTime = distinctFileNameParts.Any(itm => dateTimePatternParts.Contains(itm));
            bool isSequence = distinctFileNameParts.Contains(SequencePatternPart);
            bool isDataTimeSequence = isDataTime && isSequence;

            return isDataTimeSequence ? FileNamePatternType.DateTimeSequence : isDataTime ? FileNamePatternType.DateTime : FileNamePatternType.Sequence;
        }

        private List<string> GetDateTimeSequenceMissingFiles(List<SequencialFileNamePatternPart> fileNamePatternParts, SequencialFileNameParts lastFileNameParts, SequencialFileNameParts newFileNameParts)
        {
            List<string> missingFileNames = new List<string>();

            long maxNumber = GetMaxNumber(lastFileNameParts.SequencePart.ValueAsString.Length);

            long sequenceDifference = newFileNameParts.SequencePart.Value - lastFileNameParts.SequencePart.Value;
            if (sequenceDifference == 1)
                return null;

            DateTime lastFileDateTime = BuildDateTimePart(lastFileNameParts);
            DateTime newFileDateTime = BuildDateTimePart(newFileNameParts);

            DateTime missingFileDateTime = lastFileDateTime.CompareTo(newFileDateTime) <= 0 ? lastFileDateTime : newFileDateTime;

            long nbOfMissingFiles;
            if (sequenceDifference > 1)
                nbOfMissingFiles = sequenceDifference - 1;
            else
                nbOfMissingFiles = maxNumber - SequenceSeed + sequenceDifference;

            for (long i = 1; i <= nbOfMissingFiles; i++)
            {
                long missingFileSequence = lastFileNameParts.SequencePart.Value + i;
                if (missingFileSequence > maxNumber)
                    missingFileSequence = SequenceSeed + (missingFileSequence - maxNumber - 1);

                string missingFileName = BuildMissingFileName(fileNamePatternParts, missingFileDateTime, missingFileSequence);
                missingFileNames.Add(missingFileName);
            }

            return missingFileNames;
        }

        private List<string> GetSequenceMissingFiles(List<SequencialFileNamePatternPart> fileNamePatternParts, SequencialFileNameParts lastSequencialFileNameParts, SequencialFileNameParts newSequencialFileNameParts)
        {
            List<string> missingFileNames = new List<string>();

            long maxNumber = GetMaxNumber(lastSequencialFileNameParts.SequencePart.ValueAsString.Length);

            long sequenceDifference = newSequencialFileNameParts.SequencePart.Value - lastSequencialFileNameParts.SequencePart.Value;
            if (sequenceDifference == 1)
                return null;

            long nbOfMissingFiles;
            if (sequenceDifference > 1)
                nbOfMissingFiles = sequenceDifference - 1;
            else
                nbOfMissingFiles = maxNumber - SequenceSeed + sequenceDifference;

            for (long i = 1; i <= nbOfMissingFiles; i++)
            {
                long missingFileSequence = lastSequencialFileNameParts.SequencePart.Value + i;
                if (missingFileSequence > maxNumber)
                    missingFileSequence = SequenceSeed + (missingFileSequence - maxNumber - 1);

                string missingFileName = BuildMissingFileName(fileNamePatternParts, null, missingFileSequence);
                missingFileNames.Add(missingFileName);
            }

            return missingFileNames;
        }

        private List<string> GetDateTimeMissingFiles(List<SequencialFileNamePatternPart> fileNamePatternParts, SequencialFileNameParts lastSequencialFileNameParts, SequencialFileNameParts newSequencialFileNameParts)
        {
            List<string> missingFileNames = new List<string>();

            DateTime lastFileDateTime = BuildDateTimePart(lastSequencialFileNameParts);
            DateTime newFileDateTime = BuildDateTimePart(newSequencialFileNameParts);

            TimeSpan currentTimeSpan = newFileDateTime - lastFileDateTime;
            DateTime missingFileDateTime = lastFileDateTime;

            for (TimeSpan t = FileImportTimeInterval; t < currentTimeSpan; t += FileImportTimeInterval)
            {
                missingFileDateTime = missingFileDateTime.Add(FileImportTimeInterval);
                string missingFileName = BuildMissingFileName(fileNamePatternParts, missingFileDateTime, null);
                missingFileNames.Add(missingFileName);
            }

            return missingFileNames;
        }

        private long ParseLongWithValidate(string valueAsString)
        {
            long result;
            if (!long.TryParse(valueAsString, out result))
                throw new Exception(string.Format("invalid integer format {0}", valueAsString));

            return result;
        }

        private long ParseMonthWithValidate(string monthAsString)
        {
            switch (monthAsString.ToLower())
            {
                case "jan": return 1;

                case "feb": return 2;

                case "mar": return 3;

                case "apr": return 4;

                case "may": return 5;

                case "jun": return 6;

                case "jul": return 7;

                case "aug": return 8;

                case "sep": return 9;

                case "oct": return 10;

                case "nov": return 11;

                case "dec": return 12;

                default:
                    throw new NotSupportedException(string.Format("month: {0} not supported.", monthAsString));
            }
        }

        private string PadLeftZerosIfNecessary(string numberAsString, int patternPartLength)
        {
            if (numberAsString.Length < patternPartLength)
                numberAsString = numberAsString.PadLeft(patternPartLength - numberAsString.Length + 1, '0');

            return numberAsString;
        }

        private string GetFullPaternPart(string paternPart)
        {
            return string.Concat(PatternSeparatorAsString, paternPart, PatternSeparatorAsString);
        }

        private long GetMaxNumber(int numberLength)
        {
            if (numberLength == 0)
                return 0;

            StringBuilder sb_MaxNumber = new StringBuilder();

            for (int i = 0; i < numberLength; i++)
                sb_MaxNumber.Append("9");

            return long.Parse(sb_MaxNumber.ToString());
        }

        private DateTime BuildDateTimePart(SequencialFileNameParts sequencialFileNameParts)
        {
            int year = sequencialFileNameParts.YearPart != null ? (int)sequencialFileNameParts.YearPart.Value : 2000;
            int month = sequencialFileNameParts.MonthPart != null ? (int)sequencialFileNameParts.MonthPart.Value : 1;
            int day = sequencialFileNameParts.DayPart != null ? (int)sequencialFileNameParts.DayPart.Value : 1;
            int hour = sequencialFileNameParts.HourPart != null ? (int)sequencialFileNameParts.HourPart.Value : 0;
            int minute = sequencialFileNameParts.MinutePart != null ? (int)sequencialFileNameParts.MinutePart.Value : 0;
            int second = sequencialFileNameParts.SecondPart != null ? (int)sequencialFileNameParts.SecondPart.Value : 0;

            return new DateTime(year, month, day, hour, minute, second);
        }

        #endregion

        #region Private Classes
        private class SequencialFileNamePatternPart
        {
            public string FileNamePart { get; set; }
            public int StartIndex { get; set; }
            public int Length { get; set; }
        }

        private class SequencialFileNameParts
        {
            public SequencialFileNamePart YearPart { get; set; }
            public SequencialFileNamePart MonthPart { get; set; }
            public SequencialFileNamePart DayPart { get; set; }
            public SequencialFileNamePart HourPart { get; set; }
            public SequencialFileNamePart MinutePart { get; set; }
            public SequencialFileNamePart SecondPart { get; set; }
            public SequencialFileNamePart SequencePart { get; set; }
        }

        private class SequencialFileNamePart
        {
            public string FileNamePart { get; set; }
            public string ValueAsString { get; set; }
            public long Value { get; set; }
        }

        #endregion
    }
}