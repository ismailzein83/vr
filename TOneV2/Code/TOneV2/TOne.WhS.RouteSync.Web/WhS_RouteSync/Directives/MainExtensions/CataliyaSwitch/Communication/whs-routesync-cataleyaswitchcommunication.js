(function (app) {

    'use strict';

    whsRoutesyncCataleyaswitchcommunication.$inject = ["UtilsService", "VRUIUtilsService"];

    function whsRoutesyncCataleyaswitchcommunication(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SwitchCommunicationCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/CataliyaSwitch/Communication/Templates/CataleyaSwitchCommunicationTemplate.html"
        };

        function SwitchCommunicationCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorAPI;
            var connectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var vrConnectionId;

                    if (payload != undefined) {
                        vrConnectionId = payload.vrConnectionId;
                    }

                    var loadConnectionSelectorLoadPromise = getLoadConnectionSelectorPromise();
                    promises.push(loadConnectionSelectorLoadPromise);

                    function getLoadConnectionSelectorPromise() {
                        var connectionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        connectionSelectorReadyDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    ConnectionTypeIds: ["071D54D2-463B-4404-8219-45FCD539FF01"] // VRHttpConnectionFilter
                                }
                            };

                            if (vrConnectionId != undefined) {
                                selectorPayload.selectedIds = vrConnectionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, selectorPayload, connectionSelectorLoadPromiseDeferred);
                        });

                        return connectionSelectorLoadPromiseDeferred.promise;
                    }

                    var rootPromiseNode = {
                        promises: promises
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return connectionSelectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncCataleyaswitchcommunication', whsRoutesyncCataleyaswitchcommunication);

})(app);