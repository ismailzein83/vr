SuspiciousNumberDetailsController.$inject = ['$scope', 'StrategyAPIService', 'NormalCDRAPIService', 'SuspicionAnalysisAPIService', 'NumberProfileAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CaseManagementAPIService', 'CaseStatusEnum'];

function SuspiciousNumberDetailsController($scope, StrategyAPIService, NormalCDRAPIService, SuspicionAnalysisAPIService, NumberProfileAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CaseManagementAPIService, CaseStatusEnum) {
    var accountThresholdsGridAPI;
    var normalCDRGridAPI;
    var numberProfileGridAPI;
    var lastOccurance;
    var strategyName;
    var numberofOccurances;
    var strategiesList;
    var suspicionLevelsList;
    var isAccountThresholdsDataLoaded = false;
    var isNormalCDRDataLoaded = false;
    var isNumberProfileDataLoaded = false;
    var pageLoaded = false;

    loadParameters();
    defineScope();
    load();


    function loadParameters() {

        $scope.statuses = [];
        angular.forEach(CaseStatusEnum, function (status) {
            $scope.statuses.push({ id: status.id, name: status.name })
        });

        var parameters = VRNavigationService.getParameters($scope);

        $scope.accountNumber = undefined;

        if (parameters != undefined && parameters != null) {
            $scope.accountNumber = parameters.accountNumber;
            $scope.fromDate = parameters.fromDate;
            $scope.toDate = parameters.toDate;
            strategiesList = parameters.strategiesList;
            suspicionLevelsList = parameters.suspicionLevelsList;
        }



    }

    function defineScope() {

        $scope.filterDefinitions = [];
        $scope.aggregateDefinitions = [];
        $scope.accountThresholds = [];
        $scope.normalCDRs = [];
        $scope.numberProfiles = [];
        $scope.relatedNumbers = [];

        SuspiciousNumberDetailsController.isNumberProfileTabShown = false;
        SuspiciousNumberDetailsController.isNormalCDRTabShown = false;

        $scope.onNormalCDRsGridReady = function (api) {
            normalCDRGridAPI = api;
            if (SuspiciousNumberDetailsController.isNormalCDRTabShown) {
                return retrieveData_NormalCDRs();

            }
        };

        $scope.onNumberProfilesGridReady = function (api) {
            numberProfileGridAPI = api;
            if (SuspiciousNumberDetailsController.isNumberProfileTabShown) {
                return retrieveData_NumberProfiles();

            }
        };


        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.ApplyChangeStatus = function () {
            var accountCaseObject = BuildAccountCaseObjfromScope();
            CaseManagementAPIService.SaveAccountCase(accountCaseObject)
           .then(function (response) {
               if (VRNotificationService.notifyOnItemUpdated("AccountCase", response)) {
                   if ($scope.onAccountCaseUpdated != undefined) {
                       response.UpdatedObject.SuspicionLevelName = $scope.suspicionLevelName;
                       response.UpdatedObject.LastOccurance = lastOccurance;
                       response.UpdatedObject.StrategyName = strategyName;
                       response.UpdatedObject.NumberofOccurances = numberofOccurances;
                       response.UpdatedObject.CaseStatus = $scope.selectedStatus.name;
                       response.UpdatedObject.StatusId = $scope.selectedStatus.id;
                       response.UpdatedObject.ValidTill = $scope.validTill;
                       $scope.onAccountCaseUpdated(response.UpdatedObject);
                   }

                   $scope.modalContext.closeModal();
               }
           })
            .catch(
            function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope)
            });
        };

        $scope.groupKeySelectionChanged = function () {

            if ($scope.selectedGroupKeyIndex != undefined) {

                if ($scope.selectedGroupKeyIndex == 0 && !isNormalCDRDataLoaded) {
                    isNormalCDRDataLoaded = true;
                    return retrieveData_NormalCDRs();
                }
                else if ($scope.selectedGroupKeyIndex == 1 && !isNumberProfileDataLoaded) {
                    isNumberProfileDataLoaded = true;
                    return retrieveData_NumberProfiles();
                }

            }
        };

        $scope.selectedRelatedNumbersChanged = function () {
            if (pageLoaded) {
                $scope.accountNumber = $scope.selectedRelatedNumber
                isNormalCDRDataLoaded = false;
                isNumberProfileDataLoaded = false;
                $scope.groupKeySelectionChanged();
            }
            else {
                pageLoaded = true;
            }
        }

        $scope.dataRetrievalFunction_NormalCDRs = function (dataRetrievalInput, onResponseReady) {
            return NormalCDRAPIService.GetNormalCDRs(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }


        $scope.dataRetrievalFunction_NumberProfiles = function (dataRetrievalInput, onResponseReady) {
            return NumberProfileAPIService.GetNumberProfiles(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }




    }

    function load() {

        var number = parseInt($scope.accountNumber);

        var relatedNumbers = [];
        for (var i = 5; i >= 1; i--) {
            relatedNumbers.push(number - i);
        }
        relatedNumbers.push(number);
        for (var i = 1; i <= 5; i++) {
            relatedNumbers.push(number + i);
        }


        $scope.relatedNumbers = relatedNumbers;
        $scope.selectedRelatedNumber = $scope.accountNumber;


        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadFilters, loadAggregates])
        .then(function () {


            return SuspicionAnalysisAPIService.GetFraudResult($scope.fromDate, $scope.toDate, strategiesList.slice(0, -1), suspicionLevelsList.slice(0, -1), $scope.accountNumber).then(function (response) {

                $scope.suspicionLevelName = response.SuspicionLevelName;

                if (response.StatusId == null) {
                    $scope.selectedStatus = UtilsService.getItemByVal($scope.statuses, 1, "id");
                }
                else {
                    $scope.selectedStatus = UtilsService.getItemByVal($scope.statuses, response.StatusId, "id");
                }


                if (response.ValidTill == null) {
                    $scope.endDate = new Date();
                }
                else {
                    $scope.validTill = response.ValidTill;
                }

                lastOccurance = response.LastOccurance;
                strategyName = response.StrategyName;
                numberofOccurances = response.NumberofOccurances;
            }).finally(function () {
                $scope.isGettingData = false;
            });


        })
        .catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });


    }

    function loadFilters() {
        var index = 0;
        return StrategyAPIService.GetFilters().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.filterDefinitions.push({ filterId: ++index, description: itm.Description });
            });
        });
    }

    function loadAggregates() {
        return StrategyAPIService.GetAggregates().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.aggregateDefinitions.push({ name: itm });
            });
        });
    }

    function BuildAccountCaseObjfromScope() {
        var accountCaseObject = {
            AccountNumber: $scope.accountNumber,
            StatusId: $scope.selectedStatus.id,
            ValidTill: $scope.validTill
        };
        return accountCaseObject;
    }

    function retrieveData_AccountThresholds() {
        $scope.accountThresholds.length = 0;

        var query = {
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            AccountNumber: $scope.accountNumber
        };


        return accountThresholdsGridAPI.retrieveData(query);
    }

    function retrieveData_NormalCDRs() {

        var query = {
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            MSISDN: $scope.accountNumber
        };


        return normalCDRGridAPI.retrieveData(query);
    }

    function retrieveData_NumberProfiles() {

        var query = {
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            AccountNumber: $scope.accountNumber
        };

        return numberProfileGridAPI.retrieveData(query);
    }

}
appControllers.controller('SuspiciousNumberDetailsController', SuspiciousNumberDetailsController);
