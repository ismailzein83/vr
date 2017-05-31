'use strict';
app.directive('retailBeAccountbedefinitionFinancialaccountlocatorDefault', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new financialAccountLocatorCtor(ctrl, $scope);
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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/FinancialAccountLocator/MainExtensions/Templates/DefaultFinancialAccountLocatorTemplate.html';
            }

        };

        function financialAccountLocatorCtor(ctrl, $scope) {
            var accountBEDefinitionId;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.useFinancialAccountModule = true;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        var  financialAccountLocator = payload.financialAccountLocator;
                        if (financialAccountLocator != undefined)
                            $scope.scopeModel.useFinancialAccountModule = financialAccountLocator.UseFinancialAccountModule;
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.DefaultFinancialAccountLocator,Retail.BusinessEntity.Business",
                        UseFinancialAccountModule: $scope.scopeModel.useFinancialAccountModule
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);