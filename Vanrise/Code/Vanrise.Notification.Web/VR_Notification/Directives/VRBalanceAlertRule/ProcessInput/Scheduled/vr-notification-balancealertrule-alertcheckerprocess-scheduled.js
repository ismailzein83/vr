"use strict";

app.directive("vrNotificationBalancealertruleAlertcheckerprocessScheduled", ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
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
        templateUrl: "/Client/Modules/VR_Notification/Directives/VRBalanceAlertRule/ProcessInput/Scheduled/Templates/BalanceAlertCheckerProcessScheduledTemplate.html"
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
                    $type: "Vanrise.Notification.BP.Arguments.BalanceAlertCheckerProcessInput, Vanrise.Notification.BP.Arguments",
                    AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds()
                };
            };

            api.load = function (payload) {
                var promises = [];
                promises.push(loadAlertRuleTypeSelector(payload));
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadAlertRuleTypeSelector(payload) {
            var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            alertRuleTypeSelectorAPIReadyDeferred.promise.then(function () {

                var selectorPayload = { filter: { Filters: [] } };
                selectorPayload.filter.Filters.push({ $type: "Vanrise.Notification.Entities.VRBalanceAlertRuleTypeFilter, Vanrise.Notification.Entities" });

                if (payload != undefined && payload.data != undefined)
                    selectorPayload.selectedIds = payload.data.AlertRuleTypeId;

                VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, selectorPayload, alertRuleTypeSelectorLoadDeferred);
            });

            return alertRuleTypeSelectorLoadDeferred.promise;
        }
    }

    return directiveDefinitionObject;
}]);
