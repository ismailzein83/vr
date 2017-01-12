'use strict';

app.directive('retailBeAccountpackageGrid', ['Retail_BE_AccountPackageAPIService', 'VRNotificationService',
    function (Retail_BE_AccountPackageAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountPackageGrid = new AccountPackageGrid($scope, ctrl, $attrs);
                accountPackageGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountPackage/Templates/AccountPackageGridTemplate.html'
        };

        function AccountPackageGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var assignedToAccountId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.accountPackages = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_AccountPackageAPIService.GetFilteredAccountPackages(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        assignedToAccountId = payload.AssignedToAccountId
                    }

                    return gridAPI.retrieveData(buildGridQuery());
                };

                api.onAccountPackageAdded = function (addedAccountPackage) {
                    gridAPI.itemAdded(addedAccountPackage);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [];
            }

            function buildGridQuery() {
                return {
                    AccountBEDefinitionId: accountBEDefinitionId,
                    AssignedToAccountId: assignedToAccountId
                };
            }
        }
    }]);
