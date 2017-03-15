(function (app) {

    'use strict';

    OutputfieldvaluePricelistfield.$inject = ["UtilsService","VR_ExcelConversion_FieldTypeEnum"];

    function OutputfieldvaluePricelistfield(UtilsService,VR_ExcelConversion_FieldTypeEnum) {
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
            var selectorAPI;
            function initializeController() {
                $scope.outputFieldMappings = [{ fieldTitle: "Code", isRequired: true, type: "cell", fieldName: "Code", fieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { fieldTitle: "Zone", isRequired: true, type: "cell", fieldName: "Zone", fieldType: VR_ExcelConversion_FieldTypeEnum.String.value }, { fieldTitle: "Rate", isRequired: true, type: "cell", fieldName: "Rate", fieldType: VR_ExcelConversion_FieldTypeEnum.Decimal.value }, { fieldTitle: "Effective Date", isRequired: false, type: "cell", fieldName: "EffectiveDate", fieldType: VR_ExcelConversion_FieldTypeEnum.DateTime.value }];

                ctrl.onSelectorReady = function (api) {

                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.outputFieldValue != undefined) {
                            $scope.selectedOutputFieldMapping = UtilsService.getItemByVal($scope.outputFieldMappings, payload.outputFieldValue.FieldName, "fieldName");
                        }
                    }

                };
                api.isDateTime = function () {
                    if ($scope.selectedOutputFieldMapping != undefined && $scope.selectedOutputFieldMapping.fieldType == VR_ExcelConversion_FieldTypeEnum.DateTime.value)
                        return true;
                    return false;
                };
                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: " XBooster.PriceListConversion.MainExtensions.OutputFieldValue.PriceListField, XBooster.PriceListConversion.MainExtensions ",
                        FieldName: $scope.selectedOutputFieldMapping != undefined ? $scope.selectedOutputFieldMapping.fieldName : undefined
                    };
                    return data;
                }
            }
        }
    }

    app.directive('xboosterPricelistconversionOutputfieldvaluePricelistfield', OutputfieldvaluePricelistfield);

})(app);