app.service('VR_DataParser_ParserTypeConfigService', ['VRModalService',
    function (VRModalService) {        
        
        function addTagRecordReader(onTagRecordReaderAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onTagRecordReaderAdded = onTagRecordReaderAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/Templates/TageRecordReaderEditor.html', parameters, settings);
        }
        function editTagRecordReader(tagRecordTypeEntity, onEditTagRecordReader, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onEditTagRecordReader = onEditTagRecordReader;
            };
            var parameters = {
                tagRecordTypeEntity: tagRecordTypeEntity.Entity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/Templates/TageRecordReaderEditor.html', parameters, settings);
        }

        function addHexTLVTagType(onHexTLVTagTypeAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onHexTLVTagTypeAdded = onHexTLVTagTypeAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/TagValueParserEditor.html', parameters, settings);
        }
        function editHexTLVTagType(tagTypeEntity, onEditHexTLVTagType, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onEditHexTLVTagType = onEditHexTLVTagType;
            };
            var parameters = {
                tagTypeEntity: tagTypeEntity.Entity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/VR_DataParser/Elements/HexTLV/Directives/MainExtensions/HexTLV/MainExtensions/TagRecordReader/MainExtensions/TagValueParser/Templates/TagValueParserEditor.html', parameters, settings);
        }

        return ({
            addTagRecordReader: addTagRecordReader,
            editTagRecordReader: editTagRecordReader,
            addHexTLVTagType: addHexTLVTagType,
            editHexTLVTagType: editHexTLVTagType
        });
    }]);
