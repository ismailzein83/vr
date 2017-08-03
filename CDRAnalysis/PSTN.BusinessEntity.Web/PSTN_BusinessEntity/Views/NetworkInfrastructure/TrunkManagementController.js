(function (appControllers) {

    "use strict";
    TrunkManagementController.$inject = ["$scope", "CDRAnalysis_PSTN_TrunkService", "UtilsService", "VRNotificationService", "VRUIUtilsService", "CDRAnalysis_PSTN_TrunkAPIService"];

    function TrunkManagementController($scope, CDRAnalysis_PSTN_TrunkService, UtilsService, VRNotificationService, VRUIUtilsService, CDRAnalysis_PSTN_TrunkAPIService) {

        var trunkGridAPI;
        var switchDirectiveApi;
        var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var typeDirectiveApi;
        var typeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var directionDirectiveApi;
        var directionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var linkDirectiveApi;
        var linkReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.hasAddTrunkPermission = function () {
                return CDRAnalysis_PSTN_TrunkAPIService.HasAddTrunkPermission();
            };

            $scope.searchClicked = function () {
                var query = getFilterObj();
                trunkGridAPI.loadGrid(query);
            };

            $scope.addTrunk = function () {

                var onTrunkAdded = function (trunkObj) {
                    trunkGridAPI.onTrunkAdded(trunkObj);
                };
                CDRAnalysis_PSTN_TrunkService.addTrunk(onTrunkAdded);
            };

            // directive functions
            $scope.onTrunkGridReady = function (api) {
                trunkGridAPI = api;
                trunkGridAPI.loadGrid({});
            };

            $scope.onReadySwicth = function (api) {
                switchDirectiveApi = api;
                switchReadyPromiseDeferred.resolve();
            };
            $scope.onReadyTrunkType = function (api) {
                typeDirectiveApi = api;
                typeReadyPromiseDeferred.resolve();
            };
            $scope.onReadyTrunkDirection = function (api) {
                directionDirectiveApi = api;
                directionReadyPromiseDeferred.resolve();
            };

            $scope.onReadyTrunkLink = function (api) {
                linkDirectiveApi = api;
                linkReadyPromiseDeferred.resolve();
            };

        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSwitchSelector, loadTypeSelector, loadDirectionSelector, loadLinkSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadSwitchSelector() {
            var switchLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            switchReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {};

                    VRUIUtilsService.callDirectiveLoad(switchDirectiveApi, directivePayload, switchLoadPromiseDeferred);
                });
            return switchLoadPromiseDeferred.promise;
        }
        function loadTypeSelector() {
            var typeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            typeReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {};
                VRUIUtilsService.callDirectiveLoad(typeDirectiveApi, directivePayload, typeLoadPromiseDeferred);
            });
            return typeLoadPromiseDeferred.promise;
        }
        function loadDirectionSelector() {
            var directionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            directionReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {};
                VRUIUtilsService.callDirectiveLoad(directionDirectiveApi, directivePayload, directionLoadPromiseDeferred);
            });
            return directionLoadPromiseDeferred.promise;
        }
        function loadLinkSelector() {

            var linkLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            linkReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {};
                VRUIUtilsService.callDirectiveLoad(linkDirectiveApi, directivePayload, linkLoadPromiseDeferred);
            });
            return linkLoadPromiseDeferred.promise;

        }

        function getFilterObj() {
            return {
                Name: $scope.name,
                Symbol: $scope.symbol,
                SelectedSwitchIds: switchDirectiveApi.getSelectedIds(),
                SelectedTypes: typeDirectiveApi.getSelectedIds(),
                SelectedDirections: directionDirectiveApi.getSelectedIds(),
                IsLinkedToTrunk: ($scope.selectedLinkedToTrunkObjs.length == 1) ? $scope.selectedLinkedToTrunkObjs[0].value : null
            };
        }
    }

    appControllers.controller("PSTN_BusinessEntity_TrunkManagementController", TrunkManagementController);
})(appControllers);