BPManagementController.$inject = ['BusinessProcessAPIService'];

function BPManagementController(BusinessProcessAPIService) {

    var mainGridAPI;
    var ctrl = this;
    ctrl.definitions = [];
    ctrl.selectedDefinition = [];
    ctrl.instanceStatus = [];
    ctrl.selectedInstanceStatus = [];
    ctrl.BPInstances = [];

    ctrl.searchClicked = function () {
        mainGridAPI.clearDataAndContinuePaging();
        return getData();
    }

    ctrl.onGridReady = function (api) {
        mainGridAPI = api;
    }

    function getData() {
        
        var pageInfo = mainGridAPI.getPageInfo();
        var param = {
            DefinitionsId : getFilterIds(ctrl.selectedDefinition, "BPDefinitionID"),
            InstanceStatus: getFilterIds(ctrl.selectedInstanceStatus, "Value"),
            FromRow : pageInfo.fromRow,
            ToRow : pageInfo.toRow,
            DateFrom : ctrl.fromDate,
            DateTo : ctrl.toDate
        };
        
        return BusinessProcessAPIService.GetFilteredBProcess(param).then(function (response) {
             mainGridAPI.addItemsToSource(response);
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

    function load() {
        loadFilters();
    }

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

    load();
}
appControllers.controller('BusinessProcess_BPManagementController', BPManagementController);