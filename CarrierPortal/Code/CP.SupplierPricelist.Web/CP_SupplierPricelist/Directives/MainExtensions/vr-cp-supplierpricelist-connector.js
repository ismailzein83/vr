"use strict";

app.directive("vrCpSupplierpricelistConnector", [function () {

    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var supplierPriceListConnectorBase = new SupplierPriceListConnectorBase(ctrl, $scope);
            supplierPriceListConnectorBase.initCtrl();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/CP_SupplierPricelist/Directives/MainExtensions/Templates/SupplierpricelistConnectorTemplate.html"
    };

    function SupplierPriceListConnectorBase(ctrl, $scope) {
        this.initCtrl = initCtrl;

        function initCtrl() {
            getAPI();
        }

        function getAPI() {
            var api = {};
            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.url = payload.Url;
                    $scope.username = payload.Username;
                    $scope.password = payload.Password;
                }
            }
            api.getData = function () {
                return {
                    $type: "CP.SupplierPriceList.TOneV1Integration.SupplierPriceListConnector, CP.SupplierPriceList.TOneV1Integration",
                    Url: $scope.url,
                    Username: $scope.username,
                    Password: $scope.password
                };
            }
            if (ctrl.onReady && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }
    }
}]);
