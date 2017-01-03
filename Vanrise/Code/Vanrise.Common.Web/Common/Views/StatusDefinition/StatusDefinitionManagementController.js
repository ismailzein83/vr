(function (appControllers) {

    "use strict";

    StatusDefinitionManagementController.$inject = ['$scope', 'VR_Common_StatusDefinitionService', 'VR_Common_StatusDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function StatusDefinitionManagementController($scope, VR_Common_StatusDefinitionService, VR_Common_StatusDefinitionAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        var beDefinitionSelectorApi;
        var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.onBusinessEntityDefinitionSelectorReady = function (api) {
                beDefinitionSelectorApi = api;
                beDefinitionSelectorPromiseDeferred.resolve();
            };
            $scope.add = function () {
                var onStatusDefinitionAdded = function (addedStatusDefinition) {
                    gridAPI.onStatusDefinitionAdded(addedStatusDefinition);
                };
                VR_Common_StatusDefinitionService.addStatusDefinition(onStatusDefinitionAdded);
            };
            $scope.hasAddStatusDefinitionPermission = function () {
                return VR_Common_StatusDefinitionAPIService.HasAddStatusDefinitionPermission();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {
            $scope.isloading = true;
            loadAllControls().finally(function () {
                $scope.isloading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isloading = false;
            })
        }

        function loadAllControls() {
            function loadBusinessEntityDefinitionSelector() {
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                beDefinitionSelectorPromiseDeferred.promise.then(function () {
                    var payloadSelector = {
                        filter: {
                            Filters: [{
                                $type: "Vanrise.Common.Business.StatusDefinitionBEFilter, Vanrise.Common.Business"
                            }]
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                });
                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadBusinessEntityDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
                BusinessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
            };
        }
    }

    appControllers.controller('VR_Common_StatusDefinitionManagementController', StatusDefinitionManagementController);
})(appControllers);