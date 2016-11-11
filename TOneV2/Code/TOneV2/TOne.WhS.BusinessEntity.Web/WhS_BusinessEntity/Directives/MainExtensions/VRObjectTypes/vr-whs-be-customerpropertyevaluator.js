(function (app) {

    'use strict';

    CustomerPropertyEvaluator.$inject = ['WhS_BE_CustomerPropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function CustomerPropertyEvaluator(WhS_BE_CustomerPropertyEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selector = new CustomerSelectorPropertyEvaluator($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/CustomerPropertyEvaluatorTemplate.html'
        };

        function CustomerSelectorPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.customerFields = UtilsService.getArrayEnum(WhS_BE_CustomerPropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedCustomerField = UtilsService.getItemByVal($scope.scopeModel.customerFields, payload.objectPropertyEvaluator.CustomerField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CustomerPropertyEvaluator, TOne.WhS.BusinessEntity.MainExtensions",
                        CustomerField: $scope.scopeModel.selectedCustomerField.value
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrWhsBeCustomerpropertyevaluator', CustomerPropertyEvaluator);

})(app);
