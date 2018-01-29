(function (app) {

    'use strict';

    GenericBESerialNumberService.$inject = ['VRModalService'];

    function GenericBESerialNumberService(VRModalService) {
        return ({
            openSerialNumberPatternHelper: openSerialNumberPatternHelper
        });

        function openSerialNumberPatternHelper(onSetSerialNumberPattern, context) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSetSerialNumberPattern = onSetSerialNumberPattern;
            };
            var parameter = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Runtime/MainExtensions/OnBeforeInsertHandler/GenericBESN/Templates/SNPatternHelperEditor.html', parameter, modalSettings);
        }

    };

    app.service('VR_GenericData_GenericBESerialNumberService', GenericBESerialNumberService);

})(app);
