BPTrackingModalController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'BusinessProcessAPIService', 'BPInstanceStatusEnum', '$interval', 'LabelColorsEnum', 'BPTrackingSeverityEnum'];

function BPTrackingModalController($scope, UtilsService, VRNotificationService, VRNavigationService, BusinessProcessAPIService, BPInstanceStatusEnum, $interval, LabelColorsEnum, BPTrackingSeverityEnum) {

    "use strict";

    var mainGridApi, interval, nonClosedStatuses, lockGetData = false;

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPInstanceID = undefined;
        if (parameters !== undefined && parameters !== null) {
            $scope.BPInstanceID = parameters.BPInstanceID;
        }
    }

    function startGetData() {
        if (angular.isDefined(interval)) return;
        interval =  $interval(function callAtInterval() {
            getData();
        }, 5000);
    }

    function getData() {

        if (!lockGetData)
        {
            lockGetData = true;

            var pageInfo = mainGridApi.getPageInfo();

            return BusinessProcessAPIService.GetTrackingsByInstanceId(
                $scope.BPInstanceID,
                pageInfo.fromRow,
                pageInfo.toRow,
                $scope.lastTrackingId)
            .then(function (response) {

                $scope.lastTrackingId = UtilsService.getPropMaxValueFromArray(response.Tracking, "TrackingId");
                mainGridApi.addItemsToBegin(response.Tracking);
                var isNonClosed = false;

                for (var i = 0, len = nonClosedStatuses.length; i < len; i++) {

                    if (response.InstanceStatus === nonClosedStatuses[i]) {
                        isNonClosed = true;
                        break;
                    }
                }
                if (isNonClosed) startGetData();
                
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                lockGetData = false;
            });
        }
        return undefined;
    }

    function loadSummary() {

        BusinessProcessAPIService.GetBPInstance($scope.BPInstanceID).then(function (response) {

            $scope.process = {
                Title: response.Title,
                Date: response.CreatedTime,
                Status: response.StatusDescription
            };
        });
    }

    function loadFilters() {

        BusinessProcessAPIService.GetTrackingSeverity().then(function (response) {
            for (var i = 0 ; i < response.length ; i++) {
                $scope.trackingSeverity.push(response[i]);
            }
        });
    }

    function loadNonClosedStatuses() {
        nonClosedStatuses = [];
        BusinessProcessAPIService.GetNonClosedStatuses().then(function (response) {
            for (var i = 0 ; i < response.length ; i++) {
                nonClosedStatuses.push(response[i]);
            }
        });
    }

    function stopGetData() {
        if (angular.isDefined(interval)) {
            $interval.cancel(interval);
            interval = undefined;
        }
    }

    function defineScope() {
        $scope.lastTrackingId = 0;
        $scope.message = '';
        $scope.trackingSeverity = [];
        $scope.selectedTrackingSeverity = [];
        $scope.close = function () {
            stopGetData();
            $scope.modalContext.closeModal();
        };

        $scope.$on('$destroy', function () {
            stopGetData();
        });

        $scope.getSeverityColor = function (dataItem, colDef) {

            if (dataItem.Severity === BPTrackingSeverityEnum.Information.value) return LabelColorsEnum.Info.Color;
            if (dataItem.Severity === BPTrackingSeverityEnum.Warning.value) return LabelColorsEnum.Warning.Color;
            if (dataItem.Severity === BPTrackingSeverityEnum.Error.value) return LabelColorsEnum.Error.Color;
            if (dataItem.Severity === BPTrackingSeverityEnum.Verbose.value) return LabelColorsEnum.Primary.Color;

            return LabelColorsEnum.Info.Color;
        };

    }

    function defineGrid() {
        $scope.datasource = [];
        $scope.gridMenuActions = [];
        $scope.loadMoreData = function () {
            return getData();
        };

        $scope.onGridReady = function (api) {
            mainGridApi = api;
            $scope.isGettingData = true;
            getData().finally(function () {
                $scope.isGettingData = false;
                lockGetData = false;
            });
        };

        $scope.filterGrid = function (item) {
            var isMatch = false;
            var severity = UtilsService.getPropValuesFromArray($scope.selectedTrackingSeverity, "Value");

            if (isNullOrEmpty($scope.message) && isNullOrEmpty(severity)) return true;

            if (item['Message'].toUpperCase().indexOf($scope.message.toUpperCase()) > -1)
                isMatch = true;

            if (isNullOrEmpty(severity)) return isMatch;

            var severityMatch = false;
            for (var i = 0, len = severity.length; i < len; i++) {
                if (String(item['Severity']).toUpperCase().indexOf(severity[i].toUpperCase()) > -1) {
                    severityMatch = true;
                    break;
                }
            }

            if (severityMatch && isMatch) return true;
            return false;
        };

    }

    function load() {
        defineScope();
        loadParameters();
        defineGrid();
        loadFilters();
        loadSummary();
        loadNonClosedStatuses();
    }

    function isNullOrEmpty(value) {
        if (value) return false;
        return true;
    }

    load();
    
}
appControllers.controller("BusinessProcess_BPTrackingModalController", BPTrackingModalController);
