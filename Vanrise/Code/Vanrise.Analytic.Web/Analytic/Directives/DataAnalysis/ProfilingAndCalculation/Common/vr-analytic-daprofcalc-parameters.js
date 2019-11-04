(function (app) {

    'use strict';

    DAProfCalcParametesDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService'];

    function DAProfCalcParametesDirective(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DAProfCalcParametesDirectiveCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/Common/Templates/DAProfCalcParameters.html'
        };

        function DAProfCalcParametesDirectiveCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var parametersRecordTypeFields;

            var parametersRecordTypeSelectorAPI;
            var parametersRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var globalParametersEditorDefinitionDirectiveAPI;
            var globalParametersEditorDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var overriddenParametersEditorDefinitionDirectiveAPI;
            var overriddenParametersEditorDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var parametersRecordTypeSelectionChangedDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isLoading = true;

                $scope.scopeModel.onParametersRecordTypeSelectorReady = function (api) {
                    parametersRecordTypeSelectorAPI = api;
                    parametersRecordTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onGlobalParametersEditorDefinitionDirectiveReady = function (api) {
                    globalParametersEditorDefinitionDirectiveAPI = api;
                    globalParametersEditorDefinitionDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onOverriddenParametersEditorDefinitionDirectiveReady = function (api) {
                    overriddenParametersEditorDefinitionDirectiveAPI = api;
                    overriddenParametersEditorDefinitionDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onParametersRecordTypeSelectionChanged = function (selectedRecordType) {
                    if (selectedRecordType == undefined)
                        return;

                    if (parametersRecordTypeSelectionChangedDeferred != undefined) {
                        parametersRecordTypeSelectionChangedDeferred.resolve();
                    } else {
                        $scope.scopeModel.isLoading = true;
                        var loadGlobalParametersEditorDefinitionDeferred = UtilsService.createPromiseDeferred();
                        var loadOverriddenParametersEditorDefinitionDeferred = UtilsService.createPromiseDeferred();

                        getParametersRecordTypeFields(selectedRecordType.DataRecordTypeId).then(function () {
                            var parametersEditorDefinitionDirectivePayload = {
                                context: getParametersEditorDefinitionContext()
                            };

                            VRUIUtilsService.callDirectiveLoad(globalParametersEditorDefinitionDirectiveAPI, parametersEditorDefinitionDirectivePayload, loadGlobalParametersEditorDefinitionDeferred);
                            VRUIUtilsService.callDirectiveLoad(overriddenParametersEditorDefinitionDirectiveAPI, parametersEditorDefinitionDirectivePayload, loadOverriddenParametersEditorDefinitionDeferred);
                        });

                        UtilsService.waitMultiplePromises([loadGlobalParametersEditorDefinitionDeferred.promise, loadOverriddenParametersEditorDefinitionDeferred.promise]).then(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }

                };

                $scope.scopeModel.validateParameters = function () {
                    var globalParametersEditorDefinitionSetting = globalParametersEditorDefinitionDirectiveAPI != undefined ? globalParametersEditorDefinitionDirectiveAPI.getData() : undefined;
                    var overriddenParametersEditorDefinitionSetting = overriddenParametersEditorDefinitionDirectiveAPI != undefined ? overriddenParametersEditorDefinitionDirectiveAPI.getData() : undefined;

                    if (globalParametersEditorDefinitionSetting == undefined && overriddenParametersEditorDefinitionSetting == undefined) {
                        return "You should have At Least one Parameters Editor Definition";
                    }

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];
                    var daProfCalcParameters;
                    var parametersRecordTypeId;
                    var globalParametersEditorDefinitionSetting;
                    var overriddenParametersEditorDefinitionSetting;

                    if (payload != undefined) {
                        daProfCalcParameters = payload.daProfCalcParameters;

                        if (daProfCalcParameters != undefined) {
                            parametersRecordTypeId = daProfCalcParameters.ParametersRecordTypeId;
                            globalParametersEditorDefinitionSetting = daProfCalcParameters.GlobalParametersEditorDefinitionSetting;
                            overriddenParametersEditorDefinitionSetting = daProfCalcParameters.OverriddenParametersEditorDefinitionSetting;
                        }
                    }

                    var loadRecordTypeSelectorPromise = getLoadRecordTypeSelectorPromise();
                    initialPromises.push(loadRecordTypeSelectorPromise);

                    if (parametersRecordTypeId != undefined) {
                        var getParametersRecordTypeFieldsPromise = getParametersRecordTypeFields(parametersRecordTypeId);
                        parametersRecordTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                        initialPromises.push(parametersRecordTypeSelectionChangedDeferred.promise);
                        initialPromises.push(getParametersRecordTypeFieldsPromise);
                    }

                    var promiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var promises2 = []
                            if (parametersRecordTypeId != undefined) {
                                if (globalParametersEditorDefinitionSetting != undefined) {
                                    var loadGlobalParametersEditorDefinitionPromise = getLoadGlobalParametersEditorDefinitionPromise();
                                    promises2.push(loadGlobalParametersEditorDefinitionPromise);
                                }

                                if (overriddenParametersEditorDefinitionSetting != undefined) {
                                    var loadOverriddenParametersEditorDefinitionPromise = getLoadOverriddenParametersEditorDefinitionPromise();
                                    promises2.push(loadOverriddenParametersEditorDefinitionPromise);
                                }
                            }

                            return {
                                promises: promises2
                            };
                        }
                    };

                    function getLoadRecordTypeSelectorPromise() {
                        var loadRecordTypeSelectorDeferred = UtilsService.createPromiseDeferred();

                        parametersRecordTypeSelectorReadyDeferred.promise.then(function () {
                            var recordTypeSelectorPayload = {
                                selectedIds: parametersRecordTypeId
                            };
                            VRUIUtilsService.callDirectiveLoad(parametersRecordTypeSelectorAPI, recordTypeSelectorPayload, loadRecordTypeSelectorDeferred);
                        });

                        return loadRecordTypeSelectorDeferred.promise;
                    }

                    function getLoadGlobalParametersEditorDefinitionPromise() {
                        var loadGlobalParametersEditorDefinitionDeferred = UtilsService.createPromiseDeferred();

                        globalParametersEditorDefinitionDirectiveReadyDeferred.promise.then(function () {
                            var globalParametersEditorDefinitionDirectivePayload = {
                                settings: globalParametersEditorDefinitionSetting,
                                context: getParametersEditorDefinitionContext()
                            };

                            VRUIUtilsService.callDirectiveLoad(globalParametersEditorDefinitionDirectiveAPI, globalParametersEditorDefinitionDirectivePayload, loadGlobalParametersEditorDefinitionDeferred);
                        });

                        return loadGlobalParametersEditorDefinitionDeferred.promise;
                    }

                    function getLoadOverriddenParametersEditorDefinitionPromise() {
                        var loadOverriddenParametersEditorDefinitionDeferred = UtilsService.createPromiseDeferred();

                        overriddenParametersEditorDefinitionDirectiveReadyDeferred.promise.then(function () {
                            var overriddenParametersEditorDefinitionDirectivePayload = {
                                settings: overriddenParametersEditorDefinitionSetting,
                                context: getParametersEditorDefinitionContext()
                            };

                            VRUIUtilsService.callDirectiveLoad(overriddenParametersEditorDefinitionDirectiveAPI, overriddenParametersEditorDefinitionDirectivePayload, loadOverriddenParametersEditorDefinitionDeferred);
                        });

                        return loadOverriddenParametersEditorDefinitionDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode(promiseNode).then(function () {
                        parametersRecordTypeSelectionChangedDeferred = undefined;
                        $scope.scopeModel.isLoading = false;
                    });
                };

                api.getData = function () {
                    var parametersRecordTypeId = parametersRecordTypeSelectorAPI != undefined ? parametersRecordTypeSelectorAPI.getSelectedIds() : undefined;
                    var globalParametersEditorDefinitionSetting = globalParametersEditorDefinitionDirectiveAPI != undefined ? globalParametersEditorDefinitionDirectiveAPI.getData() : undefined;
                    var overriddenParametersEditorDefinitionSetting = overriddenParametersEditorDefinitionDirectiveAPI != undefined ? overriddenParametersEditorDefinitionDirectiveAPI.getData() : undefined;

                    if (parametersRecordTypeId == undefined)
                        return undefined;

                    if (overriddenParametersEditorDefinitionSetting == undefined && globalParametersEditorDefinitionSetting == undefined)
                        return undefined;

                    var data = {
                        ParametersRecordTypeId: parametersRecordTypeId,
                        GlobalParametersEditorDefinitionSetting: globalParametersEditorDefinitionSetting,
                        OverriddenParametersEditorDefinitionSetting: overriddenParametersEditorDefinitionSetting
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function getParametersRecordTypeFields(recordTypeId) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(recordTypeId).then(function (response) {
                    parametersRecordTypeFields = [];

                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            parametersRecordTypeFields.push(currentField.Entity);
                        }
                    }
                });
            }

            function getParametersEditorDefinitionContext() {
                var context = {};

                context.getDataRecordTypeId = function () {
                    return parametersRecordTypeSelectorAPI.getSelectedIds();
                };

                context.getRecordTypeFields = function () {
                    var data = [];
                    for (var i = 0; i < parametersRecordTypeFields.length; i++) {
                        data.push(parametersRecordTypeFields[i]);
                    }
                    return data;
                };

                context.getFields = function () {
                    var dataFields = [];

                    for (var i = 0; i < parametersRecordTypeFields.length; i++) {
                        dataFields.push({
                            FieldName: parametersRecordTypeFields[i].Name,
                            FieldTitle: parametersRecordTypeFields[i].Title,
                            Type: parametersRecordTypeFields[i].Type
                        });
                    }
                    return dataFields;
                };

                context.getFieldType = function (fieldName) {
                    for (var i = 0; i < parametersRecordTypeFields.length; i++) {
                        var field = parametersRecordTypeFields[i];
                        if (field.Name == fieldName)
                            return field.Type;
                    }
                };

                return context;
            }
        }
    }

    app.directive('vrAnalyticDaprofcalcParameters', DAProfCalcParametesDirective);

})(app);