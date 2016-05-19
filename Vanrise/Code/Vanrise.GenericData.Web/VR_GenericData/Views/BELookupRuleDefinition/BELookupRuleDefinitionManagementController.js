(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionController.$inject = ['$scope', 'VR_GenericData_BELookupRuleDefinitionService', 'VR_GenericData_BELookupRuleDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function BELookupRuleDefinitionController($scope, VR_GenericData_BELookupRuleDefinitionService, VR_GenericData_BELookupRuleDefinitionAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var beDefinitionSelectorAPI;
        var beDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                beDefinitionSelectorAPI = api;
                beDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.search = function () {
                return loadGrid();
            };

            $scope.scopeModel.add = function () {
                var onBELookupRuleDefinitionAdded = function (addedBELookupRuleDefinition) {
                    gridAPI.onBELookupRuleDefinitionAdded(addedBELookupRuleDefinition);
                };
                VR_GenericData_BELookupRuleDefinitionService.addBELookupRuleDefinition(onBELookupRuleDefinitionAdded);
            };

            $scope.scopeModel.hasAddPermission = function () {
                return VR_GenericData_BELookupRuleDefinitionAPIService.HasAddBELookupRuleDefinitionPermission();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadBEDefinitionSelector, loadGrid]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadBEDefinitionSelector() {
            var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            beDefinitionSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, undefined, beDefinitionSelectorLoadDeferred);
            });

            return beDefinitionSelectorLoadDeferred.promise;
        }

        function loadGrid() {
            var gridLoadDeferred = UtilsService.createPromiseDeferred();

            gridReadyDeferred.promise.then(function () {
                gridAPI.load(getGridQuery()).then(function () {
                    gridLoadDeferred.resolve();
                }).catch(function (error) {
                    gridLoadDeferred.reject(error);
                });
            });

            return gridLoadDeferred.promise;

            function getGridQuery() {
                return {
                    Name: $scope.scopeModel.name,
                    BusinessEntityDefinitionIds: beDefinitionSelectorAPI.getSelectedIds()
                };
            }
        }
    }

    appControllers.controller('VR_GenericData_BELookupRuleDefinitionController', BELookupRuleDefinitionController);

})(appControllers);