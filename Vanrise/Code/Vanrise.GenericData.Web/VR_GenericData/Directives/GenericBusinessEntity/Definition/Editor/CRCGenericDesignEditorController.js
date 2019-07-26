(function (appControllers) {

    "use strict";

    CRCGenericDesignEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function CRCGenericDesignEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var crcEntitySettings;
        var context;

        var genericEditorConditionalRuleAPI;
        var genericEditorConditionalRuleReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                crcEntitySettings = parameters.crcEntityObject;
                context = parameters.context;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericEditorConditionalRulesDirectiveReady = function (api) {
                genericEditorConditionalRuleAPI = api;
                genericEditorConditionalRuleReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveCRCSettings = function () {
                return saveSettings();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            }; 

        }

        function load() {
            var initialPromises = [];
            $scope.scopeModel.isLoading = true;

            function setTitle() {
                $scope.title = "Conditional Rule Container Settings";
            }

            initialPromises.push(UtilsService.waitMultipleAsyncOperations([setTitle]));
            var rootPromiseNode = {
                promises: initialPromises,
                getChildNode: function () {
                    var directivePromises = [];

                    directivePromises.push(loadGenericEditorConditionalRule());

                    return {
                        promises: directivePromises
                    };
                }
            };

            return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadGenericEditorConditionalRule() {
            var genericEditorConditionalRuleLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            genericEditorConditionalRuleReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    context: context
                };

                if (crcEntitySettings != undefined && crcEntitySettings.genericEditorConditionalRule != undefined) {
                    payload.genericEditorConditionalRule = crcEntitySettings.genericEditorConditionalRule;
                }
                VRUIUtilsService.callDirectiveLoad(genericEditorConditionalRuleAPI, payload, genericEditorConditionalRuleLoadPromiseDeferred);
            });

            return genericEditorConditionalRuleLoadPromiseDeferred.promise;
        }

        function saveSettings() {
            var CRCSettingsDefinition = buildObjectFromScope();
            if ($scope.onCRCSettingsChanged != undefined) {
                $scope.onCRCSettingsChanged(CRCSettingsDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function buildObjectFromScope() {
            return {
                genericEditorConditionalRule: genericEditorConditionalRuleAPI.getData()
            };
        }
    }

    appControllers.controller('VR_GenericData_CRCGenericDesignEditorController', CRCGenericDesignEditorController);
})(appControllers);
