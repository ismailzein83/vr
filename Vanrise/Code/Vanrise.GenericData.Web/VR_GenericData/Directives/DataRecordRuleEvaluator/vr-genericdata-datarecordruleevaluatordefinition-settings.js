(function (app) {

    'use strict';

    DataRecordRuleEvaluatorDefinitionSettings.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DataRecordRuleEvaluatorDefinitionSettings(UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordRuleEvaluatorDefinitionSettingsDirective = new DataRecordRuleEvaluatorDefinitionSettingsDirective(ctrl, $scope, $attrs);
                dataRecordRuleEvaluatorDefinitionSettingsDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordRuleEvaluator/Templates/DataRecordRuleEvaluatorDefinitionSettingsTemplate.html'
        };

        function DataRecordRuleEvaluatorDefinitionSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var dataRecordStorageSelectorAPI;
            var dataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var alertRuleTypeSelectorAPI;
            var alertRuleTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageSelectorAPI = api;
                    dataRecordStorageSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onAlertRuleTypeSelectorReady = function (api) {
                    alertRuleTypeSelectorAPI = api;
                    alertRuleTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    
                    if (payload != undefined && payload.componentType != undefined)
                        $scope.scopeModel.name = payload.componentType.Name;

                    promises.push(loadDataRecordStorageSelector());
                    promises.push(loadAlertRuleTypeSelector());

                    function loadDataRecordStorageSelector() {
                        var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordStorageSelectorReadyDeferred.promise.then(function () {
                            var dataRecordStorageSelectorPayload;

                            if (payload != undefined && payload.componentType != undefined && payload.componentType.Settings != undefined) {
                                dataRecordStorageSelectorPayload = {
                                    selectedIds: payload.componentType.Settings.DataRecordStorageIds
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, dataRecordStorageSelectorPayload, dataRecordStorageSelectorLoadDeferred);
                        });

                        return dataRecordStorageSelectorLoadDeferred.promise;
                    }

                    function loadAlertRuleTypeSelector() {
                        var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        alertRuleTypeSelectorReadyDeferred.promise.then(function () {
                            var alertRuleTypeSelectorPayload;

                             if (payload != undefined && payload.componentType != undefined && payload.componentType.Settings != undefined) {
                                alertRuleTypeSelectorPayload = {
                                    selectedIds: payload.componentType.Settings.AlertRuleTypeId
                                };
                            }

                            VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, alertRuleTypeSelectorPayload, alertRuleTypeSelectorLoadDeferred);
                        });

                        return alertRuleTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Vanrise.GenericData.Entities.DataRecordRuleEvaluatorDefinitionSettings, Vanrise.GenericData.Entities",
                            DataRecordStorageIds: dataRecordStorageSelectorAPI.getSelectedIds(),
                            AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

            }
        }
        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataDatarecordruleevaluatordefinitionSettings', DataRecordRuleEvaluatorDefinitionSettings);

})(app);
