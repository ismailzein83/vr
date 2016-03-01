"use strict";

app.directive("cpSupplierpricelistUploadsupplierpricelist", ['UtilsService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierPriceListAPIService', function (UtilsService, VRUIUtilsService, CP_SupplierPricelist_SupplierPriceListAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/CP_SupplierPricelist/Directives/MainExtensions/Templates/UploadPriceListTemplate.html"
    };


    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var customerDirectiveApi;
        var customerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.onCustomerDirectiveReady = function (api) {
                customerDirectiveApi = api;
                customerReadyPromiseDeferred.resolve();
            }
            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getData = function () {
                return {
                    $type: "CP.SupplierPricelist.Business.PriceListTasks.UploadPriceListTaskActionArgument, CP.SupplierPricelist.Business",
                    CustomerId: customerDirectiveApi.getSelectedIds(),
                    MaximumRetryCount: $scope.maximumRetryCount
                };
            };


            api.load = function (payload) {
                var data;
                if (payload != undefined && payload.data != undefined)
                    data = payload.data;
                $scope.maximumRetryCount = (data != undefined) ? data.MaximumRetryCount : undefined;
                var customerLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                customerReadyPromiseDeferred.promise.then(function () {
                    var obj = {
                        //filter: {
                        //    AssignedToCurrentSupplier: true 
                        //}
                    }
                    if (data != undefined && data.CustomerId != undefined)
                        obj.selectedIds = data.CustomerId;
                    VRUIUtilsService.callDirectiveLoad(customerDirectiveApi, obj, customerLoadPromiseDeferred);
                });
                return customerLoadPromiseDeferred.promise;
            }
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
