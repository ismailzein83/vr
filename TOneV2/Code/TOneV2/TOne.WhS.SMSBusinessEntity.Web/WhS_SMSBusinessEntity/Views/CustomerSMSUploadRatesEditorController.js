(function (appControllers) {

    'use strict';
    GenericBusinessEntityEditorUploaderController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService', 'VRNotificationService'];
    function GenericBusinessEntityEditorUploaderController($scope, VRNavigationService, UtilsService, WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService, VRNotificationService) {
        var fileId;
        var customerInfo;
        var currencyObj;
        var effectiveDate;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                customerInfo = parameters.customerInfo;
                effectiveDate = parameters.effectiveDate;
                currencyObj = parameters.currencyObj;

            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.effectiveDate = getShortDate(effectiveDate);
            $scope.scopeModel.currency = currencyObj.Symbol;

            $scope.scopeModel.uploadCustomerSMSRates = function () {
                fileId = $scope.scopeModel.document.fileId;
                var input = {
                    FileId: fileId,
                    CustomerID: customerInfo.CarrierAccountId,
                    CurrencyId: currencyObj.CurrencyId,
                    EffectiveDate: effectiveDate
                };

                return WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.UploadSMSRateChanges(input).then(function (response) {
                    if (response) {
                        if (response.ErrorMessage == undefined) {
                            VRNotificationService.showSuccess("SMS Sale Rates Finished Upload");
                            fileId = response.FileID;
                            $scope.scopeModel.isUploadingComplete = true;
                            $scope.scopeModel.addedItems = response.NumberOfItemsAdded;
                            $scope.scopeModel.failedItems = response.NumberOfItemsFailed;

                            if ($scope.scopeModel.addedItems > 0 && $scope.onSaleSMSRateChangesUploaded != undefined) {
                                var uploadResult = {
                                    processDraftID: response.ProcessDraftID,
                                    pendingChangesCount: response.PendingChangesCount
                                };
                                $scope.onSaleSMSRateChangesUploaded(uploadResult);
                            }

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

        function getShortDate(date) {
            var dateString = '';
            if (date && date instanceof Date && !isNaN(date)) {
                dateString += date.getFullYear();

                var month = "" + (parseInt(date.getMonth()) + 1);
                if (month.length == 1)
                    dateString += "-0" + month;
                else
                    dateString += "-" + month;

                var day = "" + (parseInt(date.getDate()));
                if (day.length == 1)
                    dateString += "-0" + day;
                else
                    dateString += "-" + day;
            }
            else {
                return getShortDate(UtilsService.createDateFromString(date));
            }
            return dateString;
        }
    }

    appControllers.controller('WhS_SMSBusinessEntity_CustomerSMSUploadRatesEditorController', GenericBusinessEntityEditorUploaderController);


})(appControllers);