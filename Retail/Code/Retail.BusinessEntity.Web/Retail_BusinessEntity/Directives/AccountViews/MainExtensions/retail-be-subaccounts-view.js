(function (app) {

    'use strict';

    SubAccountsViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountBEService'];

    function SubAccountsViewDirective(UtilsService, VRNotificationService, Retail_BE_AccountBEService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SubAccountsViewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/SubAccountsViewTemplate.html'
        };

        function SubAccountsViewCtor($scope, ctrl) {
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

                $scope.scopeModel.onSubAccountAdded = function () {
                    var onSubAccountAdded = function (addedSubcAccount) {
                        gridAPI.onAccountAdded(addedSubcAccount);
                    };

                    Retail_BE_AccountBEService.addAccount(accountBEDefinitionId, parentAccountId, onSubAccountAdded);
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

                    return gridAPI.load(buildGridPayload(payload)).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload(loadPayload) {
                return loadPayload;
            }
        }
    }

    app.directive('retailBeSubaccountsView', SubAccountsViewDirective);

})(app);