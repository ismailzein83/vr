using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.PackageFieldParser
{
    public class FixedLengthPackageFieldParser : PackageFieldParser
    {
        public override Guid ConfigId { get { return new Guid("96D779E4-E1E6-4E61-8BA1-112A68365751"); } }

        public int PackageLength { get; set; }

        public override int GetPackageLength(IPackageFieldParserGetLengthContext context)
        {
            return this.PackageLength;
        }
    }
}
