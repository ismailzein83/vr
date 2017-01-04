(function (app) {

    'use strict';

    AccountInfoViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function AccountInfoViewDirective(UtilsService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountInfoViewCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/AccountInfoViewTemplate.html'
        };

        function AccountInfoViewCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var accountEditorDirectiveAPI;
            var accountEditorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountEditorDirectiveReady = function (api) {
                    accountEditorDirectiveAPI = api;
                    accountEditorDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isDirectiveLoading = true;

                    var promises = [];
                    var accountBEDefinitionId;
                    var accountId;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountId = payload.parentAccountId;
                    }

                    //Loading AccountEditor directive
                    var accountEditorDirectiveLoadPromise = getAccountEditorDirectiveLoadPromise();
                    promises.push(accountEditorDirectiveLoadPromise);


                    function getAccountEditorDirectiveLoadPromise() {
                        var accountEditorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountEditorDirectiveReadyDeferred.promise.then(function () {

                            var accountEditorDirectivePayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                accountId: accountId
                            };
                            VRUIUtilsService.callDirectiveLoad(accountEditorDirectiveAPI, accountEditorDirectivePayload, accountEditorLoadDeferred);
                        });

                        return accountEditorLoadDeferred.promise.then(function () {
                            $scope.scopeModel.isDirectiveLoading = false;
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeAccountinfoView', AccountInfoViewDirective);

})(app);