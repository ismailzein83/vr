'use strict';

app.directive('vrNotificationAlertrulesettings', ['UtilsService', 'VR_Notification_AlertRuleSettingsService', 'VR_Notification_VRBalanceAlertRuleAPIService', 'VR_Notification_AlertLevelAPIService',
    function (UtilsService, VR_Notification_AlertRuleSettingsService, VR_Notification_VRBalanceAlertRuleAPIService, VR_Notification_AlertLevelAPIService) {

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
            this.initializeController = initializeController;

            var thresholdExtensionType;
            var actionExtensionType;
            var context;
            var alertLevelsInfo;
            var balanceAlertThresholdConfigs = [];
            var vrActionTarget;

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
                        balanceAlertThreshold.AlertLevelName = getAlertLevelName(balanceAlertThreshold.AlertLevelId);
                        ctrl.datasource.push({ Entity: balanceAlertThreshold });
                    };

                    VR_Notification_AlertRuleSettingsService.addAlertRuleThreshold(onAlertRuleSettingsAdded, actionExtensionType, thresholdExtensionType, vrActionTarget, getContext());
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

                api.load = function (payload) {
                    var promises = [];

                    var vrAlertRuleTypeId;
                    var alertTypeSettings;
                    var settings;

                    if (payload != undefined) {
                        vrAlertRuleTypeId = payload.vrAlertRuleTypeId;
                        alertTypeSettings = payload.alertTypeSettings;
                        settings = payload.settings;
                        context = payload.context;

                        if (alertTypeSettings != undefined) {
                            thresholdExtensionType = alertTypeSettings.ThresholdExtensionType;
                            actionExtensionType = alertTypeSettings.VRActionExtensionType;
                        }
                    }

                    if (settings != undefined && settings.ThresholdActions != undefined) {

                        var getAlertLevelsInfoPromise = getAlertLevelsInfo(alertTypeSettings.NotificationTypeId);
                        promises.push(getAlertLevelsInfoPromise);

                        getAlertLevelsInfoPromise.then(function () {
                            for (var i = 0; i < settings.ThresholdActions.length; i++) {
                                var thresholdAction = settings.ThresholdActions[i];
                                if (thresholdAction.Actions != undefined && thresholdAction.Actions.length > 0) {
                                    thresholdAction.ActionNames = getActionNames(thresholdAction.Actions);
                                }
                                if (thresholdAction.RollbackActions != undefined && thresholdAction.RollbackActions.length > 0) {
                                    thresholdAction.RollbackActionNames = getActionNames(thresholdAction.RollbackActions);
                                }
                                thresholdAction.AlertLevelName = getAlertLevelName(thresholdAction.AlertLevelId);
                                ctrl.datasource.push({ Entity: thresholdAction });
                            }
                        });
                    }
                    promises.push(loadActionTargetType());

                    function getAlertLevelsInfo(notificationTypeId) {
                        return VR_Notification_AlertLevelAPIService.GetAlertLevelsInfo({ VRNotificationTypeId: notificationTypeId }).then(function (response) {
                            alertLevelsInfo = response;
                        });
                    }
                    function loadActionTargetType() {
                        return VR_Notification_VRBalanceAlertRuleAPIService.GetVRBalanceActionTargetTypeByRuleTypeId(vrAlertRuleTypeId).then(function (response) {
                            vrActionTarget = response;
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = [];
                    if (ctrl.datasource.length > 0) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            data.push(ctrl.datasource[i].Entity);
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
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
                    thresholdActionObj.AlertLevelName = getAlertLevelName(thresholdActionObj.AlertLevelId);
                    ctrl.datasource[ctrl.datasource.indexOf(dataItem)] = { Entity: thresholdActionObj };
                };

                VR_Notification_AlertRuleSettingsService.editAlertRuleThreshold(dataItem.Entity, onThresholdActionUpdated, actionExtensionType, thresholdExtensionType, vrActionTarget, getContext());
            }

            function getVRBalanceAlertThresholdConfigs() {
                return VR_Notification_VRBalanceAlertRuleAPIService.GetVRBalanceAlertThresholdConfigs(thresholdExtensionType).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            balanceAlertThresholdConfigs.push(response[i]);
                        }
                    }
                });
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

            function getAlertLevelName(alertLevelId) {
                for (var index = 0; index < alertLevelsInfo.length; index++) {
                    var alertLevelInfo = alertLevelsInfo[index];
                    if (alertLevelInfo.VRAlertLevelId == alertLevelId)
                        return alertLevelInfo.Name;
                }
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

        }

        return directiveDefinitionObject;
    }]);