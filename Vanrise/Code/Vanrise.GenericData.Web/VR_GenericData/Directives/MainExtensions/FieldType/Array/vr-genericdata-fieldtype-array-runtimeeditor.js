(function (app) {

    'use strict';

    ArrayFieldTypeRuntimeEditorDirective.$inject = ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ArrayFieldTypeRuntimeEditorDirective(VR_GenericData_DataRecordFieldAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var arrayFieldTypeRuntimeEditor = new ArrayFieldTypeRuntimeEditor(ctrl, $scope, $attrs);
                arrayFieldTypeRuntimeEditor.initializeController();
            },
            controllerAs: 'runtimeEditorCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function ArrayFieldTypeRuntimeEditor(ctrl, $scope, $attrs) {

            var fieldTypeRuntimeEditorAPI;
            var fieldTypeRuntimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                ctrl.onFieldTypeRuntimeEditorReady = function (api) {
                    fieldTypeRuntimeEditorAPI = api;
                    fieldTypeRuntimeEditorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var fieldTitle;
                    var fieldType;
                    var fieldValue;

                    if (payload != undefined) {
                        fieldTitle = payload.fieldTitle;
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                    }

                    var promises = [];
                    var fieldTypeConfigs;

                    var getFieldTypeConfigsPromise = getFieldTypeConfigs();
                    promises.push(getFieldTypeConfigsPromise);

                    var fieldTypeRuntimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
                    promises.push(fieldTypeRuntimeEditorLoadDeferred.promise);

                    getFieldTypeConfigsPromise.then(function () {
                        var fieldTypeConfig = UtilsService.getItemByVal(fieldTypeConfigs, fieldType.FieldType.ConfigId, 'ExtensionConfigurationId');
                        ctrl.fieldTypeRuntimeEditorDirective = fieldTypeConfig.RuntimeEditor;

                        fieldTypeRuntimeEditorReadyDeferred.promise.then(function () {
                            var fieldValuePayload;
                            if (fieldValue != undefined) {
                                fieldValuePayload = (ctrl.selectionmode == 'dynamic') ? fieldValue.Values : fieldValue;
                            }
                            var fieldTypeRuntimeEditorPaylaod = {
                                fieldTitle: fieldTitle,
                                fieldType: fieldType.FieldType,
                                fieldValue: fieldValuePayload
                            };
                            VRUIUtilsService.callDirectiveLoad(fieldTypeRuntimeEditorAPI, fieldTypeRuntimeEditorPaylaod, fieldTypeRuntimeEditorLoadDeferred);
                        });
                    });

                    function getFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            fieldTypeConfigs = [];
                            for (var i = 0; i < response.length; i++) {
                                fieldTypeConfigs.push(response[i]);
                            }
                        });
                    }
                };

                api.getData = function () {
                    var retVal;
                    var data = fieldTypeRuntimeEditorAPI.getData();

                    if (ctrl.selectionmode == "dynamic") {
                        if (data != undefined && data.length > 0) {
                            retVal = {
                                $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                                Values: data
                            };
                        }
                    }
                    else {
                        if (data != undefined && data.length > 0) {
                            retVal = data;
                        }
                    }

                    return retVal;
                }

                if (ctrl.onReady != undefined) {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }

        function getDirectiveTemplate(attrs) {
            return '<vr-columns colnum="{{runtimeEditorCtrl.normalColNum * 4}}">'
                        + '<vr-directivewrapper directive="runtimeEditorCtrl.fieldTypeRuntimeEditorDirective" on-ready="runtimeEditorCtrl.onFieldTypeRuntimeEditorReady" selectionmode="multiple" normal-col-num="{{runtimeEditorCtrl.normalColNum}}" isrequired="runtimeEditorCtrl.isrequired" />'
                + '</vr-columns>';
        }
    }

    app.directive('vrGenericdataFieldtypeArrayRuntimeeditor', ArrayFieldTypeRuntimeEditorDirective);

})(app);