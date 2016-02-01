"use strict";

app.directive("vrGenericdataDatatransformationRecordtypeManagement", ["UtilsService", "VRNotificationService", "VR_GenericData_DataTransformationDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataTransformationDefinitionService) {

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

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource !=undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should Select at least one filter type ";
                }
                ctrl.addDataRecordType = function () {
                    var onDataRecordTypeAdded = function (dataRecordType) {
                       // addNeededTypes(dataRecordType);
                        ctrl.datasource.push(dataRecordType);
                    }

                    VR_GenericData_DataTransformationDefinitionService.addDataRecordType(onDataRecordTypeAdded, ctrl.datasource);
                };
                
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var obj = {
                        Types: ctrl.datasource,
                    }
                    return obj;
                }
                api.load = function (payload) {

                    if (payload != undefined) {
                        if (payload.Types && payload.Types.length > 0) {
                            for (var i = 0; i < payload.Types.length; i++) {
                                var dataItem = payload.Types[i];
                               // addNeededTypes(dataItem);
                                ctrl.datasource.push(dataItem);
                            }
                        }
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            //function addNeededTypes(dataItem)
            //{
            //    var template = UtilsService.getItemByVal(ctrl.dataRecordTypeTypeTemplates, dataItem.value, "TemplateConfigID");
            //    dataItem.TypeDescription = template != undefined ? template.Name : "";
            //}
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
                }
            }

            function editDataRecordType(dataRecordTypeObj) {
                var onDataRecordTypeUpdated = function (dataRecordType) {
                   // addNeededTypes(dataRecordType);
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordTypeObj.Name, 'Name');
                    ctrl.datasource[index] = dataRecordType;
                }

                VR_GenericData_DataTransformationDefinitionService.editDataRecordType(dataRecordTypeObj, onDataRecordTypeUpdated, ctrl.datasource);
            }

            function deleteDataRecordType(dataRecordTypeObj) {
                var onDataRecordTypeDeleted = function (dataRecordType) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordTypeObj.Name, 'Name');
                    ctrl.datasource.splice(index, 1);
                };

                VR_GenericData_DataTransformationDefinitionService.deleteDataRecordType($scope, dataRecordTypeObj, onDataRecordTypeDeleted);
            }
        }

        return directiveDefinitionObject;

    }
]);