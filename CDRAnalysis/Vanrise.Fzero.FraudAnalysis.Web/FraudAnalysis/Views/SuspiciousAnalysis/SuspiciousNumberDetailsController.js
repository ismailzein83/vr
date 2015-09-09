SuspiciousNumberDetailsController.$inject = ["$scope", "CaseManagementAPIService", "NormalCDRAPIService", "NumberProfileAPIService", "StrategyAPIService", "SuspicionLevelEnum", "CaseStatusEnum", "SuspicionOccuranceStatusEnum", "CallTypeEnum", "UtilsService", "VRNavigationService", "VRNotificationService"];

function SuspiciousNumberDetailsController($scope, CaseManagementAPIService, NormalCDRAPIService, NumberProfileAPIService, StrategyAPIService,  SuspicionLevelEnum, CaseStatusEnum, SuspicionOccuranceStatusEnum, CallTypeEnum, UtilsService, VRNavigationService, VRNotificationService) {

    var gridAPI_Occurances = undefined;
    var occurancesLoaded = false;

    var gridAPI_NormalCDRs = undefined;
    var normalCDRsLoaded = false;

    var gridAPI_NumberProfiles = undefined;
    var numberProfilesLoaded = false;

    var gridAPI_CaseHistory = undefined;
    var casesLoaded = false;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            $scope.accountNumber = parameters.AccountNumber;
            $scope.from = parameters.From;
            $scope.to = parameters.To;
        }
    }

    function defineScope() {

        $scope.selectedTabIndex = 0;
        $scope.occurances = [];
        $scope.normalCDRs = [];
        $scope.numberProfiles = [];
        $scope.aggregateDefinitions = [];
        $scope.cases = [];

        $scope.caseStatuses = [];
        $scope.selectedCaseStatus = undefined;
        $scope.whiteListSelected = false;

        $scope.onGridReady_Occurances = function (api) {
            gridAPI_Occurances = api;

            if ($scope.occurancesSelected)
                return retrieveData_Occurances();
        }

        $scope.onGridReady_NormalCDRs = function (api) {
            gridAPI_NormalCDRs = api;

            if ($scope.normalCDRsSelected)
                return retrieveData_NormalCDRs();
        }

        $scope.onGridReady_NumberProfiles = function (api) {
            gridAPI_NumberProfiles = api;

            if ($scope.numberProfilesSelected)
                return retrieveData_NumberProfiles();
        }

        $scope.onGridReady_CaseHistory = function (api) {
            gridAPI_CaseHistory = api;

            if ($scope.caseHistorySelected)
                return retrieveData_CaseHistory();
        }

        $scope.dataRetrievalFunction_Occurances = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredAccountSuspicionDetails(dataRetrievalInput)
            .then(function (response) {
                occurancesLoaded = true;

                angular.forEach(response.Data, function (item) {
                    var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", item.SuspicionLevelID);
                    item.SuspicionLevelDescription = suspicionLevel.description;

                    var detailStatus = UtilsService.getEnum(SuspicionOccuranceStatusEnum, "value", item.SuspicionOccuranceStatus);
                    item.SuspicionOccuranceStatusDescription = detailStatus.description;
                });

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        $scope.dataRetrievalFunction_NormalCDRs = function (dataRetrievalInput, onResponseReady) {

            return NormalCDRAPIService.GetNormalCDRs(dataRetrievalInput)
            .then(function (response) {
                normalCDRsLoaded = true;

                angular.forEach(response.Data, function (item) {
                    var callType = UtilsService.getEnum(CallTypeEnum, "value", item.CallType);
                    item.CallTypeDescription = callType.description;
                });

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        $scope.dataRetrievalFunction_NumberProfiles = function (dataRetrievalInput, onResponseReady) {

            return NumberProfileAPIService.GetNumberProfiles(dataRetrievalInput)
            .then(function (response) {
                numberProfilesLoaded = true;
                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        $scope.dataRetrievalFunction_CaseHistory = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredCasesByAccountNumber(dataRetrievalInput)
            .then(function (response) {
                casesLoaded = true;

                angular.forEach(response.Data, function (item) {
                    var caseStatus = UtilsService.getEnum(CaseStatusEnum, "value", item.StatusID);
                    item.CaseStatusDescription = caseStatus.description;
                });

                onResponseReady(response);
            })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        $scope.updateAccountCase = function () {

            return CaseManagementAPIService.UpdateAccountCase({
                    accountNumber: $scope.accountNumber,
                    caseStatus: $scope.selectedCaseStatus.value,
                    validTill: $scope.validTill,
                    from: $scope.from,
                    to: $scope.to
                })
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Account Case", response)) {
                        if ($scope.onAccountCaseUpdated != undefined) {

                            var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", response.UpdatedObject.SuspicionLevelID);
                            response.UpdatedObject.SuspicionLevelDescription = suspicionLevel.description;

                            var accountStatus = UtilsService.getEnum(CaseStatusEnum, "value", response.UpdatedObject.AccountStatusID);
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
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }

        $scope.filterDetails = function () {

            if ($scope.selectedTabIndex == 0)
                occurancesLoaded = false; // re-load the occurances

            else if ($scope.selectedTabIndex == 1)
                normalCDRsLoaded = false; // re-load the normal cdrs

            else if ($scope.selectedTabIndex == 2)
                numberProfilesLoaded = false; // re-load the number profiles

            else if ($scope.selectedTabIndex == 3)
                numberProfilesLoaded = false; // re-load the account cases

            return retrieveData();
        }

        $scope.onSelectedTabChanged = function () {
            return retrieveData();
        }

        $scope.toggleValidTill = function (selectedStatus) {
            $scope.whiteListSelected = (selectedStatus != undefined && selectedStatus.value == CaseStatusEnum.ClosedWhitelist.value) ? true : false;
        }
    }

    function load() {
        $scope.isInitializing = true;

        $scope.caseStatuses = UtilsService.getArrayEnum(CaseStatusEnum);

        return StrategyAPIService.GetAggregates()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.aggregateDefinitions.push({ name: item.Name });
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isInitializing = false;
            });
    }

    function retrieveData() {

        if (gridAPI_Occurances != undefined && $scope.selectedTabIndex == 0 && !occurancesLoaded)
            return retrieveData_Occurances();

        else if (gridAPI_NormalCDRs != undefined && $scope.selectedTabIndex == 1 && !normalCDRsLoaded)
            return retrieveData_NormalCDRs();

        else if (gridAPI_NumberProfiles != undefined && $scope.selectedTabIndex == 2 && !numberProfilesLoaded)
            return retrieveData_NumberProfiles();

        else if (gridAPI_CaseHistory != undefined && $scope.selectedTabIndex == 3 && !casesLoaded)
            return retrieveData_CaseHistory();
    }

    function retrieveData_Occurances() {

        var query = {
            AccountNumber: $scope.accountNumber,
            From: $scope.from,
            To: $scope.to
        };

        return gridAPI_Occurances.retrieveData(query);
    }

    function retrieveData_NormalCDRs() {

        var query = {
            FromDate: $scope.from,
            ToDate: $scope.to,
            MSISDN: $scope.accountNumber
        };

        return gridAPI_NormalCDRs.retrieveData(query);
    }

    function retrieveData_NumberProfiles() {

        var query = {
            FromDate: $scope.from,
            ToDate: $scope.to,
            AccountNumber: $scope.accountNumber
        };

        return gridAPI_NumberProfiles.retrieveData(query);
    }

    function retrieveData_CaseHistory() {

        var query = {
            AccountNumber: $scope.accountNumber,
            From: $scope.from,
            To: $scope.to
        };

        return gridAPI_CaseHistory.retrieveData(query);
    }
}

appControllers.controller("FraudAnalysis_SuspiciousNumberDetailsController", SuspiciousNumberDetailsController);
