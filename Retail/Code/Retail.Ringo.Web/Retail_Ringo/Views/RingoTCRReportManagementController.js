(function (appControllers) {
    "use strict";

    RingoTCRReportManagementController.$inject = ['$scope', 'Retail_Ringo_RingoReportSheetAPIService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Retail_Ringo_TCRReportType', 'Retail_Ringo_TCRReportMonths', 'Retail_Ringo_TCRReportYear'];

    function RingoTCRReportManagementController($scope, Retail_Ringo_RingoReportSheetAPIService, UtilsService, vrNotificationService, vrUIUtilsService, Retail_Ringo_TCRReportType, Retail_Ringo_TCRReportMonths, Retail_Ringo_TCRReportYear) {

        var operatorDirectiveApi;
        var singleOperatorDirectiveApi;
        var operatorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var singleOperatorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onOperatorSelectorDirectiveReady = function (api) {
                operatorDirectiveApi = api;
                operatorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSingleOperatorSelectorDirectiveReady = function (api) {
                singleOperatorDirectiveApi = api;
                singleOperatorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.generateReport = function () {
                var date;
                if ($scope.scopeModel.selectedTCRReportType.value == 0) {
                    date = new Date($scope.scopeModel.selectedTCRReportYear.value, $scope.scopeModel.selectedTCRReportMonth.value, 1);
                } else
                    date = $scope.scopeModel.fromDate;
                var selectedOperators;

                if ($scope.scopeModel.selectedTCRReportType.value == 1)
                    selectedOperators = singleOperatorDirectiveApi.getSelectedValues();
                else
                    selectedOperators = operatorDirectiveApi.getSelectedValues();

                var filter = {
                    ReportType: $scope.scopeModel.selectedTCRReportType.value,
                    From: date,
                    Operator: selectedOperators
                };
                return Retail_Ringo_RingoReportSheetAPIService.DownloadTCRReport(filter).then(function (response) {

                    if (response.data.byteLength > 22)
                        UtilsService.downloadFile(response.data, response.headers);
                    else
                        vrNotificationService.showWarning("No data to display");
                });
            };
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOperatorSelector, loadSingleOperatorSelector]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function load() {
            $scope.scopeModel.isloading = true;
            $scope.scopeModel.tCRReportType = UtilsService.getArrayEnum(Retail_Ringo_TCRReportType);
            $scope.scopeModel.tCRReportMonth = UtilsService.getArrayEnum(Retail_Ringo_TCRReportMonths);
            $scope.scopeModel.tCRReportYear = UtilsService.getArrayEnum(Retail_Ringo_TCRReportYear);

            loadAllControls().finally(function () {
                $scope.scopeModel.isloading = false;
            }).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isloading = false;
            });
        }

        function loadOperatorSelector() {
            var operatorPayload = {
                AccountBEDefinitionId: "CBC68CD3-2F02-4452-B261-0D32AE4F8269"
            };
            var selectorLoadDeferred = UtilsService.createPromiseDeferred();
            operatorReadyPromiseDeferred.promise.then(function () {
                vrUIUtilsService.callDirectiveLoad(operatorDirectiveApi, operatorPayload, selectorLoadDeferred);
            });

            return selectorLoadDeferred.promise;
        }

        function loadSingleOperatorSelector() {
            var operatorPayload = {
                AccountBEDefinitionId: "CBC68CD3-2F02-4452-B261-0D32AE4F8269"
            };

            var singleSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            singleOperatorReadyPromiseDeferred.promise.then(function () {
                vrUIUtilsService.callDirectiveLoad(singleOperatorDirectiveApi, operatorPayload, singleSelectorLoadDeferred);
            });

            return singleSelectorLoadDeferred.promise;
        }

    }

    appControllers.controller('Retail_Ringo_TCRReportManagementController', RingoTCRReportManagementController);
})(appControllers);