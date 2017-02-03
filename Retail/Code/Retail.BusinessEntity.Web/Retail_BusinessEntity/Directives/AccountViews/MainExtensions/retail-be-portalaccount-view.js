﻿(function (app) {

    'use strict';

    PortalAccountViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_PortalAccountService', 'Retail_BE_PortalAccountAPIService'];

    function PortalAccountViewDirective(UtilsService, VRNotificationService, Retail_BE_PortalAccountService, Retail_BE_PortalAccountAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PortalAccountViewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/PortalAccountViewTemplate.html'
        };

        function PortalAccountViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountViewDefinition;
            var accountBEDefinitionId;
            var parentAccountId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isPortalUserAccountCreated = false;

                $scope.scopeModel.onPortalAccountAdded = function () {
                    var onPortalAccountAdded = function (addedPortalAccount) {
                        $scope.scopeModel.isPortalUserAccountCreated = true;
                        $scope.scopeModel.userId = addedPortalAccount.UserId;
                        $scope.scopeModel.name = addedPortalAccount.Name;
                        $scope.scopeModel.email = addedPortalAccount.Email;
                    };

                    Retail_BE_PortalAccountService.addPortalAccount(onPortalAccountAdded, accountBEDefinitionId, parentAccountId, buildContext());
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.isGridLoading = true;

                    if (payload != undefined) {
                        accountViewDefinition = payload.accountViewDefinition;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                    }

                    Retail_BE_PortalAccountAPIService.GetPortalUserAccount(accountBEDefinitionId, parentAccountId).then(function (response) {
                        if (response != undefined) {
                            $scope.scopeModel.isPortalUserAccountCreated = true;
                            var portalAccountSettings = response;
                            $scope.scopeModel.userId = portalAccountSettings.UserId;
                            $scope.scopeModel.name = portalAccountSettings.Name;
                            $scope.scopeModel.email = portalAccountSettings.Email;
                        }
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildContext() {
                var context = {
                    getName: function () {
                        if (accountViewDefinition == undefined || accountViewDefinition.Settings == undefined)
                            return;

                        return accountViewDefinition.Settings.Name;
                    },
                    getEmail: function () {
                        if (accountViewDefinition == undefined || accountViewDefinition.Settings == undefined)
                            return;

                        return accountViewDefinition.Settings.Email;
                    },
                    getConnectionId: function () {
                        if (accountViewDefinition == undefined || accountViewDefinition.Settings == undefined)
                            return;

                        return accountViewDefinition.Settings.ConnectionId;
                    }
                }
                return context;
            }
        }
    }

    app.directive('retailBePortalaccountView', PortalAccountViewDirective);

})(app);