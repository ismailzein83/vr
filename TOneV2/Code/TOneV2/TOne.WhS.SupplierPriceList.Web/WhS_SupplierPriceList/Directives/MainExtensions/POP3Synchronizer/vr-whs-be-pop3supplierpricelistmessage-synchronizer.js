'use strict';

app.directive('vrWhsBePop3supplierpricelistmessageSynchronizer', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new Pop3SupplierPricelistMessageSynchronizer($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_SupplierPriceList/Directives/MainExtensions/POP3Synchronizer/Templates/POP3SupplierPricelistMessageSynchronizer.html'
        };

        function Pop3SupplierPricelistMessageSynchronizer($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.SupplierPriceList.Business.Pop3MailMessageSynchronizer, TOne.WhS.SupplierPriceList.Business",
                        Name: "Supplier POP3 Mail Message Synchronizer"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
