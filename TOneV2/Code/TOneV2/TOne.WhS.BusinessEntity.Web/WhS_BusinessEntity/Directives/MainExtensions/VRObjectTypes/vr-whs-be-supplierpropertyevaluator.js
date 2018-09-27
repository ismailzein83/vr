(function (app) {

    'use strict';

    SupplierPropertyEvaluator.$inject = ['WhS_BE_SupplierPropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function SupplierPropertyEvaluator(WhS_BE_SupplierPropertyEnum, UtilsService, VRUIUtilsService) {
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
                var selector = new SupplierSelectorPropertyEvaluator($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/SupplierPropertyEvaluatorTemplate.html'
        };

        function SupplierSelectorPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.supplierFields = UtilsService.getArrayEnum(WhS_BE_SupplierPropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedSupplierField = UtilsService.getItemByVal($scope.scopeModel.supplierFields, payload.objectPropertyEvaluator.SupplierField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SupplierPropertyEvaluator, TOne.WhS.BusinessEntity.MainExtensions",
                        SupplierField: $scope.scopeModel.selectedSupplierField.value
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrWhsBeSupplierpropertyevaluator', SupplierPropertyEvaluator);

})(app);
