(function (appControllers) {
    'use strict';

    MediationDefinitionManagementController.$inject = ['$scope', 'Mediation_Generic_MediationDefinitionService', 'Mediation_Generic_MediationDefinitionAPIService'];

    function MediationDefinitionManagementController($scope, Mediation_Generic_MediationDefinitionService, Mediation_Generic_MediationDefinitionAPIService) {

        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.hasAddMediationDefinition = function () {
                return Mediation_Generic_MediationDefinitionAPIService.HasAddMediationDefinition();
            };
            $scope.addMediationDefinition = function () {
                var onMediationDefinitionAdded = function (onMediationDefinitionObj) {
                    gridAPI.onMediationDefinitionAdded(onMediationDefinitionObj);
                };

                Mediation_Generic_MediationDefinitionService.addMediationDefinition(onMediationDefinitionAdded);
            };
        }

        function load() {

        }

        function getFilterObject() {
            filter = {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('Mediation_Generic_MediationDefinitionManagementController', MediationDefinitionManagementController);

})(appControllers);
