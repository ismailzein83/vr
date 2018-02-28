using System;
using System.Collections.Generic;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.RecordParsers
{
    public class PackageRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("8396B755-D0AE-4D96-B3C5-D48B1CBF230E"); } }
        public string RecordType { get; set; }
        public Dictionary<int, PackageFieldParser> Packages { get; set; }
        public int PackageTagLength { get; set; }
        public int PackageLengthByteLength { get; set; }
        public List<CompositeFieldsParser> CompositeFieldsParsers { get; set; }

        public override void Execute(IBinaryRecordParserContext context)
        {
            ParsedRecord parsedRecord = context.CreateRecord(this.RecordType, null);

            BinaryParserHelper.ReadBlockFromStream(context.RecordStream, PackageTagLength, (packageTag) =>
            {
                int packageId = ParserHelper.GetInt(packageTag.Value, 0, PackageTagLength);
                
                PackageFieldParser packageFieldParser;
                if (this.Packages != null && this.Packages.TryGetValue(packageId, out packageFieldParser))
                {
                    int packageLengthByteLengthRead = 0;

                    int? calculatedPackageLength = packageFieldParser.PackageLength;
                    if (!calculatedPackageLength.HasValue)
                    {
                        packageLengthByteLengthRead = PackageLengthByteLength;
                        BinaryParserHelper.ReadBlockFromStream(context.RecordStream, PackageLengthByteLength, (packageLength) =>
                        {
                            calculatedPackageLength = ParserHelper.GetInt(packageLength.Value, 0, PackageLengthByteLength);
                        }, true);
                    }

                    int previouslyReadBytes = packageLengthByteLengthRead + this.PackageTagLength;

                    BinaryParserHelper.ReadBlockFromStream(context.RecordStream, calculatedPackageLength.Value - previouslyReadBytes, (packageContent) =>
                    {
                        BinaryParserHelper.ExecuteFieldParser(packageFieldParser.FieldParser, parsedRecord, packageContent.Value);
                    }, true);
                }
                else
                {
                    int calculatedPackageLengthToSkip = 0;

                    BinaryParserHelper.ReadBlockFromStream(context.RecordStream, PackageLengthByteLength, (packageLength) =>
                    {
                        calculatedPackageLengthToSkip = ParserHelper.GetInt(packageLength.Value, 0, PackageLengthByteLength);
                    }, true);
                    int previouslyReadBytes = PackageLengthByteLength + this.PackageTagLength;

                    BinaryParserHelper.ReadBlockFromStream(context.RecordStream, calculatedPackageLengthToSkip - previouslyReadBytes, (packageContent) =>
                    {

                    }, true);
                }
            });

            if (this.CompositeFieldsParsers != null)
            {
                foreach (var compositeFieldsParser in this.CompositeFieldsParsers)
                {
                    compositeFieldsParser.Execute(new CompositeFieldsParserContext
                    {
                        Record = parsedRecord,
                        FileName = context.FileName
                    });
                }
            }
            context.OnRecordParsed(parsedRecord);
        }
    }

    public class PackageFieldParser
    {
        public BinaryFieldParserSettings FieldParser { get; set; }

        public int PackageTagLength { get; set; }

        public int? PackageLength { get; set; }
    }
}