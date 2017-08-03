(function (appControllers) {

    "use strict";
    SwitchManagementController.$inject = ["$scope", "CDRAnalysis_PSTN_SwitchService", "UtilsService", "VRNotificationService", "VRUIUtilsService", "CDRAnalysis_PSTN_SwitchAPIService"];

    function SwitchManagementController($scope, CDRAnalysis_PSTN_SwitchService, UtilsService, VRNotificationService, VRUIUtilsService, CDRAnalysis_PSTN_SwitchAPIService) {

    var filter = {};
    var switchAPI;
    var switchBrandReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    var switchBrandSelectorAPI;
    defineScope();
    load();

    function defineScope() {
        $scope.hasAddSwitchPermission = function () {
            return CDRAnalysis_PSTN_SwitchAPIService.HasAddSwitchPermission();
        };

        $scope.searchClicked = function () {
            setFilterObject();
            return switchAPI.retrieveData(filter);
        };

        $scope.addSwitch = function () {
            var onSwitchAdded = function (switchObj) {
                switchAPI.onSwitchAdded(switchObj);
            };
            CDRAnalysis_PSTN_SwitchService.addSwitch(onSwitchAdded);
        };
        $scope.onSwitchGridReady = function (api) {
            switchAPI = api;
            switchAPI.retrieveData({});
        };
        $scope.onSwicthBrandSelectorReady = function (api) {
            switchBrandSelectorAPI = api;
            switchBrandReadyPromiseDeferred.resolve();
        };

    }

    function setDataItemExtension(dataItem) {
        var extensionObj = {};

        extensionObj.onTrunkGridReady = function (api) {
            extensionObj.trunkGridAPI = api;
            var query = { SelectedSwitchIds: [dataItem.Entity.SwitchId] };
            extensionObj.trunkGridAPI.retrieveData(query);
            extensionObj.onTrunkGridReady = undefined;
        };

        dataItem.extensionObj = extensionObj;
    }

    function load() {
        $scope.isLoadingFilters = true;
        loadAllControls();
       
    }
    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadSwitchBrandSelector])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isLoadingFilters = false;
          });
    }
    function loadSwitchBrandSelector() {
        var switchBrandLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        switchBrandReadyPromiseDeferred.promise
            .then(function () {
                VRUIUtilsService.callDirectiveLoad(switchBrandSelectorAPI, undefined, switchBrandLoadPromiseDeferred);
            });
        return switchBrandLoadPromiseDeferred.promise;
    }
    function setFilterObject() {
        filter = {
            Name: $scope.name,
            SelectedBrandIds: switchBrandSelectorAPI.getSelectedIds(),
            AreaCode: $scope.areaCode,
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchManagementController", SwitchManagementController);
})(appControllers);