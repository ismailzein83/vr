(function (appControllers) {

    "use strict";

    StyleDefinitionManagementController.$inject = ['$scope', 'VRCommon_StyleDefinitionService', 'UtilsService', 'VRUIUtilsService'];

    function StyleDefinitionManagementController($scope, VRCommon_StyleDefinitionService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onStyleDefinitionAdded = function (addedStyleDefinition) {
                    gridAPI.onStyleDefinitionAdded(addedStyleDefinition);
                }
                VRCommon_StyleDefinitionService.addStyleDefinition(onStyleDefinitionAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {
            $scope.scopeModel.isloading = true;
            //loadAllControls().finally(function () {
            //    $scope.scopeModel.isloading = false;
            //}).catch(function (error) {
            //    VRNotificationService.notifyExceptionWithClose(error, $scope);
            //    $scope.scopeModel.isloading = false;
            //})
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadEntityTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        //function loadEntityTypeSelector() {
        //    var styleDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
        //    entityTypeAPISelectorReadyDeferred.promise.then(function () {
        //        VRUIUtilsService.callDirectiveLoad(entityTypeAPI, undefined, styleDefinitionSelectorLoadDeferred);
        //    });
        //    return styleDefinitionSelectorLoadDeferred.promise;
        //}

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
            };
        }
    }

    appControllers.controller('VRCommon_StyleDefinitionManagementController', StyleDefinitionManagementController);

})(appControllers);