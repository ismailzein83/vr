"use strict";

app.directive("vrGenericdataDatarecordtypeGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_DataRecordTypeAPIService", "VR_GenericData_DataRecordTypeService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_DataRecordTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dataRecordTypeGrid = new DataRecordTypeGrid($scope, ctrl, $attrs);
                dataRecordTypeGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/GenericData/Directives/GenericDataRecord/Templates/DataRecordTypeGrid.html"

        };

        function DataRecordTypeGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            function initializeController() {

                $scope.datarecordTypes = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_DataRecordTypeAPIService.GetFilteredDataRecordTypes(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
                defineMenuActions();
            }


            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Assign Data Record Field",
                    clicked: assignDataRecordField,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function assignDataRecordField(dataItem) {
                gridAPI.expandRow(dataItem);
                var query = {
                    DataRecordTypeId: dataItem.Entity.DataRecordTypeId,
                }
                if (dataItem.extensionObject.dataRecordFieldGridAPI != undefined)
                    dataItem.extensionObject.dataRecordFieldGridAPI.loadGrid(query);
                var onDataRecordFieldAdded = function (dataRecordFieldObj) {
                    if (dataItem.extensionObject.dataRecordFieldGridAPI != undefined)
                        dataItem.extensionObject.dataRecordFieldGridAPI.onDataRecordFieldAdded(dataRecordFieldObj);
                };
                VR_GenericData_DataRecordTypeService.addDataRecordField(onDataRecordFieldAdded, dataItem.Entity);
            }
        }

        return directiveDefinitionObject;

    }
]);