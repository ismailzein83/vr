(function (appControllers) {
    "use strict";
    routeTableRouteEditorController.$inject = ['$scope', 'NP_IVSwitch_RouteTableRouteAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RouteRuleCriteriaTypeEnum', 'WhS_Routing_RouteRuleAPIService', 'NP_IVSwitch_RouteTableViewTypeEnum'];

    function routeTableRouteEditorController($scope, NP_IVSwitch_RouteTableRouteAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Routing_RouteRuleCriteriaTypeEnum, WhS_Routing_RouteRuleAPIService, NP_IVSwitch_RouteTableViewTypeEnum) {
        var routeTableId;
        var routeTableRouteName;
        var isEditMode;
        var routeTableRouteOptions;
        $scope.scopeModel = {};

        var codeListDirectiveAPI;
        var codeListDirectiveDefferedReady = UtilsService.createPromiseDeferred();

        var supplierRouteGridAPI;
        var supplierRouteGridDefferedReady = UtilsService.createPromiseDeferred();
        var isBlockedAccount;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                routeTableId = parameters.RouteTableId;
                $scope.scopeModel.routeTableRouteName = parameters.Destination;
                routeTableRouteName = parameters.Destination;
                $scope.scopeModel.isANumber = (NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value == parameters.RouteTableViewType) ? true : false;
            }
            isEditMode = $scope.scopeModel.routeTableRouteName != undefined ? true : false;
        }

        function defineScope() {

            $scope.onCodeListDirectiveReady = function (api) {
                codeListDirectiveAPI = api;
                codeListDirectiveDefferedReady.resolve();
            };

            $scope.onSupplierRouteGridDirectiveReady = function (api) {
                supplierRouteGridAPI = api;
                supplierRouteGridDefferedReady.resolve();

            };

            $scope.scopeModel.saveRouteTableRT = function () {
                if (isEditMode)
                    return updateRouteTableRT();
                else
                    return insertRouteTableRT();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {
            if (isEditMode) {
                $scope.scopeModel.isLoading = true;

                $scope.scopeModel.addMode = false;
                getRouteTableOptions().then(function () {
                    loadAllControls().finally(function () {
                        routeTableEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                $scope.scopeModel.addMode = true;
                loadAllControls();
            }
        }

        function getRouteTableOptions() {
          return  NP_IVSwitch_RouteTableRouteAPIService.GetRouteTableRoutesOptions(routeTableId, $scope.scopeModel.routeTableRouteName).then(function (response) {
                routeTableRouteOptions = response;
                $scope.scopeModel.bNumber = routeTableRouteOptions.TechPrefix;
            });
        }

        function loadAllControls() {
            var promises = [];
            function codeListDirective() {
                    return codeListDirectiveDefferedReady.promise.then(function () {
                        var directivePayload;
                        VRUIUtilsService.callDirectiveLoad(codeListDirectiveAPI, directivePayload, undefined);
                    });
            }

            function setTitle() {
                if (isEditMode)
                    $scope.title = UtilsService.buildTitleForUpdateEditor($scope.scopeModel.routeTableRouteName, " Route Table Route");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("New Route Table Route");
            }

            function loadSupplierRouteGrid()
            {
                if (isEditMode)
                {
                    var supplierRouteGridAPILoadDeferred = UtilsService.createPromiseDeferred();
                    supplierRouteGridDefferedReady.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(supplierRouteGridAPI, routeTableRouteOptions, supplierRouteGridAPILoadDeferred);
                    });
                    return supplierRouteGridAPILoadDeferred.promise;
                }
               
            }


            return UtilsService.waitMultipleAsyncOperations([codeListDirective, setTitle, loadSupplierRouteGrid]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
                   .finally(function () {
                       $scope.scopeModel.isLoading = false;
                   });
        }

        function insertRouteTableRT() {

            $scope.scopeModel.isLoading = true;
            var routeTableObject = buildParentObjectFromScopeForAdd();
            return NP_IVSwitch_RouteTableRouteAPIService.AddRouteTableRoutes(routeTableObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Route Table Route", response, "Name")) {
                    if ($scope.onRouteTableRTAdded != undefined) {
                        $scope.onRouteTableRTAdded();
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updateRouteTableRT() {
            $scope.scopeModel.isLoading = true;
            var routeTableObject = buildParentObjectFromScopeForEdit();
            NP_IVSwitch_RouteTableRouteAPIService.UpdateRouteTableRoute(routeTableObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Route Table Route", response, "Name")) {
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
        }

        function buildParentObjectFromScopeForEdit() {
            var routeOptions = supplierRouteGridAPI.getData();
            isBlockedAccount = routeOptions.IsBlockedAccount;
            var supplierRouteGridData = routeOptions.RouteOptions;
            var routeOptionsToEdit = [];
            if (supplierRouteGridData != undefined) {
                for (var i = 0; i < supplierRouteGridData.length; i++) {
                    var routeOption = {
                        RouteId: supplierRouteGridData[i].RouteId,
                        Percentage: supplierRouteGridData[i].Percentage
                    };
                    if (supplierRouteGridData[i].BackupRouteIds != undefined)
                        routeOption.BackupOptions = supplierRouteGridData[i].BackupRouteIds;

                    routeOptionsToEdit.push(routeOption);



                }
            }
            var objectScopeForEdit = {
                RouteTableId: routeTableId,
                Destination: routeTableRouteName,
                RouteOptionsToEdit: routeOptionsToEdit,
                IsBlockedAccount: isBlockedAccount,
                TechPrefix: $scope.scopeModel.bNumber
            };
            return objectScopeForEdit;
        }

        function buildParentObjectFromScopeForAdd() {
            var routeOptions = supplierRouteGridAPI.getData();
            isBlockedAccount = routeOptions.IsBlockedAccount;
            var supplierRouteGridData = routeOptions.RouteOptions;
            var routeOptionsToAdd = [];
            if (supplierRouteGridData != undefined)
                for (var i = 0; i < supplierRouteGridData.length; i++) {


                    var routeOption = {
                        RouteId: supplierRouteGridData[i].RouteId,
                        Percentage:supplierRouteGridData[i].Percentage
                    };
                    if (supplierRouteGridData[i].BackupRouteIds !=undefined)
                        routeOption.BackupOptions = supplierRouteGridData[i].BackupRouteIds;
                    routeOptionsToAdd.push(routeOption);
                }
            var objectScopeForAdd = {
                CodeListResolver: {
                    Settings: codeListDirectiveAPI.getData()
                },
                IsBlockedAccount: isBlockedAccount,
                RouteOptionsToAdd: routeOptionsToAdd,
                RouteTableId: routeTableId,
                TechPrefix: $scope.scopeModel.bNumber
            };
            return objectScopeForAdd;
        }

    }
    appControllers.controller('NP_IVSwitch_RouteTableRouteEditorController', routeTableRouteEditorController);
})(appControllers);