(function (appControllers) {

    'use strict';

    genericRule.$inject = ['VRModalService', 'VRNotificationService', 'VR_GenericData_GenericRuleAPIService', 'UtilsService', 'VRCommon_ObjectTrackingService'];

    function genericRule(VRModalService, VRNotificationService, VR_GenericData_GenericRuleAPIService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addGenericRule: addGenericRule,
            editGenericRule: editGenericRule,
            deleteGenericRule: deleteGenericRule,            
            viewGenericRule: viewGenericRule,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToGenericRule: registerObjectTrackingDrillDownToGenericRule,
            getEntityUniqueName: getEntityUniqueName,
            uploadGenericRules: uploadGenericRules
        });

        function addGenericRule(genericRuleDefinitionId, onGenericRuleAdded, preDefinedData, accessibility) {
            var modalParameters = {
                genericRuleDefinitionId: genericRuleDefinitionId,
                preDefinedData: preDefinedData,
                accessibility: accessibility
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleAdded = onGenericRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }

        function editGenericRule(genericRuleId, genericRuleDefinitionId, onGenericRuleUpdated, accessibility) {
            var modalParameters = {
                genericRuleId: genericRuleId,
                genericRuleDefinitionId: genericRuleDefinitionId,
                accessibility: accessibility
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleUpdated = onGenericRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }
        function viewGenericRule(genericRuleId, genericRuleDefinitionId, accessibility) {
            var modalParameters = {
                genericRuleId: genericRuleId,
                genericRuleDefinitionId: genericRuleDefinitionId,
                accessibility: accessibility
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }
        function deleteGenericRule(scope, genericRule, onGenericRuleDeleted) {
            var message = 'Are you sure you want to delete the rule ?';
            VRNotificationService.showConfirmation(message).then(function (confirmed) {
                if (confirmed) {
                    VR_GenericData_GenericRuleAPIService.DeleteGenericRule(genericRule).then(function (response) {
                        if (response) {
                            var deleted = VRNotificationService.notifyOnItemDeleted('Generic Rule', response);

                            if (deleted && onGenericRuleDeleted && typeof onGenericRuleDeleted == 'function') {
                                onGenericRuleDeleted();
                            }
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }
        function getEntityUniqueName(definitionId) {
            return "VR_GenericData_GenericRule_" + definitionId;
        }
        function registerObjectTrackingDrillDownToGenericRule() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, ruleItem) {
                ruleItem.objectTrackingGridAPI = directiveAPI;
               
                var query = {
                    ObjectId: ruleItem.Entity.RuleId,
                    EntityUniqueName: getEntityUniqueName(ruleItem.Entity.DefinitionId),

                };
                return ruleItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function uploadGenericRules(ruleDefinitionId, context, criteriaFieldsToHide, criteriaFieldsValues) {
            var modalSettings = {};
            var modalParameters = {
                ruleDefinitionId: ruleDefinitionId,
                context: context,
                criteriaFieldsToHide: criteriaFieldsToHide,
                criteriaFieldsValues: criteriaFieldsValues
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleUploader.html', modalParameters, modalSettings);
        }

    };

    appControllers.service('VR_GenericData_GenericRule', genericRule);

})(appControllers);
