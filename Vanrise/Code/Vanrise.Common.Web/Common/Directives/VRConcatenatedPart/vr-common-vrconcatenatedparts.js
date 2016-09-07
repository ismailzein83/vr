"use strict";

app.directive("vrCommonVrconcatenatedparts", ["UtilsService", "VRNotificationService", "VRCommon_VRConcatenatedPartService",
    function (UtilsService, VRNotificationService, VRCommon_VRConcatenatedPartService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ConcatenatedParts($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/VRConcatenatedPart/Templates/VRConcatenatedPartsTemplate.html"

        };

        function ConcatenatedParts($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one part.";
                }

                ctrl.addConcatenatedPart = function () {
                    var onConcatenatedPartAdded = function (concatenatedPart) {
                        ctrl.datasource.push({ Entity: concatenatedPart });
                    }

                    VRCommon_VRConcatenatedPartService.addConcatenatedPart(onConcatenatedPartAdded, getContext());
                };

                ctrl.removeConcatenatedPart = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var concatenatedParts;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        concatenatedParts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            concatenatedParts.push(currentItem.Entity);
                        }
                    }
                    return concatenatedParts;
                }

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.concatenatedParts != undefined) {
                            for (var i = 0; i < payload.concatenatedParts.length; i++) {
                                var concatenatedPart = payload.concatenatedParts[i];
                                ctrl.datasource.push({ Entity: concatenatedPart });
                            }
                        }
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editConcatenatedPart,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editConcatenatedPart(concatenatedPartObj) {
                var onConcatenatedPartUpdated = function (concatenatedPart) {
                    var index = ctrl.datasource.indexOf(concatenatedPartObj);
                    ctrl.datasource[index] = { Entity: concatenatedPart };
                }

                VRCommon_VRConcatenatedPartService.editConcatenatedPart(concatenatedPartObj.Entity, onConcatenatedPartUpdated, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);