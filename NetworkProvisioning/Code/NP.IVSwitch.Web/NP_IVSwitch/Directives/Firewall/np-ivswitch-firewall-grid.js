﻿"use strict";

app.directive("npIvswitchFirewallGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_LiveCdrAPIService",
function (utilsService, vrNotificationService, NP_IVSwitch_LiveCdrAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var firewallGrid = new FirewallGrid($scope, ctrl, $attrs);
            firewallGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/Firewall/Templates/FirewallGridTemplate.html"
    };

    function FirewallGrid($scope, ctrl, $attrs) {
        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.menuActions = [];
            $scope.firewalls = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return NP_IVSwitch_LiveCdrAPIService.GetFilteredLiveCdrs(dataRetrievalInput)
                   .then(function (response) {
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       vrNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };
            defineMenuActions();
        }

        function defineAPI() {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;

}]);
