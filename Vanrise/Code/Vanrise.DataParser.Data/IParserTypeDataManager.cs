using System.Collections.Generic;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.Data
{
    public interface IParserTypeDataManager:IDataManager
    {
        List<ParserType> GetParserTypes();

        bool AreParserTypesUpdated(ref object updateHandle);

        bool Update(Entities.ParserType parserType);

        bool Insert(ParserType parserType);
    }
}