using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.FileMissingChecker
{
    //Should Be Renamed to SequencialFileMissingChecker
    public class SequenceFileMissingChecker : Vanrise.Integration.Entities.FileMissingChecker
    {
        public override Guid ConfigId { get { return new Guid("FA37168F-25B8-44B2-8D27-CA0DD3E3265E"); } }

        public string FileNamePattern { get; set; }

        public override void CheckMissingFiles(ICheckMissingFilesContext context)
        {
            List<SequencialFileNamePatternPart> sequencialFileNamePatternParts = BuildFileNamePatternParts();
            HashSet<string> distinctFileNameParts = sequencialFileNamePatternParts.Select(itm => itm.FileNamePart).ToHashSet();

            SequencialFileNameParts lastSequencialFileNameParts = BuildFileNameParts(context.LastRetrievedFileName);
            SequencialFileNameParts newSequencialFileNameParts = BuildFileNameParts(context.CurrentFileName);
        }

        private List<SequencialFileNamePatternPart> BuildFileNamePatternParts()
        {
            throw new NotImplementedException();
        }

        private SequencialFileNameParts BuildFileNameParts(string fileName)
        {
            SequencialFileNameParts sequencialFileNameParts = new SequencialFileNameParts();

            while (true)
            {
                //Searching for fileNamePart and fileNamePartValue
                string fileNamePart = "YY";
                string fileNamePartValue = "18";

                BuildFileNamePart(sequencialFileNameParts, fileNamePart, fileNamePartValue);
            }
        }

        private void BuildFileNamePart(SequencialFileNameParts fileNameParts, string fileNamePart, string fileNamePartValue)
        {
            switch (fileNamePart)
            {
                case "YY":
                    fileNameParts.YearPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseIntWithValidate(fileNamePartValue)
                    };
                    break;

                case "MMM":
                    fileNameParts.MonthPart = new SequencialFileNamePart()
                    {
                        FileNamePart = fileNamePart,
                        ValueAsString = fileNamePartValue,
                        Value = ParseMonthWithValidate(fileNamePartValue)
                    };
                    break;
            }
        }

        private string BuildMissingFileName(List<SequencialFileNamePatternPart> fileNamePatternParts, DateTime missingFileDateTime, long missingFileSequence)
        {
            string missingFileName = this.FileNamePattern;

            foreach(SequencialFileNamePatternPart fileNamePatternPart in fileNamePatternParts)
            {
                switch (fileNamePatternPart.FileNamePart)
                {
                    case "YY":
                        missingFileName = missingFileName.Replace("#YY#", missingFileDateTime.Year.ToString());
                        break;
                }
            }

            return missingFileName;
        }

        private int ParseIntWithValidate(string valueAsString)
        {
            int result;
            if (!int.TryParse(valueAsString, out result))
                throw new Exception(string.Format("invalid integer format {0}", valueAsString));

            return result;
        }

        private int ParseMonthWithValidate(string monthAsString)
        {
            switch(monthAsString)
            {
                case "JAN":
                case "jan":
                    return 1;

                default:
                    throw new NotSupportedException(string.Format("monthAsString {0} not supported.", monthAsString));
            }
        }

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
            public int Value { get; set; }
        }

        #endregion
    }
}