"use strict";
app.directive("vrWhsSmsbusinessentitySupplierpricelistGrid", ["VRNotificationService", "WhS_SMSBusinessEntity_SupplierSMSPriceListAPIService", "UtilsService",
    function (VRNotificationService, WhS_SMSBusinessEntity_SupplierSMSPriceListAPIService, UtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new SMSPriceListPlanGrid($scope, ctrl, $attrs, $element);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_SMSBusinessEntity/Directives/SupplierSMS/Templates/SupplierSMSPriceListGridTemplate.html"
    };

    function SMSPriceListPlanGrid($scope, ctrl, attrs, $element) {
        this.initializeController = initializeController;
        var gridAPI;
        var gridQuery;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.datasource = [];
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                $scope.scopeModel.isLoading = true;
                WhS_SMSBusinessEntity_SupplierSMSPriceListAPIService.GetFilteredSupplierSMSPriceLists(dataRetrievalInput).then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            $scope.scopeModel.datasource.push(response.Data[i]);
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };
        }

        function defineAPI() {

            var api = {};

            api.load = function (payload) {

                if (payload != undefined) {
                    gridQuery = payload.query;
                }

                var promise = gridAPI.retrieveData(gridQuery);
                if (promise == undefined || promise == null) {
                    promise = UtilsService.createPromiseDeferred();
                    promise.resolve();
                }

                return promise.promise;
            };

            if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

    }
}]);