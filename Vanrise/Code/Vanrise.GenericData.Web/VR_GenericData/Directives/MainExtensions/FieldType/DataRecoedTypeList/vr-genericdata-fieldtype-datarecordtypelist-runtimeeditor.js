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

                    if (payload != undefined) {
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        fieldTitle = payload.fieldTitle;
                        $scope.scopeModel.runtimeEditor = fieldType.RuntimeViewType.RuntimeEditor;

                        var runtimeEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        runtimeEditorReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(runtimeEditorDirectiveAPI, {
                                fieldTitle: fieldTitle,
                                fieldValue: fieldValue,
                                dataRecordTypeId: fieldType.DataRecordTypeId
                            }, runtimeEditorLoadPromiseDeferred);
                        });
                        return runtimeEditorLoadPromiseDeferred.promise;
                    }

                };
                api.getData = function () {
                    return runtimeEditorDirectiveAPI.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }

        function getDirectiveTemplate(attrs) {

            return '<vr-columns colnum="12">'
                + '<vr-directivewrapper directive="scopeModel.runtimeEditor" on-ready="scopeModel.onRuntimeEditorDirectiveReady"   isrequired="true"></vr-directivewrapper>'
                + '</vr-columns>';
        }

        return directiveDefinitionObject;
    }]);

