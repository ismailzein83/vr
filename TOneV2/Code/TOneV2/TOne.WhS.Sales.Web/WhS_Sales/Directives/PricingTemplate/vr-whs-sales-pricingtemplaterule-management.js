(function (app) {

    'use strict';

    PricingTemplateRuleManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'WhS_Sales_PricingTemplateService'];

    function PricingTemplateRuleManagementDirective(UtilsService, VRNotificationService, WhS_Sales_PricingTemplateService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PricingTemplateRuleManagementCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/WhS_Sales/Directives/PricingTemplate/Templates/PricingTemplateRuleManagementTemplate.html'
        };

        function PricingTemplateRuleManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.pricingTemplateRules = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddPricingTemplateRule = function () {
                    var onPricingTemplateRuleAdded = function (addedPricingTemplateRule) {
                        //extendPricingTemplateRuleObj(addedPricingTemplateRule);
                        $scope.scopeModel.pricingTemplateRules.push({ Entity: addedPricingTemplateRule });
                    };

                    WhS_Sales_PricingTemplateService.addPricingTemplateRule(onPricingTemplateRuleAdded);
                };
                $scope.scopeModel.onDeletePricingTemplateRule = function (pricingTemplateRule) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.pricingTemplateRules, pricingTemplateRule.Entity.FieldName, 'Entity.FieldName');
                            $scope.scopeModel.pricingTemplateRules.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    //var promises = [];

                    var pricingTemplateRules;

                    if (payload != undefined) {
                        pricingTemplateRules = payload.pricingTemplateRules;
                    }

                    //Loading PricingTemplateRules Grid
                    if (pricingTemplateRules != undefined) {
                        for (var index = 0; index < pricingTemplateRules.length; index++) {
                            var currentPricingTemplateRule = pricingTemplateRules[index];
                            extendPricingTemplateRuleObj(currentPricingTemplateRule);
                            $scope.scopeModel.pricingTemplateRules.push({ Entity: currentPricingTemplateRule });
                        }
                    }

                    //return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var pricingTemplateRules;
                    if ($scope.scopeModel.pricingTemplateRules.length > 0) {
                        pricingTemplateRules = [];
                        for (var i = 0; i < $scope.scopeModel.pricingTemplateRules.length; i++) {
                            var pricingTemplateRule = $scope.scopeModel.pricingTemplateRules[i].Entity;
                            pricingTemplateRules.push(pricingTemplateRule);
                        }
                    }

                    return pricingTemplateRules;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editPricingTemplateRuleDefinition
                }];
            }
            function editPricingTemplateRuleDefinition(pricingTemplateRule) {
                var onPricingTemplateRuleUpdated = function (updatedPricingTemplateRule) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.pricingTemplateRules, pricingTemplateRule.Entity.FieldName, 'Entity.FieldName');
                    extendPricingTemplateRuleObj(updatedPricingTemplateRule);
                    $scope.scopeModel.pricingTemplateRules[index] = { Entity: updatedPricingTemplateRule };
                };

                WhS_Sales_PricingTemplateService.editGridPricingTemplateRule(pricingTemplateRule.Entity, onPricingTemplateRuleUpdated);
            }

            function extendPricingTemplateRuleObj(pricingTemplateRule) {
                //if (accountFields == undefined)
                //    return;

                //for (var index = 0; index < accountFields.length; index++) {
                //    var currentAccountField = accountFields[index];
                //    if (pricingTemplateRule.FieldName == currentAccountField.Name) {
                //        pricingTemplateRule.FieldTitle = currentAccountField.Title;
                //        return;
                //    }
                //}
            }
        }
    }

    app.directive('vrWhsSalesPricingtemplateruleManagement', PricingTemplateRuleManagementDirective);

})(app);