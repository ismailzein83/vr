"use strict";

app.directive("vrGenericdataGenericbeColumndefinitionGrid", ["UtilsService", "VRUIUtilsService","VRLocalizationService",
    function (UtilsService, VRUIUtilsService, VRLocalizationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ColumnDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/ColumnDefinitionGridTemplate.html"

        };

        function ColumnDefinitionGrid($scope, ctrl, $attrs) {

            var context;
            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeFields = [];
            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                $scope.scopeModel = {};
                ctrl.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one column.";

                     return null;
                };
                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFieldSelected = function (item) {
                    addDataRecordFieldToColumns(item);
                };

                $scope.scopeModel.onFieldDeselected = function (item) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, item.Name, "entity.FieldName");
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.removeField = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    var selectedFieldIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFields, dataItem.entity.FieldName, "Name");
                    $scope.scopeModel.selectedFields.splice(selectedFieldIndex, 1);
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.deselectAllFields = function () {
                    ctrl.datasource.length = 0;
                };
                $scope.scopeModel.selectAllFields = function () {
                    $scope.scopeModel.isLoading = true; 
                    for (var i = 0; i < dataRecordTypeFields.length; i++) {
                        var field = dataRecordTypeFields[i];
                        if (ctrl.datasource.length == 0) {
                            $scope.scopeModel.selectedFields.push(field);
                            addDataRecordFieldToColumns(field);
                        }
                        else if (ctrl.datasource.findIndex(x => x.entity.FieldName === field.Name) == -1) {
                            $scope.scopeModel.selectedFields.push(field);
                            addDataRecordFieldToColumns(field);
                        }
                    }
                    $scope.scopeModel.isLoading = false;
                };
                $scope.scopeModel.showAddAllFields = function () {
                    if (ctrl.datasource.length == 0)
                        return true;
                    else if (dataRecordTypeFields != undefined && ctrl.datasource.length < dataRecordTypeFields.length)
                        return true;
                    else
                        return false;
                };
                defineAPI();
            }
            function addDataRecordFieldToColumns(item) {
                var dataItem = {
                    entity: { FieldName: item.Name, FieldTitle: item.Title },
                };
                dataItem.onColumnSettingDirectiveReady = function (api) {
                    dataItem.columnSettingsDirectiveAPI = api;
                    var setLoader = function (value) { dataItem.isColumnSettingDirectiveloading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.columnSettingsDirectiveAPI, undefined, setLoader);
                };
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    var setLoader = function (value) { dataItem.isFieldTextResourceSelectorLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                };
                ctrl.datasource.push(dataItem);
            }

            function prepareDataItem(columnObject) {

                var dataItem = {
                    entity: {
                        FieldName: columnObject.payload.FieldName,
                        FieldTitle: columnObject.payload.FieldTitle
                    },
                    oldTextResourceKey: columnObject.payload.TextResourceKey
                }; 
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    columnObject.textResourceReadyPromiseDeferred.resolve();
                };
                dataItem.onColumnSettingDirectiveReady = function (api) {
                    dataItem.columnSettingsDirectiveAPI = api;
                    columnObject.columnSettingsReadyPromiseDeferred.resolve();
                };

                columnObject.textResourceReadyPromiseDeferred.promise.then(function () {
                    var textResourcePayload = { selectedValue: columnObject.payload.TextResourceKey };
                    VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, columnObject.textResourceLoadPromiseDeferred);
                });

                columnObject.columnSettingsReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        data: columnObject.payload != undefined ? columnObject.payload.GridColumnSettings : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.columnSettingsDirectiveAPI, payload, columnObject.columnSettingsLoadPromiseDeferred);
                });
                ctrl.datasource.push(dataItem);
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var columns;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        columns = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            columns.push({
                                FieldName: currentItem.entity.FieldName,
                                FieldTitle: currentItem.entity.FieldTitle,
                                GridColumnSettings: currentItem.columnSettingsDirectiveAPI != undefined ? currentItem.columnSettingsDirectiveAPI.getData() : undefined,
                                TextResourceKey: currentItem.textResourceSeletorAPI != undefined ? currentItem.textResourceSeletorAPI.getSelectedValues() : currentItem.oldTextResourceKey
                            });
                        }
                    }
                    return columns;
                };

                api.load = function (payload) {
                    var rootPromiseNode;
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        dataRecordTypeFields = context.getRecordTypeFields();
                        api.clearDataSource();
                        var selectedIds = [];
                        if (payload.columnDefinitions != undefined && payload.columnDefinitions.length > 0) {
                            for (var i = 0; i < payload.columnDefinitions.length; i++) {
                                selectedIds.push(payload.columnDefinitions[i].FieldName);
                            }
                        }
                        if (context != undefined && context.getDataRecordTypeId() != undefined) {
                            var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                            dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                                var typeFieldsPayload = {
                                    dataRecordTypeId:context.getDataRecordTypeId(),
                                    selectedIds: selectedIds
                                };

                                VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                            });
                            promises.push(loadDataRecordTypeFieldsSelectorPromiseDeferred.promise);

                        }
                         rootPromiseNode = {
                             promises: promises,
                            getChildNode: function () {
                                var childPromises = [];
                                if (payload.columnDefinitions != undefined) {
                                    for (var i = 0; i < payload.columnDefinitions.length; i++) {
                                        var item = payload.columnDefinitions[i];

                                        var columnObject = {
                                            payload: item,
                                            textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            columnSettingsReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            columnSettingsLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                        if ($scope.scopeModel.isLocalizationEnabled)
                                            childPromises.push(columnObject.textResourceLoadPromiseDeferred.promise);
                                        childPromises.push(columnObject.columnSettingsLoadPromiseDeferred.promise);
                                        prepareDataItem(columnObject);
                                    }
                                }
                                return { promises: childPromises };
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };


                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }






        }

        return directiveDefinitionObject;

    }
]);