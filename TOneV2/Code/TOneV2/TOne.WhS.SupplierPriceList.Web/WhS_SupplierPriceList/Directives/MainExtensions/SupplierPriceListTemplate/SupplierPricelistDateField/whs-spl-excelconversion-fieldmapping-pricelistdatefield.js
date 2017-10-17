(function (app) {

    'use strict';

    fieldmappingSalePricelistDateDirective.$inject = [];

    function fieldmappingSalePricelistDateDirective() {
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
                var salePricelistDatefieldmapping = new SalePricelistDateFieldMapping($scope, ctrl, $attrs);
                salePricelistDatefieldmapping.initializeController();
            },
            controllerAs: "salePricelistDateFieldMappingCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var label = "";
            if (attrs.label != undefined)
                label = "label='" + attrs.label + "'";
            return '';
        }

        function SalePricelistDateFieldMapping($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
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
                        $type: "TOne.WhS.SupplierPriceList.MainExtensions.PricelistDateFieldMapping, TOne.WhS.SupplierPriceList.MainExtensions",
                    };
                    return data;
                }
            }
        }
    }

    app.directive('whsSplExcelconversionFieldmappingPricelistdatefield', fieldmappingSalePricelistDateDirective);

})(app);