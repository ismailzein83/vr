(function (app) {

    'use strict';

    ExecuteActionEditorDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_CallRestAPIHTTPMethodEnum'];

    function ExecuteActionEditorDefinitionDirective(UtilsService, VRUIUtilsService, VR_GenericData_ExecuteActionHTTPMethodEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ExecuteActionEditorDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/ExecuteActionEditorDefinitionSettingTemplate.html'
        };

        function ExecuteActionEditorDefinitionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var callOnLoadSectionAPI;
            var callOnLoadSectionReadyPromiseDeferred= UtilsService.createPromiseDeferred();

            var callOnValueChangedSectionAPI;
            var callOnValueChangedSectionReadyPromiseDeferred  = UtilsService.createPromiseDeferred();

            var callOnButtonClickedSectionAPI;
            var callOnButtonClickedSectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var executeActionTypeDirectiveAPI;
            var executeActionTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCallOnLoadChangedSectionReady = function (api) {
                    callOnLoadSectionAPI = api;
                    callOnLoadSectionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCallOnValueChangedSectionReady = function (api) {
                    callOnValueChangedSectionAPI = api;
                    callOnValueChangedSectionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCallOnButtonClickedChangedSectionReady = function (api) {
                    callOnButtonClickedSectionAPI = api;
                    callOnButtonClickedSectionReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onExecuteActionTypeDirectiveReady = function (api) {
                    executeActionTypeDirectiveAPI = api;
                    executeActionTypeDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var context;

                    var callOnLoadSection;
                    var callOnValueChangedSection;
                    var callOnButtonClickedSection;
                    var executeActionType;

                    if (payload != undefined) {
                        context = payload.context;

                        var settings = payload.settings;
                        if (settings != undefined) {
                            callOnLoadSection = settings.CallOnLoadSection;
                            callOnValueChangedSection = settings.CallOnValueChangedSection;
                            callOnButtonClickedSection = settings.CallOnButtonClickedSection;
                            executeActionType = settings.ExecuteActionType;
                        }
                    }

                    var callOnLoadSectionLoadPromise = loadCallOnLoadSection();
                    promises.push(callOnLoadSectionLoadPromise);

                    var callOnValueChangedSectionLoadPromise = loadCallOnValueChangedSection();
                    promises.push(callOnValueChangedSectionLoadPromise);

                    var callOnButtonClickedSectionLoadPromise = loadCallOnButtonClickedSection();
                    promises.push(callOnButtonClickedSectionLoadPromise);

                    var executeActionTypeDirectiveLoadPromise = loadExecuteActionTypeDirective();
                    promises.push(executeActionTypeDirectiveLoadPromise);

                    function loadCallOnLoadSection() {
                        var callOnLoadSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        callOnLoadSectionReadyPromiseDeferred.promise.then(function () {
                            var callOnLoadPayload = {
                                context: context
                            };

                            if (callOnLoadSection != undefined) {
                                callOnLoadPayload.definitionSettings = callOnLoadSection;
                            }

                            VRUIUtilsService.callDirectiveLoad(callOnLoadSectionAPI, callOnLoadPayload, callOnLoadSectionLoadPromiseDeferred);
                        });

                        return callOnLoadSectionLoadPromiseDeferred.promise;
                    }

                    function loadCallOnValueChangedSection() {
                        var callOnValueChangedSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        callOnValueChangedSectionReadyPromiseDeferred.promise.then(function () {
                            var callOnValueChangedPayload = {
                                context: context
                            };

                            if (callOnValueChangedSection != undefined) {
                                callOnValueChangedPayload.definitionSettings = callOnValueChangedSection;
                            }

                            VRUIUtilsService.callDirectiveLoad(callOnValueChangedSectionAPI, callOnValueChangedPayload, callOnValueChangedSectionLoadPromiseDeferred);
                        });

                        return callOnValueChangedSectionLoadPromiseDeferred.promise;
                    }

                    function loadCallOnButtonClickedSection() {
                        var callOnButtonClickedSectionLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        callOnButtonClickedSectionReadyPromiseDeferred.promise.then(function () {
                            var callOnButtonClickedPayload = {
                                context: context
                            };

                            if (callOnButtonClickedSection != undefined) {
                                callOnButtonClickedPayload.definitionSettings = callOnButtonClickedSection;
                            }

                            VRUIUtilsService.callDirectiveLoad(callOnButtonClickedSectionAPI, callOnButtonClickedPayload, callOnButtonClickedSectionLoadPromiseDeferred);
                        });

                        return callOnButtonClickedSectionLoadPromiseDeferred.promise;
                    }

                    function loadExecuteActionTypeDirective() {
                        var executeActionTypeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        executeActionTypeDirectiveReadyPromiseDeferred.promise.then(function () {
                            var executeActionTypeDirectivePayload = {
                                context: context
                            };

                            if (executeActionType != undefined) {
                                executeActionTypeDirectivePayload.settings = executeActionType;
                            }

                            VRUIUtilsService.callDirectiveLoad(executeActionTypeDirectiveAPI, executeActionTypeDirectivePayload, executeActionTypeDirectiveLoadPromiseDeferred);
                        });
                        return executeActionTypeDirectiveLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var definitionData = {
                        $type: "Vanrise.GenericData.MainExtensions.ExecuteActionEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        CallOnLoadSection: callOnLoadSectionAPI.getData(),
                        CallOnValueChangedSection: callOnValueChangedSectionAPI.getData(),
                        CallOnButtonClickedSection: callOnButtonClickedSectionAPI.getData(),
                        ExecuteActionType: executeActionTypeDirectiveAPI.getData()
                    };

                    return definitionData;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataExecuteactioneditorsettingDefinition', ExecuteActionEditorDefinitionDirective);

})(app);