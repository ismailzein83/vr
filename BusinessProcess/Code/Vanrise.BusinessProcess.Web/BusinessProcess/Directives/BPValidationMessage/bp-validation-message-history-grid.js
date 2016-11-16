"use strict";
app.directive("businessprocessBpValidationMessageHistory", ["BusinessProcess_BPValidationMessageAPIService", "VRNotificationService", "UtilsService", "BPActionSeverityEnum", "BusinessProcess_BPValidationMessageService",
function (BusinessProcess_BPValidationMessageAPIService, VRNotificationService, UtilsService, BPActionSeverityEnum, BusinessProcess_BPValidationMessageService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var bpValidationMessageGrid = new BPValidationMessageGrid($scope, ctrl, $attrs);
            bpValidationMessageGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPValidationMessage/Templates/BPValidationMessageHistoryGridTemplate.html"

    };

    function BPValidationMessageGrid($scope, ctrl) {

        var gridAPI;
        var filter = {};

        var bpInstanceId;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.bpValidationMessages = [];
            $scope.selectedActionSeverity = [];

            loadFilters();
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.load = function (query) {
                        bpInstanceId = query.BPInstanceID;
                        getFilterObject();
                        return gridAPI.retrieveData(filter);
                    };

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return BusinessProcess_BPValidationMessageAPIService.GetFilteredBPValidationMessage(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.retrieveData(filter);
            };

        }

        $scope.getSeverityColor = function (dataItem, colDef) {
            return BusinessProcess_BPValidationMessageService.getSeverityColor(dataItem.Entity.Severity);
        };

        function loadFilters() {
            $scope.actionSeverity = UtilsService.getArrayEnum(BPActionSeverityEnum);
        }

        function getFilterObject() {
            filter = {
                ProcessInstanceId: bpInstanceId,
                Severities: UtilsService.getPropValuesFromArray($scope.selectedActionSeverity, "value")
            };
        }
    }
    return directiveDefinitionObject;

}]);