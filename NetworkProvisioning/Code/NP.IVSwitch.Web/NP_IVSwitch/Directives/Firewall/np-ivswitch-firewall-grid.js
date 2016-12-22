"use strict";

app.directive("vrWhsBeSwitchGrid", ["UtilsService", "VRNotificationService",
function (utilsService, vrNotificationService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var switchGrid = new FirewallGrid($scope, ctrl, $attrs);
            switchGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/Firewall/Templates/FirewallGridTemplate.html"
    };

    function FirewallGrid($scope, ctrl, $attrs) {
        var gridAPI;

        function initializeController() {
            $scope.firewalls = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SwitchAPIService.GetFilteredSwitches(dataRetrievalInput)
                   .then(function (response) {
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       vrNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;

}]);
