(function (appControllers) {

    'use strict';

    RecurringChargeRuleSetController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function RecurringChargeRuleSetController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var recurringChargeRuleSetEntity;
        var recurringChargeRuleSetNames;

        var recurringChargeRuleSetSettingsSelectiveAPI;
        var recurringChargeRuleSetSettingsSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                recurringChargeRuleSetEntity = parameters.recurringChargeRuleSet;
                recurringChargeRuleSetNames = parameters.recurringChargeRuleSetNames;
            }

            isEditMode = (recurringChargeRuleSetEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onRecurringChargeRuleSetSettingsReady = function (api) {
                recurringChargeRuleSetSettingsSelectiveAPI = api;
                recurringChargeRuleSetSettingsSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onValidateNames = function () {
                if (recurringChargeRuleSetNames == undefined)
                    return null;

                if (recurringChargeRuleSetEntity != undefined && recurringChargeRuleSetEntity.Name == $scope.scopeModel.name)
                    return null;
               
                for (var i = 0; i < recurringChargeRuleSetNames.length; i++) {
                    var currentName = recurringChargeRuleSetNames[i];
                    if ($scope.scopeModel.name.toLowerCase() == currentName.toLowerCase()) {
                        return 'Same Rule Name Exists';
                    }
                }
                return null;
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRecurringChargeRuleSetSettingsSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((recurringChargeRuleSetEntity != undefined) ? recurringChargeRuleSetEntity.Name : null, 'Recurring Charge Rule Set') :
                UtilsService.buildTitleForAddEditor('Recurring Charge Rule Set');
        }
        function loadStaticData() {
            if (recurringChargeRuleSetEntity == undefined)
                return;

            $scope.scopeModel.name = recurringChargeRuleSetEntity.Name;
        }
        function loadRecurringChargeRuleSetSettingsSelective() {
            var recurringChargeRuleSetSettingsSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            recurringChargeRuleSetSettingsSelectiveReadyDeferred.promise.then(function () {

                var recurringChargeRuleSetSettingsSelectivePayload;
                if (recurringChargeRuleSetEntity != undefined) {
                    recurringChargeRuleSetSettingsSelectivePayload = {
                        accountRecurringChargeRuleSetSettings: recurringChargeRuleSetEntity.Settings
                    };
                }
                VRUIUtilsService.callDirectiveLoad(recurringChargeRuleSetSettingsSelectiveAPI, recurringChargeRuleSetSettingsSelectivePayload, recurringChargeRuleSetSettingsSelectiveLoadDeferred);
            });

            return recurringChargeRuleSetSettingsSelectiveLoadDeferred.promise;
        }

        function insert() {
            var recurringChargeRuleSetObject = buildRecurringChargeRuleSetObjectFromScope();

            if ($scope.onRecurringChargeRuleSetAdded != undefined && typeof ($scope.onRecurringChargeRuleSetAdded) == 'function') {
                $scope.onRecurringChargeRuleSetAdded(recurringChargeRuleSetObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var recurringChargeRuleSetObject = buildRecurringChargeRuleSetObjectFromScope();

            if ($scope.onRecurringChargeRuleSetUpdated != undefined && typeof ($scope.onRecurringChargeRuleSetUpdated) == 'function') {
                $scope.onRecurringChargeRuleSetUpdated(recurringChargeRuleSetObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildRecurringChargeRuleSetObjectFromScope() {

            return {
                Name: $scope.scopeModel.name,
                AccountRecurringChargeRuleSetId: recurringChargeRuleSetEntity != undefined ? recurringChargeRuleSetEntity.AccountRecurringChargeRuleSetId : UtilsService.guid(),
                Settings: recurringChargeRuleSetSettingsSelectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_RecurringChargeRuleSetEditorController', RecurringChargeRuleSetController);

})(appControllers);