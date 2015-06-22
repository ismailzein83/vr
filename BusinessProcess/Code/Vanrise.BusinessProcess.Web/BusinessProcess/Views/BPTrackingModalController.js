
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
        return BusinessProcessAPIService.GetTrackingsByInstanceId(ctrl.BPInstanceID).then(function (response) {

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
}

BPTrackingModalController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'BusinessProcessAPIService'];

appControllers.controller('BusinessProcess_BPTrackingModalController', BPTrackingModalController);
