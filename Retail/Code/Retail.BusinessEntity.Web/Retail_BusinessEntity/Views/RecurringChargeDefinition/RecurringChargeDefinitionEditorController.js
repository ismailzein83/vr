(function (appControllers) {

    "use strict";

    RecurringChargeDefinitionEditorController.$inject = ['$scope', 'Retail_BE_RecurringChargeDefinitionAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function RecurringChargeDefinitionEditorController($scope, Retail_BE_RecurringChargeDefinitionAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var recurringChargeDefinitionId;
        var recurringChargeDefinitionEntity;
        var settingsAPI;
        var recurringPeriodSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                recurringChargeDefinitionId = parameters.recurringChargeDefinitionId;
            }

            isEditMode = (recurringChargeDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
          

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return updateRecurringChargeDefinition();
                }
                else {
                    return insertRecurringChargeDefinition();
                }
            };
            $scope.scopeModel.onRecurringPeriodSettingsReady = function(api){
                settingsAPI = api;
                recurringPeriodSettingsReadyDeferred.resolve();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.hasSaveRecurringChargeDefinitionPermission = function () {
                if ($scope.scopeModel.isEditMode)
                    return Retail_BE_RecurringChargeDefinitionAPIService.HasUpdateRecurringChargeDefinitionPermission();
                else
                    return Retail_BE_RecurringChargeDefinitionAPIService.HasAddRecurringChargeDefinitionPermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getRecurringChargeDefinition().then(function () {
                    loadAllControls()
                        .finally(function () {
                            recurringChargeDefinitionEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getRecurringChargeDefinition() {
            return Retail_BE_RecurringChargeDefinitionAPIService.GetRecurringChargeDefinition(recurringChargeDefinitionId).then(function (response) {
                recurringChargeDefinitionEntity = response;
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData,loadRecurringPeriodSettings])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }
        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(recurringChargeDefinitionEntity ? recurringChargeDefinitionEntity.Name : undefined, 'RecurringChargeDefinition') : UtilsService.buildTitleForAddEditor('RecurringChargeDefinition');
        }
        function loadStaticData() {
            if (recurringChargeDefinitionEntity == undefined)
                return;

            $scope.scopeModel.name = recurringChargeDefinitionEntity.Name;
        }
        function loadRecurringPeriodSettings(){
            var recurringChargeSettingsLoadDeferred = UtilsService.createPromiseDeferred();
            recurringPeriodSettingsReadyDeferred.promise.then(function () {
                var recurringChargeSettingsPayload;
                if (recurringChargeDefinitionEntity != undefined && recurringChargeDefinitionEntity.Settings!=undefined) {
                    recurringChargeSettingsPayload = {
                        data: recurringChargeDefinitionEntity.Settings.RecurringPeriod
                    }
                }
                VRUIUtilsService.callDirectiveLoad(settingsAPI, recurringChargeSettingsPayload, recurringChargeSettingsLoadDeferred);
            });
            return recurringChargeSettingsLoadDeferred.promise;
        }
        function insertRecurringChargeDefinition() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_RecurringChargeDefinitionAPIService.AddRecurringChargeDefinition(buildRecurringChargeDefinitionObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("RecurringChargeDefinition", response, "Name")) {
                        if ($scope.onRecurringChargeDefinitionAdded != undefined)
                            $scope.onRecurringChargeDefinitionAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function updateRecurringChargeDefinition() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_RecurringChargeDefinitionAPIService.UpdateRecurringChargeDefinition(buildRecurringChargeDefinitionObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("RecurringChargeDefinition", response, "Name")) {
                        if ($scope.onRecurringChargeDefinitionUpdated != undefined)
                            $scope.onRecurringChargeDefinitionUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildRecurringChargeDefinitionObjFromScope() {
            var obj = {
                RecurringChargeDefinitionId: recurringChargeDefinitionId,
                Name: $scope.scopeModel.name,
                Settings:{
                    RecurringPeriod: settingsAPI.getData()
                } 
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_RecurringChargeDefinitionEditorController', RecurringChargeDefinitionEditorController);

})(appControllers);
