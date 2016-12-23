"use strict";

app.directive("vrNotificationBalancealertruleAlertthresholdprocessManual", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
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
        templateUrl: "/Client/Modules/VR_Notification/Directives/VRBalanceAlertRule/ProcessInput/Normal/Templates/BalanceAlertThresholdProcessManualTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        var alertRuleTypeSelectorAPI;
        var alertRuleTypeSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.alertRuleTypeSelectorReady = function (api) {
                alertRuleTypeSelectorAPI = api;
                alertRuleTypeSelectorAPIReadyDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getData = function () {
                return {
                    InputArguments: {
                        $type: "Vanrise.Notification.BP.Arguments.BalanceAlertThresholdUpdateProcessInput, Vanrise.Notification.BP.Arguments",
                        AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds()
                    }
                };
            };

            api.load = function (payload) {
                var promises = [];
                promises.push(loadAlertRuleTypeSelector());
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadAlertRuleTypeSelector() {
            var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            alertRuleTypeSelectorAPIReadyDeferred.promise.then(function () {

                var payload = { filter: { Filters: [] } };
                payload.filter.Filters.push({ $type: "Vanrise.Notification.Entities.VRBalanceAlertRuleTypeFilter, Vanrise.Notification.Entities" });

                VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, payload, alertRuleTypeSelectorLoadDeferred);
            });

            return alertRuleTypeSelectorLoadDeferred.promise;
        }
    }

    return directiveDefinitionObject;
}]);
