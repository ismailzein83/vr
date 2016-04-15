(function (app) {

    'use strict';

    OutputfieldvaluePricelistfield.$inject = [];

    function OutputfieldvaluePricelistfield() {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var outputPricelistfield = new OutputPricelistfield($scope, ctrl, $attrs);
                outputPricelistfield.initializeController();
            },
            controllerAs: "constantCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/XBooster_PriceListConversion/Directives/MainExtensions/PriceListTemplate/OutputFieldValue/Templates/PriceListFieldTemplate.html"

        };
        function OutputPricelistfield($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.fieldName = payload.FieldName;
                        ctrl.titleName = payload.FieldTitle;
                        if (payload.outputFieldValue != undefined) {
                            ctrl.fieldName = payload.outputFieldValue.FieldName;
                            ctrl.titleName = payload.outputFieldValue.FieldName;
                        }
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: " XBooster.PriceListConversion.MainExtensions.OutputFieldValue.PriceListField, XBooster.PriceListConversion.MainExtensions ",
                        FieldName: ctrl.fieldName
                    }
                    return data;
                }
            }
        }
    }

    app.directive('xboosterPricelistconversionOutputfieldvaluePricelistfield', OutputfieldvaluePricelistfield);

})(app);