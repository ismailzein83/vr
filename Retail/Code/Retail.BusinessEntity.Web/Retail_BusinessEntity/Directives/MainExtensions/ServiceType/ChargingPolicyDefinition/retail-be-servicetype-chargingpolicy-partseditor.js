(function (app) {

    'use strict';

    ServicetypeChargingpolicyPartseditorDirective.$inject = ['VR_Analytic_AnalyticItemActionService', 'UtilsService'];

    function ServicetypeChargingpolicyPartseditorDirective(VR_Analytic_AnalyticItemActionService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var servicetypeChargingpolicyPartseditor = new ServicetypeChargingpolicyPartseditor($scope, ctrl, $attrs);
                servicetypeChargingpolicyPartseditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/ServiceType/ChargingPolicyDefinition/Templates/ChargingPolicyPartsEditor.html"
        };

        function ServicetypeChargingpolicyPartseditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var counter = 0;
            function initializeController() {
                ctrl.parts = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addPart = function () {
                    var onPartAdded = function (partObj) {
                        ctrl.parts.push(partObj);
                    }
                    VR_Analytic_AnalyticItemActionService.addPart(onPartAdded);
                }

                ctrl.removePart = function (partObj) {
                    ctrl.parts.splice(ctrl.parts.indexOf(partObj), 1);
                }

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        ctrl.parts.length = 0;
                        if (payload.parts && payload.parts.length > 0) {
                            for (var y = 0; y < payload.parts.length; y++) {
                                var currentItemAction = payload.parts[y];
                                ctrl.parts.push(currentItemAction);
                            }
                        }
                    }
                };

                api.getData = function () {
                    var parts = [];
                    for (var i = 0; i < ctrl.parts.length ; i++) {
                        var part = ctrl.parts[i];
                        parts.push(part);
                    }
                    return parts;
                }

                return api;
            }

            function defineMenuActions() {
                ctrl.partsGridMenuActions = [{
                    name: 'Edit',
                    clicked: editPart
                }];
            }

            function editPart(part) {
                var onPartUpdated = function (partObj) {
                    ctrl.parts[ctrl.parts.indexOf(part)] = partObj;
                }
                VR_Analytic_AnalyticItemActionService.editPart(part, onPartUpdated);
            }
        }
    }

    app.directive('retailBeServicetypeChargingpolicyPartseditor', ServicetypeChargingpolicyPartseditorDirective);

})(app);