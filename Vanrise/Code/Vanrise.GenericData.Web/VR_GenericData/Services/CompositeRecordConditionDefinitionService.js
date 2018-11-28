(function (appControllers) {

    'use strict';

    CompositeRecordConditionDefinitionService.$inject = ['VRModalService', 'VRNotificationService'];

    function CompositeRecordConditionDefinitionService(VRModalService, VRNotificationService) {

        function addCompositeRecordConditionDefinition(onCompositeRecordConditionDefinitionAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onCompositeRecordConditionDefinitionAdded = onCompositeRecordConditionDefinitionAdded;
            };
            var parameters = {};
           
            VRModalService.showModal("/Client/Modules/VR_GenericData/Directives/CompositeRecordCondition/Definition/Templates/CompositeRecordConditionDefinitionEditorTemplate.html", parameters, settings);
        }

        function editCompositeRecordConditionDefinition(onCompositeRecordConditionDefinitionUpdated, compositeRecordConditionDefinition) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCompositeRecordConditionDefinitionUpdated = onCompositeRecordConditionDefinitionUpdated;
            };
            var parameters = {
                CompositeRecordConditionDefinition: compositeRecordConditionDefinition
            };

            VRModalService.showModal("/Client/Modules/VR_GenericData/Directives/CompositeRecordCondition/Definition/Templates/CompositeRecordConditionDefinitionEditorTemplate.html", parameters, settings);
        }

        return ({
            addCompositeRecordConditionDefinition: addCompositeRecordConditionDefinition,
            editCompositeRecordConditionDefinition: editCompositeRecordConditionDefinition,
        });
    }

    appControllers.service('VR_GenericData_CompositeRecordConditionDefinitionService', CompositeRecordConditionDefinitionService);

})(appControllers);
