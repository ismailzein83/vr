'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGrideditorviewRuntime', ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_DataRecordTypeService', 'VR_GenericData_GenericBusinessEntityAPIService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_DataRecordTypeService, VR_GenericData_GenericBusinessEntityAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new gridViewTypeListTypeCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/GridEditorViewTypeRuntimeTemplate.html';
            }
        };

        function gridViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            ctrl.datasource = [];
            $scope.scopeModel.columns = [];
            var dataRecordTypeId;
            var definitionSettings;
            var gridAPI;
            function initializeController() {
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onDataRecordTypeRowAdded = function () {
                    var onRowAdded = function (row) {
                        ctrl.datasource.push(row);
                    };
                    VR_GenericData_DataRecordTypeService.addListDataRecordTypeRow(dataRecordTypeId, definitionSettings, onRowAdded, $scope.scopeModel.fieldTitle);
                };
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {

                    dataRecordTypeId = payload.dataRecordTypeId;
                    definitionSettings = payload.definitionSettings != undefined ? payload.definitionSettings.Settings : undefined;
                    var fieldsValues = payload.fieldValue;
                    $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    var input = {
                        GenericEditorDefinitionSetting: definitionSettings,
                        DataRecordTypeId: dataRecordTypeId
                    };
                    var getColumnsInfoPromise = VR_GenericData_GenericBusinessEntityAPIService.GetGenericEditorColumnsInfo(input).then(function (response) {
                        if (response != undefined && response.length > 0) {
                            for (var i = 0; i < response.length; i++) {
                                var column = response[i];
                                column.Field = "Entity." + column.Field;
                                $scope.scopeModel.columns.push(column);
                            }
                        }
                    });
                    var rootPromiseNode = {
                        promises: [getColumnsInfoPromise],
                        getChildNode: function () {
                            if (fieldsValues != undefined) {
                                for (var i = 0; i < fieldsValues.length; i++) {
                                    ctrl.datasource.push({ Entity: fieldsValues[i] });
                                } 
                            }
                            return { promises: [] };
                        }
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var returnedData = []; 
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        returnedData.push(ctrl.datasource[i].Entity);
                    } 
                    return returnedData;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                    {
                        name: "Edit",
                        clicked: editRow,
                    }];

                $scope.gridMenuActions = function () {
                    return defaultMenuActions;
                };
            }

            function editRow(row) {
                var onRowUpdated = function (updatedRow) {
                    var index = ctrl.datasource.indexOf(row);
                    ctrl.datasource[index] = updatedRow;
                };
                VR_GenericData_DataRecordTypeService.editListDataRecordTypeRow(row, dataRecordTypeId, definitionSettings, onRowUpdated, $scope.scopeModel.fieldTitle);
            }
            this.initializeController = initializeController;
        }
    }]);