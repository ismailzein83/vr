(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionController.$inject = ['$scope', 'VR_GenericData_BELookupRuleDefinitionService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function BELookupRuleDefinitionController($scope, VR_GenericData_BELookupRuleDefinitionService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var beDefinitionSelectorAPI;
        var beDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.scopeModel.onBEDefinitionSelectorReady = function (api)
            {
                beDefinitionSelectorAPI = api;
                beDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.search = function ()
            {
                $scope.scopeModel.showGrid = true;
                return gridAPI.load(getGridQuery());
            };

            $scope.scopeModel.add = function ()
            {
                var onBELookupRuleDefinitionAdded = function (addedBELookupRuleDefinition)
                {
                    gridAPI.onBELookupRuleDefinitionAdded(addedBELookupRuleDefinition);
                };
                VR_GenericData_BELookupRuleDefinitionService.addBELookupRuleDefinition(onBELookupRuleDefinitionAdded);
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls()
        {
            var loadBEDefinitionSelectorPromise = loadBEDefinitionSelector();

            return UtilsService.waitMultiplePromises([loadBEDefinitionSelectorPromise, gridReadyDeferred.promise]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadBEDefinitionSelector()
        {
            var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            beDefinitionSelectorReadyDeferred.promise.then(function ()
            {
                VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, undefined, beDefinitionSelectorLoadDeferred);
            });

            return beDefinitionSelectorLoadDeferred.promise;
        }

        function getGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                BusinessEntityDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('VR_GenericData_BELookupRuleDefinitionController', BELookupRuleDefinitionController);

})(appControllers);