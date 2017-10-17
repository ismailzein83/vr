(function (app) {

    'use strict';

    fieldmappingConstantDirective.$inject = [];

    function fieldmappingConstantDirective() {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var constantfieldmapping = new ConstantFieldMapping($scope, ctrl, $attrs);
                constantfieldmapping.initializeController();
            },
            controllerAs: "constantFieldMappingCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var label = "";
            if (attrs.label != undefined)
                label = "label='" + attrs.label + "'";
            return ' <vr-columns colnum="{{constantFieldMappingCtrl.normalColNum * 1.5}}" >'
                          + ' <vr-textbox ' + label + ' value="fieldValue" isrequired="constantFieldMappingCtrl.isrequired" type="constantFieldMappingCtrl.type"> </vr-textbox>'
                           + '</vr-columns>';
        }

        function ConstantFieldMapping($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.fieldMapping != undefined) {
                            $scope.fieldValue = payload.fieldMapping.FieldValue;
                        }
                    }
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.fieldValue != undefined) {
                        data = {
                            $type: "Vanrise.ExcelConversion.MainExtensions.ConstantFieldMapping, Vanrise.ExcelConversion.MainExtensions",
                            FieldValue: $scope.fieldValue,
                        };
                    }
                    return data;
                }
            }
        }
    }

    app.directive('vrExcelconversionFieldmappingConstantfieldmapping', fieldmappingConstantDirective);

})(app);