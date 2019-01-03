(function (app) {
    'use strict';
    AccountCreditLimitPropertyEvaluator.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountCreditLimitFieldEnum'];
    function AccountCreditLimitPropertyEvaluator(UtilsService, VRUIUtilsService, Retail_BE_AccountCreditLimitFieldEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountCreditLimitPropertyEvaluatorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/AccountCreditLimitPropertyEvaluatorTemplate.html"
        };

        function AccountCreditLimitPropertyEvaluatorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.propertyFields = UtilsService.getArrayEnum(Retail_BE_AccountCreditLimitFieldEnum);
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.objectPropertyEvaluator!=undefined) {
                        $scope.scopeModel.selectedPropertyField = UtilsService.getItemByVal($scope.scopeModel.propertyFields, payload.objectPropertyEvaluator.AccountCreditLimitField, 'value');
                        $scope.scopeModel.useDescription = payload.objectPropertyEvaluator != undefined ? payload.objectPropertyEvaluator.UseDescription : undefined;
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.VRObjectTypes.AccountCreditLimitPropertyEvaluator, Retail.BusinessEntity.MainExtensions",
                        AccountCreditLimitField: $scope.scopeModel.selectedPropertyField != undefined ? $scope.scopeModel.selectedPropertyField.value : undefined,
                        UseDescription: $scope.scopeModel.useDescription
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeAccountcreditlimitPropertyevaluator', AccountCreditLimitPropertyEvaluator);

})(app);
