(function (app) {

    'use strict';

    AccountBERuntimeSelectorFilterDirective.$inject = ['Retail_BE_AccountConditionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AccountBERuntimeSelectorFilterDirective(Retail_BE_AccountConditionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBERuntimeSelectorFilterCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Account/MainExtensions/Templates/AccountBERuntimeSelectorFilterTemplate.html"
        };

        function AccountBERuntimeSelectorFilterCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountConditionSelectiveDirectiveAPI;
            var accountConditionSelectiveDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountConditionSelectiveReady = function (api) {
                    accountConditionSelectiveDirectiveAPI = api;
                    accountConditionSelectiveDirectivePromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;
                    var beRuntimeSelectorFilter;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.beDefinitionId;
                        beRuntimeSelectorFilter = payload.beRuntimeSelectorFilter;
                    }

                    var accountConditionSelectiveDirectiveLoadPromise = getAccountConditionSelectiveDirectiveLoadPromise();
                    promises.push(accountConditionSelectiveDirectiveLoadPromise);


                    function getAccountConditionSelectiveDirectiveLoadPromise() {
                        var accountConditionSelectiveDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        accountConditionSelectiveDirectivePromiseDeferred.promise.then(function () {

                            var accountConditionSelectivePayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                beFilter: beRuntimeSelectorFilter != undefined ? beRuntimeSelectorFilter.AccountCondition : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveDirectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveDirectiveLoadDeferred);
                        });

                        return accountConditionSelectiveDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var accountConditionSelectiveData = accountConditionSelectiveDirectiveAPI.getData();
                    if (accountConditionSelectiveData == undefined)
                        return;

                    var obj = {
                        $type: "Retail.BusinessEntity.Business.AccountBERuntimeSelectorFilter, Retail.BusinessEntity.Business",
                        AccountCondition: accountConditionSelectiveData
                    }

                    return obj;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeAccountbeRuntimeselectorfilter', AccountBERuntimeSelectorFilterDirective);

})(app);