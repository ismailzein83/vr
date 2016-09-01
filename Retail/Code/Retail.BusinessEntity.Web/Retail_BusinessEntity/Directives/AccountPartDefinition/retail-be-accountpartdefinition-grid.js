'use strict';

app.directive('retailBeAccountpartdefinitionGrid', ['Retail_BE_AccountPartDefinitionAPIService', 'Retail_BE_AccountPartDefinitionService', 'VRNotificationService', function (Retail_BE_AccountPartDefinitionAPIService, Retail_BE_AccountPartDefinitionService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountPartDefinitionGrid = new AccountPartDefinitionGrid($scope, ctrl, $attrs);
            accountPartDefinitionGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountPartDefinition/Templates/AccountPartDefinitionGridTemplate.html'
    };

    function AccountPartDefinitionGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.accountPartDefinitions = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_AccountPartDefinitionAPIService.GetFilteredAccountPartDefinitions(dataRetrievalInput).then(function (response) {
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

            api.onAccountPartDefinitionAdded = function (addedAccountPartDefinition) {
                gridAPI.itemAdded(addedAccountPartDefinition);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editAccountPartDefinition,
                haspermission: hasEditAccountPartDefinitionPermission
            });
        }
        function editAccountPartDefinition(accountPartDefinition) {
            var onAccountPartDefinitionUpdated = function (updatedAccountPartDefinition) {
                gridAPI.itemUpdated(updatedAccountPartDefinition);
            };
            Retail_BE_AccountPartDefinitionService.editAccountPartDefinition(accountPartDefinition.Entity.AccountPartDefinitionId, onAccountPartDefinitionUpdated);
        }
        function hasEditAccountPartDefinitionPermission() {
            return Retail_BE_AccountPartDefinitionAPIService.HasUpdateAccountPartDefinitionPermission();
        }
    }
}]);
