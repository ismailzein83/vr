'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGrideditorviewDefinition', ['VRUIUtilsService', 'UtilsService','VR_GenericData_DataRecordFieldAPIService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new gridEditorViewTypeListTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeDefinition/Templates/GridEditorViewTypeDefinitionTemplate.html';
            }
        };

        function gridEditorViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            var editorDefinitionAPI;
            var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context = {};
            var settings;
            var dataRecordTypeFields = [];

            function initializeController() {

                $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
                    editorDefinitionAPI = api;
                    editorDefinitionReadyPromiseDeferred.resolve();
                };
                defineAPI();

            }
            function loadEditorDefinitionDirective() {
                var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                editorDefinitionReadyPromiseDeferred.promise.then(function () {
                    var editorPayload = {
                        settings: settings,
                        context: context
                    };
                    VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, editorPayload, loadEditorDefinitionDirectivePromiseDeferred);
                });
                return loadEditorDefinitionDirectivePromiseDeferred.promise;
            }

            function getDataRecordFieldsInfo(dataRecordTypeId) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId, null).then(function (response) {
                    dataRecordTypeFields.length = 0;
                    if (response != undefined)
                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            dataRecordTypeFields.push(currentField.Entity);
                        }
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    settings = payload.settings;
                    if (settings != undefined)
                        settings = settings.Settings;
                    var dataRecordTypeId = payload.dataRecordTypeId; 
                    var rootPromiseNode = {
                        promises: [getDataRecordFieldsInfo(dataRecordTypeId)]
                    };
                   
                    rootPromiseNode.getChildNode = function () {
                        context.getRecordTypeFields = function () {
                            var data = [];
                            for (var i = 0; i < dataRecordTypeFields.length; i++) {
                                data.push(dataRecordTypeFields[i]);
                            }
                            return data;
                        };
                        context.getDataRecordTypeId = function () {
                            return dataRecordTypeId;
                        };
                        context.getFieldType = function (fieldName) {
                            for (var i = 0; i < dataRecordTypeFields.length; i++) {
                                var field = dataRecordTypeFields[i];
                                if (field.Name == fieldName)
                                    return field.Type;
                            }
                        };
                        return { promises: [loadEditorDefinitionDirective()] };
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.GridEditorViewListRecordRuntimeViewType, Vanrise.GenericData.MainExtensions",
                        Settings: editorDefinitionAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);