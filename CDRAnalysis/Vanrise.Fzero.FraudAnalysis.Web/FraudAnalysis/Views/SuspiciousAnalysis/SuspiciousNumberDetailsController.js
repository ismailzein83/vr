SuspiciousNumberDetailsController.$inject = ['$scope', 'StrategyAPIService','OperatorTypeEnum', 'NormalCDRAPIService', 'SuspicionAnalysisAPIService', 'NumberProfileAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CaseManagementAPIService', 'CaseStatusEnum'];

function SuspiciousNumberDetailsController($scope, StrategyAPIService, OperatorTypeEnum, NormalCDRAPIService, SuspicionAnalysisAPIService, NumberProfileAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CaseManagementAPIService, CaseStatusEnum) {
    var normalCDRGridAPI;
    var numberProfileGridAPI;
    var relatedCaseGridAPI;

    var lastOccurance;
    var strategyName;
    var numberofOccurances;
    var strategiesList;
    var suspicionLevelsList;
    var isNormalCDRDataLoaded = false;
    var isNumberProfileDataLoaded = false;
    var isRelatedCaseDataLoaded = false;
    var pageLoaded = false;
    var defaultOperatorType;

    loadParameters();
    defineScope();
    load();


    function loadParameters() {

        $scope.statuses = [];
        $scope.selectedStatus = '';
        angular.forEach(CaseStatusEnum, function (status) {
            if (status != CaseStatusEnum.Open)
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
        $scope.normalCDRs = [];
        $scope.numberProfiles = [];
        $scope.relatedCases = [];
        $scope.relatedNumbers = [];
        $scope.operatorTypes = [];
        
        SuspiciousNumberDetailsController.isNormalCDRTabShown = true;
        SuspiciousNumberDetailsController.isNumberProfileTabShown = false;
        SuspiciousNumberDetailsController.isRelatedCaseTabShown = false;


        angular.forEach(OperatorTypeEnum, function (itm) {
            $scope.operatorTypes.push({ value: itm.value, name: itm.name })
        });


        SuspicionAnalysisAPIService.GetOperatorType()
         .then(function (response) {
             defaultOperatorType = UtilsService.getItemByVal($scope.operatorTypes, response, "value")
         });




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


        $scope.onRelatedCasesGridReady = function (api) {
            relatedCaseGridAPI = api;
            if (SuspiciousNumberDetailsController.isRelatedCaseTabShown) {
                return retrieveData_RelatedCases();

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
                else if ($scope.selectedGroupKeyIndex == 2 && !isRelatedCaseDataLoaded) {
                    isRelatedCaseDataLoaded = true;
                    return retrieveData_RelatedCases();
                }

            }
        };

        $scope.onvaluechanged = function () {
            if (pageLoaded) {
                isNormalCDRDataLoaded = false;
                isNumberProfileDataLoaded = false;
                isRelatedCaseDataLoaded = false;
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

        $scope.dataRetrievalFunction_RelatedCases = function (dataRetrievalInput, onResponseReady) {
            return CaseManagementAPIService.GetFilteredAccountCases(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }

       
        $scope.renderinMobileOnly = function () {
            console.log('defaultOperatorType.value')
            console.log(defaultOperatorType.value)
            console.log('OperatorTypeEnum.Mobile.value')
            console.log(OperatorTypeEnum.Mobile.value)
            return defaultOperatorType.value == OperatorTypeEnum.Mobile.value;
        }


        $scope.renderinPSTNOnly = function () {
            console.log('defaultOperatorType.value')
            console.log(defaultOperatorType.value)
            console.log('OperatorTypeEnum.Mobile.value')
            console.log(OperatorTypeEnum.Mobile.value)
            return defaultOperatorType.value == OperatorTypeEnum.PSTN.value;
        }


    }

    function load() {

        var number = parseInt($scope.accountNumber);

        var relatedNumbers = [];
        for (var i = 10; i >= 1; i--) {
            relatedNumbers.push(number - i);
        }
        relatedNumbers.push(number);
        for (var i = 1; i <= 10; i++) {
            relatedNumbers.push(number + i);
        }


        $scope.relatedNumbers = relatedNumbers;
        $scope.selectedRelatedNumber = $scope.accountNumber;


        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadFilters, loadAggregates])
        .then(function () {


            return SuspicionAnalysisAPIService.GetFraudResult($scope.fromDate, $scope.toDate, strategiesList.slice(0, -1), suspicionLevelsList.slice(0, -1), $scope.accountNumber).then(function (response) {

                $scope.suspicionLevelName = response.SuspicionLevelName;


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
        var validTill = null;
        if ($scope.selectedStatus.id == 3 || $scope.selectedStatus.id == 4) //Fruad or Whitelist
        {
            validTill = $scope.validTill;
        }

        var accountCaseObject = {
            AccountNumber: $scope.accountNumber,
            StatusId: $scope.selectedStatus.id,
            ValidTill: validTill,
            SuspicionLevelID: $scope.relatedCases[$scope.relatedCases.length - 1].SuspicionLevelID,
            StrategyId: $scope.relatedCases[$scope.relatedCases.length - 1].StrategyId
        };
        return accountCaseObject;
    }

    function retrieveData_NormalCDRs() {
        if (normalCDRGridAPI != undefined)
        {
            var query = {
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate,
                MSISDN: $scope.selectedRelatedNumber
            };


            return normalCDRGridAPI.retrieveData(query);
        }
        
    }

    function retrieveData_NumberProfiles() {

        var query = {
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            AccountNumber: $scope.selectedRelatedNumber
        };

        return numberProfileGridAPI.retrieveData(query);
    }

    function retrieveData_RelatedCases() {

        var query = {
            AccountNumber: $scope.selectedRelatedNumber
        };

        return relatedCaseGridAPI.retrieveData(query);
    }

   


}
appControllers.controller('SuspiciousNumberDetailsController', SuspiciousNumberDetailsController);
