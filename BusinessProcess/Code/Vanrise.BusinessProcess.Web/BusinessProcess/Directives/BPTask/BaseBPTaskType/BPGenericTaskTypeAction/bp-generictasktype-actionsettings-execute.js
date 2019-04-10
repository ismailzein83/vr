"use strict";

app.directive("businessprocessGenerictasktypeActionsettingsExecute", ["UtilsService","VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var generictasktypeActionExecute = new GenerictasktypeActionExecute($scope, ctrl, $attrs);
                generictasktypeActionExecute.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/BPTask/BaseBPTaskType/BPGenericTaskTypeAction/Templates/ExecuteBPGenericTaskTypeActionTemplate.html'
        };

        function GenerictasktypeActionExecute($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataRecordFields = [];
                $scope.scopeModel.defaultFieldValues = [];


                $scope.scopeModel.validateDefaultFieldValues = function () {
                    if ($scope.scopeModel.defaultFieldValues != undefined && $scope.scopeModel.defaultFieldValues.length > 0) {
                        var isValid = true;
                        for (var i = 0; i < $scope.scopeModel.defaultFieldValues.length; i++) {
                            var defaultValue = $scope.scopeModel.defaultFieldValues[i];
                            var elementFound = false;
                            for (var j = i + 1; j < $scope.scopeModel.defaultFieldValues.length; j++) {
                                if (defaultValue.FieldName == $scope.scopeModel.defaultFieldValues[j].FieldName) {
                                    elementFound = true;
                                    break;
                                }
                            }
                            if (elementFound) {
                                isValid = false;
                                break;
                            }
                        }
                        if (isValid)
                            return null;
                        else
                            return 'One or more default values are mapped to the same field.';
                    }
                    return null;
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };


                $scope.scopeModel.addDefaultFieldValue = function () {
                    if ($scope.scopeModel.selectedDefaultFieldValue != undefined) {
                        var dataItem = {
                            FieldName: $scope.scopeModel.selectedDefaultFieldValue.FieldName,
                            RuntimeEditor: $scope.scopeModel.selectedDefaultFieldValue.Type != undefined ? $scope.scopeModel.selectedDefaultFieldValue.Type.RuntimeEditor : undefined,
                            onEditorRuntimeDirectiveReady: function (api) {
                                dataItem.directiveAPI = api;
                                var setLoader = function (value) { dataItem.isLoadingDirective = value; };
                                var directivePayload = {
                                    fieldType: $scope.scopeModel.selectedDefaultFieldValue.Type,
                                    fieldTitle: $scope.scopeModel.selectedDefaultFieldValue.FieldTitle
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, directivePayload, setLoader);
                                $scope.scopeModel.selectedDefaultFieldValue = undefined;
                            }
                        };
                        $scope.scopeModel.defaultFieldValues.push(dataItem);
                    }
                };

                $scope.scopeModel.removeDefaultFieldValue = function (defaultFieldValue) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.defaultFieldValues, defaultFieldValue.FieldName, 'FieldName');
                    if (index > -1)
                        $scope.scopeModel.defaultFieldValues.splice(index, 1);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    function addDefaultFieldValue(index) {
                        var defaultFieldValue = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, payload.settings.DefaultFieldValues[index].FieldName, 'FieldName');
                        var fieldValue = payload.settings.DefaultFieldValues[index].FieldValue;
                        if (defaultFieldValue != undefined) {
                            var dataItem = {
                                FieldName: defaultFieldValue.FieldName,
                                RuntimeEditor: defaultFieldValue.Type != undefined ? defaultFieldValue.Type.RuntimeEditor : undefined,
                                directiveLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                onEditorRuntimeDirectiveReady: function (api) {
                                    dataItem.directiveAPI = api;
                                    var directivePayload = {
                                        fieldValue: fieldValue,
                                        fieldType: defaultFieldValue.Type,
                                        fieldTitle: defaultFieldValue.FieldTitle
                                    };
                                    VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, directivePayload, dataItem.directiveLoadPromiseDeferred);
                                }
                            };
                            promises.push(dataItem.directiveLoadPromiseDeferred.promise);
                        }
            
                        $scope.scopeModel.defaultFieldValues.push(dataItem);
                    }


                    if (payload != undefined) {
                        context = payload.context;
                        if (context != undefined && context.getFields != undefined && typeof (context.getFields) == 'function') {
                            $scope.scopeModel.dataRecordFields = context.getFields();
                            if ($scope.scopeModel.dataRecordFields != undefined && $scope.scopeModel.dataRecordFields.length > 0 && payload.settings!=undefined) {
                                $scope.scopeModel.selectedNotesMappingField = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, payload.settings.NotesMappingField, 'FieldName');
                                $scope.scopeModel.selectedDecisionMappingField = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, payload.settings.DecisionMappingField, 'FieldName');
                                if (payload.settings.DefaultFieldValues != undefined && payload.settings.DefaultFieldValues.length > 0) {
                                    for (var i = 0; i < payload.settings.DefaultFieldValues.length; i++) {
                                        addDefaultFieldValue(i);
                                    }
                                }
                            }
                        }
                    }

                    var rootPromiseNode = {
                        promises: promises
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };
                function getDefaultFieldValues() {
                    var defaultFieldValues = [];
                    if ($scope.scopeModel.defaultFieldValues!=undefined && $scope.scopeModel.defaultFieldValues.length>0) {
                        for (var i = 0; i < $scope.scopeModel.defaultFieldValues.length; i++) {
                            var defaultFieldValueItem = $scope.scopeModel.defaultFieldValues[i];
                            defaultFieldValues.push({
                                FieldName: defaultFieldValueItem.FieldName,
                                FieldValue: defaultFieldValueItem.directiveAPI != undefined ? defaultFieldValueItem.directiveAPI.getData() : undefined
                            });
                        }
                    }
                    return defaultFieldValues;
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.ExecuteBPGenericTaskTypeAction, Vanrise.BusinessProcess.MainExtensions",
                        DefaultFieldValues: getDefaultFieldValues(),
                        DecisionMappingField: $scope.scopeModel.selectedDecisionMappingField != undefined ? $scope.scopeModel.selectedDecisionMappingField.FieldName : undefined,
                        NotesMappingField: $scope.scopeModel.selectedNotesMappingField != undefined ? $scope.scopeModel.selectedNotesMappingField.FieldName : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);