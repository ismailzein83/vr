﻿BPTrackingModalController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'BusinessProcessAPIService', 'BPInstanceStatusEnum', '$interval'];

function BPTrackingModalController($scope, UtilsService, VRNotificationService, VRNavigationService, BusinessProcessAPIService, BPInstanceStatusEnum, $interval) {

    "use strict";

    var mainGridAPI, interval, lockGetData = false;

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPInstanceID = undefined;
        if (parameters !== undefined && parameters !== null) {
            $scope.BPInstanceID = parameters.BPInstanceID;
        }
    }

    function getData() {

        if (!lockGetData)
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
                $scope.BPInstanceID,
                pageInfo.fromRow,
                pageInfo.toRow,
                UtilsService.getPropValuesFromArray($scope.selectedTrackingSeverity, "Value"),
                $scope.message,
                $scope.lastTrackingId)
            .then(function (response) {

                $scope.lastTrackingId = UtilsService.getPropMaxValueFromArray(response.Tracking, "TrackingId");
                mainGridAPI.addItemsToSource(response.Tracking);
                
                if (response.InstanceStatus === BPInstanceStatusEnum.Running.value) startGetData();

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                lockGetData = false;
            });
        }
    }

    function loadFilters() {

        BusinessProcessAPIService.GetTrackingSeverity().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.trackingSeverity.push(item);
            });
        });
    }

    function load() {
        $scope.isGettingData = true;
        getData().finally(function () {
            $scope.isGettingData = false;
            lockGetData = false;
        });
    }

    function defineGrid() {
        $scope.datasource = [];
        $scope.gridMenuActions = [];
        $scope.loadMoreData = function () {
            return getData();
        };

        $scope.onGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.filterGrid = function (item) {
            var isMatch = false;
            var severity = UtilsService.getPropValuesFromArray($scope.selectedTrackingSeverity, "Value");

            if (isNullOrEmpty($scope.message) && isNullOrEmpty(severity)) return true;

            if (item['Message'].includes($scope.message))
                isMatch = true;

            if (isNullOrEmpty(severity)) return isMatch;

            var severityMatch = false;
            for (var i = 0, len = severity.length; i < len; i++) {
                if (String(item['Severity']).includes(severity[i]))
                    severityMatch = true;
            }

            if (severityMatch && isMatch) return true;
            return false;
        };

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

    }

    function isNullOrEmpty(value) {
        if (value) return false;
        return true;
    }

    function startGetData() {
        if (angular.isDefined(interval)) return;
        interval =  $interval(function callAtInterval() {
            getData();
        }, 5000);
    }

    function stopGetData() {
        if (angular.isDefined(interval)) {
            $interval.cancel(interval);
            interval = undefined;
        }
    }

    defineScope();
    loadParameters();
    load();
    defineGrid();
    loadFilters();
}
appControllers.controller('BusinessProcess_BPTrackingModalController', BPTrackingModalController);
