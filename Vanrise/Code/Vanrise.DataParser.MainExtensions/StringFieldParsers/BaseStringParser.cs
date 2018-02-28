using System;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.StringFieldParsers
{
    public abstract class BaseStringParser
    {
        public abstract Guid ConfigId { get; }
        public abstract void Execute(IBaseStringParserContext context);
    }

    public interface IBaseStringParserContext
    {
        string FieldValue { get; }
        ParsedRecord Record { get; }
    }
}