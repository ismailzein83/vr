"use strict";

app.directive("vrCommonDatarecordfieldGrid", ["UtilsService", "VRNotificationService", "VRCommon_DataRecordFieldService", "VRCommon_DataRecordFieldAPIService",
    function (UtilsService, VRNotificationService, VRCommon_DataRecordFieldService, VRCommon_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dataRecordFieldGrid = new DataRecordFieldGrid($scope, ctrl, $attrs);
                dataRecordFieldGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/GenericDataRecord/Templates/DataRecordFieldGrid.html"

        };

        function DataRecordFieldGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.datarecordfields = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        }
                        directiveAPI.onDataRecordFieldAdded = function (dataRecordFieldObj) {
                            gridAPI.itemAdded(dataRecordFieldObj);
                        }
                        directiveAPI.onDataRecordFieldUpdated = function (dataRecordFieldObj) {
                            gridAPI.itemUpdated(dataRecordFieldObj);
                        }
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_DataRecordFieldAPIService.GetFilteredDataRecordFields(dataRetrievalInput)
                        .then(function (response) {
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
                    gridAPI.itemUpdated(dataRecordField);
                }

                VRCommon_DataRecordFieldService.editDataRecordField(dataRecordFieldObj.Entity, onDataRecordFieldUpdated);
            }

            function deleteDataRecordField(dataRecordFieldObj) {
                var onDataRecordFieldDeleted = function (dataRecordField) {
                    gridAPI.itemDeleted(dataRecordField);
                };

                VRCommon_DataRecordFieldService.deleteDataRecordField($scope, dataRecordFieldObj, onDataRecordFieldDeleted);
            }
        }

        return directiveDefinitionObject;

    }
]);