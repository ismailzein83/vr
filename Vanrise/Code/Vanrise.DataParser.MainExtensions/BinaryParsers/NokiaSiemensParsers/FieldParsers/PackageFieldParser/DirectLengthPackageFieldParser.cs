using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.PackageFieldParser
{
    public class DirectLengthPackageFieldParser : PackageFieldParser
    {
        public override Guid ConfigId { get { return new Guid("A102ED18-FDD9-4E82-BE06-8A10B1CACE06"); } }

        public override int GetPackageLength(IPackageFieldParserGetLengthContext context)
        {
            context.PackageLengthByteLengthRead = context.PackageLengthByteLength.Value;

            int calculatedPackageLength = 0;
            BinaryParserHelper.ReadBlockFromStream(context.Stream, context.PackageLengthByteLength.Value, (packageLength) =>
            {
                calculatedPackageLength = ParserHelper.GetInt(packageLength.Value, 0, context.PackageLengthByteLength.Value);
            }, true);
            return calculatedPackageLength;
        }
    }
}