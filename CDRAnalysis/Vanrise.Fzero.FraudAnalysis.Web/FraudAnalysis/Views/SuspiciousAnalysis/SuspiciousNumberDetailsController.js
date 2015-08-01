SuspiciousNumberDetailsController.$inject = ['$scope', 'StrategyAPIService', 'NormalCDRAPIService' , 'SuspicionAnalysisAPIService', 'NumberProfileAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CaseManagementAPIService'];

function SuspiciousNumberDetailsController($scope, StrategyAPIService, NormalCDRAPIService, SuspicionAnalysisAPIService, NumberProfileAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CaseManagementAPIService) {
    var subscriberThresholdsGridAPI;
    var normalCDRGridAPI;
    var numberProfileGridAPI;
    var lastOccurance;
    var strategyName;
    var numberofOccurances;
    var strategiesList;
    var suspicionLevelsList;
    var isSubscriberThresholdsDataLoaded = false;
    var isNormalCDRDataLoaded = false;
    var isNumberProfileDataLoaded = false;
    var pageLoaded = false;

    loadParameters();
    defineScope();
    load();


    function loadParameters() {

        $scope.statuses = [{ id: 1, name: 'Pending' }, { id: 2, name: 'Ignored' }, { id: 3, name: 'Confirmed' }, { id: 4, name: 'White List' }];

        var parameters = VRNavigationService.getParameters($scope);
       
        $scope.subscriberNumber = undefined;

        if (parameters != undefined && parameters != null)
        {
            $scope.subscriberNumber = parameters.subscriberNumber;
            $scope.fromDate = parameters.fromDate;
            $scope.toDate = parameters.toDate;

            strategiesList = parameters.strategiesList;
            suspicionLevelsList = parameters.suspicionLevelsList;
        }
        
        
        
    }
    

   

    function defineScope() {

        $scope.filterDefinitions = [];
        $scope.aggregateDefinitions = [];
        $scope.subscriberThresholds = [];
        $scope.normalCDRs = [];
        $scope.numberProfiles = [];
        $scope.relatedNumbers = [];
        
        SuspiciousNumberDetailsController.isSubscriberThresholdsTabShown = true;
        SuspiciousNumberDetailsController.isNumberProfileTabShown = false;
        SuspiciousNumberDetailsController.isNormalCDRTabShown = false;

        $scope.onSubscriberThresholdsGridReady = function (api) {
            subscriberThresholdsGridAPI = api;
            if (SuspiciousNumberDetailsController.isSubscriberThresholdsTabShown) {
                getSubscriberThresholds();
            }
        };

        $scope.onNormalCDRsGridReady = function (api) {
            normalCDRGridAPI = api;
            if (SuspiciousNumberDetailsController.isNormalCDRTabShown) {
                getNormalCDRs();

            }
        };

        $scope.onNumberProfilesGridReady = function (api) {
            numberProfileGridAPI = api;
            if (SuspiciousNumberDetailsController.isNumberProfileTabShown) {
                getNumberProfiles();

            }
        };

        $scope.loadMoreDataSubscriberThresholds = function () {
            return getSubscriberThresholds();
        }

        

        $scope.loadMoreDataNormalCDRs = function () {
            return getNormalCDRs();
        }


       

        $scope.loadMoreDataNumberProfiles = function () {
            return getNumberProfiles();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.ApplyChangeStatus = function () {
            var subscriberCaseObject = BuildSubscriberCaseObjfromScope();
            CaseManagementAPIService.SaveSubscriberCase(subscriberCaseObject)
           .then(function (response) {
               if (VRNotificationService.notifyOnItemUpdated("SubscriberCase", response)) {
                   if ($scope.onSubscriberCaseUpdated != undefined)
                   {
                       response.UpdatedObject.SuspicionLevelName = $scope.suspicionLevelName;
                       response.UpdatedObject.LastOccurance = lastOccurance;
                       response.UpdatedObject.StrategyName = strategyName;
                       response.UpdatedObject.NumberofOccurances = numberofOccurances;
                       response.UpdatedObject.CaseStatus = $scope.selectedStatus.name;
                       response.UpdatedObject.StatusId = $scope.selectedStatus.id;
                       response.UpdatedObject.ValidTill = $scope.validTill;
                       $scope.onSubscriberCaseUpdated(response.UpdatedObject);
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


                if ($scope.selectedGroupKeyIndex == 0 && !isSubscriberThresholdsDataLoaded) {
                    getSubscriberThresholds();
                    isSubscriberThresholdsDataLoaded = true;
                }
                else if ($scope.selectedGroupKeyIndex == 1 && !isNormalCDRDataLoaded) {
                    getNormalCDRs();
                    isNormalCDRDataLoaded = true;
                }
                else if ($scope.selectedGroupKeyIndex == 2 && !isNumberProfileDataLoaded) {
                    getNumberProfiles();
                    isNumberProfileDataLoaded = true;
                }

            }
        };

        $scope.selectedRelatedNumbersChanged = function () {
            if (pageLoaded)
            {
                $scope.subscriberNumber = $scope.selectedRelatedNumber
                isSubscriberThresholdsDataLoaded=false;
                isNormalCDRDataLoaded=false;
                isNumberProfileDataLoaded = false;
                $scope.groupKeySelectionChanged();
            }
            else
            {
                pageLoaded = true;
            }
        }

    }

    function load() {

        var number = parseInt($scope.subscriberNumber);

        var relatedNumbers = [];
        for (var i = 5; i >= 1; i--) {
            relatedNumbers.push(number - i);
        }
        relatedNumbers.push(number);
        for (var i = 1; i <= 5; i++) {
            relatedNumbers.push(number + i);
        }


        $scope.relatedNumbers = relatedNumbers;
        $scope.selectedRelatedNumber = $scope.subscriberNumber;


        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadAggregates], [loadFilters])
        .then(function () {


            SuspicionAnalysisAPIService.GetFraudResult($scope.fromDate, $scope.toDate, strategiesList.slice(0, -1), suspicionLevelsList.slice(0, -1), $scope.subscriberNumber).then(function (response) {

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
            });

            $scope.isGettingData = false;

        })
        .catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });


    }

    function loadFilters() {
        console.log('loadFilters')
        return StrategyAPIService.GetFilters().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.filterDefinitions.push({ filterId: itm.FilterId, description: itm.Description });
            });
        });
    }


    function loadAggregates() {
        console.log('loadAggregates')
        return StrategyAPIService.GetAggregates().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.aggregateDefinitions.push({ aggregateId: itm.AggregateId, name: itm.Name });
            });
        });
    }

    function BuildSubscriberCaseObjfromScope()
    {
        var subscriberCaseObject = {
            SubscriberNumber: $scope.subscriberNumber,
            StatusId: $scope.selectedStatus.id,
            ValidTill: $scope.validTill
        };
        return subscriberCaseObject;
    }
    
    function getSubscriberThresholds() {
        $scope.subscriberThresholds.length = 0;

        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';


        var pageInfo = subscriberThresholdsGridAPI.getPageInfo();

        $scope.isGettingSubscriberThresholds = true;
        return SuspicionAnalysisAPIService.GetSubscriberThresholds(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.subscriberThresholds.push(itm);
            });
        }).finally(function () {
            $scope.isGettingSubscriberThresholds = false;
        });
    }

    function getNormalCDRs() {
        $scope.normalCDRs.length = 0;

        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';
        var pageInfo = normalCDRGridAPI.getPageInfo();
        $scope.isGettingNormalCDRs = true;
        return NormalCDRAPIService.GetNormalCDRs(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.normalCDRs.push(itm);
            });
        }).finally(function () {
            $scope.isGettingNormalCDRs = false;
        });
    }

    function getNumberProfiles() {
        $scope.numberProfiles.length = 0;
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';


        var pageInfo = numberProfileGridAPI.getPageInfo();

        $scope.isGettingNumberProfiles = true;
        return NumberProfileAPIService.GetNumberProfiles(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.numberProfiles.push(itm);
            });
        }).finally(function () {
            $scope.isGettingNumberProfiles = false;
        });
    }

    
    
}
appControllers.controller('SuspiciousNumberDetailsController', SuspiciousNumberDetailsController);
