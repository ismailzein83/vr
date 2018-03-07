using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.PackageFieldParser
{
    public interface IPackageFieldParserGetLengthContext
    {
        Stream Stream { get; }

        int? PackageLengthByteLength { get; }

        int PackageLengthByteLengthRead { set; }
    }

    public class PackageFieldParserGetLengthContext : IPackageFieldParserGetLengthContext
    {
        public Stream Stream { get; set; }

        public int? PackageLengthByteLength { get; set; }

        public int PackageLengthByteLengthRead { get; set; }
    }
}
