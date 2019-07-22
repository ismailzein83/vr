(function (app) {

    'use strict';

    CallRestAPIEditorRuntimeDirective.$inject = ['UtilsService', 'BaseAPIService', 'VRButtonTypeEnum', 'VR_GenericData_CallRestAPIHTTPMethodEnum'];

    function CallRestAPIEditorRuntimeDirective(UtilsService, BaseAPIService, VRButtonTypeEnum, VR_GenericData_CallRestAPIHTTPMethodEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CallRestAPIEditorRuntimeCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorExecuteActionSetting/GetBusinessEntity/Templates/GetBusinessEntityExecuteActionRuntimeTemplate.html'
        };

        function CallRestAPIEditorRuntimeCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var genericContext;
            var executeActionEditorContext;

            var apiAction;
            var httpMethodType;
            var inputItems = [];
            var outputItems = [];

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.isInputValid = function () {
                    if (executeActionEditorContext == undefined)
                        return null;

                    if (inputItems == undefined) {
                        executeActionEditorContext.setButton(false, undefined);
                        return null;
                    }

                    var fieldValues = genericContext.getFieldValues();
                    var requiredInputs = [];
                    for (var i = 0; i < inputItems.length; i++) {
                        var input = inputItems[i];
                        if (input.IsRequired && fieldValues[input.FieldName] == undefined) {
                            requiredInputs.push(input.PropertyName);
                        }
                    }

                    if (requiredInputs.length > 0) {
                        var title = "Required Fields: " + requiredInputs.join(", ");
                        executeActionEditorContext.setButton(true, title);
                        return null;
                    }

                    executeActionEditorContext.setButton(false,undefined);
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var definitionSettings;

                    if (payload != undefined) {
                        definitionSettings = payload.definitionSettings;
                        genericContext = payload.genericContext;
                        executeActionEditorContext = payload.executeActionEditorContext;
                    }

                    if (definitionSettings != undefined) {
                        apiAction = definitionSettings.APIAction;
                        httpMethodType = definitionSettings.HTTPMethodType;
                        inputItems = definitionSettings.InputItems;
                        outputItems = definitionSettings.OutputItems;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.execute = function () {
                    return callRestAPIAction();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function callRestAPIAction() {
                var callAPIActionDeferred = UtilsService.createPromiseDeferred();

                var getFieldValuesPromise = genericContext.getFieldValuesPromise();
                getFieldValuesPromise.then(function (currentFieldValues) {

                    var requiredInputs = [];

                    var inputFields;
                    if (inputItems != undefined) {
                        inputFields = {};
                        for (var i = 0; i < inputItems.length; i++) {
                            var input = inputItems[i];
                            if (input.FieldName in currentFieldValues)
                                inputFields[input.PropertyName] = currentFieldValues[input.FieldName];

                            if (input.IsRequired && currentFieldValues[input.FieldName] == undefined) {
                                requiredInputs.push(input.PropertyName);
                            }
                        }
                    }

                    if (requiredInputs.length == 0) {
                        switch (httpMethodType) {
                            case VR_GenericData_CallRestAPIHTTPMethodEnum.Get.value: {
                                BaseAPIService.get(apiAction, inputFields).then(function (response) {
                                    genericContext.setFieldValues(getOutputFields(response, currentFieldValues)).then(function () {
                                        callAPIActionDeferred.resolve();
                                    });
                                });
                                break;
                            }

                            case VR_GenericData_CallRestAPIHTTPMethodEnum.Post.value: {
                                BaseAPIService.post(apiAction, inputFields).then(function (response) {
                                    genericContext.setFieldValues(getOutputFields(response, currentFieldValues)).then(function () {
                                        callAPIActionDeferred.resolve();
                                    });
                                });
                                break;
                            }
                        }
                    }
                    else {
                        callAPIActionDeferred.resolve();
                    }
                });

                function getOutputFields(response, currentFieldValues) {
                    if (response && typeof (response) == "object") {
                        var outputFields = {};
                        if (outputItems != undefined) {
                            for (var i = 0; i < outputItems.length; i++) {
                                var output = outputItems[i];
                                if (output.PropertyName in response && currentFieldValues[output.FieldName] != response[output.PropertyName])
                                    outputFields[output.FieldName] = response[output.PropertyName];
                            }
                        }
                        return outputFields;
                    }

                    return undefined;
                }

                return callAPIActionDeferred.promise;
            }
        }
    }

    app.directive('vrGenericdataExecuteactioneditorsettingActionruntimeGetbusinessentityaction', CallRestAPIEditorRuntimeDirective);

})(app);