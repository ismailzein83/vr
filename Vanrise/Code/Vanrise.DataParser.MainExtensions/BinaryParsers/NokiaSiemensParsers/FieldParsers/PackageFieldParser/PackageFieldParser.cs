using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers.PackageFieldParser
{
    public abstract class PackageFieldParser
    {
        public abstract Guid ConfigId { get; }  

        public BinaryFieldParserSettings FieldParser { get; set; }

        public abstract int GetPackageLength(IPackageFieldParserGetLengthContext context);
    }
}