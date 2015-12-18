"use strict";

app.directive("vrCommonDatarecordtypeGrid", ["UtilsService", "VRNotificationService", "VRCommon_DataRecordTypeAPIService",
function (UtilsService, VRNotificationService, VRCommon_DataRecordTypeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
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
                           // clicked: editDataRecordField,
                        }
            ];

            $scope.gridMenuActions = function (dataItem) {
                return defaultMenuActions;
            }
        }
    }

    return directiveDefinitionObject;

}]);
