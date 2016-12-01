(function (app) {

    'use strict';

    ServicetypeChargingpolicyPartseditorDirective.$inject = ['Retail_BE_ServiceTypeService', 'UtilsService'];

    function ServicetypeChargingpolicyPartseditorDirective(Retail_BE_ServiceTypeService, UtilsService) {
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
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ServiceType/ChargingPolicyDefinition/Templates/ChargingPolicyPartsEditor.html"
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
                    var onPartTypeAdded = function (partObj) {
                        ctrl.parts.push({
                            Entity: partObj.Part
                        });
                    };
                    Retail_BE_ServiceTypeService.addPartType(onPartTypeAdded, getContext());
                };

                ctrl.removePart = function (partObj) {
                    ctrl.parts.splice(ctrl.parts.indexOf(partObj), 1);
                };

                ctrl.validateParts = function () {
                    if (ctrl.parts.length > 0)
                        return null;
                    return "One part at least should be added.";
                };
                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.parts.length = 0;
                        if (payload.parts) {
                            for (var i = 0; i < payload.parts.length; i++) {
                                var currentItemAction = payload.parts[i];
                                ctrl.parts.push({
                                    Entity: currentItemAction
                                });
                            }
                        }
                    }
                };

                api.getData = function () {
                    var parts = [];
                    for (var i = 0; i < ctrl.parts.length; i++) {
                        var part = ctrl.parts[i];
                        parts.push(part.Entity);
                    }
                    return parts;
                };

                return api;
            }

            function defineMenuActions() {
                ctrl.partsGridMenuActions = [{
                    name: 'Edit',
                    clicked: editPartType
                }];
            }

            function editPartType(part) {
                var onPartTypeUpdated = function (partObj) {
                    ctrl.parts[ctrl.parts.indexOf(part)] = {
                        Entity: partObj.Part
                    };
                };
                Retail_BE_ServiceTypeService.editPartType(part.Entity, onPartTypeUpdated, getContext(part));
            }

            function getContext(part)
            {
                var context = {
                    checkIfPartTypeUsed: function (partTypeId) {
                        if (part != undefined && partTypeId == part.Entity.PartTypeId)
                            return false;
                        if (UtilsService.getItemByVal(ctrl.parts, partTypeId, "Entity.PartTypeId") != undefined)
                            return true;
                        return false;
                    }
                };
                return context;
            }
        }
    }

    app.directive('retailBeServicetypeChargingpolicyPartseditor', ServicetypeChargingpolicyPartseditorDirective);

})(app);