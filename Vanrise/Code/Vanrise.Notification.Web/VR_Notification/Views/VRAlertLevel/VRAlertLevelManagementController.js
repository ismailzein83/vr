(function (appControllers) {

    "use strict";

    AlertLevelManagementController.$inject = ['$scope', 'VR_Notification_AlertLevelService', 'VR_Notification_AlertLevelAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function AlertLevelManagementController($scope, VR_Notification_AlertLevelService, VR_Notification_AlertLevelAPIService, VRNotificationService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        var beDefinitionSelectorApi;
       // var beSelectorApi;
        var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
       // var beSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
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
            //$scope.onAlertLevelSelectorReady = function (api) {
            //    beSelectorApi = api;
            //    beSelectorPromiseDeferred.resolve();
            //}
            $scope.add = function () {
                var onAlertLevelAdded = function (addedAlertLevel) {
                    
                    gridAPI.onAlertLevelAdded(addedAlertLevel);
                };
                VR_Notification_AlertLevelService.addAlertLevel(onAlertLevelAdded);
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
                                $type: "Vanrise.Notification.Business.VRAlertLevelBEDefinitionFilter, Vanrise.Notification.Business"
                            }]
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                });
                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }
            //function loadBusinessEntitySelector() {
            //    var businessEntitySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            //    beSelectorPromiseDeferred.promise.then(function () {
            //        var payloadSelector = {
            //            filter: {
            //                Filters: [{
            //                    $type: "Vanrise.Notification.Business.BEAlertLevelDefinitionFilter ,Vanrise.Notification.Business",
            //                    BusinessEntityDefinitionId: '43D17D21-8A2D-4E6B-BDF1-94764AE63D5F'
            //                }],
            //                BusinessEntityDefinitionId: '43D17D21-8A2D-4E6B-BDF1-94764AE63D5F'
            //            }
            //        };
            //        VRUIUtilsService.callDirectiveLoad(beSelectorApi, payloadSelector, businessEntitySelectorLoadDeferred);
            //    });
            //    return businessEntitySelectorLoadDeferred.promise;
            //}
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

    appControllers.controller('VR_Notification_AlertLevelManagementController', AlertLevelManagementController);
})(appControllers);