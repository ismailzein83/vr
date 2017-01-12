(function (app) {

    'use strict';

    AccountServicesViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountServiceService'];

    function AccountServicesViewDirective(UtilsService, VRNotificationService, Retail_BE_AccountServiceService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountServicesViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/AccountServicesViewTemplate.html'
        };

        function AccountServicesViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var parentAccountId;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAccountServiceAdded = function () {
                    var onAccountServiceAdded = function (addedService) {
                        gridAPI.onAccountServiceAdded(addedService);
                    };

                    Retail_BE_AccountServiceService.addAccountService(accountBEDefinitionId, parentAccountId, onAccountServiceAdded);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.isGridLoading = true;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                    }

                    return gridAPI.loadGrid(buildGridPayload()).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload() {

                var accountServiceGridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    AccountId: parentAccountId
                };
                return accountServiceGridPayload;
            }
        }
    }

    app.directive('retailBeAccountservicesView', AccountServicesViewDirective);

})(app);