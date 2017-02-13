(function (app) {

    'use strict';

    RetailAccountPropertyEvaluator.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountTypeAPIService'];

    function RetailAccountPropertyEvaluator(UtilsService, VRUIUtilsService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailAccountPropertyEvaluatorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/RetailAccountPropertyEvaluatorTemplate.html"
        };

        function RetailAccountPropertyEvaluatorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var accountGenericFieldDefinitionSelectorAPI;
            var accountGenericFieldDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onAccountGenericFieldDefinitionSelectorReady = function (api) {
                    accountGenericFieldDefinitionSelectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;
                    var genericFieldDefinition;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.objectType != undefined ? payload.objectType.AccountBEDefinitionId : undefined;
                        genericFieldDefinition = payload.objectPropertyEvaluator != undefined ? payload.objectPropertyEvaluator.GenericFieldDefinition : undefined;
                        $scope.scopeModel.useDescription = payload.objectPropertyEvaluator != undefined ? payload.objectPropertyEvaluator.UseDescription : undefined;
                    }

                    var accountGenericFieldDefinitionSelectorLoadPromise = getAccountGenericFieldDefinitionSelectorLoadPromise();
                    promises.push(accountGenericFieldDefinitionSelectorLoadPromise);

                    function getAccountGenericFieldDefinitionSelectorLoadPromise() {
                        var accountGenericFieldDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var accountGenericFieldDefinitionSelectorPayload = {
                            genericFieldDefinition: genericFieldDefinition
                        };
                        if (accountBEDefinitionId != undefined) {
                            accountGenericFieldDefinitionSelectorPayload.accountBEDefinitionId = accountBEDefinitionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(accountGenericFieldDefinitionSelectorAPI, accountGenericFieldDefinitionSelectorPayload, accountGenericFieldDefinitionSelectorLoadDeferred);

                        return accountGenericFieldDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.VRObjectTypes.RetailAccountPropertyEvaluator, Retail.BusinessEntity.MainExtensions",
                        GenericFieldDefinition: accountGenericFieldDefinitionSelectorAPI.getData(),
                        UseDescription: $scope.scopeModel.useDescription
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeRetailaccountPropertyevaluator', RetailAccountPropertyEvaluator);

})(app);
