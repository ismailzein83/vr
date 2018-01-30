(function (app) {

    'use strict';

    GenericBEDefinitionService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function GenericBEDefinitionService(VRModalService, VRNotificationService, UtilsService, VRUIUtilsService) {
        var modalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/ColumnDefinitionEditor.html";

        return ({
            addGenericBEColumnDefinition: addGenericBEColumnDefinition,
            editGenericBEColumnDefinition: editGenericBEColumnDefinition
        });

        function addGenericBEColumnDefinition(onGenericBEColumnDefinitionAdded, context) {
            var parameters = {
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEColumnDefinitionAdded = onGenericBEColumnDefinitionAdded;
            };

            VRModalService.showModal(modalPath, parameters, settings);
        }

        function editGenericBEColumnDefinition(onGenericBEColumnDefinitionUpdated, columnDefinition, context) {
            var parameters = {
                columnDefinition: columnDefinition,
                context: context
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEColumnDefinitionUpdated = onGenericBEColumnDefinitionUpdated;
            };
            VRModalService.showModal(modalPath, parameters, settings);
        }


    };

    app.service('VR_GenericData_GenericBEDefinitionService', GenericBEDefinitionService);

})(app);