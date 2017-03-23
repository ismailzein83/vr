﻿'use strict';
app.directive('vrGenericdataDatarecordalertruleRecordfilterEditor', ['UtilsService', 'VR_GenericData_DataRecordAlertRuleService', 'VR_GenericData_RecordFilterAPIService',
function (UtilsService, VR_GenericData_DataRecordAlertRuleService, VR_GenericData_RecordFilterAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
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
        templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Directive/Templates/AlertRuleRecordFilterTemplate.html'
    };

    function DataRecordAlertRule(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;
        var context;
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

            api.getData = function () {
                var data;
                if (ctrl.datasource.length > 0) {
                    data = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        data.push(ctrl.datasource[i].Entity);
                    }
                }
                var obj = {
                    $type: 'Vanrise.GenericData.Notification.RecordAlertRuleSettings, Vanrise.GenericData.Notification',
                    RecordAlertRuleConfigs: data
                };
                return obj;
            };

            api.load = function (payload) {
                var promises = [];
                var settings;
                if (payload != undefined) {
                    settings = payload.settings;
                    context = payload.context;
                }

                ctrl.datasource.length = 0;
                if (settings != undefined && settings.RecordAlertRuleConfigs != undefined) {
                    for (var x = 0; x < settings.RecordAlertRuleConfigs.length; x++) {
                        var currentRecordAlertRuleConfig = settings.RecordAlertRuleConfigs[x];

                        var promise = VR_GenericData_RecordFilterAPIService.BuildRecordFilterGroupExpression({ RecordFields: context.recordfields, FilterGroup: currentRecordAlertRuleConfig.FilterGroup }).then(function (response) {
                            ctrl.datasource.push({ FilterExpression: response, Entity: currentRecordAlertRuleConfig, ActionNames: buildActionNames(currentRecordAlertRuleConfig.Actions) });
                        });
                        promises.push(promise);

                    }
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
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
    };

    return directiveDefinitionObject;
}]);