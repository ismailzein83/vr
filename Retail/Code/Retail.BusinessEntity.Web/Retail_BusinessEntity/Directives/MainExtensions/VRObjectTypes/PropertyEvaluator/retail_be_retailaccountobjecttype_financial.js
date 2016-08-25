(function (app) {

    'use strict';

    RetailAccountFinancial.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService','Retail_BE_FinancialRetailAccountEnum'];

    function RetailAccountFinancial(UtilsService, VRUIUtilsService, VRNotificationService, Retail_BE_FinancialRetailAccountEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var retailAccountObjectType = new RetailAccountObjectType($scope, ctrl, $attrs);
                retailAccountObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/VRObjectTypes/PropertyEvaluator/Templates/FinancialRetailAccountEvaluatorTemplate.html"

        };
        function RetailAccountObjectType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.financialRetailAccount = UtilsService.getArrayEnum(Retail_BE_FinancialRetailAccountEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                    {
                        $scope.scopeModel.selectedFinancialRetailAccount = UtilsService.getItemByVal($scope.scopeModel.financialRetailAccount, payload.objectPropertyEvaluator.FinancialField, "value");
                    }
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.VRObjectTypes.FinancialRetailAccountPropertyEvaluator, Retail.BusinessEntity.MainExtensions",
                        FinancialField: $scope.scopeModel.selectedFinancialRetailAccount.value
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeRetailaccountobjecttypeFinancial', RetailAccountFinancial);

})(app);