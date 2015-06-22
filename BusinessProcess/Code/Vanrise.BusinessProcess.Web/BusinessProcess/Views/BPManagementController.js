
function BPManagementController(BusinessProcessAPIService, VRModalService) {

    "use strict";

    var mainGridAPI, ctrl = this;
    ctrl.definitions = [];
    ctrl.selectedDefinition = [];
    ctrl.instanceStatus = [];
    ctrl.selectedInstanceStatus = [];

    function showBPTrackingModal(BPInstanceObj) {

        VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
            BPInstanceID: BPInstanceObj.ProcessInstanceID
        }, {
            onScopeReady: function (modalScope) {
                modalScope.title = "Tracking";
            }
        });
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

        var pageInfo = mainGridAPI.getPageInfo();

        return BusinessProcessAPIService.GetFilteredBProcess({
            DefinitionsId: getFilterIds(ctrl.selectedDefinition, "BPDefinitionID"),
            InstanceStatus: getFilterIds(ctrl.selectedInstanceStatus, "Value"),
            FromRow: pageInfo.fromRow,
            ToRow: pageInfo.toRow,
            DateFrom: ctrl.fromDate,
            DateTo: ctrl.toDate
        }).then(function (response) {
            mainGridAPI.addItemsToSource(response);
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
        ctrl.gridMenuActions = [{
            name: "Tracking",
            clicked: showBPTrackingModal
        }];
    }

    ctrl.searchClicked = function () {
        mainGridAPI.clearDataAndContinuePaging();
        return getData();
    };

    function loadFilters() {

        BusinessProcessAPIService.GetDefinitions().then(function (response) {
            angular.forEach(response, function (item) {
                ctrl.definitions.push(item);
            });
        });

        BusinessProcessAPIService.GetStatusList().then(function (response) {
            angular.forEach(response, function (item) {
                ctrl.instanceStatus.push(item);
            });
        });
    }

    function load() {
        loadFilters();
    }

    load();
    defineGrid();
}

BPManagementController.$inject = ['BusinessProcessAPIService', 'VRModalService'];
appControllers.controller('BusinessProcess_BPManagementController', BPManagementController);