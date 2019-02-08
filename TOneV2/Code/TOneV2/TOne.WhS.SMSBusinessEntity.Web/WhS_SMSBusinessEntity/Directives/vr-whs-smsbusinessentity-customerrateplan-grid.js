﻿"use strict";
app.directive("vrWhsSmsbusinessentityCustomerrateplanGrid", ["VRNotificationService", "WhS_SMSBusinessEntity_CustomerSMSRateAPIService", "WhS_SMSBusinessEntity_CustomerRatePlanService",
    function ( VRNotificationService, WhS_SMSBusinessEntity_CustomerSMSRateAPIService, WhS_SMSBusinessEntity_CustomerRatePlanService) {
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
            templateUrl: "/Client/Modules/WhS_SMSBusinessEntity/Directives/Templates/CustomerSMSRatePlanGridTemplate.html"
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
                    WhS_SMSBusinessEntity_CustomerSMSRateAPIService.GetFilteredCustomerSMSRate(dataRetrievalInput).then(function (response) {
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
                    WhS_SMSBusinessEntity_CustomerRatePlanService.viewFutureSMSRate(dataItem.MobileNetworkName, dataItem.FutureRate);
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        gridQuery = payload.query;
                    }

                    return gridAPI.retrieveData(gridQuery);
                };

                api.cleanGrid = function () {
                    gridAPI.clearAll();
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

        }
    }]);