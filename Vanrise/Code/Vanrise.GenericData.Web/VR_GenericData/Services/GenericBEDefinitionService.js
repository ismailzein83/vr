(function (app) {

    'use strict';

    GenericBEDefinitionService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function GenericBEDefinitionService(VRModalService, VRNotificationService, UtilsService, VRUIUtilsService) {
        var columnModalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/ColumnDefinitionEditor.html";
        var viewModalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/ViewDefinitionEditor.html";
        return ({
            addGenericBEColumnDefinition: addGenericBEColumnDefinition,
            editGenericBEColumnDefinition: editGenericBEColumnDefinition,
            addGenericBEViewDefinition: addGenericBEViewDefinition,
            editGenericBEViewDefinition: editGenericBEViewDefinition
        });

        function addGenericBEColumnDefinition(onGenericBEColumnDefinitionAdded, context) {
            var parameters = {
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEColumnDefinitionAdded = onGenericBEColumnDefinitionAdded;
            };

            VRModalService.showModal(columnModalPath, parameters, settings);
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
            VRModalService.showModal(columnModalPath, parameters, settings);
        }


        function addGenericBEViewDefinition(onGenericBEViewDefinitionAdded) {
            var parameters = {
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEViewDefinitionAdded = onGenericBEViewDefinitionAdded;
            };

            VRModalService.showModal(viewModalPath, parameters, settings);
        }

        function editGenericBEViewDefinition(onGenericBEViewDefinitionUpdated, viewDefinition) {
            var parameters = {
                viewDefinition: viewDefinition
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEViewDefinitionUpdated = onGenericBEViewDefinitionUpdated;
            };
            VRModalService.showModal(viewModalPath, parameters, settings);
        }


    };

    app.service('VR_GenericData_GenericBEDefinitionService', GenericBEDefinitionService);

})(app);