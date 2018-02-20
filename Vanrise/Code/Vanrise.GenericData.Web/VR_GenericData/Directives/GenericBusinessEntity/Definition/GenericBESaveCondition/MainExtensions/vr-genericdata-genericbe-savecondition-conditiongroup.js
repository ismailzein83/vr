"use strict";

app.directive("vrGenericdataGenericbeSaveconditionConditiongroup", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ConditionGroupGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBESaveCondition/MainExtensions/Templates/SaveConditionConditionGroupTemplate.html"
        };

        function ConditionGroupGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one condition.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each condition should be unique.";

                    return null;
                };

                ctrl.addConditionGroup = function () {
                    var onConditionGroupAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEConditionGroup(onConditionGroupAdded, getContext());
                };
              
                ctrl.removeConditionGroup = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };


                defineMenuActions();

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var conditions;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        conditions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            conditions.push({
                                Name: currentItem.Name,
                                ApplicableOnOldEntity: currentItem.ApplicableOnOldEntity,
                                Condition: currentItem.Condition
                            });
                        }
                    }

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBESaveConditions.ConditionGroupSaveCondition, Vanrise.GenericData.MainExtensions",
                        Operator: $scope.scopeModel.operator,
                        Conditions: conditions
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        api.clearDataSource();
                        if (payload.settings != undefined && payload.settings.Conditions != undefined) {
                            $scope.scopeModel.operator = payload.settings.Operator;

                            var data = payload.settings.Conditions;
                            for (var i = 0; i < data.length; i++) {
                                ctrl.datasource.push(data[i]);
                            }
                        }
                    }
                };


                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editConditionGroup
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editConditionGroup(conditionGroupObj) {
                var onConditionalHandlerUpdated = function (conditionalGroup) {
                    var index = ctrl.datasource.indexOf(conditionGroupObj);
                    ctrl.datasource[index] = conditionalGroup;
                };
                VR_GenericData_GenericBEDefinitionService.editGenericBEConditionGroup(onConditionalHandlerUpdated, conditionGroupObj, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i];
                    for (var j = 0; j < ctrl.datasource.length; j++) {
                        if (i != j && ctrl.datasource[j].Name == currentItem.Name)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);