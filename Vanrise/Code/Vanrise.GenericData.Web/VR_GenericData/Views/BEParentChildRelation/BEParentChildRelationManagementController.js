(function (appControllers) {

    'use strict';

    BEParentChildRelationController.$inject = ['$scope', 'VR_GenericData_BEParentChildRelationService', 'VR_GenericData_BEParentChildRelationAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function BEParentChildRelationController($scope, VR_GenericData_BEParentChildRelationService, VR_GenericData_BEParentChildRelationAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var beParentChildRelationDefinitionId = "271a98fb-0704-4519-ae0d-01969b9ac0e0";

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.add = function () {
                var onBEParentChildRelationAdded = function (addedBEParentChildRelation) {
                    gridAPI.onBEParentChildRelationAdded(addedBEParentChildRelation);
                };

                VR_GenericData_BEParentChildRelationService.addBEParentChildRelation(beParentChildRelationDefinitionId, undefined, undefined, onBEParentChildRelationAdded);
            };
            $scope.scopeModel.search = function () {
                return loadGrid();
            };
            
            //$scope.scopeModel.hasAddPermission = function () {
            //    return VR_GenericData_BEParentChildRelationAPIService.HasAddBEParentChildRelationPermission();
            //};
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadGrid() {
            var gridLoadDeferred = UtilsService.createPromiseDeferred();

            gridReadyDeferred.promise.then(function () {
                gridAPI.load({}).then(function () {
                    gridLoadDeferred.resolve();
                }).catch(function (error) {
                    gridLoadDeferred.reject(error);
                });
            });

            //function getGridQuery() {
            //    return {
            //        Name: $scope.scopeModel.name,
            //        BusinessEntityDefinitionIds: beDefinitionSelectorAPI.getSelectedIds()
            //    };
            //}

            return gridLoadDeferred.promise;
        }
    }

    appControllers.controller('VR_GenericData_BEParentChildRelationController', BEParentChildRelationController);

})(appControllers);