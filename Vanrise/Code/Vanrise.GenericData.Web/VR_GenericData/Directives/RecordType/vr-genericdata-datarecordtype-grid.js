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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordType/Templates/DataRecordTypeGrid.html"

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
                        };
                        directiveAPI.onDataRecordTypeAdded = function (onDataRecordTypeObj) {
                            gridAPI.itemAdded(onDataRecordTypeObj);
                        };
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
                    name: "Edit",
                    clicked: editDataRecordField,
                    haspermission: hasEditDataRecordTypePermission
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function hasEditDataRecordTypePermission() {
                return VR_GenericData_DataRecordTypeAPIService.HasUpdateDataRecordType()
            }
            function editDataRecordField(dataItem) {
                var onDataRecordFieldUpdated = function (dataRecordFieldObj) {
                    gridAPI.itemUpdated(dataRecordFieldObj);
                };

                VR_GenericData_DataRecordTypeService.editDataRecordType(dataItem.Entity.DataRecordTypeId, onDataRecordFieldUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);