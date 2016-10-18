"use strict";

app.directive("vrGenericdataDatarecordfieldManagement", ["UtilsService", "VRNotificationService", "VR_GenericData_DataRecordFieldService", "VR_GenericData_DataRecordFieldAPIService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordFieldService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dataRecordFieldManagement = new DataRecordFieldManagement($scope, ctrl, $attrs);
                dataRecordFieldManagement.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordField/Templates/DataRecordFieldManagement.html"

        };

        function DataRecordFieldManagement($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.fieldTypeConfigs = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should Select at least one filter type ";
                }

                ctrl.addDataRecordField = function () {
                    var onDataRecordFieldAdded = function (dataRecordField) {
                        addNeededFields(dataRecordField);
                        ctrl.datasource.push(dataRecordField);
                    }

                    VR_GenericData_DataRecordFieldService.addDataRecordField(onDataRecordFieldAdded, ctrl.datasource);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var fields;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        fields = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            fields.push({
                                Name: currentItem.Name,
                                Type: currentItem.Type,
                                Title: currentItem.Title,
                                Formula: currentItem.Formula
                            });
                        }

                    }
                    var obj = {
                        Fields: fields,
                    }
                    return obj;
                }

                api.load = function (payload) {

                    return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.fieldTypeConfigs.push(item);
                        });
                        if (payload != undefined) {
                            if (payload.Fields && payload.Fields.length > 0) {
                                for (var i = 0; i < payload.Fields.length; i++) {
                                    var dataItem = payload.Fields[i];
                                    addNeededFields(dataItem);
                                    ctrl.datasource.push(dataItem);
                                }
                            }
                        }
                    });
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addNeededFields(dataItem) {
                var template = UtilsService.getItemByVal(ctrl.fieldTypeConfigs, dataItem.Type.ConfigId, "ExtensionConfigurationId");
                dataItem.TypeDescription = template != undefined ? template.Name : "";
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editDataRecordField,
                },
                {
                    name: "Delete",
                    clicked: deleteDataRecordField,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editDataRecordField(dataRecordFieldObj) {
                var onDataRecordFieldUpdated = function (dataRecordField) {
                    addNeededFields(dataRecordField);
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordFieldObj.Name, 'Name');
                    ctrl.datasource[index] = dataRecordField;
                }

                VR_GenericData_DataRecordFieldService.editDataRecordField(dataRecordFieldObj, onDataRecordFieldUpdated, ctrl.datasource);
            }

            function deleteDataRecordField(dataRecordFieldObj) {
                var onDataRecordFieldDeleted = function (dataRecordField) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordFieldObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                VR_GenericData_DataRecordFieldService.deleteDataRecordField($scope, dataRecordFieldObj, onDataRecordFieldDeleted);
            }
        }

        return directiveDefinitionObject;

    }
]);