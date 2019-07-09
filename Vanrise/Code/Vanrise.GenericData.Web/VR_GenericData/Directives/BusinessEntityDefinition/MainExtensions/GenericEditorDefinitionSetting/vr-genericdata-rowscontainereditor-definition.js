(function (app) {

    'use strict';

    RowsContainerEditorDefinitionDirective.$inject = ['VRNotificationService', 'VR_GenericData_GenericBEDefinitionService', 'UtilsService'];

    function RowsContainerEditorDefinitionDirective(VRNotificationService, VR_GenericData_GenericBEDefinitionService, UtilsService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/RowsContainerEditorDefinitionSettingTemplate.html'
        };

        function RowsContainerEditorDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.addRowContainer = function () {
                    var onRowContainerAdded = function (addedItem) {
                        $scope.scopeModel.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBERowContainer(onRowContainerAdded, getContext());
                };

                $scope.scopeModel.disableAddRowContainer = function () {
                    if (context == undefined)
                        return true;

                    return false;
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
                        return "You Should add at least one row.";

                    return null;
                };

                defineAPI();

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                        context = payload.context;
                    }

                    if (settings != undefined) {
                        loadRowContainerDataSource();
                    }

                    function loadRowContainerDataSource() {
                        var rowContainers = settings.RowContainers;
                        for (var i = 0; i < rowContainers.length; i++) {

                            var rowSettings = rowContainers[i].RowSettings;

                            var rowFields = [];
                            for (var j = 0; j < rowSettings.length; j++) {
                                rowFields.push(rowSettings[j]);
                            }

                            rowFields.numberOfFields = rowFields.length + ' Field Types';
                            $scope.scopeModel.datasource.push(rowFields);
                        }
                    }
                };

                api.getData = function () {
                    var rowContainers = [];
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        rowContainers.push({ RowSettings: getRowContainerEntity($scope.scopeModel.datasource[i]) });
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

            function defineMenuActions() {
                var defaultMenuActions = [
                    {
                        name: "Edit",
                        clicked: editRowContainer
                    },
                    {
                        name: "Delete",
                        clicked: deleteRowContainer
                    }];

                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function deleteRowContainer(rowContainerObj) {
                VRNotificationService.showConfirmation().then(function (response) {
                    if (response) {
                        var index = $scope.scopeModel.datasource.indexOf(rowContainerObj);
                        if (index != -1)
                            $scope.scopeModel.datasource.splice(index, 1);
                    }
                });
            }

            function editRowContainer(rowContainerEntityObj) {
                var onRowContainerUpdated = function (rowContainer) {
                    var index = $scope.scopeModel.datasource.indexOf(rowContainerEntityObj);
                    $scope.scopeModel.datasource[index] = rowContainer;
                };

                VR_GenericData_GenericBEDefinitionService.editGenericBERowContainer(onRowContainerUpdated, getRowContainerEntity(rowContainerEntityObj), getContext());
            }

            function getRowContainerEntity(rowContainerEntityObj) {
                var rowContainerEntity = [];
                for (var i = 0; i < rowContainerEntityObj.length; i++) {
                    var obj = UtilsService.cloneObject(rowContainerEntityObj[i], true);
                    rowContainerEntity.push(obj);
                }

                return rowContainerEntity;
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