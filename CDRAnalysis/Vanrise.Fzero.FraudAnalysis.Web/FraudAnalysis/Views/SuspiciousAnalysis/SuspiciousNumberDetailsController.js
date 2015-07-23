SuspiciousNumberDetailsController.$inject = ['$scope', 'StrategyAPIService', 'SuspicionAnalysisAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CaseManagementAPIService'];

function SuspiciousNumberDetailsController($scope, StrategyAPIService, SuspicionAnalysisAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CaseManagementAPIService) {
    var subscriberThresholdsGridAPI;
    var normalCDRGridAPI;
    var numberProfileGridAPI;
    var lastOccurance;
    var strategyName;
    var numberofOccurances;

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
            $scope.suspicionLevelName = parameters.suspicionLevelName;
            $scope.fromDate = parameters.fromDate;
            $scope.toDate = parameters.toDate;
            $scope.selectedStatus = UtilsService.getItemByVal($scope.statuses, parameters.statusId, "id");
            $scope.endDate = parameters.validTill;
            lastOccurance = parameters.lastOccurance;
            strategyName = parameters.strategyName;
            numberofOccurances = parameters.numberofOccurances;

        }
            
        
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
        return SuspicionAnalysisAPIService.GetNormalCDRs(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
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
        return SuspicionAnalysisAPIService.GetNumberProfiles(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.numberProfiles.push(itm);
            });
        }).finally(function () {
            $scope.isGettingNumberProfiles = false;
        });
    }




    function defineScope() {

        $scope.subscriberThresholds = [];
        $scope.normalCDRs = [];
        $scope.numberProfiles = [];
        $scope.relatedNumbers = [];
        $scope.endDate = new Date();


        


        SuspiciousNumberDetailsController.isSubscriberThresholdsTabShown = true;
        SuspiciousNumberDetailsController.isNumberProfileTabShown = false;
        SuspiciousNumberDetailsController.isNormalCDRTabShown = false;


        $scope.onSubscriberThresholdsGridReady = function (api) {
            subscriberThresholdsGridAPI = api;
        };

        $scope.loadMoreDataSubscriberThresholds = function () {
            return getSubscriberThresholds();
        }


        $scope.onNormalCDRsGridReady = function (api) {
            normalCDRGridAPI = api;
        };

        $scope.loadMoreDataNormalCDRs = function () {
            return getNormalCDRs();
        }


        $scope.onNumberProfilesGridReady = function (api) {
            numberProfileGridAPI = api;
        };

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
                       response.UpdatedObject.ValidTill = $scope.endDate;
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

    }


    function BuildSubscriberCaseObjfromScope()
    {
        var subscriberCaseObject = {
            SubscriberNumber: $scope.subscriberNumber,
            StatusId: $scope.selectedStatus.id,
            ValidTill: $scope.endDate
        };
        return subscriberCaseObject;
    }


    $scope.selectedRelatedNumbersChanged  = function () {

        $scope.subscriberNumber = $scope.selectedRelatedNumber

        normalCDRGridAPI.clearDataAndContinuePaging();
        numberProfileGridAPI.clearDataAndContinuePaging();
        subscriberThresholdsGridAPI.clearDataAndContinuePaging();

        getSubscriberThresholds();
        getNormalCDRs();
        getNumberProfiles();
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
    }

   


    SuspiciousNumberDetailsController.viewVisibilityChanged = function () {

        isNormalCDRTabShown = !isNormalCDRTabShown;
        isNumberProfileTabShown = !isNumberProfileTabShown;
        isSubscriberThresholdsTabShown = !isSubscriberThresholdsTabShown;
    };







}
appControllers.controller('SuspiciousNumberDetailsController', SuspiciousNumberDetailsController);
