using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;
using Vanrise.Common.Business;
using Vanrise.DataParser.Entities.HexTLV;
using Vanrise.DataParser.BinaryParser;
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
        public IEnumerable<BaseStringConfig> GetBaseStringParserTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<BaseStringConfig>(BaseStringConfig.EXTENSION_TYPE);
        }
        public IEnumerable<CompositeFieldsConfig> GetCompositeFieldsReaderTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<CompositeFieldsConfig>(CompositeFieldsConfig.EXTENSION_TYPE);
        }

        public IEnumerable<PackageFieldsConfig> GetPackageFieldsReaderTemplateConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<PackageFieldsConfig>(PackageFieldsConfig.EXTENSION_TYPE);
        }
    }
}
