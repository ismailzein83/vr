(function (appControllers) {

    "use strict";

    switchIdentificationRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_SwitchIdentificationRuleAPIService', 'VRValidationService'];

    function switchIdentificationRuleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_SwitchIdentificationRuleAPIService, VRValidationService) {

        var isEditMode;
        var ruleId;
        var switchDirectiveAPI;
        var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var dataSourceDirectiveAPI;
        var dataSourceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var switchRuleEntity;

        loadParameters();
        defineScope();
        load();
        function loadParameters() {
            
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                ruleId = parameters.RuleId
            }
            isEditMode = (ruleId != undefined);
        }

        function defineScope() {

            $scope.scopeModal = {};
            $scope.scopeModal.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModal.beginEffectiveDate, $scope.scopeModal.endEffectiveDate);
            }

            $scope.scopeModal.SaveSwitchRule = function () {
                if (isEditMode) {
                    return updateSwitchRule();
                }
                else {
                    return insertSwitchRule();
                }
            };

            $scope.scopeModal.onSwitchDirectiveReady = function (api) {
                switchDirectiveAPI = api;
                switchReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.onDataSourceDirectiveReady = function (api) {
                dataSourceDirectiveAPI = api;
                dataSourceReadyPromiseDeferred.resolve();
            }

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Switch Identification Rule");
                getSwitchRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            switchRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Switch Identification Rule");
                loadAllControls();
            }

        }

        function setDefaultValues() {
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadSwitches, loadDataSources])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }

        function loadSwitches() {
            var loadSwitchPromiseDeferred = UtilsService.createPromiseDeferred();
            switchReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: switchRuleEntity != undefined ? switchRuleEntity.Settings.SwitchId : undefined
                }
                VRUIUtilsService.callDirectiveLoad(switchDirectiveAPI, directivePayload, loadSwitchPromiseDeferred);
            });
            return loadSwitchPromiseDeferred.promise;
        }

        function loadDataSources() {
            var loadDataSourcePromiseDeferred = UtilsService.createPromiseDeferred();
            dataSourceReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: switchRuleEntity != undefined ? switchRuleEntity.Criteria.DataSources : undefined
                }
                VRUIUtilsService.callDirectiveLoad(dataSourceDirectiveAPI, directivePayload, loadDataSourcePromiseDeferred);
            });
            return loadDataSourcePromiseDeferred.promise;
        }

        function loadFilterBySection() {
            if (switchRuleEntity != undefined) {
                $scope.scopeModal.beginEffectiveDate = switchRuleEntity.BeginEffectiveTime;
                $scope.scopeModal.endEffectiveDate = switchRuleEntity.EndEffectiveTime;
                $scope.scopeModal.description = switchRuleEntity.Description;
            }
        }

        function getSwitchRule() {
            return WhS_CDRProcessing_SwitchIdentificationRuleAPIService.GetRule(ruleId).then(function (switchRule) {
                switchRuleEntity = switchRule;
            });
        }

        function buildSwitchRuleObjectObjFromScope() {
            
            var settings = {
                SwitchId: $scope.scopeModal.selectedSwitch.SwitchId
            }
            var criteria = {
                DataSources: dataSourceDirectiveAPI.getSelectedIds(),
            }
            var switchRule = {
                Settings: settings,
                Description: $scope.scopeModal.description,
                Criteria: criteria,
                BeginEffectiveTime: $scope.scopeModal.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModal.endEffectiveDate
            }
           
            return switchRule;
        }

        function insertSwitchRule() {

            var switchRuleObject = buildSwitchRuleObjectObjFromScope();
            return WhS_CDRProcessing_SwitchIdentificationRuleAPIService.AddRule(switchRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch Rule", response)) {
                    if ($scope.onSwitchIdentificationRuleAdded != undefined)
                        $scope.onSwitchIdentificationRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateSwitchRule() {
            var switchRuleObject = buildSwitchRuleObjectObjFromScope();
            switchRuleObject.RuleId = ruleId;
            WhS_CDRProcessing_SwitchIdentificationRuleAPIService.UpdateRule(switchRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch Rule", response)) {
                    if ($scope.onSwitchIdentificationRuleUpdated != undefined)
                        $scope.onSwitchIdentificationRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_CDRProcessing_SwitchIdentificationRuleEditorController', switchIdentificationRuleEditorController);
})(appControllers);
