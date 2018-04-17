
app.service('VR_DataParser_ParserTypeService', ['VRModalService',
    function (VRModalService) {  
        return ({
            editParserType: editParserType,
            addParserType: addParserType
        });

        function editParserType(parserTypeId, onParserTypeUpdated) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onParserTypeUpdated = onParserTypeUpdated;
            };
            var parameters = {
                parserTypeId: parserTypeId
            };
            VRModalService.showModal('/Client/Modules/VR_DataParser/Elements/ParserType/Views/ParserTypeEditor.html', parameters, settings);
        }

        function addParserType(onParserTypeAdded) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onParserTypeAdded = onParserTypeAdded;
            };
            var parameters = {};
            VRModalService.showModal('/Client/Modules/VR_DataParser/Elements/ParserType/Views/ParserTypeEditor.html', parameters, settings);
        }
    }]);
