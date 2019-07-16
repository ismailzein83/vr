//(function (app) {

//    'use strict';

//    RowsContainerEditorFieldsDefinitionDirective.$inject = ['VRNotificationService', 'VR_GenericData_GenericBEDefinitionService', 'UtilsService'];

//    function RowsContainerEditorFieldsDefinitionDirective(VRNotificationService, VR_GenericData_GenericBEDefinitionService, UtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                normalColNum: '@'
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new RowsContainerEditorDefinitionCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/RowsContainerEditorDefinitionSettingTemplate.html'
//        };

//        function RowsContainerEditorDefinitionCtor($scope, ctrl) {
//            this.initializeController = initializeController;

//            var context;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.datasource = [];

//                $scope.scopeModel.addField = function () {

//                    var dataItem= prepareFieldData(undefined, undefined);
//                    gridAPI.expandRow(dataItem);
//                    ctrl.datasource.push(dataItem);
//                };

//                $scope.scopeModel.deleteFunction = function (rowContainerObj) {
//                    VRNotificationService.showConfirmation().then(function (response) {
//                        if (response) {
//                            var index = $scope.scopeModel.datasource.indexOf(rowContainerObj);
//                            if (index != -1)
//                                $scope.scopeModel.datasource.splice(index, 1);
//                        }
//                    });
//                };

//                $scope.scopeModel.disableAddRowContainer = function () {
//                    if (context == undefined)
//                        return true;

//                    return false;
//                };

//                $scope.scopeModel.isValid = function () {
//                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
//                        return "You Should add at least one row.";

//                    return null;
//                };

//                defineAPI();

//                defineMenuActions();
//            }
//            function prepareFieldData(field, fieldEditorDefinitionLoadPromiseDeferred) {
//                if (field == undefined)
//                    field = {};

//                var fieldData = {
//                    directiveAPI: undefined,
//                    fieldEditorDefinitionReadyPromiseDeferred: UtilsService.createPromiseDeferred()
//                };

//                fieldData.onGenericBEFieldEditorDefinitionDirectiveReady = function (api) {
//                    fieldData.directiveAPI = api;
//                    fieldData.fieldEditorDefinitionReadyPromiseDeferred.resolve();
//                };
//                var payload = {
//                    settings: field,
//                    context: context,
//                    containerType: VR_GenericData_ContainerTypeEnum.Row.value
//                };
//                if (fieldEditorDefinitionLoadPromiseDeferred != undefined) {
//                    fieldData.fieldEditorDefinitionReadyPromiseDeferred.promise.then(function () {

//                        VRUIUtilsService.callDirectiveLoad(fieldData.directiveAPI, payload, fieldEditorDefinitionLoadPromiseDeferred);
//                    });
//                }
//                else {
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldData.directiveAPI, payload, setLoader);
//                }

//                fieldData.onFieldEditorDefinitionSelectionChanged = function (selectedFieldEditor) {
//                    fieldData.title = "";
//                    if (selectedFieldEditor == undefined)
//                        return;

//                    fieldData.title = selectedFieldEditor.Title;
//                };

//                return fieldData;
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var settings;

//                    if (payload != undefined) {
//                        settings = payload.settings;
//                        context = payload.context;
//                    }

//                    if (settings != undefined) {
//                        loadRowContainerDataSource();
//                    }

//                    function loadRowContainerDataSource() {
//                        var rowContainers = settings.RowContainers;
//                        for (var i = 0; i < rowContainers.length; i++) {

//                            var rowSettings = rowContainers[i].RowSettings;

//                            var rowFields = [];
//                            for (var j = 0; j < rowSettings.length; j++) {
//                                rowFields.push(rowSettings[j]);
//                            }

//                            rowFields.numberOfFields = rowFields.length + ' Field Types';
//                            $scope.scopeModel.datasource.push(rowFields);
//                        }
//                    }
//                };

//                api.getData = function () {
//                    var rowContainers = [];
//                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
//                        rowContainers.push({ RowSettings: getRowContainerEntity($scope.scopeModel.datasource[i]) });
//                    }

//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.RowsContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
//                        RowContainers: rowContainers
//                    };
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }

//            function defineMenuActions() {
//                $scope.scopeModel.gridMenuActions = [
//                    {
//                        name: "Edit",
//                        clicked: editRowContainer
//                    }];
//            }

//            function editRowContainer(rowContainerEntityObj) {
//                var onRowContainerUpdated = function (rowContainer) {
//                    var index = $scope.scopeModel.datasource.indexOf(rowContainerEntityObj);
//                    $scope.scopeModel.datasource[index] = rowContainer;
//                };

//                VR_GenericData_GenericBEDefinitionService.editGenericBERowContainer(onRowContainerUpdated, getRowContainerEntity(rowContainerEntityObj), getContext());
//            }

//            function getRowContainerEntity(rowContainerEntityObj) {
//                var rowContainerEntity = [];
//                for (var i = 0; i < rowContainerEntityObj.length; i++) {
//                    var obj = UtilsService.cloneObject(rowContainerEntityObj[i], true);
//                    rowContainerEntity.push(obj);
//                }

//                return rowContainerEntity;
//            }

//            function getContext() {

//                var currentContext = {
//                    getFields: function () {
//                        return context.getFields();
//                    },
//                    getFilteredFields: function () {
//                        return context.getRecordTypeFields();
//                    },
//                    getRecordTypeFields: function () {
//                        return context.getRecordTypeFields();
//                    },
//                    getDataRecordTypeId: function () {
//                        return context.getDataRecordTypeId();
//                    },
//                    getFieldType: function (fieldName) {
//                        return context.getFieldType(fieldName);
//                    }
//                };
//                return currentContext;
//            }
//        }
//    }

//    app.directive('vrGenericdataRowscontainereditorFieldsDefinition', RowsContainerEditorFieldsDefinitionDirective);

//})(app);