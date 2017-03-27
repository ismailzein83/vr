(function (appControllers) {

    "use strict";

    VRAlertLevelEditorController.$inject = ['$scope', 'UtilsService', 'VR_Notification_AlertLevelAPIService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function VRAlertLevelEditorController($scope, UtilsService, VR_Notification_AlertLevelAPIService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var alertLevelId;
        var alertLevelEntity;

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
                alertLevelId = parameters.alertLevelId;
            }

            isEditMode = (alertLevelId != undefined);
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

          
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getAlertLevel().then(function () {
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

        function getAlertLevel() {
            return VR_Notification_AlertLevelAPIService.GetAlertLevel(alertLevelId).then(function (response) {
                alertLevelEntity = response;
            });
        }

        function loadAllControls() {
            function setTitle() {
                if (isEditMode) {
                    var alertLevelName = (alertLevelEntity != undefined) ? alertLevelEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(alertLevelEntity, 'Alert Level');

                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Alert Level');
                }
            }
            function loadStaticData() {
                if (alertLevelEntity == undefined)
                    return;
                $scope.scopeModel.name = alertLevelEntity.Name;
                $scope.scopeModel.weight = alertLevelEntity.Settings.Weight;
               
            }
            function loadStyleDefinitionSelector() {
                var styleDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                styleDefinitionSelectorReadyDeferred.promise.then(function () {
                    var styleDefinitionSelectorPayload = null;
                    if (isEditMode) {
                        styleDefinitionSelectorPayload = {
                            selectedIds: alertLevelEntity.Settings.StyleDefinitionId
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
                        selectedIds: alertLevelEntity != undefined ? alertLevelEntity.BusinessEntityDefinitionId : undefined,
                        filter: {
                            Filters: [{
                                $type: "Vanrise.Notification.Business.VRAlertLevelBEDefinitionFilter, Vanrise.Notification.Business"
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
            return VR_Notification_AlertLevelAPIService.AddAlertLevel(buildAlertLevelObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Alert Level', response, 'Name')) {
                    if ($scope.onAlertLevelAdded != undefined)
                        $scope.onAlertLevelAdded(response.InsertedObject);
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
            return VR_Notification_AlertLevelAPIService.UpdateAlertLevel(buildAlertLevelObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Alert Level', response, 'Name')) {
                   
                    if ($scope.onAlertLevelUpdated != undefined) {
                        $scope.onAlertLevelUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildAlertLevelObjFromScope() {
            var settings = {
                StyleDefinitionId: styleDefinitionAPI.getSelectedIds(),
                Weight: $scope.scopeModel.weight
               
            };
            return {
                VRAlertLevelId: alertLevelEntity != undefined ? alertLevelEntity.VRAlertLevelId : undefined,
                Name: $scope.scopeModel.name,
                BusinessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                Settings: settings,
            };
        }
    }

    appControllers.controller('VR_Notification_AlertLevelEditorController', VRAlertLevelEditorController);

})(appControllers);