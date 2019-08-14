(function (appControllers) {

    "use strict";

    DataAnalysisDefinitionManagementController.$inject = ['$scope', 'VR_Analytic_DataAnalysisDefinitionService', 'VR_Analytic_DataAnalysisDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function DataAnalysisDefinitionManagementController($scope, VR_Analytic_DataAnalysisDefinitionService, VR_Analytic_DataAnalysisDefinitionAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;
 
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.scopeModel.add = function () {
                var onDataAnalysisDefinitionAdded = function (addedDataAnalysisDefinition) {
                    gridAPI.onDataAnalysisDefinitionAdded(addedDataAnalysisDefinition);
                };

                VR_Analytic_DataAnalysisDefinitionService.addDataAnalysisDefinition(onDataAnalysisDefinitionAdded);
            };

            $scope.hasAddDataAnalysisDefinitionPermission = function () {
                return VR_Analytic_DataAnalysisDefinitionAPIService.HasAddDataAnalysisDefinitionPermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }
             function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitPromiseNode({ promises: [loadDevProjectSelector()] })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                DevProjectIds: devProjectDirectiveApi != undefined ? devProjectDirectiveApi.getSelectedIds() : undefined
            };
        }
    }

    appControllers.controller('VR_Analytic_DataAnalysisDefinitionManagementController', DataAnalysisDefinitionManagementController);

})(appControllers);