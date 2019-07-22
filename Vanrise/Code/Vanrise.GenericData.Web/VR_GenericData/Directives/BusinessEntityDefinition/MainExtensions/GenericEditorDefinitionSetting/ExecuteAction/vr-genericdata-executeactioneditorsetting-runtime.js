(function (app) {

    'use strict';

    ExecuteActionEditorRuntimeDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRButtonTypeEnum', 'VR_GenericData_GenericBusinessEntityService'];

    function ExecuteActionEditorRuntimeDirective(UtilsService, VRUIUtilsService, VRButtonTypeEnum, VR_GenericData_GenericBusinessEntityService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ExecuteActionEditorRuntimeCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/ExecuteAction/Templates/ExecuteActionEditorRuntimeSettingTemplate.html'
        };

        function ExecuteActionEditorRuntimeCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var executeActionTypeAPI;
            var executeActionTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var genericContext;

            var callOnLoad;
            var callOnValueChanged;
            var onFieldsChanged;
            var localFieldValuesByFieldName = {};

            var directiveId = UtilsService.guid();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isButtonDisabled = true;

                $scope.scopeModel.onExecuteActionTypeDirectiveReady = function (api) {
                    executeActionTypeAPI = api;
                    executeActionTypeDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onButtonClicked = function () {
                    return executeAction();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var definitionSettings;
                    var allFieldValuesByName;

                    if (payload != undefined) {
                        definitionSettings = payload.definitionSettings;
                        genericContext = payload.genericContext;
                        allFieldValuesByName = payload.allFieldValuesByName;
                    }

                    if (definitionSettings != undefined) {
                        loadOnLoadData(definitionSettings.CallOnLoadSection);
                        loadOnValueChangedData(definitionSettings.CallOnValueChangedSection);
                        loadOnButtonClickedData(definitionSettings.CallOnButtonClickedSection);

                        var executeActionTypeDirectiveLoadedPromise = loadExecuteActionTypeDirective(definitionSettings.ExecuteActionType);
                        promises.push(executeActionTypeDirectiveLoadedPromise);
                    }

                    function loadOnLoadData(callOnLoadDefinition) {
                        if (callOnLoadDefinition == undefined || !callOnLoadDefinition.CallOnLoad)
                            return;

                        callOnLoad = callOnLoadDefinition.CallOnLoad;
                    }

                    function loadOnValueChangedData(callOnValueChangedDefinition) {
                        if (callOnValueChangedDefinition == undefined || !callOnValueChangedDefinition.CallOnValueChanged)
                            return;

                        callOnValueChanged = callOnValueChangedDefinition.CallOnValueChanged;
                        onFieldsChanged = callOnValueChangedDefinition.OnFieldsChanged;

                        if (onFieldsChanged != undefined && allFieldValuesByName != undefined)
                            evaluateLocalFieldsChanged(allFieldValuesByName);
                    }

                    function loadOnButtonClickedData(callOnButtonClickedDefinition) {
                        if (callOnButtonClickedDefinition == undefined || !callOnButtonClickedDefinition.CallOnButtonClicked)
                            return;

                        $scope.scopeModel.callOnButtonClicked = callOnButtonClickedDefinition.CallOnButtonClicked;
                        $scope.scopeModel.buttonType = UtilsService.getEnumDescription(VRButtonTypeEnum, callOnButtonClickedDefinition.VRButtonType);
                    }

                    function loadExecuteActionTypeDirective(executeActionType) {
                        $scope.scopeModel.actionRuntimeEditor = executeActionType.ActionRuntimeEditor;

                        var executeActionTypeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        executeActionTypeDirectiveReadyPromiseDeferred.promise.then(function () {
                            var executeActionTypeDirectivePayload = {
                                executeActionEditorContext: getExecuteActionEditorContext(),
                                genericContext: genericContext
                            };

                            if (executeActionType != undefined) {
                                executeActionTypeDirectivePayload.definitionSettings = executeActionType;
                            }

                            VRUIUtilsService.callDirectiveLoad(executeActionTypeAPI, executeActionTypeDirectivePayload, executeActionTypeDirectiveLoadPromiseDeferred);
                        });

                        return executeActionTypeDirectiveLoadPromiseDeferred.promise;
                    }

                    var directiveLoadPromise = UtilsService.waitMultiplePromises(promises);

                    if (callOnLoad) {
                        directiveLoadPromise.then(function () {
                            executeAction();
                        });
                    }

                    return directiveLoadPromise;
                };

                api.setData = function (dicData) {
                };

                api.onFieldValueChanged = function (changedFields) {
                    if (onFieldsChanged == undefined)
                        return;

                    if (callOnValueChanged && evaluateLocalFieldsChanged(changedFields)) {
                        return executeAction();
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function evaluateLocalFieldsChanged(changedFields) {
                var changedLocalFields = {};

                for (var i = 0; i < onFieldsChanged.length; i++) {
                    var currentFieldName = onFieldsChanged[i].FieldName;
                    changedLocalFields[currentFieldName] = changedFields[currentFieldName];
                }

                return VR_GenericData_GenericBusinessEntityService.tryUpdateAllFieldValuesByFieldNames(changedLocalFields, localFieldValuesByFieldName);
            }

            function getExecuteActionEditorContext() {
                return {
                    setButton: function (isDisabled, title) {
                        $scope.scopeModel.isButtonDisabled = isDisabled;
                        $scope.scopeModel.buttonTitle = title;
                    }
                };
            }

            function executeAction() {
                if (executeActionTypeAPI == undefined)
                    return UtilsService.waitMultiplePromises([]);

                genericContext.setLoader(directiveId, true);
                var executeActionPromise = executeActionTypeAPI.execute();
                executeActionPromise.then(function () {
                    genericContext.setLoader(directiveId, false);
                });

                return executeActionPromise;
            }
        }
    }

    app.directive('vrGenericdataExecuteactioneditorsettingRuntime', ExecuteActionEditorRuntimeDirective);

})(app);