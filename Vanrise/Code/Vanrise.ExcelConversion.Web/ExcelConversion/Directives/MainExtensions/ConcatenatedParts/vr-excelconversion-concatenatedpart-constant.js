(function (app) {

    'use strict';

    concatenatedpartConstantDirective.$inject = [];

    function concatenatedpartConstantDirective() {
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
                $scope.colnum = ctrl.normalColNum || 3;
                var concatenatedpartConstant = new ConcatenatedpartConstant($scope, ctrl, $attrs);
                concatenatedpartConstant.initializeController();
            },
            controllerAs: "constantCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var label = "";
            if (label != undefined)
                label = attrs.label;
            return "<vr-row removeline> <vr-columns colnum='{{colnum * 2}}'><vr-textbox value='constantCtrl.value' " + label + " isrequired='{{!constantCtrl.useSapce}}'  vr-disabled='constantCtrl.useSapce'> </vr-textbox></vr-columns><vr-columns colnum='{{colnum}}'><vr-label>Use Sapce</vr-label></vr-columns><vr-columns colnum='{{colnum}}'><vr-switch  value='constantCtrl.useSapce'   onvaluechanged='constantCtrl.onUseSapceValueChanged()'> </vr-switch>   </vr-columns></vr-row>";
        }
        function ConcatenatedpartConstant($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            ctrl.useSapce = false;
            ctrl.onUseSapceValueChanged = function () {
                if (ctrl.useSapce == true)
                    ctrl.value = null;
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.concatenatedPart != undefined) {
                        if (payload.concatenatedPart.Constant == " ")
                            ctrl.useSapce = true;
                        else
                            ctrl.value = payload.concatenatedPart.Constant;
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Vanrise.ExcelConversion.MainExtensions.ConcatenatedParts.ConstantConcatenatedPart, Vanrise.ExcelConversion.MainExtensions ",
                        Constant: ctrl.useSapce ? " " : ctrl.value
                    };
                    return data;
                }
            }
        }
    }

    app.directive('vrExcelconversionConcatenatedpartConstant', concatenatedpartConstantDirective);

})(app);