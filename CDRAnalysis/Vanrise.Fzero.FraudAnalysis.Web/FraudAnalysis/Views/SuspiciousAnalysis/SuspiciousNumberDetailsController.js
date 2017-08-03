"use strict";

SuspiciousNumberDetailsController.$inject = ["$scope", "CDRAPIService", "NumberProfileAPIService", "StrategyAPIService", "VR_Sec_UserAPIService", "CDRAnalysis_FA_SuspicionLevelEnum", "CDRAnalysis_FA_CaseStatusEnum", "LabelColorsEnum", "UtilsService", "VRNavigationService", "VRNotificationService", "VRModalService", "VRValidationService",'CDRAnalysis_FA_AccountCaseAPIService','CDRAnalysis_FA_RelatedNumberAPIService','CDRAnalysis_FA_StrategyExecutionItemAPIService'];

function SuspiciousNumberDetailsController($scope, CDRAPIService, NumberProfileAPIService, StrategyAPIService, VR_Sec_UserAPIService, CDRAnalysis_FA_SuspicionLevelEnum, CDRAnalysis_FA_CaseStatusEnum, LabelColorsEnum, UtilsService, VRNavigationService, VRNotificationService, VRModalService, VRValidationService, CDRAnalysis_FA_AccountCaseAPIService, CDRAnalysis_FA_RelatedNumberAPIService, CDRAnalysis_FA_StrategyExecutionItemAPIService) {
    var gridOccurances_ReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    var gridAPI_Occurances = undefined;
    var occurancesLoaded = false;

    var gridAPI_CDRs = undefined;


    var gridAPI_NumberProfiles = undefined;

    var gridAPI_CaseHistory = undefined;
    var casesLoaded = false;

    var gridAPI_RelatedNumbers = undefined;
    var sequencedNumbers = [];
    var IMEIs = [];

    var modalLevel = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            $scope.accountNumber = parameters.AccountNumber;
            $scope.fromDate = parameters.FromDate;
            $scope.toDate = parameters.ToDate;
            $scope.caseID = parameters.CaseID;

            modalLevel = parameters.ModalLevel;
            $scope.showRelatedNumbers = (modalLevel == 1);
        }
    }

    function defineScope() {

        $scope.hasUpdateAccountCasePermission = function () {
            return CDRAnalysis_FA_AccountCaseAPIService.HasUpdateAccountCasePermission();
        };

        $scope.users = []; // the users array must be defined on the scope so that it can be passed to the case logs subgrid via viewScope
        $scope.selectedTabIndex = 0;

        // Current Occurances
        $scope.occurances = [];
        $scope.showOccurancesGrid = true;
        $scope.message = undefined;

        // Normal CDRs
        $scope.fromDate_CDRs = $scope.fromDate;
        $scope.toDate_CDRs = $scope.toDate;

        $scope.validateTimeRangeCDRs = function () {
            return VRValidationService.validateTimeRange($scope.fromDate_CDRs, $scope.toDate_CDRs);
        };



        $scope.cdrs = [];

        // Number Profiles
        $scope.profileSources = [
            { value: 0, description: "From Strategy Execution" },
            { value: 1, description: "From Profiling" }
        ];
        $scope.selectedProfileSource = $scope.profileSources[0];

        $scope.fromDate_NumberProfiles = $scope.fromDate;
        $scope.toDate_NumberProfiles = $scope.toDate;

        $scope.validateTimeRangeNumberProfiles = function () {
            return VRValidationService.validateTimeRange($scope.fromDate_NumberProfiles, $scope.toDate_NumberProfiles);
        };

        $scope.aggregateDefinitions = []; // column names
        $scope.detailAggregateValues = [];
        $scope.numberProfile = [];

        $scope.showProfileOptions = true;
        $scope.showDate = false;

        // Case History
        $scope.cases = [];

        // Related Numbers
        $scope.reasons = [
            { value: 0, description: "Sequence" },
            { value: 1, description: "IMEIs" }
        ];
        $scope.selectedReason = $scope.reasons[0];
        $scope.relatedNumbers = [];

        // Update Case
        $scope.caseStatuses = UtilsService.getArrayEnum(CDRAnalysis_FA_CaseStatusEnum);
        $scope.selectedCaseStatus = undefined;
        $scope.updateReason = undefined;
        $scope.validTill = undefined;
        $scope.whiteListSelected = false;

        $scope.onGridReady_Occurances = function (api) {
            gridAPI_Occurances = api;
            gridOccurances_ReadyPromiseDeferred.resolve();
        };

        $scope.onGridReady_CDRs = function (api) {
            gridAPI_CDRs = api;
        };

        $scope.onGridReady_NumberProfiles = function (api) {
            gridAPI_NumberProfiles = api;
        };

        $scope.onGridReady_CaseHistory = function (api) {
            gridAPI_CaseHistory = api;
        };

        $scope.onGridReady_RelatedNumbers = function (api) {
            gridAPI_RelatedNumbers = api;

            if (sequencedNumbers.length == 0) {
                setSequencedNumbers();

                angular.forEach(sequencedNumbers, function (item) {
                    $scope.relatedNumbers.push(item);
                });
            }
        };

        $scope.relatedNumberClicked = function (dataItem) {
            openRelatedNumber(dataItem.RelatedNumber);
        };

        $scope.dataRetrievalFunction_Occurances = function (dataRetrievalInput, onResponseReady) {

            return CDRAnalysis_FA_StrategyExecutionItemAPIService.GetFilteredDetailsByCaseID(dataRetrievalInput)
            .then(function (response) {

                occurancesLoaded = true;

                if (response.Data != undefined) { // else, the export button was clicked

                    if (response.Data.length > 0) {
                        for (var i = 0; i < response.Data.length; i++) {
                            $scope.detailAggregateValues.push(UtilsService.cloneObject(response.Data[i]));
                        }
                    }
                    else {
                        setNoOccurences();
                    }

                    angular.forEach(response.Data, function (item) {
                        var suspicionLevel = UtilsService.getEnum(CDRAnalysis_FA_SuspicionLevelEnum, "value", item.SuspicionLevelID);
                        item.SuspicionLevelDescription = suspicionLevel.description;
                    });
                }

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        };

        $scope.dataRetrievalFunction_CDRs = function (dataRetrievalInput, onResponseReady) {

            return CDRAPIService.GetCDRs(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        };

        $scope.dataRetrievalFunction_NumberProfiles = function (dataRetrievalInput, onResponseReady) {

            return NumberProfileAPIService.GetNumberProfiles(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        };

        $scope.dataRetrievalFunction_CaseHistory = function (dataRetrievalInput, onResponseReady) {

            return CDRAnalysis_FA_AccountCaseAPIService.GetFilteredCasesByAccountNumber(dataRetrievalInput)
            .then(function (response) {
                casesLoaded = true;

                angular.forEach(response.Data, function (item) {
                    var caseStatus = UtilsService.getEnum(CDRAnalysis_FA_CaseStatusEnum, "value", item.StatusID);
                    item.CaseStatusDescription = caseStatus.description;

                    var user = UtilsService.getItemByVal($scope.users, item.UserID, "UserId");
                    item.UserName = (user != null) ? user.Name : "System";
                });

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        };
        

        $scope.updateAccountCase = function () {
            return CDRAnalysis_FA_AccountCaseAPIService.UpdateAccountCase({
                AccountNumber: $scope.accountNumber,
                CaseStatus: $scope.selectedCaseStatus.value,
                ValidTill: $scope.validTill,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate,
                Reason: $scope.updateReason
            })
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Account Case", response)) {
                        if ($scope.onAccountCaseUpdated != undefined) {

                            if (response.UpdatedObject.SuspicionLevelID != 0) {
                                var suspicionLevel = UtilsService.getEnum(CDRAnalysis_FA_SuspicionLevelEnum, "value", response.UpdatedObject.SuspicionLevelID);
                                response.UpdatedObject.SuspicionLevelDescription = suspicionLevel.description;
                            }

                            var accountStatus = UtilsService.getEnum(CDRAnalysis_FA_CaseStatusEnum, "value", response.UpdatedObject.Status);
                            response.UpdatedObject.AccountStatusDescription = accountStatus.description;

                            $scope.onAccountCaseUpdated(response.UpdatedObject);
                        }

                        $scope.modalContext.closeModal();
                    }

                    $scope.modalContext.closeModal();
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        
        $scope.onSelectedTabChanged = function () {
            return onSelectedTabChanged();
        };

        $scope.onProfileSourceChanged = function () {

            if ($scope.selectedProfileSource != null) {
                if ($scope.selectedProfileSource.value == 1) {
                    $scope.showDate = true;
                    retrieveData_NumberProfiles();
                }
                else
                    $scope.showDate = false;
            }
        };

        $scope.toggleValidTill = function (selectedStatus) {
            $scope.whiteListSelected = (selectedStatus != undefined && selectedStatus.value == CDRAnalysis_FA_CaseStatusEnum.ClosedWhitelist.value);
        };

        $scope.onReasonChanged = function () {

            if ($scope.selectedReason != undefined) {
                if ($scope.selectedReason.value == 0) {

                    if (sequencedNumbers.length == 0)
                        setSequencedNumbers();

                    angular.forEach(sequencedNumbers, function (item) {
                        $scope.relatedNumbers.push(item);
                    });
                }
                else {

                    if (IMEIs.length == 0)
                        loadRelatedNumbers();
                    else {
                        $scope.relatedNumbers = [];

                        angular.forEach(IMEIs, function (item) {
                            $scope.relatedNumbers.push(item);
                        });
                    }
                }
            }
        };

        $scope.getSuspicionLevelColor = function (dataItem) {

            if (dataItem.SuspicionLevelID == CDRAnalysis_FA_SuspicionLevelEnum.Suspicious.value) return LabelColorsEnum.WarningLevel1.color;
            else if (dataItem.SuspicionLevelID == CDRAnalysis_FA_SuspicionLevelEnum.HighlySuspicious.value) return LabelColorsEnum.WarningLevel2.color;
            else if (dataItem.SuspicionLevelID == CDRAnalysis_FA_SuspicionLevelEnum.Fraud.value) return LabelColorsEnum.Error.color;
        };

        $scope.getCaseStatusColor = function (dataItem) {

            if (dataItem.StatusID == CDRAnalysis_FA_CaseStatusEnum.Open.value) return LabelColorsEnum.New.color;
            else if (dataItem.StatusID == CDRAnalysis_FA_CaseStatusEnum.Pending.value) return LabelColorsEnum.Processing.color;
            else if (dataItem.StatusID == CDRAnalysis_FA_CaseStatusEnum.ClosedFraud.value) return LabelColorsEnum.Error.color;
            else if (dataItem.StatusID == CDRAnalysis_FA_CaseStatusEnum.ClosedWhitelist.value) return LabelColorsEnum.Success.color;
        };

        $scope.searchCDRs = retrieveData_CDRs;

        $scope.searchNumberProfiles = retrieveData_NumberProfiles;
    }

    function load() {
        
        var allPromises = [];

        var loadPrerequisiteDataPromise = UtilsService.waitMultipleAsyncOperations([loadAggregateDefinitions, loadUsers]);
        allPromises.push(loadPrerequisiteDataPromise);

        var loadMainSectionsPromiseDeferred = UtilsService.createPromiseDeferred();
        allPromises.push(loadMainSectionsPromiseDeferred.promise);

        loadPrerequisiteDataPromise.then(function () {
            UtilsService.waitMultipleAsyncOperations([loadAccountCaseAndOccurances])
            .then(function () {
                loadMainSectionsPromiseDeferred.resolve();
            })
            .catch(function (error) {
                loadMainSectionsPromiseDeferred.reject(error);
            });
        });

        $scope.isLoading = true;
        return UtilsService.waitMultiplePromises(allPromises)
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function onSelectedTabChanged() {        
        if (gridAPI_CaseHistory != undefined && $scope.selectedTabIndex == 3 && !casesLoaded)
            retrieveData_CaseHistory();
    }

    function setNoOccurences() {
        $scope.showOccurancesGrid = false;
        $scope.message = "No open occurences were found for the current account number";
        $scope.showProfileOptions = false;
        $scope.showDate = true;
    }

    function retrieveData_CDRs() {
        var query = {
            MSISDN: $scope.accountNumber,
            FromDate: $scope.fromDate_CDRs,
            ToDate: $scope.toDate_CDRs,
        };

        return gridAPI_CDRs.retrieveData(query)
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });

    }

    function retrieveData_NumberProfiles() {
       
        var query = {
            AccountNumber: $scope.accountNumber,
            FromDate: $scope.fromDate_NumberProfiles,
            ToDate: $scope.toDate_NumberProfiles
        };

        return gridAPI_NumberProfiles.retrieveData(query)
              .catch(function (error) {
                  VRNotificationService.notifyException(error, $scope);
              });
    }

    function retrieveData_CaseHistory() {
        $scope.isLoading = true;
        var query = {
            AccountNumber: $scope.accountNumber
        };

        return gridAPI_CaseHistory.retrieveData(query)
              .catch(function (error) {
                  VRNotificationService.notifyException(error, $scope);
              })
              .finally(function () {
                  $scope.isLoading = false;
              });

    }

    function loadAccountCaseAndOccurances() {

        $scope.isLoading = true;

        var promises = [];

        var getAccountCasePromise = getAccountCase();
        promises.push(getAccountCasePromise);

        var getOccurencesPromiseDeferred = UtilsService.createPromiseDeferred();
        promises.push(getOccurencesPromiseDeferred.promise);


        function getAccountCase() {
            var local_getAccountCasePromise;
            if ($scope.caseID == undefined)
                local_getAccountCasePromise = CDRAnalysis_FA_AccountCaseAPIService.GetLastAccountCase($scope.accountNumber);
            else
                local_getAccountCasePromise = CDRAnalysis_FA_AccountCaseAPIService.GetAccountCase($scope.caseID);
            local_getAccountCasePromise
                .then(function (response) {
                    if (response != null) {
                        $scope.caseID = response.CaseID;
                        $scope.caseStatuses.splice(0, 1); // remove the open option

                        if (response.StatusID == CDRAnalysis_FA_CaseStatusEnum.Pending.value)
                            $scope.caseStatuses.splice(0, 1); // remove the pending option

                        else if (response.StatusID == CDRAnalysis_FA_CaseStatusEnum.ClosedFraud.value)
                            $scope.caseStatuses.splice(1, 1); // remove the closed: fruad option

                        else if (response.StatusID == CDRAnalysis_FA_CaseStatusEnum.ClosedWhitelist.value)
                            $scope.caseStatuses.splice(2, 1); // remove the closed: white list option

                        var accountStatus = UtilsService.getEnum(CDRAnalysis_FA_CaseStatusEnum, "value", response.StatusID);
                        $scope.status = response.StatusID;
                        $scope.accountStatusDescription = accountStatus.description;
                        
                    }
                    getOccurances();                   
                });
            return local_getAccountCasePromise;
        }

        function getOccurances() {
            if ($scope.caseID != undefined) {
                var query = {
                    CaseID: $scope.caseID,
                    FromDate: $scope.fromDate,
                    ToDate: $scope.toDate
                };

                gridAPI_Occurances.retrieveData(query)
                     .then(function () {
                         getOccurencesPromiseDeferred.resolve();
                     })
                     .catch(function (error) {
                         getOccurencesPromiseDeferred.reject(error);
                     });
            }
            else {
                setNoOccurences();
                getOccurencesPromiseDeferred.resolve();
            }
        }

        return UtilsService.waitMultiplePromises(promises);
    }

    function loadAggregateDefinitions() {
        return StrategyAPIService.GetAggregates()
            .then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.aggregateDefinitions.push({ name: item.Name, numberPrecision: item.NumberPrecision });
                });
            })
    }

    function loadUsers() {
        return VR_Sec_UserAPIService.GetUsers()
            .then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.users.push(item);
                });
            });
    }

    function loadRelatedNumbers() {
        $scope.relatedNumbers = [];

        return CDRAnalysis_FA_RelatedNumberAPIService.GetRelatedNumbersByAccountNumber($scope.accountNumber)
            .then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.relatedNumbers.push({ RelatedNumber: item.RelatedAccountNumber });
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function setSequencedNumbers() {

        for (var i = 10; i >= 1; i--) {
            sequencedNumbers.push({ RelatedNumber: ($scope.accountNumber - i) });
        }

        for (var i = 1; i <= 10; i++) {
            sequencedNumbers.push({ RelatedNumber: (Number($scope.accountNumber) + i) });
        }
    }

    function openRelatedNumber(relatedNumber) {
        var modalSettings = {};

        var parameters = {
            AccountNumber: relatedNumber,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            ModalLevel: modalLevel + 1
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details";
            modalScope.onAccountCaseUpdated = function (accountSuspicionSummary) {
                $scope.onAccountCaseUpdated(accountSuspicionSummary);
            };
        };

        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousAnalysis/SuspiciousNumberDetails.html", parameters, modalSettings);
    }
}

appControllers.controller("FraudAnalysis_SuspiciousNumberDetailsController", SuspiciousNumberDetailsController);
