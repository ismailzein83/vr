"use strict";
app.directive("vrWhsSmsbusinessentityCostanalysisGrid", ["VRNotificationService", "WhS_SMSBusinessEntity_SupplierSMSRateAPIService", "UtilsService",
    function (VRNotificationService, WhS_SMSBusinessEntity_SupplierSMSRateAPIService, UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SMSCostAnalysisGrid($scope, ctrl, $attrs, $element);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_SMSBusinessEntity/Directives/SupplierSMS/Templates/SMSCostAnalysisGridTemplate.html"
        };

        function SMSCostAnalysisGrid($scope, ctrl, attrs, $element) {
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
                    WhS_SMSBusinessEntity_SupplierSMSRateAPIService.GetFilteredSMSCostDetails(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var smsCostDetail = response.Data[i];
                                buildTitleToDisplay(smsCostDetail);
                                $scope.scopeModel.datasource.push(smsCostDetail);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                function buildTitleToDisplay(smsCostDetail) {

                    var totalWhiteSpaceLength = 30;

                    if (smsCostDetail.CostOptions != undefined) {
                        for (var x = 0; x < smsCostDetail.CostOptions.length; x++) {
                            var costOption = smsCostDetail.CostOptions[x];

                            var supplierRateLength = costOption.SupplierRate.toString().length + 2; //2 is length of " ()"
                            var remainingWhiteSpaceLength = totalWhiteSpaceLength - supplierRateLength;

                            var title = costOption.SupplierName;
                            if (title.length > remainingWhiteSpaceLength) {
                                title = title.substring(0, remainingWhiteSpaceLength - 2); //2 is length of "..."
                                title += "...";
                            }
                            costOption.titleToDisplay = title + ' (' + costOption.SupplierRate + ')';
                        }
                    }
                }
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