'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistRuntimeeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                $scope.scopeModel = {};
                var ctor = new dataRecordTypeListCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'runtimeEditorCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function dataRecordTypeListCtor(ctrl, $scope, $attrs) {

            var fieldType;
            var fieldTitle;
            var fieldName;
            var fieldWidth;
            var fieldViewSettings;

            var runtimeEditorDirectiveAPI;
            var runtimeEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRuntimeEditorDirectiveReady = function (api) {
                    runtimeEditorDirectiveAPI = api;
                    runtimeEditorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fieldValue;

                    var promises = [];
                    var rootPromiseNode = {
                        promises: promises
                    };

                    if (payload != undefined) {
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        fieldTitle = payload.fieldTitle;
                        fieldName = payload.fieldName;
                        fieldWidth = payload.fieldWidth;
                        fieldViewSettings = payload.fieldViewSettings;
                        var readOnly = payload.readOnly;
                        if (fieldViewSettings != undefined) {
                            $scope.scopeModel.runtimeEditor = fieldViewSettings.RuntimeEditor;
                            var runtimeEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(runtimeEditorLoadPromiseDeferred.promise);

                            runtimeEditorReadyPromiseDeferred.promise.then(function () {
                                VRUIUtilsService.callDirectiveLoad(runtimeEditorDirectiveAPI, {
                                    fieldTitle: fieldTitle,
                                    fieldValue: fieldValue,
                                    dataRecordTypeId: fieldType.DataRecordTypeId,
                                    definitionSettings: fieldViewSettings,
                                    fieldType: fieldType,
                                    readOnly: readOnly,
                                    fieldWidth: fieldWidth
                                }, runtimeEditorLoadPromiseDeferred);
                            });
                        }
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);

                };
                api.getData = function () {
                    return runtimeEditorDirectiveAPI != undefined ? runtimeEditorDirectiveAPI.getData() : undefined;
                };
                api.setOnlyViewMode = function () {
                    runtimeEditorReadyPromiseDeferred.promise.then(function () {
                        runtimeEditorDirectiveAPI.setOnlyViewMode();
                    });
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    if (fieldValuesByNames == undefined || !(fieldName in fieldValuesByNames))
                        return;

                    var setFieldValuesDeferred = UtilsService.createPromiseDeferred();

                    var payload = {
                        fieldTitle: fieldTitle,
                        fieldValue: fieldValuesByNames[fieldName],
                        dataRecordTypeId: fieldType.DataRecordTypeId,
                        definitionSettings: fieldViewSettings,
                        fieldType: fieldType
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isDirectiveLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, runtimeEditorDirectiveAPI, payload, setLoader, undefined).then(function () {
                        setFieldValuesDeferred.resolve();
                    });

                    return setFieldValuesDeferred.promise;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }

        function getDirectiveTemplate(attrs) {
            return '<span vr-loader="scopeModel.isDirectiveLoading">'
                + '<vr-directivewrapper directive="scopeModel.runtimeEditor" on-ready="scopeModel.onRuntimeEditorDirectiveReady" normal-col-num="{{runtimeEditorCtrl.normalColNum}}" isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>'
                + '</span>';
        }

        return directiveDefinitionObject;
    }]);

