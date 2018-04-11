using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;
using Vanrise.Common.Business;
namespace Vanrise.DataParser.Business
{
    public class ParserTypeConfigsManager
    {
        public IEnumerable<ParserTypeExtendedSettingsConfig> GetParserTypeTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<ParserTypeExtendedSettingsConfig>(ParserTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<HexTLVRecordReadersConfig> GetRecordeReaderTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<HexTLVRecordReadersConfig>(HexTLVRecordReadersConfig.EXTENSION_TYPE);
        }
        public IEnumerable<HexTLVTagValueParserConfig> GetTagValueParserTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<HexTLVTagValueParserConfig>(HexTLVTagValueParserConfig.EXTENSION_TYPE);
        }
        public IEnumerable<BinaryRecordReadersConfig> GetBinaryRecordReaderTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<BinaryRecordReadersConfig>(BinaryRecordReadersConfig.EXTENSION_TYPE);
        }
    }
}
