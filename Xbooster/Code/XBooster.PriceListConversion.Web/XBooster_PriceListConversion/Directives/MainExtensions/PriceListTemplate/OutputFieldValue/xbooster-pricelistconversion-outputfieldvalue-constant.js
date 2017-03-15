(function (app) {

    'use strict';

    OutputfieldvalueConstant.$inject = [];

    function OutputfieldvalueConstant() {
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
                var outputConstant = new OutputConstant($scope, ctrl, $attrs);
                outputConstant.initializeController();
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
            return "<vr-columns colnum='{{constantCtrl.normalColNum}}'><vr-textbox value='constantCtrl.constant' " + label + " isrequired='true'> </vr-textbox>   </vr-columns>"
        }
        function OutputConstant($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.constant = payload.Constant;
                        if(payload.outputFieldValue != undefined)
                        {
                            ctrl.constant = payload.outputFieldValue.Value;
                        }
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: " XBooster.PriceListConversion.MainExtensions.OutputFieldValue.Constant, XBooster.PriceListConversion.MainExtensions ",
                        Value: ctrl.constant
                    };
                    return data;
                }
            }
        }
    }

    app.directive('xboosterPricelistconversionOutputfieldvalueConstant', OutputfieldvalueConstant);

})(app);