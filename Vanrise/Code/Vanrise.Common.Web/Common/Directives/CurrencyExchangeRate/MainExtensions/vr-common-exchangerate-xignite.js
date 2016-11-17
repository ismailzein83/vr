"use strict";

app.directive("vrCommonExchangerateFxsauder", ['VRCommon_ConnectionStringService', function (VRCommon_ConnectionStringService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Common/Directives/CurrencyExchangeRate/MainExtensions/Templates/ExchangeRateXigniteTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.url = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};         
            ctrl.connections = [];
            ctrl.isConnectionsGridValid = function () {
                if (ctrl.connections.length == 0 && $scope.enableRateUpdate == false) {
                    return 'At least one connection string must be added.';
                }
                return null;
            };
            ctrl.addConnection = function () {
                var onConnectionAdded = function (connectionObj) {
                    ctrl.connections.push(connectionObj);
                };
                VRCommon_ConnectionStringService.addConnectionString(onConnectionAdded);
            };
            ctrl.removeConnection = function (source) {
                ctrl.connections.splice(ctrl.connections.indexOf(source), 1);
            };

            ctrl.connectionsGridMenuActions = [{
                name: 'Edit',
                clicked: editSource
            }];
            function editSource(connection) {
                var onConnectionUpdated = function (connectionObj) {
                    ctrl.connections[ctrl.connections.indexOf(connection)] = connectionObj;
                };

                VRCommon_ConnectionStringService.editConnectionString(connection, onConnectionUpdated);
            }

            api.getData = function () {
                
                return {   
                    $type: "Vanrise.Common.MainExtensions.ExchangeRateTaskActionArgument, Vanrise.Common.MainExtensions",
                    URL: $scope.url,
                    Token: $scope.token,
                    ConnectionStrings: ctrl.connections,
                    EnableRateUpdate: $scope.enableRateUpdate
                };
            };


            api.load = function (payload) {
                $scope.url = "http://globalcurrencies.xignite.com/";
                $scope.enableRateUpdate = true;

                if (payload != undefined && payload.data != undefined) {
                    $scope.url = payload.data.URL;
                    $scope.token = payload.data.Token;
                    $scope.enableRateUpdate = payload.data.EnableRateUpdate;
                    if (payload.data.ConnectionStrings && payload.data.ConnectionStrings.length > 0) {
                        for (var y = 0; y < payload.data.ConnectionStrings.length; y++) {
                            var currentObj = payload.data.ConnectionStrings[y];
                            ctrl.connections.push(currentObj);
                        }
                    }

                }
            };


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
