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

        var selectedCustomerDeffered = UtilsService.createPromiseDeferred();

        var selectedEndPoints = [];
        var routeTableViewType;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            var viewId = parameters.viewId;
            VR_Sec_ViewAPIService.GetView(viewId).then(function (response) {
                if (response.Settings.Type == 0)
                    routeTableViewType = NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value;
                else
                    routeTableViewType = NP_IVSwitch_RouteTableViewTypeEnum.Whitelist.value;


            });
        };


        function defineScope() {
            $scope.scopeModel = {};

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

            function gridDirective() {
                return GridDirectiveDefferedReady.promise.then(function () {
                    gridApi.load({ Filter: getFilter(), RouteTableViewType: routeTableViewType });
                });
            };

            function cusomerDirective() {
                return CustomerSelectorDirectiveDefferedReady.promise.then(function () {

                    var directivePayload;
                    VRUIUtilsService.callDirectiveLoad(customerSelectorDirectiveAPI, undefined, undefined)
                });
            };

            return UtilsService.waitMultipleAsyncOperations([gridDirective, cusomerDirective]).then(function () {

                selectedCustomerDeffered = undefined;

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () { $scope.scopeModel.isLoading = false; });

        };


        function getEndPoints() {
            var endPointsArray = [];
            var endPoints = endPointSelectorDirectiveAPI.getSelectedIds();
            if (endPoints != undefined)
                for (var index = 0; index < endPoints.length; index++) {
                    var itemTab = endPoints[index];
                    endPointsArray.push(itemTab);
                }
            return endPointsArray;
        };


        function getCustomerIds() {
            var customerIdsArray = [];
            var customerIds = customerSelectorDirectiveAPI.getSelectedIds();
            if (customerIds != undefined)
                for (var index = 0; index < customerIds.length; index++) {
                    var itemTab = customerIds[index];
                    customerIdsArray.push(itemTab);
                }

            return customerIdsArray;
        };


        function getFilter() {

            return {
                query: {
                    RouteTableViewType: (routeTableViewType == NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value) ? NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value : NP_IVSwitch_RouteTableViewTypeEnum.Whitelist.value,
                    Name: $scope.scopeModel.name,
                    CustomerIds: getCustomerIds(),
                    EndPoints: getEndPoints()
                       }
                  };
        };


    };

    appControllers.controller('NP_IVSwitch_RouteTableManagementController', routeTableManagementController);
})(appControllers);