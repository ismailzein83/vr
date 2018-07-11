'use strict';

app.directive('demoModuleBilling', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new Billing($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        //controllerAs: 'ctrl',
        //bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Directives/Templates/BillingTemplate.html"
    };

    function Billing($scope, attrs) {

        var invoiceGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var paymentGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var invoiceGridApi;
        var paymentGridApi;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onInvoiceGridReady = function (api) {
                invoiceGridApi = api;
                invoiceGridReadyPromiseDeferred.resolve();
            }


            $scope.scopeModel.onPaymentGridReady = function (api) {
                paymentGridApi = api;
                paymentGridReadyPromiseDeferred.resolve();
            }

            UtilsService.waitMultiplePromises([invoiceGridReadyPromiseDeferred.promise, paymentGridReadyPromiseDeferred.promise]).then(function () {
                defineAPI();
            });

        };

        function defineAPI() {
            var api = {};

            api.load = function () {
                var promises = [];
                return UtilsService.waitMultiplePromises([invoiceGridApi.load(), paymentGridApi.load()]);
            };

            if ($scope.onReady != undefined && typeof ($scope.onReady) == "function") {
                $scope.onReady(api);
            }

        };

    };

    return directiveDefinitionObject;

}]);