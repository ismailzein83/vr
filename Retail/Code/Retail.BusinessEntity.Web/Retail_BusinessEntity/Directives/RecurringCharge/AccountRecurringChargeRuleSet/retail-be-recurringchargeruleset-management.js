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

            //var productDefinitionId;
            //var packageNameByIds;

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

                    Retail_BE_RecurringChargeService.addAccountRecurringChargeRuleSet(onRecurringChargeRuleSetAdded);
                };
                $scope.scopeModel.onDeleteRecurringChargeRuleSet = function (deletedRecurringChargeRuleSet) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.recurringChargeRuleSets, deletedRecurringChargeRuleSet.Entity.PackageName, 'Entity.PackageName');
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
                        //productDefinitionId = payload.productDefinitionId;
                        //packageNameByIds = payload.packageNameByIds;
                        recurringChargeRuleSets = payload.recurringChargeRuleSets;
                    }

                    //Loading Grid
                    var recurringChargeRuleSetsByPriority = {};
                    if (recurringChargeRuleSets != undefined) {
                        for (var index = 0; index < recurringChargeRuleSets.length; index++) {
                            var currentRecurringChargeRuleSet = recurringChargeRuleSets[index];
                            //currentRecurringChargeRuleSet = extendRecurringChargeRuleSetObj(currentRecurringChargeRuleSet);
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
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.recurringChargeRuleSets, updatedRecurringChargeRuleSet.PackageName, 'Entity.PackageName');
                    $scope.scopeModel.recurringChargeRuleSets[index] = { Entity: updatedRecurringChargeRuleSet };
                };

                console.log(recurringChargeRuleSet);

                Retail_BE_RecurringChargeService.editAccountRecurringChargeRuleSet(recurringChargeRuleSet.Entity, onRecurringChargeRuleSetUpdated);
            }

            //function extendRecurringChargeRuleSetObj(recurringChargeRuleSet) {
            //    if (recurringChargeRuleSet == undefined)
            //        return;

            //    recurringChargeRuleSet.PackageName = packageNameByIds[recurringChargeRuleSet.PackageId];
            //    return recurringChargeRuleSet;
            //}
            //function getExcludedPackageIds() {
            //    var recurringChargeRuleSets = $scope.scopeModel.recurringChargeRuleSets;
            //    if (recurringChargeRuleSets.length == 0)
            //        return;

            //    var excludedPackageIds = [];
            //    for (var i = 0; i < recurringChargeRuleSets.length; i++) {
            //        var currentRecurringChargeRuleSet = recurringChargeRuleSets[i].Entity;
            //        excludedPackageIds.push(currentRecurringChargeRuleSet.PackageId);
            //    }
            //    return excludedPackageIds;
            //}
        }
    }

    app.directive('retailBeRecurringchargerulesetManagement', RecurringChargeRuleSetManagementDirective);

})(app);