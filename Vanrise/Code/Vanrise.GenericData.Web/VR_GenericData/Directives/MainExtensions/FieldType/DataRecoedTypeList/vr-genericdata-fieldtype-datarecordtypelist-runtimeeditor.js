'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistRuntimeeditor', ['UtilsService','VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
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
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function dataRecordTypeListCtor(ctrl, $scope, $attrs) {

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
                    var fieldType;
                    var fieldTitle;
                    var fieldViewSettings;
                    var promises = [];
                    var rootPromiseNode = {
                        promises: promises
                    };
                    if (payload != undefined) {
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        fieldTitle = payload.fieldTitle;
                        fieldViewSettings = payload.fieldViewSettings;
                        $scope.scopeModel.runtimeEditor = fieldViewSettings != undefined ? fieldViewSettings.RuntimeEditor : undefined;
                        var runtimeEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(runtimeEditorLoadPromiseDeferred.promise);

                        runtimeEditorReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(runtimeEditorDirectiveAPI, {
                                fieldTitle: fieldTitle,
                                fieldValue: fieldValue,
                                dataRecordTypeId: fieldType.DataRecordTypeId,
                                definitionSettings: fieldViewSettings,
                                fieldType: fieldType
                            }, runtimeEditorLoadPromiseDeferred);
                        });
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);

                };
                api.getData = function () {
                    return runtimeEditorDirectiveAPI != undefined ? runtimeEditorDirectiveAPI.getData() : undefined;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }

        function getDirectiveTemplate(attrs) {

            return '<vr-columns colnum="12">'
                + '<vr-directivewrapper directive="scopeModel.runtimeEditor" on-ready="scopeModel.onRuntimeEditorDirectiveReady" normal-col-num="{{runtimeEditorCtrl.normalColNum}}"   isrequired="true"></vr-directivewrapper>'
                + '</vr-columns>';
        }

        return directiveDefinitionObject;
    }]);

