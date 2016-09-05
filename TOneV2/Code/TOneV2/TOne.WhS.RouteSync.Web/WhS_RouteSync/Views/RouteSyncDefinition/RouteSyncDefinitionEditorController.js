(function (appControllers) {

    "use strict";

    RouteSyncDefinitionEditorController.$inject = ['$scope', 'WhS_RouteSync_RouteSyncDefinitionAPIService', 'WhS_RouteSync_SwitchAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function RouteSyncDefinitionEditorController($scope, WhS_RouteSync_RouteSyncDefinitionAPIService, WhS_RouteSync_SwitchAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var routeSyncDefinitionId;
        var routeSyncDefinitionEntity;

        var switchSelectorAPI;
        var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var routeReaderSelectiveAPI;
        var routeReaderSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                routeSyncDefinitionId = parameters.routeSyncDefinitionId;
            }

            isEditMode = (routeSyncDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSwitchSelectorReady = function (api) {
                switchSelectorAPI = api;
                switchSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onRouteReaderSelectiveReady = function (api) {
                routeReaderSelectiveAPI = api;
                routeReaderSelectiveReadyDeferred.resolve();
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
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getRouteSyncDefinition().then(function () {
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

        function getRouteSyncDefinition() {
            return WhS_RouteSync_RouteSyncDefinitionAPIService.GetRouteSyncDefinition(routeSyncDefinitionId).then(function (response) {
                routeSyncDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSwitchSelector, loadRouteReaderSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var routeSyncDefinitionName = (routeSyncDefinitionEntity != undefined) ? routeSyncDefinitionEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(routeSyncDefinitionName, 'Route Sync Definition');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Route Sync Definition');
                }
            }
            function loadStaticData() {
                if (routeSyncDefinitionEntity == undefined)
                    return;
                $scope.scopeModel.name = routeSyncDefinitionEntity.Name;
            }
            function loadSwitchSelector() {
                var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                switchSelectorReadyDeferred.promise.then(function () {
                    var switchSelectorPayload;
                    if (routeSyncDefinitionEntity != undefined) {
                        switchSelectorPayload = { selectedIds: routeSyncDefinitionEntity.Settings.SwitchIds };
                    }
                    VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, switchSelectorPayload, switchSelectorLoadDeferred);
                });

                return switchSelectorLoadDeferred.promise;
            }
            function loadRouteReaderSelective() {
                var routeReaderSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

                routeReaderSelectiveReadyDeferred.promise.then(function () {
                    var routeReaderSelectivePayload;
                    if (routeSyncDefinitionEntity != undefined && routeSyncDefinitionEntity.Settings != undefined) {
                        routeReaderSelectivePayload = { routeReader: routeSyncDefinitionEntity.Settings.RouteReader };
                    }
                    VRUIUtilsService.callDirectiveLoad(routeReaderSelectiveAPI, routeReaderSelectivePayload, routeReaderSelectiveLoadDeferred);
                });

                return routeReaderSelectiveLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return WhS_RouteSync_RouteSyncDefinitionAPIService.AddRouteSyncDefinition(buildRouteSyncDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('RouteSyncDefinition', response, 'Name')) {
                    if ($scope.onRouteSyncDefinitionAdded != undefined)
                        $scope.onRouteSyncDefinitionAdded(response.InsertedObject);
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
            return WhS_RouteSync_RouteSyncDefinitionAPIService.UpdateRouteSyncDefinition(buildRouteSyncDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('RouteSyncDefinition', response, 'Name')) {
                    if ($scope.onRouteSyncDefinitionUpdated != undefined) {
                        $scope.onRouteSyncDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildRouteSyncDefinitionObjFromScope() {
            var routeReader = routeReaderSelectiveAPI.getData();
            var switchIds = switchSelectorAPI.getSelectedIds();

            return {
                RouteSyncDefinitionId: routeSyncDefinitionEntity != undefined ? routeSyncDefinitionEntity.RouteSyncDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                Settings: {
                    RouteReader: routeReader,
                    SwitchIds: switchIds
                }
            };
        }
    }

    appControllers.controller('WhS_RouteSync_RouteSyncDefinitionEditorController', RouteSyncDefinitionEditorController);

})(appControllers);