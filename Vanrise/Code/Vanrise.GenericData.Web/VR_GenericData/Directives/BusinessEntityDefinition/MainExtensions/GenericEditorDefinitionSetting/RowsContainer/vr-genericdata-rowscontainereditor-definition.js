﻿(function (app) {

    'use strict';

    RowsContainerEditorDefinitionDirective.$inject = ['VRNotificationService', 'VRUIUtilsService', 'UtilsService'];

    function RowsContainerEditorDefinitionDirective(VRNotificationService, VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RowsContainerEditorDefinitionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/RowsContainer/Templates/RowsContainerEditorDefinitionSettingTemplate.html'
        };

        function RowsContainerEditorDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.dragsettings = {
                    handle: '.vr-control-label'
                };

                $scope.scopeModel.addRowContainer = function () {
                    var dataItem = {
                        entity: {
                            numberOfFields:'0 Field Types'
                        }
                    };

                    dataItem.onRowContainerFieldsDirectiveReady = function (api) {
                        dataItem.rowContainerFieldsDirectiveAPI = api;
                        var setLoader = function (value) { dataItem.isRowContainerFieldsDirectiveLoading = value; };
                        var payload = {
                            context: getContext(),
                            setFieldsNumber: function (fieldsNumber) {
                                dataItem.entity.numberOfFields = fieldsNumber + ' Field Types';
                            }
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.rowContainerFieldsDirectiveAPI, payload, setLoader);
                    };

                    dataItem.removeRow = function () {
                        VRNotificationService.showConfirmation().then(function (response) {
                            if (response) {
                                var index = $scope.scopeModel.datasource.indexOf(dataItem);
                                if (index != -1)
                                    $scope.scopeModel.datasource.splice(index, 1);
                            }
                        });
                    };

                    $scope.scopeModel.datasource.push(dataItem);
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
                        return "You Should add at least one row.";

                    return null;
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var settings;
                    var promises = [];
                    if (payload != undefined) {
                        settings = payload.settings;
                        context = payload.context;
                    }

                    if (settings != undefined) {
                        var rowContainers = settings.RowContainers;
                        for (var i = 0; i < rowContainers.length; i++) {

                            var rowSettings = rowContainers[i].RowSettings;
                            var rowObject = { payload: rowSettings };

                            prepareRow(rowObject);
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    var rowContainers = [];
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var row = $scope.scopeModel.datasource[i];
                        rowContainers.push({ RowSettings: row.rowContainerFieldsDirectiveAPI != undefined ? row.rowContainerFieldsDirectiveAPI.getData() : row.oldRowSettings });
                    }

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.RowsContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        RowContainers: rowContainers
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function prepareRow(rowObject) {
                var dataItem = {
                    entity: {
                        numberOfFields: rowObject.payload != undefined ? rowObject.payload.length + ' Field Types' : '0 Field Types'
                    },
                    oldRowSettings: rowObject.payload
                };

                dataItem.onRowContainerFieldsDirectiveReady = function (api) {
                    dataItem.rowContainerFieldsDirectiveAPI = api;
                    var setLoader = function (value) { dataItem.isRowContainerFieldsDirectiveLoading = value; };
                    var payload = {
                        context: getContext(),
                        fields: rowObject.payload,
                        setFieldsNumber: function (fieldsNumber) {
                            dataItem.entity.numberOfFields = fieldsNumber + ' Field Types';
                        }
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.rowContainerFieldsDirectiveAPI, payload, setLoader);
                };

                dataItem.removeRow = function () {
                    VRNotificationService.showConfirmation().then(function (response) {
                        if (response) {
                            var index = $scope.scopeModel.datasource.indexOf(dataItem);
                            if (index != -1)
                                $scope.scopeModel.datasource.splice(index, 1);
                        }
                    });
                };

                $scope.scopeModel.datasource.push(dataItem);
            }

            function getContext() {

                var currentContext = {
                    getFields: function () {
                        return context.getFields();
                    },
                    getFilteredFields: function () {
                        return context.getRecordTypeFields();
                    },
                    getRecordTypeFields: function () {
                        return context.getRecordTypeFields();
                    },
                    getDataRecordTypeId: function () {
                        return context.getDataRecordTypeId();
                    },
                    getFieldType: function (fieldName) {
                        return context.getFieldType(fieldName);
                    }
                };
                return currentContext;
            }
        }
    }

    app.directive('vrGenericdataRowscontainereditorDefinition', RowsContainerEditorDefinitionDirective);

})(app);