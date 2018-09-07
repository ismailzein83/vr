(function (appControllers) {
    "use strict";
    routeTableRouteEditorController.$inject = ['$scope', 'NP_IVSwitch_RouteTableRouteAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RouteRuleCriteriaTypeEnum', 'WhS_Routing_RouteRuleAPIService'];

    function routeTableRouteEditorController($scope, NP_IVSwitch_RouteTableRouteAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Routing_RouteRuleCriteriaTypeEnum, WhS_Routing_RouteRuleAPIService) {
        var routeTableId;
        var isEditMode;
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
            }
            isEditMode = $scope.scopeModel.routeTableRouteName != undefined ? true : false;
        };

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

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
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
        };

        function getRouteTableOptions() {
            return NP_IVSwitch_RouteTableRouteAPIService.GetRouteTableRoutesOptions(routeTableId, $scope.scopeModel.routeTableRouteName).then(function (response) {
                var directivePayload = response;
                VRUIUtilsService.callDirectiveLoad(supplierRouteGridAPI, directivePayload, undefined)

            });
        };

        function loadAllControls() {
            function codeListDirective() {
                return codeListDirectiveDefferedReady.promise.then(function () {
                    var directivePayload;
                    VRUIUtilsService.callDirectiveLoad(codeListDirectiveAPI, directivePayload, undefined)
                })
            };
            function supplierRouteGrid() {
                return supplierRouteGridDefferedReady.promise.then(function () {


                    var directivePayload;
                    VRUIUtilsService.callDirectiveLoad(supplierRouteGridAPI, directivePayload, undefined);

                })
            };
            function setTitle() {
                if (isEditMode)
                    $scope.title = UtilsService.buildTitleForUpdateEditor($scope.scopeModel.routeTableRouteName, " Route Table Route");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("New Route Table Route");
            };
        return UtilsService.waitMultipleAsyncOperations([codeListDirective, setTitle, supplierRouteGrid]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
            });
        };

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

        };

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
        };

        function buildParentObjectFromScopeForEdit() {
            isBlockedAccount = supplierRouteGridAPI.getData().IsBlockedAccount;
            var supplierRouteGridData = supplierRouteGridAPI.getData().GridData;
            var preference = supplierRouteGridData.length;
            var routePreferences = [];
            for (var i = 0; i < supplierRouteGridData.length; i++) {
                var tab = {
                    RouteId: supplierRouteGridData[i].routeDirectiveAPI.getSelectedIds(),
                    Preference: preference,
                    Percentage: supplierRouteGridData[i].percentage
                }
                routePreferences.push(tab);
                preference--;
            };
            var scopeObject = {
                RouteTableId: routeTableId,
                Destination: $scope.scopeModel.routeTableRouteName,
                RouteOptionsToEdit: routePreferences,
                IsBlockedAccount: isBlockedAccount
            };
            return scopeObject;
        };

        function buildParentObjectFromScopeForAdd() {
            var supplierRouteGridData = supplierRouteGridAPI.getData().GridData;
            isBlockedAccount = supplierRouteGridAPI.getData().IsBlockedAccount;
            var preference = supplierRouteGridData.length;
            var routePreferences = [];
            for (var i = 0; i < supplierRouteGridData.length; i++) {

                var routePreference = {
                    RouteId: supplierRouteGridData[i].routeDirectiveAPI.getSelectedIds(),
                    Preference: preference,
                    Percentage: supplierRouteGridData[i].percentage
                };
                routePreferences.push(routePreference);
                preference--;

            };
            var objectScope = {
                CodeListResolver: {
                    Settings: codeListDirectiveAPI.getData()
                },
                IsBlockedAccount: isBlockedAccount,
                RouteOptionstoAdd: routePreferences,
                RouteTableId: routeTableId
            };
            return objectScope;
        };

    };
    appControllers.controller('NP_IVSwitch_RouteTableRouteEditorController', routeTableRouteEditorController);
})(appControllers);