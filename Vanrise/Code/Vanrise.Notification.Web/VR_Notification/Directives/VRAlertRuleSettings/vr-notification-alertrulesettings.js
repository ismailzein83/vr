'use strict';
app.directive('vrNotificationAlertrulesettings', ['UtilsService', 'VR_Notification_AlertRuleSettingsService','VR_Notification_VRBalanceAlertRuleAPIService',
function (UtilsService, VR_Notification_AlertRuleSettingsService, VR_Notification_VRBalanceAlertRuleAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AlertRuleSettings(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Notification/Directives/VRAlertRuleSettings/Templates/VRAlertRuleSettingsTemplate.html"

    };


    function AlertRuleSettings(ctrl, $scope, $attrs) {
        var actionExtensionType;
        var thresholdExtensionType;
        var balanceAlertThresholdConfigs = [];
        var context;
        function initializeController() {

            ctrl.datasource = [];

            ctrl.isValid = function () {
                if (ctrl.datasource.length > 0)
                    return null;

                return "You Should Select at least one threshold action.";
            };

            ctrl.addThresholdAction = function () {
                var onAlertRuleSettingsAdded = function (balanceAlertThreshold) {
                    balanceAlertThreshold.ActionNames = getActionNames(balanceAlertThreshold.Actions);
                    balanceAlertThreshold.RollbackActionNames = getActionNames(balanceAlertThreshold.RollbackActions);
                    ctrl.datasource.push({ Entity: balanceAlertThreshold });
                };
                VR_Notification_AlertRuleSettingsService.addAlertRuleThreshold(onAlertRuleSettingsAdded, actionExtensionType, thresholdExtensionType, getContext());
            };

            ctrl.removeThresholdAction = function (dataItem) {
                var index = ctrl.datasource.indexOf(dataItem);
                ctrl.datasource.splice(index, 1);
            };

            defineAPI();
            defineMenuActions();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var data = [];
                if (ctrl.datasource.length > 0) {
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        data.push(ctrl.datasource[i].Entity);
                    }
                }
                return data;
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    if (payload.alertTypeSettings != undefined) {
                        actionExtensionType = payload.alertTypeSettings.VRActionExtensionType;
                        thresholdExtensionType = payload.alertTypeSettings.ThresholdExtensionType;
                        context = payload.context;
                        if (payload.settings != undefined) {
                            if (payload.settings.ThresholdActions != undefined) {
                                for (var i = 0; i < payload.settings.ThresholdActions.length; i++) {
                                    var thresholdAction = payload.settings.ThresholdActions[i];
                                    if (thresholdAction.Actions != undefined && thresholdAction.Actions.length > 0) {
                                        thresholdAction.ActionNames = getActionNames(thresholdAction.Actions);
                                    }
                                    if (thresholdAction.RollbackActions != undefined && thresholdAction.RollbackActions.length > 0) {
                                        thresholdAction.RollbackActionNames = getActionNames(thresholdAction.RollbackActions);
                                    }
                                    ctrl.datasource.push({ Entity: thresholdAction });
                                }
                            }
                        }

                    }
                }


            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getActionNames(actions) {
            if (actions != undefined) {
                var actionNames;
                for (var i = 0; i < actions.length; i++) {
                    var action = actions[i];
                    if (actionNames == undefined)
                        actionNames = "";
                    else
                        actionNames += ", ";
                    actionNames += action.ActionName;

                }
                return actionNames;
            }

        }

        function defineMenuActions() {

            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editThresholdAction,
            }];
        }

        function editThresholdAction(dataItem) {
            var onThresholdActionUpdated = function (thresholdActionObj) {
                thresholdActionObj.ActionNames = getActionNames(thresholdActionObj.Actions);
                thresholdActionObj.RollbackActionNames = getActionNames(thresholdActionObj.RollbackActions);
                ctrl.datasource[ctrl.datasource.indexOf(dataItem)] = { Entity: thresholdActionObj };
            };
            VR_Notification_AlertRuleSettingsService.editAlertRuleThreshold(dataItem.Entity, onThresholdActionUpdated, actionExtensionType, thresholdExtensionType, getContext());
        }

        function getVRBalanceAlertThresholdConfigs()
        {
            return VR_Notification_VRBalanceAlertRuleAPIService.GetVRBalanceAlertThresholdConfigs(thresholdExtensionType).then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        balanceAlertThresholdConfigs.push(response[i]);
                    }
                }
            });
        }

        function getContext()
        {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);