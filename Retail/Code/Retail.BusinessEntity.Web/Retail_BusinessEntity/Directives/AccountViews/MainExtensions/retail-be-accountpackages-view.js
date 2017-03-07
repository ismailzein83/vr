﻿(function (app) {

    'use strict';

    AccountPackagesViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountPackageService', 'Retail_BE_AccountPackageAPIService'];

    function AccountPackagesViewDirective(UtilsService, VRNotificationService, Retail_BE_AccountPackageService, Retail_BE_AccountPackageAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountPackagesViewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/AccountPackagesViewTemplate.html'
        };

        function AccountPackagesViewCtor($scope, ctrl) {
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
                $scope.scopeModel.hasAssignAccountPackagePermission = function () {
                    return Retail_BE_AccountPackageAPIService.HasAddAccountPackagePermission();
                };
                $scope.scopeModel.onAccountPackageAdded = function () {
                    var onAccountPackageAdded = function (addedPackage) {
                        gridAPI.onAccountPackageAdded(addedPackage);
                    };

                    Retail_BE_AccountPackageService.assignPackageToAccount(accountBEDefinitionId, parentAccountId, onAccountPackageAdded);
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

                    return gridAPI.load(buildGridPayload()).then(function () {
                        $scope.scopeModel.isGridLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildGridPayload() {

                var accountPackageGridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    AssignedToAccountId: parentAccountId
                };
                return accountPackageGridPayload;
            }
        }
    }

    app.directive('retailBeAccountpackagesView', AccountPackagesViewDirective);

})(app);