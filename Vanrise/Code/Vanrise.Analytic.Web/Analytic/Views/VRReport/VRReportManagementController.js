//(function (appControllers) {

//    "use strict";

//    DataAnalysisDefinitionManagementController.$inject = ['$scope', 'VR_Analytic_DataAnalysisDefinitionService', 'VR_Analytic_DataAnalysisDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

//    function DataAnalysisDefinitionManagementController($scope, VR_Analytic_DataAnalysisDefinitionService, VR_Analytic_DataAnalysisDefinitionAPIService, UtilsService, VRUIUtilsService) {

//        var gridAPI;

//        defineScope();
//        load();


//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.search = function () {
//                var query = buildGridQuery();
//                return gridAPI.load(query);
//            };
//            $scope.scopeModel.add = function () {
//                var onDataAnalysisDefinitionAdded = function (addedDataAnalysisDefinition) {
//                    gridAPI.onDataAnalysisDefinitionAdded(addedDataAnalysisDefinition);
//                };

//                VR_Analytic_DataAnalysisDefinitionService.addDataAnalysisDefinition(onDataAnalysisDefinitionAdded);
//            };

//            $scope.hasAddDataAnalysisDefinitionPermission = function () {
//                return VR_Analytic_DataAnalysisDefinitionAPIService.HasAddDataAnalysisDefinitionPermission()
//            };

//            $scope.scopeModel.onGridReady = function (api) {
//                gridAPI = api;
//                gridAPI.load({});
//            };
//        }
//        function load() {

//        }

//        function buildGridQuery() {
//            return {
//                Name: $scope.scopeModel.name,
//            };
//        }
//    }

//    appControllers.controller('VR_Analytic_DataAnalysisDefinitionManagementController', DataAnalysisDefinitionManagementController);

//})(appControllers);