'use strict';

app.directive('retailBeActiondefinitionGrid', ['Retail_BE_ActionDefinitionAPIService', 'Retail_BE_ActionDefinitionService', 'VRNotificationService', function (Retail_BE_ActionDefinitionAPIService, Retail_BE_ActionDefinitionService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var actionDefinitionGrid = new ActionDefinitionGrid($scope, ctrl, $attrs);
            actionDefinitionGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ActionDefinition/Templates/ActionDefinitionGridTemplate.html'
    };

    function ActionDefinitionGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.actionDefinitions = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_ActionDefinitionAPIService.GetFilteredActionDefinitions(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        }
        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onActionDefinitionAdded = function (addedActionDefinition) {
                gridAPI.itemAdded(addedActionDefinition);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editActionDefinition,
                haspermission: hasEditActionDefinitionPermission
            });
        }
        function editActionDefinition(actionDefinition) {
            var onActionDefinitionUpdated = function (updatedActionDefinition) {
                gridAPI.itemUpdated(updatedActionDefinition);
            };
            Retail_BE_ActionDefinitionService.editActionDefinition(actionDefinition.Entity.ActionDefinitionId, onActionDefinitionUpdated);
        }
        function hasEditActionDefinitionPermission() {
            return Retail_BE_ActionDefinitionAPIService.HasUpdateActionDefinitionPermission();
        }
    }
}]);
