"use strict";

app.directive("vrAccountbalanceUpdateprocessScheduled", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/VR_AccountBalance/Directives/ProcessInput/Scheduled/Templates/AccountBalanceUpdateProcessScheduledTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var accountTypeSelectorAPI;
        var accountTypeSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.accountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorAPIReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getData = function () {
                return {
                    InputArguments: {
                        $type: "Vanrise.AccountBalance.BP.Arguments.AccountBalanceUpdateProcessInput, Vanrise.AccountBalance.BP.Arguments",
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds()
                    }
                };
            };

            api.load = function (payload) {
                var promises = [];
                promises.push(loadAccountTypeSelector());
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadAccountTypeSelector() {
            var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            accountTypeSelectorAPIReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, undefined, accountTypeSelectorLoadDeferred);
            });

            return accountTypeSelectorLoadDeferred.promise;
        }
    }

    return directiveDefinitionObject;
}]);
