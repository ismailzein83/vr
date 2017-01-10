(function (app) {

    'use strict';

    RecurringChargeRuleManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_RecurringChargeService'];

    function RecurringChargeRuleManagementDirective(UtilsService, VRNotificationService, Retail_BE_RecurringChargeService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecurringChargeRuleManagementCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRule/Templates/RecurringChargeRuleManagementTemplate.html'
        };

        function RecurringChargeRuleManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.recurringChargeRules = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddRecurringChargeRule = function () {
                    var onRecurringChargeRuleAdded = function (addedRecurringChargeRule) {
                        $scope.scopeModel.recurringChargeRules.push({ Entity: addedRecurringChargeRule });
                    };

                    Retail_BE_RecurringChargeService.addAccountRecurringChargeRule(getRecurringChargeRuleNames(), onRecurringChargeRuleAdded);
                };
                $scope.scopeModel.onDeleteRecurringChargeRule = function (deletedRecurringChargeRule) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.recurringChargeRules, deletedRecurringChargeRule.Entity.Name, 'Entity.Name');
                            $scope.scopeModel.recurringChargeRules.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var recurringChargeRules;

                    if (payload != undefined) {
                        recurringChargeRules = payload.chargeRules;
                    }

                    //Loading Grid
                    if (recurringChargeRules != undefined) {
                        for (var index = 0; index < recurringChargeRules.length; index++) {
                            var currentRecurringChargeRule = recurringChargeRules[index];
                            $scope.scopeModel.recurringChargeRules.push({ Entity: currentRecurringChargeRule });
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var recurringChargeRules;
                    if ($scope.scopeModel.recurringChargeRules.length > 0) {
                        recurringChargeRules = [];
                        for (var index = 0; index < $scope.scopeModel.recurringChargeRules.length; index++) {
                            var recurringChargeRule = $scope.scopeModel.recurringChargeRules[index].Entity;
                            recurringChargeRules.push(recurringChargeRule);
                        }
                    }
                    return recurringChargeRules;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editRecurringChargeRule
                }];
            }
            function editRecurringChargeRule(recurringChargeRule) {
                var onRecurringChargeRuleUpdated = function (updatedRecurringChargeRule) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.recurringChargeRules, recurringChargeRule.Entity.Name, 'Entity.Name');
                    $scope.scopeModel.recurringChargeRules[index] = { Entity: updatedRecurringChargeRule };
                };

                Retail_BE_RecurringChargeService.editAccountRecurringChargeRule(recurringChargeRule.Entity, getRecurringChargeRuleNames(), onRecurringChargeRuleUpdated);
            }

            function getRecurringChargeRuleNames() {
                if ($scope.scopeModel.recurringChargeRules == undefined)
                    return;

                var names = [];
                for (var index = 0; index < $scope.scopeModel.recurringChargeRules.length; index++) {
                    names.push($scope.scopeModel.recurringChargeRules[index].Entity.Name)
                }
                return names;
            }
        }
    }

    app.directive('retailBeRecurringchargeruleManagement', RecurringChargeRuleManagementDirective);

})(app);