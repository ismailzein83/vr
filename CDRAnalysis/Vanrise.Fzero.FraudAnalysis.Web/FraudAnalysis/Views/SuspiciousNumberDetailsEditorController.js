StrategyEditorController.$inject = ['$scope', 'StrategyAPIService', 'FraudResultAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function StrategyEditorController($scope, StrategyAPIService,FraudResultAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    var normalCDRGridAPI;

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
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';


        var pageInfo = normalCDRGridAPI.getPageInfo();
       

        return FraudResultAPIService.GetNormalCDRs(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.normalCDRs.push(itm);
            });
        });
    }


    function getNumberProfiles() {
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';


        var pageInfo = normalCDRGridAPI.getPageInfo();


        return FraudResultAPIService.GetNumberProfiles(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, $scope.subscriberNumber).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.numberProfiles.push(itm);
            });
        });
    }




    function defineScope() {
       
        $scope.normalCDRs = [];
        $scope.numberProfiles = [];
        $scope.relatedNumbers = [];

        StrategyEditorController.isNormalCDRTabShown = true;
        StrategyEditorController.isNumberProfileTabShown = false;

        $scope.onNormalCDRsGridReady = function (api) {
            normalCDRGridAPI = api;
            getNormalCDRs();
        };

        $scope.loadMoreDataNormalCDRs = function () {
            return getNormalCDRs();
        }


        $scope.onNumberProfilesGridReady = function (api) {
            normalCDRGridAPI = api;
            getNumberProfiles();
        };

        $scope.loadMoreDataNumberProfiles = function () {
            return getNumberProfiles();
        }
      

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {

        $scope.isGettingData = true;
        UtilsService.waitMultipleAsyncOperations([loadRelatedNumbers])
        .then(function () {
            $scope.isGettingData = false;
        })
        .catch(function (error) {
            $scope.isGettingData = false;
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
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



    function loadRelatedNumbers() {
        return StrategyAPIService.GetFilters().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.relatedNumbers.push({ filterId: itm.FilterId, description: itm.Description });
            });
        });
    }



    StrategyEditorController.viewVisibilityChanged = function () {
      
        isNormalCDRTabShown = !isNormalCDRTabShown;
        isNumberProfileTabShown = !isNumberProfileTabShown;
    };







}
appControllers.controller('StrategyEditorController', StrategyEditorController);
