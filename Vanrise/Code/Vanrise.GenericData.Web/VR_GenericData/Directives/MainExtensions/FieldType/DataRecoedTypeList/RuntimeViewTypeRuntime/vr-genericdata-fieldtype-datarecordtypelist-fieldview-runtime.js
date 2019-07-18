'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistFieldviewRuntime', ['VRUIUtilsService', 'UtilsService','VR_GenericData_DataRecordFieldAPIService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired:"="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new fieldViewTypeListTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrlFieldView',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/FieldViewTypeRuntimeTemplate.html';
            }
        };

        function fieldViewTypeListTypeCtor(ctrl, $scope) {
            this.initializeController = initializeController;
            var runtimeEditorDirectiveAPI;
            var runtimeEditorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeId;
            var fieldName;
            var dataRecordTypeFields;
            var definitionSettings;
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
                    var promises = [];
                  
                    if (payload != undefined) {
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        fieldTitle = payload.fieldTitle;
                        definitionSettings = payload.definitionSettings;

                        fieldName = definitionSettings != undefined && definitionSettings.RecordField != undefined ? definitionSettings.RecordField.Name : undefined;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        promises.push(getDataRecordFields());

                    }

                    function getDataRecordFields() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                            dataRecordTypeFields = response;
                        });
                    }

                    var rootPromiseNode = {
                        promises: promises,
                        getChildNode: function () {
                            var childPromises = [];
                            var targetFieldType = UtilsService.getItemByVal(dataRecordTypeFields, fieldName, "Entity.Name");
                            $scope.scopeModel.runtimeEditor = targetFieldType != undefined && targetFieldType.Entity != undefined && targetFieldType.Entity.Type != undefined ? targetFieldType.Entity.Type.RuntimeEditor : undefined;
                            var runtimeEditorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            var fieldValues = [];
                            if (fieldValue != undefined && fieldValue.length > 0) {
                                for (var i = 0; i < fieldValue.length; i++) {
                                    fieldValues.push(fieldValue[i][fieldName]);
                                }
                            }

                            runtimeEditorReadyPromiseDeferred.promise.then(function () {
                                VRUIUtilsService.callDirectiveLoad(runtimeEditorDirectiveAPI, {
                                    fieldTitle: targetFieldType.Entity.Title,
                                    fieldValue: fieldValues,
                                    fieldType: targetFieldType.Entity.Type
                                }, runtimeEditorLoadPromiseDeferred);
                            });

                            childPromises.push(runtimeEditorLoadPromiseDeferred.promise);
                            return {
                                promises: childPromises
                            };
                        }
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var values = runtimeEditorDirectiveAPI.getData();
                    var returnedData = [];
                    if (values != undefined && values.length > 0) {
                        for (var i = 0; i < values.length; i++) {
                            var listData = {};
                            listData[fieldName] = values[i];
                            returnedData.push(listData);
                        }
                    }
                  
                    return returnedData;
                };
                api.setOnlyViewMode = function () {
                    UtilsService.setContextReadOnly($scope);
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }]);