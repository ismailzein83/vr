(function (app) {

    'use strict';

    PortalAccountViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_PortalAccountService', 'Retail_BE_PortalAccountAPIService', 'Retail_BE_AccountTypeAPIService'];

    function PortalAccountViewDirective(UtilsService, VRNotificationService, Retail_BE_PortalAccountService, Retail_BE_PortalAccountAPIService, Retail_BE_AccountTypeAPIService) {
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

            var portalAccountGridAPI;
            var portalAccountReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onPortalAccountGridReady = function (api) {
                    portalAccountGridAPI = api;
                    portalAccountReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                var promises = [];
                api.load = function (payload) {
                    $scope.scopeModel.isGridLoading = true;
                    var accountViewDefinitionId;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                        accountViewDefinition = payload.accountViewDefinition;

                        if (accountViewDefinition != undefined) {
                            accountViewDefinitionId = accountViewDefinition.AccountViewDefinitionId;
                        }
                    }
                   
                    Retail_BE_PortalAccountAPIService.GetPortalAccountSettings(accountBEDefinitionId, parentAccountId, accountViewDefinitionId).then(function (response) {
                        var portalAccountSettings = response;
                        portalAccountReadyDeferred.promise.then(function () {
                            var portalAccountGridPayload = {};
                            if (payload != undefined) {
                                portalAccountGridPayload.accountBEDefinitionId = payload.accountBEDefinitionId;
                                portalAccountGridPayload.parentAccountId = payload.parentAccountId;
                                portalAccountGridPayload.accountViewDefinition = payload.accountViewDefinition;
                                portalAccountGridPayload.context = buildContext();
                                if (payload.accountViewDefinition != undefined && payload.accountViewDefinition.Settings != undefined)
                                    if (response != undefined) {
                                        portalAccountGridPayload.portalAccounts = portalAccountSettings.AdditionalUsers;
                                       
                                    }
                            }
                            return portalAccountGridAPI.load(portalAccountGridPayload).then(function () {
                                $scope.scopeModel.isGridLoading = false;
                            });;
                        });

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

})(app);