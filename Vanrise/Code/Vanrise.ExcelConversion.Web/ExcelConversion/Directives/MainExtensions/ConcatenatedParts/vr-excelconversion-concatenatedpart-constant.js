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
            return "<vr-row removeline> <vr-columns colnum='{{colnum * 2}}'><vr-textbox value='constantCtrl.value' " + label + " isrequired='{{!constantCtrl.useSpace}}'  vr-disabled='constantCtrl.useSpace'> </vr-textbox></vr-columns><vr-columns colnum='{{colnum}}'><vr-label>Use Space</vr-label></vr-columns><vr-columns colnum='{{colnum}}'><vr-switch  value='constantCtrl.useSpace'   onvaluechanged='constantCtrl.onuseSpaceValueChanged()'> </vr-switch>   </vr-columns></vr-row>";
        }
        function ConcatenatedpartConstant($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            ctrl.useSpace = false;
            ctrl.onuseSpaceValueChanged = function () {
                if (ctrl.useSpace == true)
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
                            ctrl.useSpace = true;
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
                        Constant: ctrl.useSpace ? " " : ctrl.value
                    };
                    return data;
                }
            }
        }
    }

    app.directive('vrExcelconversionConcatenatedpartConstant', concatenatedpartConstantDirective);

})(app);