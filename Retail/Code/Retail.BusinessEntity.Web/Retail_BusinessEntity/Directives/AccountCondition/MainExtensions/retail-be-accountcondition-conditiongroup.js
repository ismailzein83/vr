(function (app) {

    'use strict';

    ConditionGroupDirective.$inject = ['Retail_BE_AccountConditionService', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_LogicalOperatorEnum'];

    function ConditionGroupDirective(Retail_BE_AccountConditionService, UtilsService, VRUIUtilsService, Retail_BE_LogicalOperatorEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ConditionGroupCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountCondition/MainExtensions/Templates/ConditionGroupAccountConditionTemplate.html"
        };

        function ConditionGroupCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var gridAPI;
            function initializeController() {
                $scope.scopeModel = {}; 
                $scope.scopeModel.logicalOperators = UtilsService.getArrayEnum(Retail_BE_LogicalOperatorEnum);;

                ctrl.datasource = [];

                ctrl.addAccountCondition = function () {
                    var onAccountConditionItemAdded = function (accountCondition) {
                        ctrl.datasource.push({ Entity: accountCondition });
                    };
                    Retail_BE_AccountConditionService.addAccountCondition(accountBEDefinitionId, onAccountConditionItemAdded);
                };

                ctrl.removeAccountCondition = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var groupConditionAccountCondition;
                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        groupConditionAccountCondition = payload.accountCondition;
                        if (groupConditionAccountCondition != undefined) {

                            $scope.scopeModel.selectedLogicalOperator = UtilsService.getItemByVal($scope.scopeModel.logicalOperators, groupConditionAccountCondition.LogicalOperator, 'value');
                            if (groupConditionAccountCondition.AccountConditionItems != undefined) {
                                for (var i = 0; i < groupConditionAccountCondition.AccountConditionItems.length; i++) {
                                    var accountCondition = groupConditionAccountCondition.AccountConditionItems[i];
                                    ctrl.datasource.push({ Entity: accountCondition });
                                }
                            }
                        }

                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var accountConditions;
                    if (ctrl.datasource != undefined) {
                        accountConditions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            accountConditions.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountConditions.ConditionGroupAccountCondition, Retail.BusinessEntity.MainExtensions",
                        AccountConditionItems: accountConditions,
                        LogicalOperator : $scope.scopeModel.selectedLogicalOperator.value

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editAccountCondition,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }
            function editAccountCondition(accountConditionObj) {
                var onAccountConditionItemUpdated = function (accountCondition) {
                    var index = ctrl.datasource.indexOf(accountConditionObj);
                    ctrl.datasource[index] = { Entity: accountCondition };
                };

                Retail_BE_AccountConditionService.editAccountCondition(accountConditionObj.Entity, accountBEDefinitionId, onAccountConditionItemUpdated);
            }
        }
    }

    app.directive('retailBeAccountconditionConditiongroup', ConditionGroupDirective);

})(app);