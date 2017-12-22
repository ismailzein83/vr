'use strict';

app.directive('vrWhsBePop3supplierpricelistmessagefilter', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new Pop3SupplierPricelistMessageFilter($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_SupplierPriceList/Directives/MainExtensions/POP3Filter/Templates/POP3SupplierPricelistMessageFilterTemplate.html'
        };

        function Pop3SupplierPricelistMessageFilter($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {

                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "TOne.WhS.SupplierPriceList.MainExtensions.Pop3SupplierPricelistMessageFilter, TOne.WhS.SupplierPriceList.MainExtensions"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
