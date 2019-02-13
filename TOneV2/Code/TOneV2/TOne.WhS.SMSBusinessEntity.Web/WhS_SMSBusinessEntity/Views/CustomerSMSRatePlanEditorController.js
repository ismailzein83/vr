(function (appControllers) {

    "use strict";

    customerSMSRatePlanEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService', 'VRDateTimeService', 'WhS_SMSBusinessEntity_CustomerRatePlanService', 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPInstanceService', 'WhS_BP_CreateProcessResultEnum', 'BPInstanceStatusEnum'];

    function customerSMSRatePlanEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService, VRDateTimeService, WhS_SMSBusinessEntity_CustomerRatePlanService, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, WhS_BP_CreateProcessResultEnum, BPInstanceStatusEnum) {

        var customerInfo;
        var processDraftID;
        var isCustomerSMSRateDraftExist;

        var allCountryLetters = "All";
        var modifiedSmsRates = [];

        var isEffectiveDateChanged = false;

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadParameters();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                customerInfo = parameters.customerInfo;
                processDraftID = parameters.processDraftID;
                $scope.scopeModel.isCustomerSMSRateDraftExist = parameters.isCustomerSMSRateDraftExist;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.allDataSource = [];
            $scope.scopeModel.datasource = [];
            $scope.scopeModel.countryLetters = [];

            $scope.scopeModel.loadMoreData = function () {
                $scope.scopeModel.isLoading = true;
                getDataGridDataPage();
                $scope.scopeModel.isLoading = false;
            };

            $scope.scopeModel.onCountryLetterSelectionChanged = function () {
                
                var onSaveLoadedDeferred = UtilsService.createPromiseDeferred();
                if (modifiedSmsRates.length > 0 || (isEffectiveDateChanged && $scope.scopeModel.isCustomerSMSRateDraftExist)) {
                    saveChanges().then(function () {
                        onSaveLoadedDeferred.resolve();
                        isEffectiveDateChanged = false;
                        $scope.scopeModel.isCustomerSMSRateDraftExist = true;
                        modifiedSmsRates.length = 0;
                    });
                }
                else {
                    onSaveLoadedDeferred.resolve();
                }

                var countryItemsLoadedDeferred = UtilsService.createPromiseDeferred();
                onSaveLoadedDeferred.promise.then(function () {
                    loadCountryItems().then(function () {
                        $scope.scopeModel.isLoading = false;
                        countryItemsLoadedDeferred.resolve();
                    });
                });

                function loadCountryItems() {
                    gridAPI.clearDataAndContinuePaging();
                    $scope.scopeModel.datasource.length = 0;
                    $scope.scopeModel.allDataSource.length = 0;
                    var letter;
                    if ($scope.scopeModel.selectedCountryLetterIndex != $scope.scopeModel.countryLetters.length - 1) // case when any letter != All is selected
                        letter = $scope.scopeModel.countryLetters[$scope.scopeModel.selectedCountryLetterIndex];

                    return loadAllDataGrid(letter);
                }

                return countryItemsLoadedDeferred.promise;
            };

            $scope.scopeModel.onRateValueChanged = function (currentSMSRateDetails) {

                if (!tryUpdateCustomerSMSRateChanges(currentSMSRateDetails)) {
                    var rate = currentSMSRateDetails.NewRate;
                    if (rate != undefined) {
                        var obj = {
                            MobileNetworkID: currentSMSRateDetails.MobileNetworkID,
                            NewRate: rate
                        };

                        modifiedSmsRates.push(obj);
                    }
                }

                function tryUpdateCustomerSMSRateChanges(currentSMSRateDetails) {
                    for (var i = 0; i < modifiedSmsRates.length; i++) {
                        var modifiedSMSRate = modifiedSmsRates[i];
                        if (currentSMSRateDetails.MobileNetworkID == modifiedSMSRate.MobileNetworkID) {
                            modifiedSMSRate.NewRate = currentSMSRateDetails.NewRate;

                            return true;
                        }
                    }
                    return false;
                }
            };

            $scope.scopeModel.onEffectiveDateChanged = function () {
                isEffectiveDateChanged = true;
            };

            $scope.scopeModel.onFutureRateClicked = function (dataItem) {
                WhS_SMSBusinessEntity_CustomerRatePlanService.viewFutureSMSRate(dataItem.MobileNetworkName, dataItem.FutureRate);
            };

            $scope.scopeModel.saveChanges = function () {
                $scope.scopeModel.isLoading = true;
                onSaveOrApplyClicked().finally(function () {
                    $scope.modalContext.closeModal();
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.applyChanges = function () {
                $scope.scopeModel.isLoading = true;
                onSaveOrApplyClicked().then(function () {

                    var inputArguments = {
                        $type: "TOne.WhS.SMSBusinessEntity.BP.Arguments.SMSSaleRateInput, TOne.WhS.SMSBusinessEntity.BP.Arguments",
                        CustomerID: customerInfo.CarrierAccountId,
                        ProcessDraftID: processDraftID
                    };

                    var input = {
                        InputArguments: inputArguments
                    };

                    BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                        if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                            var processTrackingContext = {
                                onClose: function (bpInstanceClosureContext) {
                                    //if (bpInstanceClosureContext != undefined && bpInstanceClosureContext.bpInstanceStatusValue === BPInstanceStatusEnum.Completed.value) {
                                    if ($scope.onSaleSMSRatesApplied != undefined && typeof ($scope.onSaleSMSRatesApplied) == "function")
                                        $scope.onSaleSMSRatesApplied();
                                    //}
                                }
                            };

                            BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, processTrackingContext);
                        }

                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                        $scope.modalContext.closeModal();
                    });
                });
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            function loadAllControls() {

                var loadDraftDataPromiseDeferred = loadDraftData();

                var rootPromiseNode = {
                    promises: [loadDraftDataPromiseDeferred, gridReadyDeferred.promise],
                    getChildNode: function () {
                        var letter = $scope.scopeModel.countryLetters.length > 0 ? $scope.scopeModel.countryLetters[0] : undefined;
                        var loadDraftDataPromiseDeferred = loadAllDataGrid(letter);

                        return {
                            promises: [loadDraftDataPromiseDeferred]
                        };
                    }
                };

                function loadDraftData() {
                    return WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.GetDraftData(getDraftDataQuery()).then(function (response) {
                        if (response != undefined) {
                            if (response.CountryLetters && response.CountryLetters.length > 0) {
                                for (var i = 0; i < response.CountryLetters.length; i++) {
                                    $scope.scopeModel.countryLetters.push(response.CountryLetters[i]);
                                }

                                $scope.scopeModel.countryLetters.push(allCountryLetters);
                                $scope.scopeModel.selectedCountryLetterIndex = 0;
                            }
                        }

                        $scope.scopeModel.effectiveDate = response != undefined && response.DraftEffectiveDate != undefined ? response.DraftEffectiveDate : VRDateTimeService.getTodayDate();
                    });
                }

                return UtilsService.waitPromiseNode(rootPromiseNode);
            }

            return loadAllControls().catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadAllDataGrid(letter) {
            return WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.GetFilteredChanges(getCustomerSMSChangesQuery(customerInfo.CarrierAccountId, letter)).then(function (response) {
                if (response && response.length > 0) {
                    $scope.scopeModel.allDataSource = response;
                    getDataGridDataPage();
                }
            });
        }

        function getDataGridDataPage() {

            var pageInfo = gridAPI.getPageInfo();
            var toRow = Math.min($scope.scopeModel.allDataSource.length, pageInfo.toRow);

            var newDataSourceOnScrolling = $scope.scopeModel.allDataSource.slice(pageInfo.fromRow - 1, toRow);
            $scope.scopeModel.datasource = $scope.scopeModel.datasource.concat(newDataSourceOnScrolling);
        }

        function getCustomerSMSChangesQuery(customerID, countryChar) {
            return {
                ProcessDraftID: processDraftID,
                CustomerID: customerID,
                Filter: { CountryChar: countryChar }
            };
        }

        function getDraftDataQuery() {
            return {
                ProcessDraftID: processDraftID
            };
        }

        function buildDraftFromScope() {
            return {
                ProcessDraftID: processDraftID,
                CustomerID: customerInfo.CarrierAccountId,
                CurrencyId: customerInfo.CurrencyId,
                SMSRates: modifiedSmsRates,
                EffectiveDate: $scope.scopeModel.effectiveDate
            };
        }

        function onSaveOrApplyClicked() {

            if (modifiedSmsRates.length > 0 || (isEffectiveDateChanged && $scope.scopeModel.isCustomerSMSRateDraftExist)) {
                return saveChanges();
            }
            else {
                var onSaveLoadedPromiseDeferred = UtilsService.createPromiseDeferred();
                onSaveLoadedPromiseDeferred.resolve();
                return onSaveLoadedPromiseDeferred.promise;
            }
        }

        function saveChanges() {

            var customerDraftToUpdate = buildDraftFromScope();

            return WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.InsertOrUpdateChanges(customerDraftToUpdate).then(function (response) {
                if (response) {
                    processDraftID = response.ProcessDraftID;
                    VRNotificationService.showSuccess("Draft Saved Successfully");
                    if ($scope.onDraftSaved != undefined && typeof ($scope.onDraftSaved) == "function")
                        $scope.onDraftSaved(response.ProcessDraftID);
                }
                else {
                    VRNotificationService.showInformation("Draft Not Saved");
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_SMSBusinessEntity_CustomerSMSRatePlanEditorController', customerSMSRatePlanEditorController);
})(appControllers);
