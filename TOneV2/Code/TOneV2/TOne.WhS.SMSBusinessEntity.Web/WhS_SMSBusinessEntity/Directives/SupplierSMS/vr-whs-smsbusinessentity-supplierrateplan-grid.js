"use strict";
app.directive("vrWhsSmsbusinessentitySupplierrateplanGrid", ["VRNotificationService", "WhS_SMSBusinessEntity_SupplierSMSRateAPIService", "WhS_SMSBusinessEntity_SupplierRatePlanService", "UtilsService", function (VRNotificationService, WhS_SMSBusinessEntity_SupplierSMSRateAPIService, WhS_SMSBusinessEntity_SupplierRatePlanService, UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SmsRatePlanGrid($scope, ctrl, $attrs, $element);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_SMSBusinessEntity/Directives/SupplierSMS/Templates/SupplierSMSRatePlanGridTemplate.html"
        };

        function SmsRatePlanGrid($scope, ctrl, attrs, $element) {
            this.initializeController = initializeController;
            var gridAPI;
            var gridQuery;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    $scope.scopeModel.isLoading = true;
                    WhS_SMSBusinessEntity_SupplierSMSRateAPIService.GetFilteredSupplierSMSRate(dataRetrievalInput).then(function (response) {
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

                $scope.scopeModel.onFutureRateClicked = function (dataItem) {
                    WhS_SMSBusinessEntity_SupplierRatePlanService.viewFutureSMSRate(dataItem.MobileNetworkName, dataItem.FutureRate);
                };

                defineAPI();
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

                api.cleanGrid = function () {
                    gridAPI.clearAll();
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

        }
    }]);