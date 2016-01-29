"use strict";

app.directive("vrGenericdataDatarecordfieldManagement", ["UtilsService", "VRNotificationService", "VR_GenericData_DataRecordFieldService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordFieldService) {

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
                ctrl.isValid = function () {

                    if (ctrl.datasource.length > 0)
                        return null;
                    return "You Should Select at least one filter type ";
                }
                ctrl.addDataRecordField = function () {
                    var onDataRecordFieldAdded = function (dataRecordField) {
                        ctrl.datasource.push(dataRecordField);
                    }

                    VR_GenericData_DataRecordFieldService.addDataRecordField(onDataRecordFieldAdded);
                };
                
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var obj = {
                        Fields: ctrl.datasource,
                    }
                    return obj;
                }
                api.load = function (payload) {
                    if(payload != undefined)
                    {
                        if(payload.Fields && payload.Fields.length > 0)
                        {
                            for(var i=0; i<payload.Fields.length;i++)
                            {
                                ctrl.datasource.push(payload.Fields[i]);
                            }
                        }
                       
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editDataRecordField,
                }//,
                //{
                //    name: "Delete",
                //    clicked: deleteDataRecordField,
                    //}
                    ];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editDataRecordField(dataRecordFieldObj) {
                var onDataRecordFieldUpdated = function (dataRecordField) {
                 ///   gridAPI.itemUpdated(dataRecordField);
                }

                VR_GenericData_DataRecordFieldService.editDataRecordField(dataRecordFieldObj.Entity, onDataRecordFieldUpdated);
            }

            //function deleteDataRecordField(dataRecordFieldObj) {
            //    var onDataRecordFieldDeleted = function (dataRecordField) {
            //        gridAPI.itemDeleted(dataRecordField);
            //    };

            //    VR_GenericData_DataRecordFieldService.deleteDataRecordField($scope, dataRecordFieldObj, onDataRecordFieldDeleted);
            //}
        }

        return directiveDefinitionObject;

    }
]);