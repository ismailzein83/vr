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

            var area;
            var site;
            var selectedValues;
            var parentFieldValues;
            var isAddMode;

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


                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        parentFieldValues = payload.parentFieldValues;
                        isAddMode = payload.isAddMode;

                        if (isAddMode) {
                            if (parentFieldValues != undefined) {
                                area = parentFieldValues.Area.value != undefined ? parentFieldValues.Area.value : parentFieldValues.Area;
                                site = parentFieldValues.Site.value != undefined ? parentFieldValues.Site.value : parentFieldValues.Site;
                            }
                        } else {
                            area = selectedValues.Area;
                            site = selectedValues.Site;
                        }
                    }

                    var loadFirstPortDirectivePromise = loadFirstPortDirective();
                    promises.push(loadFirstPortDirectivePromise);
                    var loadSecondPortDirectivePromise = loadSecondPortDirective();
                    promises.push(loadSecondPortDirectivePromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Port1: firstPortDirectiveAPI.getSelectedIds(),
                        Port2: secondPortDirectiveAPI.getSelectedIds()
                    };
                };

                api.clearDataSource = function () {
                    firstPortDirectiveAPI.clearDataSource();
                    secondPortDirectiveAPI.clearDataSource();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function loadFirstPortDirective() {
                var firstPortLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                firstPortDirectiveReadyDeferred.promise.then(function () {
                    var firstPortDirectivePayload = {
                        selectedIds: selectedValues != undefined ? selectedValues.Port1 : undefined,
                        area: area,
                        site: site,
                        connectionDirection: 1,
                        isAddMode: isAddMode
                    };

                    if (parentFieldValues != undefined) {
                        firstPortDirectivePayload.parentFieldValues = {
                            NodeId: parentFieldValues.Port1Node != undefined ? parentFieldValues.Port1Node.value : undefined,
                            NodePartId: parentFieldValues.Port1Part != undefined ? parentFieldValues.Port1Part.value : undefined,
                            NodePortId: parentFieldValues.Port1 != undefined ? parentFieldValues.Port1.value : undefined
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(firstPortDirectiveAPI, firstPortDirectivePayload, firstPortLoadPromiseDeferred);

                });
                return firstPortLoadPromiseDeferred.promise;
            }

            function loadSecondPortDirective() {
                var secondPortLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                secondPortDirectiveReadyDeferred.promise.then(function () {
                    var secondPortDirectivePayload = {
                        selectedIds: selectedValues != undefined ? selectedValues.Port2 : undefined,
                        area: area,
                        site: site,
                        connectionDirection: 2,
                        isAddMode: isAddMode
                    };
                    if (parentFieldValues != undefined) {
                        secondPortDirectivePayload.parentFieldValues = {
                            NodeId: parentFieldValues.Port2Node != undefined ? parentFieldValues.Port2Node.value : undefined,
                            NodePartId: parentFieldValues.Port2Part != undefined ? parentFieldValues.Port2Part.value : undefined,
                            NodePortId: parentFieldValues.Port2 != undefined ? parentFieldValues.Port2.value : undefined,
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(secondPortDirectiveAPI, secondPortDirectivePayload, secondPortLoadPromiseDeferred);

                });
                return secondPortLoadPromiseDeferred.promise;
            }

        }
        return directiveDefinitionObject;
    }]);

