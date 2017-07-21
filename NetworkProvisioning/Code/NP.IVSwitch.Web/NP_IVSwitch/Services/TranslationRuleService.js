
(function (appControllers) {

    "use strict";

    TranslationRuleService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function TranslationRuleService(NPModalService, VRCommon_ObjectTrackingService, UtilsService) {
        var drillDownDefinitions = [];
        function addTranslationRule(onTranslationRuleAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onTranslationRuleAdded = onTranslationRuleAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/TranslationRule/TranslationRuleEditor.html', null, settings);
        };

        function editTranslationRule(TranslationRuleId, onTranslationRuleUpdated) {
            var settings = {};

            var parameters = {
                TranslationRuleId: TranslationRuleId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onTranslationRuleUpdated = onTranslationRuleUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/TranslationRule/TranslationRuleEditor.html', parameters, settings);
        }

        function viewHistoryTranslationRule(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/TranslationRule/TranslationRuleEditor.html', modalParameters, modalSettings);
        };

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "NP_IVSwitch_TranslationRule_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryTranslationRule(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }


        function getEntityUniqueName() {
            return "NP_IVSwitch_TranslationRule";
        }

        function registerObjectTrackingDrillDownToTranslationRule() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, translationRuleItem) {

                translationRuleItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: translationRuleItem.Entity.TranslationRuleId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return translationRuleItem.objectTrackingGridAPI.load(query);
            };


            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addTranslationRule: addTranslationRule,
            editTranslationRule: editTranslationRule,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToTranslationRule: registerObjectTrackingDrillDownToTranslationRule,
            registerHistoryViewAction: registerHistoryViewAction
        };
    }

    appControllers.service('NP_IVSwitch_TranslationRuleService', TranslationRuleService);

})(appControllers);