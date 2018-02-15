(function (app) {

    'use strict';

    GenericBEDefinitionService.$inject = ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function GenericBEDefinitionService(VRModalService, VRNotificationService, UtilsService, VRUIUtilsService) {
        var columnModalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/ColumnDefinitionEditor.html";
        var viewModalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/ViewDefinitionEditor.html";
        var tabModalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/GenericBETabContainerEditorController.html";
        var basicAdvancefilterModalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/BasicAdvancedFilterEditor.html";
        var actionModalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/ActionDefinitionEditor.html";
        var gridActionModalPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/GridActionDefinitionEditor.html";
        var conditionalHandlerPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/OnAfterSaveHandler/Templates/AfterSaveHandlerConditionalEditor.html";
        var conditionGroupPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/GenericBESaveCondition/MainExtensions/Templates/GenericBEConditionGroupEditor.html";
        var partDefinitionPath = "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/OnBeforeInsertHandler/Templates/SerialNumberPartDefinitionEditor.html";

        return ({
            addGenericBEColumnDefinition: addGenericBEColumnDefinition,
            editGenericBEColumnDefinition: editGenericBEColumnDefinition,
            addGenericBEViewDefinition: addGenericBEViewDefinition,
            editGenericBEViewDefinition: editGenericBEViewDefinition,
            addGenericBETabContainer: addGenericBETabContainer,
            editGenericBETabContainer: editGenericBETabContainer,
            addGenericBEBasicAdvanceFilter: addGenericBEBasicAdvanceFilter,
            editGenericBEBasicAdvanceFilter: editGenericBEBasicAdvanceFilter,
            addGenericBEActionDefinition: addGenericBEActionDefinition,
            editGenericBEActionDefinition: editGenericBEActionDefinition,
            addGenericBEGridActionDefinition: addGenericBEGridActionDefinition,
            editGenericBEGridActionDefinition: editGenericBEGridActionDefinition,
            addGenericBEConditionalHandler: addGenericBEConditionalHandler,
            editGenericBEConditionalHandler: editGenericBEConditionalHandler,
            addGenericBEConditionGroup: addGenericBEConditionGroup,
            editGenericBEConditionGroup: editGenericBEConditionGroup,
            addGenericBESerialNumberPart: addGenericBESerialNumberPart,
            editGenericBESerialNumberPart:editGenericBESerialNumberPart

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


        function addGenericBETabContainer(onTabContainerAdded, context) {
            var parameters = {
                context: context

            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onTabContainerAdded = onTabContainerAdded;
            };

            VRModalService.showModal(tabModalPath, parameters, settings);
        }

        function editGenericBETabContainer(onTabContainerUpdated, tabDefinition, context) {
            var parameters = {
                tabDefinition: tabDefinition,
                context: context

            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onTabContainerUpdated = onTabContainerUpdated;
            };
            VRModalService.showModal(tabModalPath, parameters, settings);
        }

        function addGenericBEBasicAdvanceFilter(onGenericBEFilterDefinitionAdded, context) {
            var parameters = {
                context: context

            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEFilterDefinitionAdded = onGenericBEFilterDefinitionAdded;
            };

            VRModalService.showModal(basicAdvancefilterModalPath, parameters, settings);
        }

        function editGenericBEBasicAdvanceFilter(onGenericBEFilterDefinitionUpdated, filterDefinition, context) {
            var parameters = {
                filterDefinition: filterDefinition,
                context: context

            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEFilterDefinitionUpdated = onGenericBEFilterDefinitionUpdated;
            };
            VRModalService.showModal(basicAdvancefilterModalPath, parameters, settings);
        }

        function addGenericBEActionDefinition(onGenericBEActionDefinitionAdded, context) {
            var parameters = {
                context: context

            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEActionDefinitionAdded = onGenericBEActionDefinitionAdded;
            };

            VRModalService.showModal(actionModalPath, parameters, settings);
        }

        function editGenericBEActionDefinition(onGenericBEActionDefinitionUpdated, actionDefinition, context) {
            var parameters = {
                actionDefinition: actionDefinition,
                context: context
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEActionDefinitionUpdated = onGenericBEActionDefinitionUpdated;
            };
            VRModalService.showModal(actionModalPath, parameters, settings);
        }


        
        function addGenericBEGridActionDefinition(onGenericBEGridActionDefinitionAdded, context) {
            var parameters = {
                context: context

            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEGridActionDefinitionAdded = onGenericBEGridActionDefinitionAdded;
            };

            VRModalService.showModal(gridActionModalPath, parameters, settings);
        }

        function editGenericBEGridActionDefinition(onGenericBEGridActionDefinitionUpdated, gridActionDefinition, context) {
            var parameters = {
                gridActionDefinition: gridActionDefinition,
                context: context
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEGridActionDefinitionUpdated = onGenericBEGridActionDefinitionUpdated;
            };
            VRModalService.showModal(gridActionModalPath, parameters, settings);
        }

        function addGenericBEConditionalHandler(onConditionalHandlerAdded, context) {
            var parameters = {
                context: context

            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onConditionalHandlerAdded = onConditionalHandlerAdded;
            };

            VRModalService.showModal(conditionalHandlerPath, parameters, settings);
        }

        function editGenericBEConditionalHandler(onConditionalHandlerUpdated, conditionalHandler, context) {
            var parameters = {
                conditionalHandler: conditionalHandler,
                context: context
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onConditionalHandlerUpdated = onConditionalHandlerUpdated;
            };
            VRModalService.showModal(conditionalHandlerPath, parameters, settings);
        }

        function addGenericBEConditionGroup(onGenericBEConditionGroupAdded, context) {
            var parameters = {
                context: context

            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEConditionGroupAdded = onGenericBEConditionGroupAdded;
            };

            VRModalService.showModal(conditionGroupPath, parameters, settings);
        }

        function editGenericBEConditionGroup(onGenericBEConditionGroupUpdated, conditionGroup, context) {
            var parameters = {
                conditionGroup: conditionGroup,
                context: context
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEConditionGroupUpdated = onGenericBEConditionGroupUpdated;
            };
            VRModalService.showModal(conditionGroupPath, parameters, settings);
        }


        function addGenericBESerialNumberPart(onGenericBESerialNumberPartAdded, context) {
            var parameters = {
                context: context

            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBESerialNumberPartAdded = onGenericBESerialNumberPartAdded;
            };

            VRModalService.showModal(partDefinitionPath, parameters, settings);
        }

        function editGenericBESerialNumberPart(onGenericBESerialNumberPartUpdated, partDefinition, context) {
            var parameters = {
                partDefinition: partDefinition,
                context: context
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBESerialNumberPartUpdated = onGenericBESerialNumberPartUpdated;
            };
            VRModalService.showModal(partDefinitionPath, parameters, settings);
        }

    };

    app.service('VR_GenericData_GenericBEDefinitionService', GenericBEDefinitionService);

})(app);