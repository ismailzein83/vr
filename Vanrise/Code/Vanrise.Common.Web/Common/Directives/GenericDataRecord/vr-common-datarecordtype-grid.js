"use strict";

app.directive("vrCommonDatarecordtypeGrid", ["UtilsService", "VRNotificationService", "VRCommon_DataRecordTypeAPIService", "VRCommon_DataRecordFieldService",
    function (UtilsService, VRNotificationService, VRCommon_DataRecordTypeAPIService, VRCommon_DataRecordFieldService) {

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
            templateUrl: "/Client/Modules/Common/Directives/GenericDataRecord/Templates/DataRecordTypeGrid.html"

        };

        function DataRecordTypeGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

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
                    return VRCommon_DataRecordTypeAPIService.GetFilteredDataRecordTypes(dataRetrievalInput)
                        .then(function (response) {
                            if (response.Data != undefined) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    setDataItemExtension(response.Data[i]);
                                }
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };
                defineMenuActions();
            }

            function setDataItemExtension(dataItem) {
                var extensionObject = {};
                var query = {
                    DataRecordTypeId: dataItem.Entity.DataRecordTypeId,
                }
                extensionObject.onGridReady = function (api) {
                    extensionObject.dataRecordFieldGridAPI = api;
                    extensionObject.dataRecordFieldGridAPI.loadGrid(query);
                    extensionObject.onGridReady = undefined;
                };
                dataItem.extensionObject = extensionObject;

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
                VRCommon_DataRecordFieldService.addDataRecordField(onDataRecordFieldAdded, dataItem.Entity);
            }
        }

        return directiveDefinitionObject;

    }
]);