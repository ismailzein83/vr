"use strict";

app.directive('vrWhsRoutingCustomerrouteGrid', ['VRNotificationService', 'WhS_Routing_CustomerRouteAPIService', 'WhS_BE_MainService',
function (VRNotificationService, WhS_Routing_CustomerRouteAPIService, WhS_BE_MainService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new customerRouteGrid($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteGridTemplate.html"

    };

    function customerRouteGrid($scope, ctrl, $attrs) {
        var gridAPI;

        function initializeController() {
            $scope.customerRoutes = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_Routing_CustomerRouteAPIService.GetFilteredCustomerRoutes(dataRetrievalInput)
                   .then(function (response) {
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       VRNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Applied Rule",
                clicked: openRouteRuleEditor,
            }
            ];
        }

        function openRouteRuleEditor(dataItem) {
            WhS_Routing_RouteRuleService.editRouteRule(dataItem.ExecutedRuleId);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
