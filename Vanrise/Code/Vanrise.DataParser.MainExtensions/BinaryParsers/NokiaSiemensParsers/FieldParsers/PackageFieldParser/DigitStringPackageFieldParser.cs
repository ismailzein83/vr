using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.PackageFieldParser
{
    public class DigitStringPackageFieldParser : PackageFieldParser
    {
        public override Guid ConfigId { get { return new Guid("E3599F8A-B3DC-4E9B-970A-39DC23E0E7A9"); } }

        public override int GetPackageLength(IPackageFieldParserGetLengthContext context)
        {
            context.PackageLengthByteLengthRead = context.PackageLengthByteLength.Value;

            int calculatedPackageLength = 0;
            BinaryParserHelper.ReadBlockFromStream(context.Stream, context.PackageLengthByteLength.Value, (packageLength) =>
            {
                int numberOfDigits = ParserHelper.GetInt(packageLength.Value, 0, context.PackageLengthByteLength.Value);
                calculatedPackageLength = 2 + (int)((numberOfDigits + 1) / 2);
            }, true);
            return calculatedPackageLength;
        }
    }
}
