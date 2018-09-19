(function (appControllers) {
    "use strict";

    routeTableManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'NP_IVSwitch_RouteTableService', 'VRNavigationService', 'VR_Sec_ViewAPIService', 'NP_IVSwitch_RouteTableViewTypeEnum'];

    function routeTableManagementController($scope, UtilsService, VRUIUtilsService, NP_IVSwitch_RouteTableService, VRNavigationService, VR_Sec_ViewAPIService, NP_IVSwitch_RouteTableViewTypeEnum) {
        var gridApi;
        var GridDirectiveDefferedReady = UtilsService.createPromiseDeferred();   

        var customerSelectorDirectiveAPI;
        var CustomerSelectorDirectiveDefferedReady = UtilsService.createPromiseDeferred();

        var endPointSelectorDirectiveAPI;
        var endPointSelectorDirectiveDefferedReady = UtilsService.createPromiseDeferred();

        var routeTableViewTypesSelectorAPI;
        var routeTableViewTypesSelectorDefferedReady = UtilsService.createPromiseDeferred();

        var selectedCustomerDeffered = UtilsService.createPromiseDeferred();
        var deffered = UtilsService.createPromiseDeferred();

        var loadPageViewTypeDefferedReady = UtilsService.createPromiseDeferred();


        var selectedEndPoints = [];
        var viewId;
        var routeTableViewType;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined)
              viewId = parameters.viewId;
        };

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.routeTableViewType = [];

            $scope.scopeModel.onRouteTableViewTypesReady = function(api)
            {
            routeTableViewTypesSelectorAPI = api;
            routeTableViewTypesSelectorDefferedReady.resolve();
            };

            $scope.scopeModel.onRouteTableViewSelectionChanged = function (option) {
                if (option != undefined) {
                    routeTableViewType = option.value;
                    $scope.scopeModel.isLoading = true;
                    loadAllControls().catch(function (error) {
                        $scope.scopeModel.isLoading = false;
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });

                }
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;
                GridDirectiveDefferedReady.resolve();
            };

            $scope.scopeModel.onCustomerSelectorDirectiveReady = function (api) {
                customerSelectorDirectiveAPI = api;
                CustomerSelectorDirectiveDefferedReady.resolve();
            };

            $scope.scopeModel.onEndPointSelectorDirectiveReady = function (api) {
                endPointSelectorDirectiveAPI = api;
                endPointSelectorDirectiveDefferedReady.resolve();
            };

            $scope.scopeModel.onCustomerSelectionChanged = function (option) {
                var customerIds = [];
                for (var i = 0; i < option.length; i++)
                    customerIds.push(option[i].CarrierAccountId);
                $scope.scopeModel.selectedvalues = option;
                if (option != undefined) {
                    if (selectedCustomerDeffered != undefined) {

                        selectedCustomerDeffered.resolve();
                    }
                    else {

                        var endPointPayload = {
                            filter: {
                                AssignableToCarrierAccountId: null,
                                CustomerIds: customerIds

                            },
                            selectedIds: selectedEndPoints
                        };
                        var modifiedByFieldSetLoader = function (value) { $scope.scopeModel.isModifiedBySelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, endPointSelectorDirectiveAPI, endPointPayload, modifiedByFieldSetLoader);

                    }
                }


            };

            $scope.scopeModel.search = function () {
                return gridApi.load({ Filter: getFilter(), RouteTableViewType: routeTableViewType });
            };

            $scope.scopeModel.addRouteTable = function () {
                var onRouteTableAdded = function (routeTable) {
                    if (gridApi != undefined) {
                        gridApi.onRouteTableAdded(routeTable);
                    }
                };
                NP_IVSwitch_RouteTableService.addRouteTable({ onRouteTableAdded: onRouteTableAdded, RouteTableViewType: routeTableViewType });
             };
        };

       
        function load() {
            $scope.scopeModel.isLoading = true;
            loadPageViewType().then(function () {
                loadAllControls().catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadPageViewType() {
            return VR_Sec_ViewAPIService.GetView(viewId).then(function (response) {
                routeTableViewTypesSelectorDefferedReady.promise.then(function () {
                    var routTableViewTypes = UtilsService.getArrayEnum(NP_IVSwitch_RouteTableViewTypeEnum);
                    if (routTableViewTypes != undefined)
                    {
                        for (var i = 0; i < routTableViewTypes.length; i++) {
                            var item = routTableViewTypes[i];
                            if (response.Settings.Types.includes(item.value))
                                $scope.scopeModel.routeTableViewType.push(item);
                        }
                        routeTableViewTypesSelectorAPI.selectFirstItem();
                    }
                });
            });
        }

        function getEndPoints() {
            var endPointsArray = [];
            var endPoints = endPointSelectorDirectiveAPI.getSelectedIds();
            if (endPoints != undefined)
                for (var index = 0; index < endPoints.length; index++) {
                    var itemTab = endPoints[index];
                    endPointsArray.push(itemTab);
                }
            return endPointsArray;
        }

        function getCustomerIds() {
            var customerIdsArray = [];
            var customerIds = customerSelectorDirectiveAPI.getSelectedIds();
            if (customerIds != undefined)
                for (var index = 0; index < customerIds.length; index++) {
                    var itemTab = customerIds[index];
                    customerIdsArray.push(itemTab);
                }

            return customerIdsArray;
        }

        function getFilter() {

            return {
                query: {
                    RouteTableViewType: (routeTableViewType == NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value) ? NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value : NP_IVSwitch_RouteTableViewTypeEnum.Whitelist.value,
                    Name: $scope.scopeModel.name,
                    CustomerIds: getCustomerIds(),
                    EndPoints: getEndPoints()
                       }
                  };
        }

        function loadAllControls()
        {

            function gridDirective() {
                return GridDirectiveDefferedReady.promise.then(function () {
                    gridApi.load({ Filter: getFilter(), RouteTableViewType: routeTableViewType });
                });
            }

            function customerDirective() {
                return CustomerSelectorDirectiveDefferedReady.promise.then(function () {
                    var directivePayload;
                    VRUIUtilsService.callDirectiveLoad(customerSelectorDirectiveAPI, undefined, undefined)
                });
            }

            return UtilsService.waitMultipleAsyncOperations([gridDirective, customerDirective]).then(function () {
                selectedCustomerDeffered = undefined;
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () { $scope.scopeModel.isLoading = false; });

        }

    };

    appControllers.controller('NP_IVSwitch_RouteTableManagementController', routeTableManagementController);
})(appControllers);