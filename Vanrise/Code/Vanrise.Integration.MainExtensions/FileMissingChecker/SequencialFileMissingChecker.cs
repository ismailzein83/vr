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

        public long ResetSequenceNumber { get; set; }

        public override void CheckMissingFiles(ICheckMissingFilesContext context)
        { 
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrEmpty(FileNamePattern))
                errorMessages.Add("Empty File Name Pattern");

            if (string.IsNullOrEmpty(context.CurrentFileName))
                errorMessages.Add("Current File Name is Empty");

            if (errorMessages.Count > 0)
            {
                context.ErrorMessages = errorMessages.Count > 0 ? errorMessages : null;
                return;
            }

            if (string.IsNullOrEmpty(context.LastRetrievedFileName) || string.Compare(context.CurrentFileName, context.LastRetrievedFileName) == 0)
                return;

            List<SequencialFileNamePatternPart> fileNamePatternParts = BuildFileNamePatternParts();

            SequencialFileNameParts lastSequencialFileNameParts;
            SequencialFileNameParts newSequencialFileNameParts;
            if (!TryBuildFileNameParts(context.LastRetrievedFileName, fileNamePatternParts, out lastSequencialFileNameParts) ||
                !TryBuildFileNameParts(context.CurrentFileName, fileNamePatternParts, out newSequencialFileNameParts))
            {
                errorMessages.Add("Invalid Pattern");
                context.ErrorMessages = errorMessages;
                return;
            }

            HashSet<string> distinctFileNameParts = Vanrise.Common.ExtensionMethods.ToHashSet(fileNamePatternParts.Select(itm => itm.FileNamePart));
            FileNamePatternType patternType = GetFileNamePatternType(distinctFileNameParts);
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
            StringBuilder sb_previousConstant = new StringBuilder();

            while (!string.IsNullOrEmpty(fileNamePattern))
            {
                int startSeparatorIndex = fileNamePattern.IndexOf(PatternSeparatorAsString);
                if (startSeparatorIndex == -1)
                {
                    sb_previousConstant.Append(fileNamePattern);
                    AddPatternPartToList(fileNamePatternParts, sb_previousConstant.ToString(), fileNameIndex, sb_previousConstant.Length, true);
                    break;
                }

                string constantPart = fileNamePattern.Substring(0, startSeparatorIndex);
                sb_previousConstant.Append(constantPart);

                fileNamePattern = fileNamePattern.Substring(startSeparatorIndex + PatternSeparatorAsString.Length);
                int endSeparatorIndex = fileNamePattern.IndexOf(PatternSeparatorAsString);
                if (endSeparatorIndex == -1)
                {
                    sb_previousConstant.Append(string.Concat(PatternSeparatorAsString, fileNamePattern));
                    AddPatternPartToList(fileNamePatternParts, sb_previousConstant.ToString(), fileNameIndex, sb_previousConstant.Length, true);
                    break;
                }

                int patternPartLength;

                string patternPart = fileNamePattern.Substring(0, endSeparatorIndex);
                if (IsSequencePart(patternPart, out patternPartLength))
                    patternPart = SequencePatternPart;
                else
                    patternPartLength = patternPart.Length;

                if (allPatternParts.Contains(patternPart))
                {
                    if (sb_previousConstant.Length > 0)
                    {
                        AddPatternPartToList(fileNamePatternParts, sb_previousConstant.ToString(), fileNameIndex, sb_previousConstant.Length, true);
                        fileNameIndex += sb_previousConstant.Length;
                        sb_previousConstant = new StringBuilder();
                    }
                    AddPatternPartToList(fileNamePatternParts, patternPart, fileNameIndex, patternPartLength, false);
                    fileNameIndex += patternPartLength;

                    fileNamePattern = fileNamePattern.Substring(endSeparatorIndex + PatternSeparatorAsString.Length);
                }
                else
                {
                    sb_previousConstant.Append(string.Concat(PatternSeparatorAsString, patternPart));
                    fileNamePattern = fileNamePattern.Substring(patternPart.Length);
                }
            }

            return fileNamePatternParts.Count > 0 ? fileNamePatternParts : null;
        }

        private void AddPatternPartToList(List<SequencialFileNamePatternPart> fileNamePatternParts, string fileNamePart, int startIndex, int length, bool isConstant)
        {
            var sequencialFileNamePatternPart = new SequencialFileNamePatternPart() { FileNamePart = fileNamePart, StartIndex = startIndex, Length = length, IsConstant = isConstant };
            fileNamePatternParts.Add(sequencialFileNamePatternPart);
        }

        private bool IsSequencePart(string fileNamePatternPart, out int fileNamePatternPartLength)
        {
            fileNamePatternPartLength = -1;

            if (!fileNamePatternPart.StartsWith("S"))
                return false;

            string lengthAsString = fileNamePatternPart.Substring(1);
            if ((lengthAsString.Length != 1 && lengthAsString.Length != 2) || !Int32.TryParse(lengthAsString, out fileNamePatternPartLength))
                return false;

            return true;
        }

        private bool TryBuildFileNameParts(string fileName, List<SequencialFileNamePatternPart> fileNamePatternParts, out SequencialFileNameParts sequencialFileNameParts)
        {
            sequencialFileNameParts = new SequencialFileNameParts();

            foreach (var fileNamePatternPart in fileNamePatternParts)
            {
                if (fileNamePatternPart.IsConstant)
                {
                    string fileNameConstantPart = fileName.Substring(fileNamePatternPart.StartIndex, fileNamePatternPart.Length);
                    if (fileNamePatternPart.FileNamePart.CompareTo(fileNameConstantPart) != 0)
                        return false;
                }
                else
                {
                    string fileNamePart = fileNamePatternPart.FileNamePart;
                    int startIndex = fileNamePatternPart.StartIndex;
                    int length = fileNamePatternPart.Length;

                    if ((startIndex + length) > fileName.Length)
                        return false;

                    string fileNamePartValue = fileName.Substring(startIndex, length);
                    if (!TryBuildFileNamePart(sequencialFileNameParts, fileNamePart, fileNamePartValue))
                        return false;
                }
            }

            return true;
        }

        private bool TryBuildFileNamePart(SequencialFileNameParts fileNameParts, string fileNamePart, string fileNamePartValue)
        {
            long? value = ParseLongWithValidate(fileNamePartValue);
            long? monthValue = ParseMonthWithValidate(fileNamePartValue);

            if (!value.HasValue && !monthValue.HasValue)
                return false;

            switch (fileNamePart)
            {
                case LongYearPatternPart:
                    fileNameParts.YearPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = value.Value
                    };
                    break;

                case ShortYearPatternPart:
                    fileNameParts.YearPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = value.Value
                    };
                    break;

                case LongMonthPatternPart:
                    if (monthValue == -1)
                        return false;

                    fileNameParts.MonthPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = monthValue.Value
                    };
                    break;

                case ShortMonthPatternPart:
                    fileNameParts.MonthPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = value.Value
                    };
                    break;

                case DayPatternPart:
                    fileNameParts.DayPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = value.Value
                    };
                    break;

                case HourPatternPart:
                    fileNameParts.HourPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = value.Value
                    };
                    break;

                case MinutePatternPart:
                    fileNameParts.MinutePart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = value.Value
                    };
                    break;

                case SecondPatternPart:
                    fileNameParts.SecondPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = value.Value
                    };
                    break;

                case SequencePatternPart:
                    fileNameParts.SequencePart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = value.Value
                    };
                    break;

                default:
                    return false;
            }

            return true;
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
            long sequenceDifference = newFileNameParts.SequencePart.Value - lastFileNameParts.SequencePart.Value;
            if (sequenceDifference == 1 || sequenceDifference == 0)
                return null;

            DateTime lastFileDateTime = BuildDateTimePart(lastFileNameParts);
            DateTime newFileDateTime = BuildDateTimePart(newFileNameParts);

            long maxNumber = GetMaxNumber(lastFileNameParts.SequencePart.ValueAsString.Length);

            long nbOfMissingFiles;
            if (sequenceDifference > 1)
                nbOfMissingFiles = sequenceDifference - 1;
            else
                nbOfMissingFiles = maxNumber - ResetSequenceNumber + sequenceDifference;

            DateTime missingFileDateTime = lastFileDateTime.CompareTo(newFileDateTime) <= 0 ? lastFileDateTime : newFileDateTime;
            IEnumerable<SequencialFileNamePatternPart> variablePatternParts = fileNamePatternParts.Where(itm => !itm.IsConstant);
            List<string> missingFileNames = new List<string>();

            for (long i = 1; i <= nbOfMissingFiles; i++)
            {
                long missingFileSequence = lastFileNameParts.SequencePart.Value + i;
                if (missingFileSequence > maxNumber)
                    missingFileSequence = ResetSequenceNumber + (missingFileSequence - maxNumber - 1);

                string missingFileName = BuildMissingFileName(variablePatternParts, missingFileDateTime, missingFileSequence);
                missingFileNames.Add(missingFileName);
            }

            return missingFileNames;
        }

        private List<string> GetSequenceMissingFiles(List<SequencialFileNamePatternPart> fileNamePatternParts, SequencialFileNameParts lastSequencialFileNameParts, SequencialFileNameParts newSequencialFileNameParts)
        {
            long sequenceDifference = newSequencialFileNameParts.SequencePart.Value - lastSequencialFileNameParts.SequencePart.Value;
            if (sequenceDifference == 1 || sequenceDifference == 0)
                return null;

            long maxNumber = GetMaxNumber(lastSequencialFileNameParts.SequencePart.ValueAsString.Length);

            long nbOfMissingFiles;
            if (sequenceDifference > 1)
                nbOfMissingFiles = sequenceDifference - 1;
            else
                nbOfMissingFiles = maxNumber - ResetSequenceNumber + sequenceDifference;

            List<string> missingFileNames = new List<string>();

            for (long i = 1; i <= nbOfMissingFiles; i++)
            {
                long missingFileSequence = lastSequencialFileNameParts.SequencePart.Value + i;
                if (missingFileSequence > maxNumber)
                    missingFileSequence = ResetSequenceNumber + (missingFileSequence - maxNumber - 1);

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

        private string BuildMissingFileName(IEnumerable<SequencialFileNamePatternPart> variablePatternParts, DateTime? missingFileDateTime, long? missingFileSequence)
        {
            string missingFileName = this.FileNamePattern;

            foreach (SequencialFileNamePatternPart variablePatternPart in variablePatternParts)
            {
                switch (variablePatternPart.FileNamePart)
                {
                    case LongYearPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(LongYearPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Year.ToString(), variablePatternPart.Length));
                        break;

                    case ShortYearPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(ShortYearPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Year.ToString(), variablePatternPart.Length));
                        break;

                    case ShortMonthPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(ShortMonthPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Month.ToString(), variablePatternPart.Length));
                        break;

                    case DayPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(DayPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Day.ToString(), variablePatternPart.Length));
                        break;

                    case HourPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(HourPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Hour.ToString(), variablePatternPart.Length));
                        break;

                    case MinutePatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(MinutePatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Minute.ToString(), variablePatternPart.Length));
                        break;

                    case SecondPatternPart:
                        missingFileName = missingFileName.Replace(GetFullPaternPart(SecondPatternPart), PadLeftZerosIfNecessary(missingFileDateTime.Value.Second.ToString(), variablePatternPart.Length));
                        break;

                    case SequencePatternPart:
                        missingFileName = missingFileName.Replace("#S" + variablePatternPart.Length + "#", PadLeftZerosIfNecessary(missingFileSequence.Value.ToString(), variablePatternPart.Length));
                        break;
                }
            }

            return missingFileName;
        }

        private long? ParseLongWithValidate(string valueAsString)
        {
            long value;
            if (!long.TryParse(valueAsString, out value))
                return null;

            return value;
        }

        private long? ParseMonthWithValidate(string monthAsString)
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

                default: return null;
            }
        }

        private string PadLeftZerosIfNecessary(string numberAsString, int patternPartLength)
        {
            if (numberAsString.Length < patternPartLength)
                numberAsString = numberAsString.PadLeft(patternPartLength, '0');

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
            public bool IsConstant { get; set; }
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