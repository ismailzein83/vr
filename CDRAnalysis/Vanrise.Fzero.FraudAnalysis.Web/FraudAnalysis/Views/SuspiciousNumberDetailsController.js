SuspiciousNumberDetailsController.$inject = ['$scope', 'StrategyAPIService', 'SuspicionAnalysisAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function SuspiciousNumberDetailsController($scope, StrategyAPIService, SuspicionAnalysisAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var normalCDRGridAPI;
    var numberProfileGridAPI;

    loadParameters();
    defineScope();
    load();


    function loadParameters() {

        var parameters = VRNavigationService.getParameters($scope);

        $scope.subscriberNumber = undefined;

        if (parameters != undefined && parameters != null)
            $scope.subscriberNumber = parameters.subscriberNumber;
        $scope.suspicionLevelName = parameters.suspicionLevelName;
        $scope.fromDate = parameters.fromDate;
        $scope.toDate = parameters.toDate;
        
    }

    function getNormalCDRs() {
        $scope.normalCDRs.length = 0;
        
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';


        var pageInfo = normalCDRGridAPI.getPageInfo();


        return SuspicionAnalysisAPIService.GetNormalCDRs(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.normalCDRs.push(itm);
                console.log(itm)
            });
        });
    }


    function getNumberProfiles() {
        $scope.numberProfiles.length = 0;
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';


        var pageInfo = numberProfileGridAPI.getPageInfo();


        return SuspicionAnalysisAPIService.GetNumberProfiles(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.numberProfiles.push(itm);
            });
        });
    }




    function defineScope() {

        $scope.normalCDRs = [];
        $scope.numberProfiles = [];
        $scope.relatedNumbers = [];


        SuspiciousNumberDetailsController.isNormalCDRTabShown = true;
        SuspiciousNumberDetailsController.isNumberProfileTabShown = false;

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
    }


    $scope.selectedRelatedNumbersChanged  = function () {

        $scope.subscriberNumber = $scope.selectedRelatedNumber

        normalCDRGridAPI.clearDataAndContinuePaging();
        numberProfileGridAPI.clearDataAndContinuePaging();

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

    function getStrategy() {

        return StrategyAPIService.GetStrategy($scope.strategyId)
           .then(function (response) {
               fillScopeFromStrategyObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }






    SuspiciousNumberDetailsController.viewVisibilityChanged = function () {

        isNormalCDRTabShown = !isNormalCDRTabShown;
        isNumberProfileTabShown = !isNumberProfileTabShown;
    };







}
appControllers.controller('SuspiciousNumberDetailsController', SuspiciousNumberDetailsController);
