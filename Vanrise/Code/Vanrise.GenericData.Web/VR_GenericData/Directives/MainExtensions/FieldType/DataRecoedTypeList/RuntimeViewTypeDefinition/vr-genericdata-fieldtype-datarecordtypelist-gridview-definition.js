'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistGridviewDefinition', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeDefinition/Templates/GridViewTypeDefinitionTemplate.html';
            }
        };

        function gridViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel.availableFields = [];
                $scope.scopeModel.selectedFields = [];

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectField = function (field) {
                    var dataItem = {
                        entity: { FieldName: field.Name},
                    };

                    dataItem.onColumnSettingDirectiveReady = function (api) {
                        dataItem.columnSettingsDirectiveAPI = api;
                        var setLoader = function (value) { dataItem.isColumnSettingDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, undefined, setLoader);
                    };
                    $scope.scopeModel.availableFields.push(dataItem);
                };

                $scope.scopeModel.onDeSelectField = function (field) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.availableFields, field.Name, "entity.FieldName");
                    if (index != -1) {
                        $scope.scopeModel.availableFields.splice(index, 1);
                    }
                };

                $scope.scopeModel.onRemoveField = function (item) {
                    var index = $scope.scopeModel.availableFields.indexOf(item);
                    var selectedFieldIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFields, item.entity.FieldName, "Name");

                    if (index != -1) {
                        $scope.scopeModel.availableFields.splice(index, 1);
                    }

                    if (selectedFieldIndex != -1) {
                        $scope.scopeModel.selectedFields.splice(selectedFieldIndex, 1);
                    }
                };
                $scope.scopeModel.deselectAllFields = function () {
                    $scope.scopeModel.availableFields.length = 0;
                };
                defineAPI();

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var recordTypeFields=[];
                    if (payload != undefined) {
                        var dataRecordTypeId = payload.fieldType != undefined ? payload.fieldType.DataRecordTypeId : payload.dataRecordTypeId;
                        var settings = payload.settings;
                        var fieldsPayload = {
                            dataRecordTypeId: dataRecordTypeId,
                        };
                        if (settings != undefined) {
                            $scope.scopeModel.hideAddButton = settings.HideAddButton;
                            $scope.scopeModel.hideSection = settings.HideSection;
                            $scope.scopeModel.enableDraggableRow = settings.EnableDraggableRow;
                            $scope.scopeModel.hideRemoveIcon = settings.HideRemoveIcon;
                            var availableFields = settings.AvailableFields;
                            var selectedIds = [];
                            if (availableFields != undefined && availableFields.length > 0) {
                                for (var i = 0; i < availableFields.length; i++) {
                                    var field = availableFields[i];
                                    selectedIds.push(field.FieldName);
                                    recordTypeFields.push(field);
                                }
                            }
                            fieldsPayload.selectedIds = selectedIds;
                        }
                        if (dataRecordTypeId != undefined) {
                            var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                            dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                                VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                            });
                            promises.push(loadDataRecordTypeFieldsSelectorPromiseDeferred.promise);
                           
                        }
                    }
                    function prepareDataItem(genericBEFieldObject) {

                        var dataItem = {
                            entity: genericBEFieldObject.payload
                        };

                        dataItem.onColumnSettingDirectiveReady = function (api) {
                            dataItem.columnSettingsDirectiveAPI = api;
                            genericBEFieldObject.columnSettingsReadyPromiseDeferred.resolve();
                        };

                        genericBEFieldObject.columnSettingsReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                data: genericBEFieldObject.payload != undefined && genericBEFieldObject.payload.GridColumnSettings != undefined ? genericBEFieldObject.payload.GridColumnSettings : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dataItem.columnSettingsDirectiveAPI, payload, genericBEFieldObject.columnSettingsLoadPromiseDeferred);
                        });
                        $scope.scopeModel.availableFields.push(dataItem);
                    }
                    return UtilsService.waitPromiseNode({
                        promises: promises,
                        getChildNode: function () {
                            loadGenericFields();

                            function loadGenericFields() {
                                if (recordTypeFields != undefined) {
                                    for (var i = 0; i < recordTypeFields.length; i++) {
                                        var selectedField = recordTypeFields[i];
                                        var genericBEFieldObject = {
                                            payload: selectedField,
                                            columnSettingsReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            columnSettingsLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                        promises.push(genericBEFieldObject.columnSettingsLoadPromiseDeferred.promise);
                                        prepareDataItem(genericBEFieldObject);
                                    }
                                }
                            }
                            return { promises: [] };
                        }
                    });
                };

                api.getData = function () {
                    var availableFields = [];

                    for (var i = 0; i < $scope.scopeModel.availableFields.length; i++) {
                        var selectedField = $scope.scopeModel.availableFields[i];
                        availableFields.push({
                            FieldName: selectedField.entity.FieldName,
                            IsRequired: selectedField.entity.IsRequired,
                            IsDisabled: selectedField.entity.IsDisabled,
                            ShowAsLabel: selectedField.entity.ShowAsLabel,
                            ReadOnly: selectedField.entity.ReadOnly,
                            GridColumnSettings: selectedField.columnSettingsDirectiveAPI.getData()
                        });
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.GridViewListRecordRuntimeViewType, Vanrise.GenericData.MainExtensions",
                        HideAddButton: $scope.scopeModel.hideAddButton,
                        HideSection: $scope.scopeModel.hideSection,
                        EnableDraggableRow: $scope.scopeModel.enableDraggableRow,
                        HideRemoveIcon: $scope.scopeModel.hideRemoveIcon,
                        AvailableFields: availableFields
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);