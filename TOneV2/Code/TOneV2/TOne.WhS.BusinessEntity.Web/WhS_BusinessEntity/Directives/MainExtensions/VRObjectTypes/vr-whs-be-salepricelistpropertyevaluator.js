(function (app) {

    'use strict';

    SalePriceListPropertyEvaluator.$inject = ['WhS_BE_SalePriceListPropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function SalePriceListPropertyEvaluator(WhS_BE_SalePriceListPropertyEnum, UtilsService, VRUIUtilsService) {
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
                var selector = new SalePriceListSelectorPropertyEvaluator($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/SalePriceListPropertyEvaluatorTemplate.html'
        };

        function SalePriceListSelectorPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.salePriceListFields = UtilsService.getArrayEnum(WhS_BE_SalePriceListPropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedSalePriceListField = UtilsService.getItemByVal($scope.scopeModel.salePriceListFields, payload.objectPropertyEvaluator.SalePriceListField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SalePriceListPropertyEvaluator, TOne.WhS.BusinessEntity.MainExtensions",
                        SalePriceListField: $scope.scopeModel.selectedSalePriceListField.value
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrWhsBeSalepricelistpropertyevaluator', SalePriceListPropertyEvaluator);

})(app);
