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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/CallRestAPIOLD/Templates/CallRestAPIEditorRuntimeSettingTemplate.html'
        };

        function CallRestAPIEditorRuntimeCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var genericContext;

            var apiAction;
            var httpMethodType;
            var inputItems = [];
            var outputItems = [];

            var directiveId = UtilsService.guid();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isButtonDisabled = true;

                $scope.scopeModel.isInputValid = function () {

                    if (genericContext == undefined) {
                        return "genericContext is undefined";
                    }

                    if (genericContext.getFieldValues == undefined || typeof (genericContext.getFieldValues) != "function") {
                        return "getFieldValues function is not defined on genericContext.";
                    }

                    if (genericContext.setFieldValues == undefined || typeof (genericContext.setFieldValues) != "function") {
                        return "setFieldValues function is not defined on genericContext.";
                    }

                    if (inputItems == undefined) {
                        $scope.scopeModel.isButtonDisabled = false;
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
                        $scope.scopeModel.isButtonDisabled = true;
                        $scope.scopeModel.buttonTitle = "Required Fields: " + requiredInputs.join(", ");
                        return null;
                    }

                    $scope.scopeModel.isButtonDisabled = false;
                    return null;
                };

                $scope.scopeModel.callApi = function () {
                    return callAPIAction();
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
                    }

                    if (definitionSettings != undefined) {
                        $scope.scopeModel.buttonType = UtilsService.getEnumDescription(VRButtonTypeEnum, definitionSettings.VRButtonType);
                        $scope.scopeModel.callOnLoad = definitionSettings.CallOnLoad;
                        $scope.scopeModel.callOnValueChanged = definitionSettings.CallOnValueChanged;
                        apiAction = definitionSettings.APIAction;
                        httpMethodType = definitionSettings.HTTPMethodType;
                        inputItems = definitionSettings.InputItems;
                        outputItems = definitionSettings.OutputItems;
                    }

                    if ($scope.scopeModel.callOnLoad || $scope.scopeModel.callOnValueChanged) {
                        callAPIAction();
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    if ($scope.scopeModel.callOnValueChanged) {
                        callAPIAction();
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function callAPIAction() {
                var callAPIActionDeferred = UtilsService.createPromiseDeferred();

                genericContext.setLoader(directiveId,true);

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

                callAPIActionDeferred.promise.then(function () {
                    genericContext.setLoader(directiveId, false);
                });

                return callAPIActionDeferred.promise;
            }
        }
    }

    app.directive('vrGenericdataCallrestapieditorsettingRuntime', CallRestAPIEditorRuntimeDirective);

})(app);