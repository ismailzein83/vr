'use strict';

app.directive('vrGenericdataDatarecordalertruleRecordfilterManagement', ['UtilsService', 'VR_GenericData_DataRecordAlertRuleService', 'VR_GenericData_RecordFilterAPIService', 'VR_Notification_AlertLevelAPIService',
function (UtilsService, VR_GenericData_DataRecordAlertRuleService, VR_GenericData_RecordFilterAPIService, VR_Notification_AlertLevelAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new DataRecordAlertRule(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/AlertRule/Directive/Templates/DataRecordAlertRuleRecordFilterManagementTemplate.html'
    };

    function DataRecordAlertRule(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        var alertLevelsInfo;
        var context;

        var getAlertLevelsInfoPromise;

        function initializeController() {
            ctrl.datasource = [];

            ctrl.isValid = function () {
                if (ctrl.datasource.length > 0)
                    return null;
                return "You Should Add at least one Item.";
            };

            ctrl.addDataRecordAlertRule = function () {
                var onDataRecordAlertRuleAdded = function (dataRecordAlertRuleObj) {
                    ctrl.datasource.push(dataRecordAlertRuleObj);
                };

                VR_GenericData_DataRecordAlertRuleService.addDataRecordAlertRule(context, onDataRecordAlertRuleAdded);
            };

            ctrl.removeDataRecordAlertRule = function (dataItem) {
                var index = ctrl.datasource.indexOf(dataItem);
                ctrl.datasource.splice(index, 1);
            };

            defineAPI();

            defineMenuActions();
        };

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var settings;

                if (payload != undefined) {
                    settings = payload.settings;
                    context = payload.context;
                }

                var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                VR_Notification_AlertLevelAPIService.GetAlertLevelsInfo({ VRNotificationTypeId: context.notificationTypeId }).then(function (response) {
                    alertLevelsInfo = response;

                    var fillDataSourcePromisesDeferred = [];

                    if (settings != undefined && settings.RecordAlertRuleConfigs != undefined) {
                        for (var x = 0; x < settings.RecordAlertRuleConfigs.length; x++) {
                            var currentRecordAlertRuleConfig = settings.RecordAlertRuleConfigs[x];
                            fillDataSourcePromisesDeferred.push(fillDataSource(currentRecordAlertRuleConfig));
                        }
                    }

                    UtilsService.waitMultiplePromises(fillDataSourcePromisesDeferred).then(function () {
                        loadPromiseDeferred.resolve();
                    });
                });

                return loadPromiseDeferred.promise;
            };

            api.getData = function () {
                var data;

                if (ctrl.datasource.length > 0) {
                    data = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        data.push(ctrl.datasource[i].Entity);
                    }
                }

                return {
                    $type: 'Vanrise.GenericData.Notification.RecordAlertRuleSettings, Vanrise.GenericData.Notification',
                    RecordAlertRuleConfigs: data
                };
            };

            api.hasData = function () {
                return ctrl.datasource.length > 0;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };

        function fillDataSource(currentRecordAlertRuleConfig) {
            return VR_GenericData_RecordFilterAPIService.BuildRecordFilterGroupExpression({ RecordFields: context.recordfields, FilterGroup: currentRecordAlertRuleConfig.FilterGroup }).then(function (response) {
                ctrl.datasource.push({
                    AlertLevelName: getAlertLevelName(currentRecordAlertRuleConfig.AlertLevelId),
                    FilterExpression: response,
                    Entity: currentRecordAlertRuleConfig,
                    ActionNames: buildActionNames(currentRecordAlertRuleConfig.Actions)
                });
            });
        };

        function defineMenuActions() {

            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editDataRecordAlertRule,
            }];
        };

        function editDataRecordAlertRule(dataItem) {
            var onDataRecordAlertRuleUpdated = function (dataRecordAlertRuleObj) {
                ctrl.datasource[ctrl.datasource.indexOf(dataItem)] = dataRecordAlertRuleObj;
            };

            VR_GenericData_DataRecordAlertRuleService.editDataRecordAlertRule(dataItem.Entity, context, onDataRecordAlertRuleUpdated);
        };

        function buildActionNames(actions) {
            if (actions == undefined || actions == null || actions.length == 0)
                return null;

            var actionNames = [];
            for (var x = 0; x < actions.length; x++) {
                var currentAction = actions[x];
                actionNames.push(currentAction.ActionName);
            }

            return actionNames.join();
        };

        function getAlertLevelName(alertLevelId) {
            for (var index = 0; index < alertLevelsInfo.length; index++) {
                var alertLevelInfo = alertLevelsInfo[index];
                if (alertLevelInfo.VRAlertLevelId == alertLevelId)
                    return alertLevelInfo.Name;
            }
        }
    };

    return directiveDefinitionObject;
}]);