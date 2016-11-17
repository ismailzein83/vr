"use strict";

app.directive("vrBebridgeProcessScheduled", ["VRUIUtilsService", "UtilsService",
    function (VRUIUtilsService, UtilsService) {
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
            templateUrl: "/Client/Modules/VR_BEBridge/Directives/ProcessInput/Scheduled/Templates/BEBridgeProcessScheduledTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;
            var beRecieveDefinitionSelectorAPI;
            var beRecieveDefinitionSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onBERecieveDefintionSelectorReady = function (api) {
                    beRecieveDefinitionSelectorAPI = api;
                    beRecieveDefinitionSelectorAPIReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.BEBridge.BP.Arguments.SourceBESyncProcessInput, Vanrise.BEBridge.BP.Arguments",
                            BEReceiveDefinitionIds: beRecieveDefinitionSelectorAPI.getSelectedIds()
                        }
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    promises.push(loadRouteSyncDefinitionSelector());
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadRouteSyncDefinitionSelector() {
                var beRecieveDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                beRecieveDefinitionSelectorAPIReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(beRecieveDefinitionSelectorAPI, undefined, beRecieveDefinitionSelectorLoadDeferred);
                });

                return beRecieveDefinitionSelectorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);
