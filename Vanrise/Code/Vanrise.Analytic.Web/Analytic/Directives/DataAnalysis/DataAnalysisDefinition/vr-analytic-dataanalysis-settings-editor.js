(function (app) {

    'use strict';

    DataAnalysisSettingsEditorDirective.$inject = ['VR_Analytic_DataAnalysisItemDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_Analytic_DataAnalysisParametersType'];

    function DataAnalysisSettingsEditorDirective(VR_Analytic_DataAnalysisItemDefinitionAPIService, UtilsService, VRUIUtilsService, VR_Analytic_DataAnalysisParametersType) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataAnalysisSettingsCtor = new DataAnalysisSettings($scope, ctrl, $attrs);
                dataAnalysisSettingsCtor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/DataAnalysisDefinition/Templates/DataAnalysisSettingsEditorTemplate.html'
        };

        function DataAnalysisSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataAnalysisItemTabs = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    VR_Analytic_DataAnalysisItemDefinitionAPIService.GetDataAnalysisItemDefinitionsHavingParameters(VR_Analytic_DataAnalysisParametersType.Global.value).then(function (response) {
                        if (response != undefined) {
                            for (var i = 0; i < response.length; i++) {
                                var currentDataAnalysisItem = response[i];
                                var dataAnalysisItemDef = {
                                    payload: currentDataAnalysisItem,
                                    parameterDirectiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    parameterDirectiveLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                };

                                addDataAnalysisItemTab(dataAnalysisItemDef);
                                initialPromises.push(dataAnalysisItemDef.parameterDirectiveLoadPromiseDeferred.promise);
                            }
                        }
                    });

                    function addDataAnalysisItemTab(dataAnalysisItemDef) {
                        var dataAnalysisItem = {
                            dataAnalysisItemId: dataAnalysisItemDef.payload.DataAnalysisItemDefinitionId,
                            tabTitle: dataAnalysisItemDef.payload.Settings.Title
                        };

                        dataAnalysisItem.onParametersEditorDirectiveReady = function (api) {
                            dataAnalysisItem.parameterEditorRuntimeAPI = api;
                            dataAnalysisItemDef.parameterDirectiveReadyPromiseDeferred.resolve();
                        };

                        dataAnalysisItemDef.parameterDirectiveReadyPromiseDeferred.promise.then(function () {
                            var dataAnalysisItemDefPayload = dataAnalysisItemDef.payload;
                            var dataAnalysisItemSettings = dataAnalysisItemDefPayload.Settings;
                            var daProfCalcParameters = dataAnalysisItemSettings.DAProfCalcParameters;

                            var parametersEditorPayload = {
                                selectedValues: getParameterValues(dataAnalysisItemDefPayload.DataAnalysisItemDefinitionId),
                                dataRecordTypeId: daProfCalcParameters != undefined ? daProfCalcParameters.ParametersRecordTypeId : undefined,
                                definitionSettings: daProfCalcParameters != undefined ? daProfCalcParameters.GlobalParametersEditorDefinitionSetting : undefined,
                                runtimeEditor: daProfCalcParameters != undefined && daProfCalcParameters.GlobalParametersEditorDefinitionSetting != undefined ? daProfCalcParameters.GlobalParametersEditorDefinitionSetting.RuntimeEditor : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dataAnalysisItem.parameterEditorRuntimeAPI, parametersEditorPayload, dataAnalysisItemDef.parameterDirectiveLoadPromiseDeferred);
                        });

                        $scope.scopeModel.dataAnalysisItemTabs.push(dataAnalysisItem);

                        function getParameterValues(dataAnalysisItemId) {
                            if (payload == undefined || payload.data == undefined || payload.data.Parameters == undefined)
                                return undefined;

                            var dataAnalysisItemsSettingsByItemId = payload.data.Parameters.DataAnalysisItemParameterSettingsByItemId;
                            if (dataAnalysisItemsSettingsByItemId == undefined)
                                return undefined;

                            var savedDataAnalysisItemSettings = dataAnalysisItemsSettingsByItemId[dataAnalysisItemId];
                            if (savedDataAnalysisItemSettings == undefined || savedDataAnalysisItemSettings.ParameterSettings == undefined)
                                return undefined;

                            return savedDataAnalysisItemSettings.ParameterSettings.ParameterValues;
                        }
                    }

                    var rootPromiseNode = {
                        promises: initialPromises
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    function getDataAnalysisItemsSettingsByItemId() {
                        var dataAnalysisItemsSettingsByItemId = {};

                        for (var i = 0; i < $scope.scopeModel.dataAnalysisItemTabs.length; i++) {
                            var currentDataAnalysisItemTab = $scope.scopeModel.dataAnalysisItemTabs[i];
                            var parameterValues = {};
                            if (currentDataAnalysisItemTab.parameterEditorRuntimeAPI != undefined)
                                currentDataAnalysisItemTab.parameterEditorRuntimeAPI.setData(parameterValues);

                            dataAnalysisItemsSettingsByItemId[currentDataAnalysisItemTab.dataAnalysisItemId] = {
                                DataAnalysisItemDefinitionId: currentDataAnalysisItemTab.dataAnalysisItemId,
                                ParameterSettings: { ParameterValues: parameterValues }
                            };
                        }

                        return dataAnalysisItemsSettingsByItemId;
                    }

                    return {
                        $type: "Vanrise.Analytic.Entities.DataAnalysisSettings, Vanrise.Analytic.Entities",
                        Parameters: { DataAnalysisItemParameterSettingsByItemId: getDataAnalysisItemsSettingsByItemId() }
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticDataanalysisSettingsEditor', DataAnalysisSettingsEditorDirective);

})(app);