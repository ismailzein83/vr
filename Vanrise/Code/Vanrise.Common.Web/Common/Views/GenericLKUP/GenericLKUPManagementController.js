(function (appControllers) {

    "use strict";

    GenericLKUPManagementController.$inject = ['$scope', 'VR_Common_GenericLKUPService', 'VR_Common_GnericLKUPAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function GenericLKUPManagementController($scope, VR_Common_GenericLKUPService, VR_Common_GnericLKUPAPIService, VRNotificationService, UtilsService, VRUIUtilsService) {

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
                var onGenericLKUPAdded = function (addedGenericLKUP) {

                    gridAPI.onGenericLKUPAdded(addedGenericLKUP);
                };
                VR_Common_GenericLKUPService.addGenericLKUP(onGenericLKUPAdded);
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
                                $type: "Vanrise.Common.Business.GenericLKUPBEDefinitionFilter, Vanrise.Common.Business"
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
                BusinessEntityDefinitionIds: beDefinitionSelectorApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_Common_GenericLKUPManagementController', GenericLKUPManagementController);
})(appControllers);