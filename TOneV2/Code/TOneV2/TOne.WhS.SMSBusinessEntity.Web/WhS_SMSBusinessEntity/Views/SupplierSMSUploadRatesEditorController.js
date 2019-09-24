(function (appControllers) {

    'use strict';
    GenericBusinessEntityEditorUploaderController.$inject = ['$scope', 'VRNavigationService', 'VRUIUtilsService', 'UtilsService', 'WhS_SMSBusinessEntity_SupplierSMSRateChangesAPIService', 'VRNotificationService'];
    function GenericBusinessEntityEditorUploaderController($scope, VRNavigationService, VRUIUtilsService, UtilsService, WhS_SMSBusinessEntity_SupplierSMSRateChangesAPIService, VRNotificationService) {
        var fileId;
        var supplierInfo;
        var currencyObj;
        var effectiveDate;
        var definitionTitle;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                supplierInfo = parameters.supplierInfo;
                currencyObj = parameters.currencyObj;
                effectiveDate = parameters.effectiveDate;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.effectiveDate = getShortDate(effectiveDate);
            $scope.scopeModel.currency = currencyObj.Symbol;

            $scope.scopeModel.uploadSupplierSMSRates = function () {
                fileId = $scope.scopeModel.document.fileId;
                var input = {
                    FileId: fileId,
                    SupplierID: supplierInfo.CarrierAccountId,
                    CurrencyId: currencyObj.CurrencyId,
                    EffectiveDate: effectiveDate
                };

                return WhS_SMSBusinessEntity_SupplierSMSRateChangesAPIService.UploadSMSRateChanges(input).then(function (response) {
                    if (response) {
                        if (response.ErrorMessage == undefined) {
                            VRNotificationService.showSuccess("SMS Supplier Rates Finished Upload");
                            fileId = response.FileID;
                            $scope.scopeModel.isUploadingComplete = true;
                            $scope.scopeModel.addedItems = response.NumberOfItemsAdded;
                            $scope.scopeModel.failedItems = response.NumberOfItemsFailed;

                            if ($scope.scopeModel.addedItems > 0 && $scope.onSupplierSMSRateChangesUploaded != undefined) {
                                var uploadResult = {
                                    processDraftID: response.ProcessDraftID,
                                    pendingChangesCount: response.PendingChangesCount
                                };
                                $scope.onSupplierSMSRateChangesUploaded(uploadResult);
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
                return WhS_SMSBusinessEntity_SupplierSMSRateChangesAPIService.DownloadImportSupplierSMSRateTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };

            $scope.scopeModel.downloadLog = function () {
                if (fileId != undefined) {
                    return WhS_SMSBusinessEntity_SupplierSMSRateChangesAPIService.DownloadImportedSupplierSMSRateLog(fileId).then(function (response) {
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
            return dateString;
        }
    }

    appControllers.controller('WhS_SMSBusinessEntity_SupplierSMSUploadRatesEditorController', GenericBusinessEntityEditorUploaderController);


})(appControllers);