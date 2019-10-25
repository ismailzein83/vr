"use strict";

app.directive("retailNimConnectionDirective", ["VRUIUtilsService", "UtilsService",
    function (VRUIUtilsService, UtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                hideremoveicon: '@',
                normalColNum: '@',
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ConnectionCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_NIM/Directives/Connection/Templates/ConnectionDirectiveTemplate.html"
        };

        function ConnectionCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var firstPortDirectiveAPI;
            var firstPortDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var secondPortDirectiveAPI;
            var secondPortDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFirstPortDirectiveReady = function (api) {
                    firstPortDirectiveAPI = api;
                    firstPortDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onSecondPortDirectiveReady = function (api) {
                    secondPortDirectiveAPI = api;
                    secondPortDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([firstPortDirectiveReadyDeferred.promise, secondPortDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var connectionType;
                    var firstPort;
                    var secondPort;


                    if (payload != undefined) {
                        firstPort = payload.Port1;
                        secondPort = payload.Port2;
                    }
                    var loadFirstPortDirectivePromise = loadFirstPortDirective(firstPort);
                    promises.push(loadFirstPortDirectivePromise);
                    var loadSecondPortDirectivePromise = loadSecondPortDirective(secondPort);
                    promises.push(loadSecondPortDirectivePromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Port1: firstPortDirectiveAPI.getSelectedIds(),
                        Port2: secondPortDirectiveAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function loadFirstPortDirective(firstPort) {
                var firstPortLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                firstPortDirectiveReadyDeferred.promise.then(function () {
                    var firstPortDirectivePayload = {
                        selectedIds: firstPort
                    };
                    VRUIUtilsService.callDirectiveLoad(firstPortDirectiveAPI, firstPortDirectivePayload, firstPortLoadPromiseDeferred);

                });
                return firstPortLoadPromiseDeferred.promise;
            }

            function loadSecondPortDirective(secondPort) {
                var secondPortLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                secondPortDirectiveReadyDeferred.promise.then(function () {
                    var secondPortDirectivePayload = {
                        selectedIds: secondPort
                    };
                    VRUIUtilsService.callDirectiveLoad(secondPortDirectiveAPI, secondPortDirectivePayload, secondPortLoadPromiseDeferred);

                });
                return secondPortLoadPromiseDeferred.promise;
            }

        }
        return directiveDefinitionObject;
    }]);

