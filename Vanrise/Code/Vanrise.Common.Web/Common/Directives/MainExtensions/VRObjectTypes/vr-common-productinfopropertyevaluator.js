(function (app) {

    'use strict';

    ProductInfoPropertyEvaluator.$inject = ['VRCommon_ProductInfoPropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function ProductInfoPropertyEvaluator(VRCommon_ProductInfoPropertyEnum, UtilsService, VRUIUtilsService) {
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
                var vrProductInfoPropertyEvaluatorSecurity = new VRProductInfoPropertyEvaluator($scope, ctrl, $attrs);
                vrProductInfoPropertyEvaluatorSecurity.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/MainExtensions/VRObjectTypes/Templates/ProductInfoPropertyEvaluatorTemplate.html'
        };

        function VRProductInfoPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.productFields = UtilsService.getArrayEnum(VRCommon_ProductInfoPropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedProductField = UtilsService.getItemByVal($scope.scopeModel.productFields, payload.objectPropertyEvaluator.ProductField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Common.MainExtensions.VRObjectTypes.ProductInfoPropertyEvaluator, Vanrise.Common.MainExtensions",
                        ProductField: $scope.scopeModel.selectedProductField.value
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrCommonProductinfopropertyevaluator', ProductInfoPropertyEvaluator);

})(app);
