SuspicionAnalysisController.$inject = ['$scope', 'StrategyAPIService', 'SuspicionAnalysisAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService','UtilsService'];

function SuspicionAnalysisController($scope, StrategyAPIService, SuspicionAnalysisAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();




    function defineScope() {

        $scope.statuses = [{ id: 2, name: 'Pending' }, { id: 3, name: 'Closed: Fraud' }, { id: 4, name: 'Closed: White List' }, { id: 5, name: 'Cancelled' }, { id: 1, name: 'Opened' }];
        $scope.selectedStatus=[];
        $scope.selectedStatus.push(UtilsService.getItemByVal($scope.statuses, 1, "id"));

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;


        $scope.suspicionLevels = [
                        { id: 2, name: 'Suspicious' }, { id: 3, name: 'Highly Suspicious' }, { id: 4, name: 'Fraud' }

        ];

        $scope.strategies = [];

        loadStrategies();

        $scope.selectedSuspicionLevels = [];
        $scope.selectedStrategies = [];

        $scope.gridMenuActions = [];

        $scope.fraudResults = [];


        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            return retrieveData();
        };
        $scope.searchClicked = function () {
            return retrieveData();
        }


        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SuspicionAnalysisAPIService.GetFilteredSuspiciousNumbers(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }



        defineMenuActions();
    }


    function removeLastComma(strng) {
        var n = strng.lastIndexOf(",");
        var a = strng.substring(0, n)
        return a;
    }


    function retrieveData() {

        var suspicionLevelsList = '';

        angular.forEach($scope.selectedSuspicionLevels, function (itm) {
            suspicionLevelsList = suspicionLevelsList + itm.id + ','
        });


        var strategiesList = '';

        angular.forEach($scope.selectedStrategies, function (itm) {
            strategiesList = strategiesList + itm.id + ','
        });


        var caseStatusesList = '';

        angular.forEach($scope.selectedStatus, function (itm) {
            caseStatusesList = caseStatusesList + itm.id + ','
        });



        var query = {
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            SuspicionLevelsList: removeLastComma(suspicionLevelsList),
            StrategiesList: removeLastComma(strategiesList),
            CaseStatusesList: removeLastComma(caseStatusesList)
        };

      
        return mainGridAPI.retrieveData(query);
    }

    function load() {

    }


    function loadStrategies() {
        return StrategyAPIService.GetAllStrategies().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name });
            });
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Details",
            clicked: detailFraudResult
        }];
    }



    function detailFraudResult(fruadResult) {
        
        var suspicionLevelsList = '';

        angular.forEach($scope.selectedSuspicionLevels, function (itm) {
            suspicionLevelsList = suspicionLevelsList + itm.id + ','
        });


        var strategiesList = '';

        angular.forEach($scope.selectedStrategies, function (itm) {
            strategiesList = strategiesList + itm.id + ','
        });

        var params = {
            subscriberNumber: fruadResult.SubscriberNumber,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate,
            strategiesList: strategiesList,
            suspicionLevelsList: suspicionLevelsList
        };

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details & Related Numbers";
            modalScope.onSubscriberCaseUpdated = function (subscriberCase) {

                mainGridAPI.itemUpdated(subscriberCase);
            }
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousAnalysis/SuspiciousNumberDetails.html", params, settings);
        
    }


}
appControllers.controller('FraudAnalysis_SuspicionAnalysisController', SuspicionAnalysisController);
