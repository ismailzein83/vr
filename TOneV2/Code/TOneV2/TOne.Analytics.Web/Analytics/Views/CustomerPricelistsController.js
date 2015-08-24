CustomerPricelistsController.$inject = ['$scope', 'RawCDRLogAPIService', 'UtilsService', '$q', 'BusinessEntityAPIService_temp', 'RawCDRLogMeasureEnum', 'VRModalService', 'VRNotificationService'];

function CustomerPricelistsController($scope, RawCDRLogAPIService, UtilsService, $q, BusinessEntityAPIService, RawCDRLogMeasureEnum, VRModalService, VRNotificationService) {
    var mainGridAPI;
    var measures = [];
    var CDROption = [];
    var isFilterScreenReady;
    defineScope();
    load();
    function defineScope() {
      
        $scope.data = [];
        $scope.measures = measures;
        $scope.onInfoClick = function () {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "CDR Table Definition ";
            };
            VRModalService.showModal('/Client/Modules/Analytics/Views/RawCDRLogTemplate/RawCDRLogTemplate.html', null, settings);

        }
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return RawCDRLogAPIService.GetRawCDRData(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
            }).catch(function (error) {
                console.log(error.ExceptionMessage);
                // VRNotificationService.notifyException("Sintex Error", "dsad");

            });
        };

        $scope.getData = function () {
            return retrieveData();
        };


    }

    function retrieveData() {
        var filter = buildFilter();
        var query = {
            Switches: filter.SwitchIds,
        }
        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadMeasures();
        loadSwitches();
    }
    function buildFilter() {
        var filter = {};
        filter.SwitchIds = UtilsService.getPropValuesFromArray($scope.selectedSwitches, "SwitchId");
        return filter;
    }

    function loadSwitches() {
        return BusinessEntityAPIService.GetSwitches().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.switches.push(itm);
            });
        });
    }


    function loadMeasures() {
        for (var prop in RawCDRLogMeasureEnum) {
            measures.push(RawCDRLogMeasureEnum[prop]);
        }
    }


};
appControllers.controller('Analytics_CustomerPricelistsController', CustomerPricelistsController);