(function (appControllers) {

    "use strict";

    StatusDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VR_Common_StatusDefinitionAPIService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function StatusDefinitionEditorController($scope, UtilsService, VR_Common_StatusDefinitionAPIService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var statusDefinitionId;
        var statusDefinitionEntity;

        var styleDefinitionAPI;
        var styleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var beDefinitionSelectorApi;
        var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                statusDefinitionId = parameters.statusDefinitionId;
            }

            isEditMode = (statusDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                beDefinitionSelectorApi = api;
                beDefinitionSelectorPromiseDeferred.resolve();
            };
            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onStyleDefinitionSelectorReady = function (api) {
                styleDefinitionAPI = api;
                styleDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.hasSaveStatusDefinitionPermission = function () {
                if (isEditMode)
                    return VR_Common_StatusDefinitionAPIService.HasUpdateStatusDefinitionPermission();
                else
                    return VR_Common_StatusDefinitionAPIService.HasAddStatusDefinitionPermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getStatusDefinition().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getStatusDefinition() {
            return VR_Common_StatusDefinitionAPIService.GetStatusDefinition(statusDefinitionId).then(function (response) {
                statusDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            function setTitle() {
                if (isEditMode) {
                    var statusDefinitionName = (statusDefinitionEntity != undefined) ? statusDefinitionEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(statusDefinitionName, 'Status Definition');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Status Definition');
                }
            }
            function loadStaticData() {
                if (statusDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = statusDefinitionEntity.Name;
                $scope.scopeModel.HasInitialCharge = statusDefinitionEntity.Settings.HasInitialCharge;
                $scope.scopeModel.HasRecurringCharge = statusDefinitionEntity.Settings.HasRecurringCharge;
            }
            function loadStyleDefinitionSelector() {
                var styleDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                styleDefinitionSelectorReadyDeferred.promise.then(function () {
                    var styleDefinitionSelectorPayload = null;
                    if (isEditMode) {
                        styleDefinitionSelectorPayload = {
                            selectedIds: statusDefinitionEntity.Settings.StyleDefinitionId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(styleDefinitionAPI, styleDefinitionSelectorPayload, styleDefinitionSelectorLoadDeferred);
                });
                return styleDefinitionSelectorLoadDeferred.promise;
            }
            function loadBusinessEntityDefinitionSelector() {
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                beDefinitionSelectorPromiseDeferred.promise.then(function () {
                    var payloadSelector = {
                        selectedIds: statusDefinitionEntity != undefined ? statusDefinitionEntity.BusinessEntityDefinitionId : undefined,
                        filter: {
                            Filters: [{
                                $type: "Vanrise.Common.Business.StatusDefinitionBEFilter, Vanrise.Common.Business"
                            }]
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                });
                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadStyleDefinitionSelector, loadBusinessEntityDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VR_Common_StatusDefinitionAPIService.AddStatusDefinition(buildStatusDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Status Definition', response, 'Name')) {
                    if ($scope.onStatusDefinitionAdded != undefined)
                        $scope.onStatusDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return VR_Common_StatusDefinitionAPIService.UpdateStatusDefinition(buildStatusDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Status Definition', response, 'Name')) {
                    if ($scope.onStatusDefinitionUpdated != undefined) {
                        $scope.onStatusDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildStatusDefinitionObjFromScope() {
            var settings = {
                StyleDefinitionId: styleDefinitionAPI.getSelectedIds(),
                HasInitialCharge: $scope.scopeModel.HasInitialCharge,
                HasRecurringCharge: $scope.scopeModel.HasRecurringCharge
            };
            return {
                StatusDefinitionId: statusDefinitionEntity != undefined ? statusDefinitionEntity.StatusDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                BusinessEntityDefinitionId:beDefinitionSelectorApi.getSelectedIds(),
                Settings: settings,
            };
        }
    }

    appControllers.controller('VR_Common_StatusDefinitionEditorController', StatusDefinitionEditorController);

})(appControllers);