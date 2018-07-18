(function (appControllers) {

    "use strict";
    routeTableEditorController.$inject = ['$scope', 'NP_IVSwitch_RouteTableAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function routeTableEditorController($scope, NP_IVSwitch_RouteTableAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var routeTableId;
        var routeTableEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                routeTableId = parameters.routeTableId;
            }
            isEditMode = (routeTableId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveRouteTable = function () {
                if (isEditMode)
                    return updateRouteTable();
                else
                    return insertRouteTable();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getRouteTable().then(function () {
                    loadAllControls().finally(function () {
                        routeTableEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getRouteTable() {
            return NP_IVSwitch_RouteTableAPIService.GetRouteTableById(routeTableId).then(function (response) {
                routeTableEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && routeTableEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(routeTableEntity.Name, "Route Table");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Route Table");
            };

            function loadStaticData() {
                if (routeTableEntity != undefined) {
                    $scope.scopeModel.name = routeTableEntity.Name;
                    $scope.scopeModel.description = routeTableEntity.Description;
                    $scope.scopeModel.pScore = routeTableEntity.PScore;
                }
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildParentObjectFromScope() {
            var object = {
                RouteTable: {
                    RouteTableId: (routeTableId != undefined) ? routeTableId : undefined,
                    Name: $scope.scopeModel.name,
                    Description: $scope.scopeModel.name,
                    PScore: $scope.scopeModel.pScore
                },
                EndPoints : null
            };
            return object;
        };

        function insertRouteTable() {

            $scope.scopeModel.isLoading = true;
            var routeTableObject = buildParentObjectFromScope();
            return NP_IVSwitch_RouteTableAPIService.AddRouteTable(routeTableObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("RouteTable", response, "Name")) {
                    if ($scope.onRouteTableAdded != undefined) {
                        $scope.onRouteTableAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function updateRouteTable() {
            $scope.scopeModel.isLoading = true;
            var routeTableObject = buildParentObjectFromScope();
            NP_IVSwitch_RouteTableAPIService.UpdateRouteTable(routeTableObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("RouteTable", response, "Name")) {
                    if ($scope.onRouteTableUpdated != undefined) {
                        $scope.onRouteTableUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        };

    };
    appControllers.controller('NP_IVSwitch_RouteTableEditorController', routeTableEditorController);
})(appControllers);