"use strict";

app.directive("npIvswitchRouteoptionGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_CustomerRouteAPIService",
    function (utilsService, vrNotificationService, npIvSwitchCustomerRouteApiService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var routeManagement = new RouteManagement($scope, ctrl, $attrs);
                routeManagement.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/NP_IVSwitch/Directives/CustomerRoute/Templates/RouteOptionGridTemplate.html"
        };

        function RouteManagement($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                }
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return npIvSwitchCustomerRouteApiService.GetFilteredCustomerRouteOptions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);