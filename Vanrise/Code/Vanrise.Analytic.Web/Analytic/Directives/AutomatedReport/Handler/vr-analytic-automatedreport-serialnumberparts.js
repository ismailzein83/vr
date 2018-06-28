"use strict";

app.directive("vrAnalyticAutomatedreportSerialnumberparts", ["UtilsService", "VRNotificationService", "VR_Analytic_AutomatedReportSerialNumberSettingsService",
    function (UtilsService, VRNotificationService, VR_Analytic_AutomatedReportSerialNumberSettingsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SerialNumberParts($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Analytic/Directives/AutomatedReport/Handler/Templates/SerialNumberPartsTemplate.html"

        };

        function SerialNumberParts($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "At least one part should be added.";
                };

                ctrl.addSerialNumberPart = function () {
                    var onSerialNumberPartAdded = function (serialNumberPart) {
                        ctrl.datasource.push({ Entity: serialNumberPart });
                    };

                    VR_Analytic_AutomatedReportSerialNumberSettingsService.addSerialNumberPart(onSerialNumberPartAdded, getContext());
                };

                ctrl.removeSerialNumberPart = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var serialNumberParts;
                    if (ctrl.datasource != undefined) {
                        serialNumberParts = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            serialNumberParts.push(currentItem.Entity);
                        }
                    }
                    return serialNumberParts;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.serialNumberParts != undefined) {
                            for (var i = 0; i < payload.serialNumberParts.length; i++) {
                                var serialNumberPart = payload.serialNumberParts[i];
                                ctrl.datasource.push({ Entity: serialNumberPart });
                            }
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSerialNumberPart,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editSerialNumberPart(serialNumberPartObj) {
                var onSerialNumberPartUpdated = function (serialNumberPart) {
                    var index = ctrl.datasource.indexOf(serialNumberPartObj);
                    ctrl.datasource[index] = { Entity: serialNumberPart };
                };

                VR_Analytic_AutomatedReportSerialNumberSettingsService.editSerialNumberPart(serialNumberPartObj.Entity, onSerialNumberPartUpdated, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);