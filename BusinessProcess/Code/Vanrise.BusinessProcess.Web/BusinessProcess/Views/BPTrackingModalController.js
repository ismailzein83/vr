function BPTrackingModalController($scope, VRNotificationService, VRNavigationService, BusinessProcessAPIService) {

    "use strict";

    var mainGridAPI, ctrl = this;

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

    function getData() {
        var pageInfo = {
            fromRow: 0,
            toRow: 10
        };

        if (mainGridAPI !== undefined) {
            pageInfo = mainGridAPI.getPageInfo();
        }

        return BusinessProcessAPIService.GetTrackingsByInstanceId(ctrl.BPInstanceID, pageInfo.fromRow, pageInfo.toRow).then(function (response) {

            mainGridAPI.addItemsToSource(response);

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
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
        ctrl.mainGridPagerSettings = {
            currentPage: 1,
            totalDataCount: 0,
            pageChanged: function () {
                return getData();
            }
        };

        ctrl.onGridReady = function (api) {
            mainGridAPI = api;
        };
    }

    loadParameters();
    load();
    defineGrid();
}

BPTrackingModalController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'BusinessProcessAPIService'];

appControllers.controller('BusinessProcess_BPTrackingModalController', BPTrackingModalController);
