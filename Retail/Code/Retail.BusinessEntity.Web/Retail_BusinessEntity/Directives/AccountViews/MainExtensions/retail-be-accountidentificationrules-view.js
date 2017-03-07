﻿(function (app) {

    'use strict';

    AccountIdentificationRulesViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_AccountIdentificationService'];

    function AccountIdentificationRulesViewDirective(UtilsService, VRNotificationService, Retail_BE_AccountIdentificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountIdentificationRulesViewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/AccountIdentificationRulesViewTemplate.html'
        };

        function AccountIdentificationRulesViewCtor($scope, ctrl) {
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

                $scope.scopeModel.onAccountIdentificationRuleAdded = function () {
                    var onAccountIdentificationRuleAdded = function (addedService) {
                        gridAPI.onAccountIdentificationRuleAdded(addedService);
                    };
                    Retail_BE_AccountIdentificationService.assignIdentificationRuleToAccount(accountBEDefinitionId, parentAccountId, onAccountIdentificationRuleAdded);
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

                var accountIdentificationRulesGridPayload = {
                    accountBEDefinitionId: accountBEDefinitionId,
                    AccountId: parentAccountId
                };
                return accountIdentificationRulesGridPayload;
            }
        }
    }

    app.directive('retailBeAccountidentificationrulesView', AccountIdentificationRulesViewDirective);

})(app);