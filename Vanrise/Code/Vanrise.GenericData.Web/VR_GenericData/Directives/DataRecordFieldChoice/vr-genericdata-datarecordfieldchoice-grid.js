"use strict";

app.directive("vrGenericdataDatarecordfieldchoiceGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_DataRecordFieldChoiceAPIService", "VR_GenericData_DataRecordFieldChoiceService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordFieldChoiceAPIService, VR_GenericData_DataRecordFieldChoiceService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var datarecordfieldchoiceGrid = new DataRecordFieldChoiceGrid($scope, ctrl, $attrs);
                datarecordfieldchoiceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/DataRecordFieldChoice/Templates/DataRecordFieldChoiceGrid.html"

        };

        function DataRecordFieldChoiceGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            function initializeController() {

                $scope.dataRecordFieldChoice = [];

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onDataRecordFieldChoiceAdded = function (onDataRecordFieldChoiceObj) {
                            gridAPI.itemAdded(onDataRecordFieldChoiceObj);
                        };
                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_DataRecordFieldChoiceAPIService.GetFilteredDataRecordFieldChoices(dataRetrievalInput)
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
                    clicked: editDataRecordFieldChoice,
                    haspermission: hasEditDataRecordFieldChoicePermission
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function hasEditDataRecordFieldChoicePermission() {
                return VR_GenericData_DataRecordFieldChoiceAPIService.HasUpdateDataRecordFieldChoice();
            };
            function editDataRecordFieldChoice(dataItem) {
                var onDataRecordFieldChoiceUpdated = function (dataRecordFieldChoiceObj) {
                    gridAPI.itemUpdated(dataRecordFieldChoiceObj);
                };
                VR_GenericData_DataRecordFieldChoiceService.editDataRecordFieldChoice(dataItem.Entity.DataRecordFieldChoiceId, onDataRecordFieldChoiceUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);