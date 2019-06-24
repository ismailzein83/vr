"use strict";

app.directive("vrGenericdataBeStatushistoryGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_BusinessEntityStatusHistoryAPIService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_BusinessEntityStatusHistoryAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var dataRecordTypeGrid = new BEStatusHistoryGrid($scope, ctrl, $attrs);
                dataRecordTypeGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityStatusHistory/Templates/BusinessEntityStatusHistoryGrid.html"

        };

        function BEStatusHistoryGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.statusHistories = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                function defineAPI() {
                    var api = {};

                    api.loadGrid = function (payload) {
                        var gridQuery;
                        if (payload != undefined)
                            gridQuery = payload.query;
                        return gridAPI.retrieveData(gridQuery);
                    };

                    if (ctrl.onReady != null)
                        ctrl.onReady(api);
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_GenericData_BusinessEntityStatusHistoryAPIService.GetFilteredBusinessEntitiesStatusHistory(dataRetrievalInput)
                        .then(function (response) {
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

            }
        }
        return directiveDefinitionObject;
    }
]);