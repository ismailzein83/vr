(function (app) {

    'use strict';

    concatenatedpartConstantDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function concatenatedpartConstantDirective(UtilsService, VRUIUtilsService) {
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
                var concatenatedpartConstant = new ConcatenatedpartConstant($scope, ctrl, $attrs);
                concatenatedpartConstant.initializeController();
            },
            controllerAs: "constantCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs)
        {
            var label="";
            if(label !=undefined)
                label = attrs.label;
            return "<vr-row removeline> <vr-columns colnum='{{constantCtrl.normalColNum * 2}}'><vr-textbox value='constantCtrl.value' " + label + " isrequired='true'>    </vr-columns></vr-textbox></vr-row>"
        }
        function ConcatenatedpartConstant($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "ExcelConversion.MainExtensions.ConcatenatedParts.ConstantConcatenatedPart, ExcelConversion.MainExtensions ",
                        Constant:ctrl.value
                    }
                    return data;
                }
            }
        }
    }

    app.directive('excelconversionConcatenatedpartConstant', concatenatedpartConstantDirective);

})(app);