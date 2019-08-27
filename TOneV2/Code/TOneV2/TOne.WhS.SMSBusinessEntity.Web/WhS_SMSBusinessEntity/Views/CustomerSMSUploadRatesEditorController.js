(function (appControllers) {

    'use strict';
    GenericBusinessEntityEditorUploaderController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService', 'VRNotificationService'];
    function GenericBusinessEntityEditorUploaderController($scope, VRNavigationService, UtilsService, WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService, VRNotificationService) {
        var fileId;
        var customerInfo;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                customerInfo = parameters.customerInfo;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.uploadCustomerSMSRates = function () {
                fileId = $scope.scopeModel.document.fileId;
                var input = {
                    FileId: fileId,
                    CustomerID: customerInfo.CarrierAccountId,
                    CurrencyId: customerInfo.CurrencyId
                };

                return WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.UploadSMSRateChanges(input).then(function (response) {
                    if (response) {
                        if (response.ErrorMessage == undefined) {
                            VRNotificationService.showSuccess("SMS Sale Rates Finished Upload");
                            fileId = response.FileID;
                            $scope.scopeModel.isUploadingComplete = true;
                            $scope.scopeModel.addedItems = response.NumberOfItemsAdded;
                            $scope.scopeModel.failedItems = response.NumberOfItemsFailed;
                        }
                        else {
                            VRNotificationService.showPromptWarning(response.ErrorMessage);
                        }
                    }
                });

            };

            $scope.scopeModel.onFileChanged = function (file) {
                $scope.scopeModel.isUploadingComplete = false;
            };

            $scope.scopeModel.downloadTemplate = function () {
                return WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.DownloadImportCustomerSMSRateTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };

            $scope.scopeModel.downloadLog = function () {
                if (fileId != undefined) {
                    return WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.DownloadImportedCustomerSMSRateLog(fileId).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }

    appControllers.controller('WhS_SMSBusinessEntity_CustomerSMSUploadRatesEditorController', GenericBusinessEntityEditorUploaderController);


})(appControllers);