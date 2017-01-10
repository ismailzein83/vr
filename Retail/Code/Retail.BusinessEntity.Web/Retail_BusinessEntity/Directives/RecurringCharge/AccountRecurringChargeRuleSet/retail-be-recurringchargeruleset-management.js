(function (app) {

    'use strict';

    RecurringChargeRuleSetManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_RecurringChargeService'];

    function RecurringChargeRuleSetManagementDirective(UtilsService, VRNotificationService, Retail_BE_RecurringChargeService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecurringChargeRuleSetManagementCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/RecurringCharge/AccountRecurringChargeRuleSet/Templates/RecurringChargeRuleSetManagementTemplate.html'
        };

        function RecurringChargeRuleSetManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.recurringChargeRuleSets = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddRecurringChargeRuleSet = function () {
                    var onRecurringChargeRuleSetAdded = function (addedRecurringChargeRuleSet) {
                        $scope.scopeModel.recurringChargeRuleSets.push({ Entity: addedRecurringChargeRuleSet });
                    };

                    Retail_BE_RecurringChargeService.addAccountRecurringChargeRuleSet(getRecurringChargeRuleSetNames(), onRecurringChargeRuleSetAdded);
                };
                $scope.scopeModel.onDeleteRecurringChargeRuleSet = function (deletedRecurringChargeRuleSet) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.recurringChargeRuleSets, deletedRecurringChargeRuleSet.Entity.Name, 'Entity.Name');
                            $scope.scopeModel.recurringChargeRuleSets.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var recurringChargeRuleSets;

                    if (payload != undefined) {
                        recurringChargeRuleSets = payload.recurringChargeRuleSets;
                    }

                    //Loading Grid
                    var recurringChargeRuleSetsByPriority = {};
                    if (recurringChargeRuleSets != undefined) {
                        for (var index = 0; index < recurringChargeRuleSets.length; index++) {
                            var currentRecurringChargeRuleSet = recurringChargeRuleSets[index];
                            $scope.scopeModel.recurringChargeRuleSets.push({ Entity: currentRecurringChargeRuleSet });
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var recurringChargeRuleSets;
                    if ($scope.scopeModel.recurringChargeRuleSets.length > 0) {
                        recurringChargeRuleSets = [];
                        for (var index = 0; index < $scope.scopeModel.recurringChargeRuleSets.length; index++) {
                            var recurringChargeRuleSet = $scope.scopeModel.recurringChargeRuleSets[index].Entity;
                            recurringChargeRuleSets.push(recurringChargeRuleSet);
                        }
                    }
                    return recurringChargeRuleSets;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editRecurringChargeRuleSet
                }];
            }
            function editRecurringChargeRuleSet(recurringChargeRuleSet) {
                var onRecurringChargeRuleSetUpdated = function (updatedRecurringChargeRuleSet) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.recurringChargeRuleSets, updatedRecurringChargeRuleSet.Name, 'Entity.Name');
                    $scope.scopeModel.recurringChargeRuleSets[index] = { Entity: updatedRecurringChargeRuleSet };
                };

                Retail_BE_RecurringChargeService.editAccountRecurringChargeRuleSet(recurringChargeRuleSet.Entity, getRecurringChargeRuleSetNames(), onRecurringChargeRuleSetUpdated);
            }

            function getRecurringChargeRuleSetNames() {
                if ($scope.scopeModel.recurringChargeRuleSets == undefined)
                    return;

                var names = [];
                for (var index = 0; index < $scope.scopeModel.recurringChargeRuleSets.length; index++) {
                    names.push($scope.scopeModel.recurringChargeRuleSets[index].Entity.Name)
                }
                return names;
            }
        }
    }

    app.directive('retailBeRecurringchargerulesetManagement', RecurringChargeRuleSetManagementDirective);

})(app);