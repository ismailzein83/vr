"use strict";

app.directive("vrAnalyticAnalytictableGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticTableService', 'UtilsService', 'VR_Analytic_AnalyticTableAPIService', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticTableService, UtilsService, VR_Analytic_AnalyticTableAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticTableGrid = new AnalyticTableGrid($scope, ctrl, $attrs);
            analyticTableGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticTable/Templates/AnalyticTableGridTemplate.html"

    };

    function AnalyticTableGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.tables = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onViewAdded = function (viewObj) {
                        gridAPI.itemAdded(viewObj);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Analytic_AnalyticTableAPIService.GetFilteredAnalyticTables(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editTable,
                //haspermission: hasUpdateViewPermission // System Entities:Assign Permissions
            }];
        }
        //function hasUpdateViewPermission() {
        //    return VR_Sec_ViewAPIService.HasUpdateViewPermission();
        //}
        function editTable(dataItem) {
            var onEditTable = function (tableObj) {
                gridAPI.itemUpdated(tableObj);
            }


            VR_Analytic_AnalyticTableService.editAnalyticTable(dataItem.Entity.AnalyticTableId, onEditTable);

        }
    }

    return directiveDefinitionObject;

}]);