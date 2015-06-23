function BPTrackingModalController($scope, VRNotificationService, VRNavigationService, BusinessProcessAPIService, BPInstanceStatusEnum, $interval) {

    "use strict";

    var mainGridAPI, ctrl = this, isRunning = false, lockGetData = false;
    ctrl.lastTrackingId = 0;
    ctrl.message = '';
    ctrl.trackingSeverity = [];
    ctrl.selectedTrackingSeverity = [];
    ctrl.close =  function () {
        $scope.modalContext.closeModal();
    };

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        ctrl.BPInstanceID = undefined;
        if (parameters !== undefined && parameters !== null) {
            ctrl.BPInstanceID = parameters.BPInstanceID;
        }
    }

    function getFilterIds(values, idProp) {
        var filterIds;
        if (values.length > 0) {
            filterIds = [];
            angular.forEach(values, function (val) {
                filterIds.push(val[idProp]);
            });
        }
        return filterIds;
    }

    function getMaxValue(values, idProp) {
        var max = undefined;
        if (values.length > 0) {

            angular.forEach(values, function (val) {
                if(val === undefined)
                    max = val[idProp];
                if (val[idProp] > max)
                    max = val[idProp];
            });
        }
        return max;
    }

    function getData() {

        if (! lockGetData)
        {
            lockGetData = true;

            var pageInfo = {
                fromRow: 1,
                toRow: 10
            };

            if (mainGridAPI !== undefined) {
                pageInfo = mainGridAPI.getPageInfo();
            }

            return BusinessProcessAPIService.GetTrackingsByInstanceId(
                ctrl.BPInstanceID,
                pageInfo.fromRow,
                pageInfo.toRow,
                getFilterIds(ctrl.selectedTrackingSeverity, "Value"),
                ctrl.message,
                ctrl.lastTrackingId)
            .then(function (response) {

                ctrl.lastTrackingId = getMaxValue(response.Tracking, "TrackingId");
                mainGridAPI.addItemsToSource(response.Tracking);
                isRunning = (response.InstanceStatus === BPInstanceStatusEnum.Running.value);
                
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                lockGetData = false;
            });
        }
    }

    ctrl.searchClicked = function () {
        mainGridAPI.clearDataAndContinuePaging();
        return getData();
    };

    function loadFilters() {

        BusinessProcessAPIService.GetTrackingSeverity().then(function (response) {
            angular.forEach(response, function (item) {
                ctrl.trackingSeverity.push(item);
            });
        });
    }

    function load() {
        ctrl.isGettingData = true;
        getData().finally(function () {
            ctrl.isGettingData = false;
            lockGetData = false;
        });
    }

    function defineGrid() {
        ctrl.datasource = [];
        ctrl.gridMenuActions = [];
        ctrl.loadMoreData = function () {
            return getData();
        };

        ctrl.onGridReady = function (api) {
            mainGridAPI = api;
        };
    }

    loadParameters();
    load();
    defineGrid();
    loadFilters();

    $interval(function callAtInterval() {
        getData();
    }, 5000);

}

BPTrackingModalController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'BusinessProcessAPIService', 'BPInstanceStatusEnum', '$interval'];

appControllers.controller('BusinessProcess_BPTrackingModalController', BPTrackingModalController);
