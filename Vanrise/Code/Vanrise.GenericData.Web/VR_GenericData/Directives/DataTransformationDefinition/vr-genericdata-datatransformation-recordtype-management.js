"use strict";

app.directive("vrGenericdataDatatransformationRecordtypeManagement", ["UtilsService", "VRNotificationService", "VR_GenericData_DataTransformationDefinitionService","VR_GenericData_DataRecordTypeAPIService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataTransformationDefinitionService, VR_GenericData_DataRecordTypeAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dataRecordTypeManagement = new DataRecordTypeManagement($scope, ctrl, $attrs);
                dataRecordTypeManagement.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/DataTransformationDefinition/Templates/DataTransformationRecordTypeManagement.html"

        };

        function DataRecordTypeManagement($scope, ctrl, $attrs) {
            var recordTypes;
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should Select at least one filter type ";
                };

                ctrl.addDataRecordType = function () {
                    var onDataRecordTypeAdded = function (dataRecordType) {
                        addNeededTypes(dataRecordType);
                        ctrl.datasource.push(dataRecordType);
                    };

                    VR_GenericData_DataTransformationDefinitionService.addDataRecordType(onDataRecordTypeAdded, ctrl.datasource);
                };
                
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {

                    var recordTypes;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        recordTypes = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            recordTypes.push({
                                RecordName: ctrl.datasource[i].RecordName,
                                DataRecordTypeId: ctrl.datasource[i].DataRecordTypeId,
                                FullTypeName: ctrl.datasource[i].FullTypeName,
                                IsArray: ctrl.datasource[i].IsArray,
                            });
                        }

                    }
                    var obj = {
                        RecordTypes: recordTypes,
                    };
                    return obj;
                };

                api.load = function (payload) {

                    return getDataRecordTypeInfo().then(function () {
                        if (payload != undefined && payload.RecordTypes) {
                            for (var i = 0; i < payload.RecordTypes.length; i++) {
                                var dataItem = payload.RecordTypes[i];
                                addNeededTypes(dataItem);
                                ctrl.datasource.push(dataItem);
                            }
                        }
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addNeededTypes(dataItem)
            {
                var template = UtilsService.getItemByVal(recordTypes, dataItem.DataRecordTypeId, "DataRecordTypeId");

                dataItem.TypeDescription = template != undefined ? template.Name : "Dynamic";
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editDataRecordType,
                },
                {
                    name: "Delete",
                    clicked: deleteDataRecordType,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editDataRecordType(dataRecordTypeObj) {
                var onDataRecordTypeUpdated = function (dataRecordType) {
                    addNeededTypes(dataRecordType);
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordTypeObj.RecordName, 'RecordName');
                    ctrl.datasource[index] = dataRecordType;
                };

                VR_GenericData_DataTransformationDefinitionService.editDataRecordType(dataRecordTypeObj, onDataRecordTypeUpdated, ctrl.datasource);
            }

            function deleteDataRecordType(dataRecordTypeObj) {
                var onDataRecordTypeDeleted = function (dataRecordType) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordTypeObj.RecordName, 'RecordName');
                    ctrl.datasource.splice(index, 1);
                };

                VR_GenericData_DataTransformationDefinitionService.deleteDataRecordType($scope, dataRecordTypeObj, onDataRecordTypeDeleted);
            }

            function getDataRecordTypeInfo()
            {
              return  VR_GenericData_DataRecordTypeAPIService.GetDataRecordTypeInfo().then(function (response) {
                    recordTypes = response;
                });
            }
        }

        return directiveDefinitionObject;

    }
]);