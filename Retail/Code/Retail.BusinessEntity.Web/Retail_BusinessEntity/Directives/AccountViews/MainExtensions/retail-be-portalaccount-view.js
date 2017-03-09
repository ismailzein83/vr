﻿(function (app) {

    'use strict';

    PortalAccountViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_PortalAccountService', 'Retail_BE_PortalAccountAPIService'];

    function PortalAccountViewDirective(UtilsService, VRNotificationService, Retail_BE_PortalAccountService, Retail_BE_PortalAccountAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
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
                $scope.scopeModel.sectionMenuActions = [];
                $scope.scopeModel.isPortalUserAccountCreated = false;

                $scope.scopeModel.addPortalAccount = function () {
                    var onPortalAccountAdded = function (addedPortalAccount) {
                        $scope.scopeModel.isPortalUserAccountCreated = true;
                        $scope.scopeModel.name = addedPortalAccount.Name;
                        $scope.scopeModel.email = addedPortalAccount.Email;

                        $scope.scopeModel.sectionMenuActions.length = 0;
                        $scope.scopeModel.sectionMenuActions.push({
                            name: 'Reset Password',
                            clicked: function () {
                                Retail_BE_PortalAccountService.resetPassword(accountBEDefinitionId, parentAccountId, buildContext());
                            }
                        });
                    };

                    Retail_BE_PortalAccountService.addPortalAccount(onPortalAccountAdded, accountBEDefinitionId, parentAccountId, buildContext());
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        accountViewDefinition = payload.accountViewDefinition;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                    }

                    Retail_BE_PortalAccountAPIService.GetPortalAccountSettings(accountBEDefinitionId, parentAccountId).then(function (response) {
                        if (response != undefined) {
                            var portalAccountSettings = response;
                            $scope.scopeModel.isPortalUserAccountCreated = true;
                            $scope.scopeModel.name = portalAccountSettings.Name;
                            $scope.scopeModel.email = portalAccountSettings.Email;

                            $scope.scopeModel.sectionMenuActions.push({
                                name: 'Reset Password',
                                clicked: function () {
                                    Retail_BE_PortalAccountService.resetPassword(accountBEDefinitionId, parentAccountId, buildContext());
                                }
                            });
                        }
                        else {
                            $scope.scopeModel.sectionMenuActions.push({
                                name: 'Add Portal Account',
                                clicked: $scope.scopeModel.addPortalAccount
                            });
                        }
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildContext() {
                var context = {
                    getAccountViewDefinitionId: function () {
                        if (accountViewDefinition == undefined)
                            return;

                        return accountViewDefinition.AccountViewDefinitionId;
                    },
                    getName: function () {
                        if (accountViewDefinition == undefined || accountViewDefinition.Settings == undefined)
                            return;

                        return accountViewDefinition.Settings.AccountNameMappingField;
                    },
                    getEmail: function () {
                        if (accountViewDefinition == undefined || accountViewDefinition.Settings == undefined)
                            return;

                        return accountViewDefinition.Settings.AccountEmailMappingField;
                    }
                };
                return context;
            }
        }
    }

    app.directive('retailBePortalaccountView', PortalAccountViewDirective);

}) (app);