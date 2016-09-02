'use strict';

app.directive('retailBeAccounttypeGrid', ['Retail_BE_AccountTypeAPIService', 'Retail_BE_AccountTypeService', 'VRNotificationService', function (Retail_BE_AccountTypeAPIService, Retail_BE_AccountTypeService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeGrid = new AccountTypeGrid($scope, ctrl, $attrs);
            accountTypeGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclaredInfo/OperatorDeclaredInfoGridTemplate.html'
    };

    function AccountTypeGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.accountTypes = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_AccountTypeAPIService.GetFilteredAccountTypes(dataRetrievalInput).then(function (response) {
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

            api.onAccountTypeAdded = function (addedAccountType) {
                gridAPI.itemAdded(addedAccountType);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editAccountType,
                haspermission: hasEditAccountTypePermission
            });
        }
        function editAccountType(accountType) {
            var onAccountTypeUpdated = function (updatedAccountType) {
                gridAPI.itemUpdated(updatedAccountType);
            };
            Retail_BE_AccountTypeService.editAccountType(accountType.Entity.AccountTypeId, onAccountTypeUpdated);
        }
        function hasEditAccountTypePermission() {
            return Retail_BE_AccountTypeAPIService.HasUpdateAccountTypePermission();
        }
    }
}]);
