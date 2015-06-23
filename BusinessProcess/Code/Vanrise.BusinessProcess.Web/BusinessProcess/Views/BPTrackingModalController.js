function BPTrackingModalController($scope, VRNotificationService, VRNavigationService, BusinessProcessAPIService) {

    "use strict";

    var mainGridAPI, ctrl = this;

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

    function getData() {
        var pageInfo = {
            fromRow: 0,
            toRow: 10
        };

        if (mainGridAPI !== undefined) {
            pageInfo = mainGridAPI.getPageInfo();
        }

        return BusinessProcessAPIService.GetTrackingsByInstanceId(ctrl.BPInstanceID,pageInfo.fromRow,
            pageInfo.toRow, getFilterIds(ctrl.selectedTrackingSeverity, "Value"), ctrl.message
        ).then(function (response) {

            mainGridAPI.addItemsToSource(response);

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });
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
}

BPTrackingModalController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'BusinessProcessAPIService'];

appControllers.controller('BusinessProcess_BPTrackingModalController', BPTrackingModalController);
