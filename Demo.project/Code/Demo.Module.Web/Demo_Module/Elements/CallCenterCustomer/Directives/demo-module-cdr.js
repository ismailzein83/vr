"use strict"
app.directive("demoModuleCdr", ["VRValidationService","UtilsService", "VRNotificationService", "Demo_Module_CDRAPIService", "CDRTypeEnum", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (VRValidationService,UtilsService, VRNotificationService, Demo_Module_CDRAPIService, CDRTypeEnum, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var cdrGrid = new CdrGrid($scope, ctrl, $attrs);
            cdrGrid.initializeController();
        },
       // controllerAs: 'ctrl',
       // bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Directives/Templates/CdrTemplate.html"
    };

    function CdrGrid($scope,attrs, ctrl) {

        var gridApi;
        var selectorAPI;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.selectedvalues = [];
            $scope.scopeModel.cdr = [];
            $scope.scopeModel.types = UtilsService.getArrayEnum(CDRTypeEnum);

            $scope.scopeModel.search = function () {
                return gridApi.retrieveData(getFilter());
            };

            $scope.scopeModel.validateDates = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.From, $scope.scopeModel.To);
            };

            $scope.scopeModel.onSelectorReady = function (api) {

            }

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if ($scope.onReady != undefined && typeof ($scope.onReady) == "function") {
                    $scope.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function () {
                        return gridApi.retrieveData(getFilter());
                    };

                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_CDRAPIService.GetCDR(dataRetrievalInput)
                .then(function (response) {
                    if (response !=undefined) {
                        $scope.scopeModel.cdr = response;
                    }

                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [];
        };


        function getFilter() {
            if ($scope.scopeModel.selectedvalues != undefined) {
                var typeIds = [];
                for (var i = 0; i < $scope.scopeModel.selectedvalues.length; i++) {
                    var typeId = $scope.scopeModel.selectedvalues[i].value;
                    typeIds.push(typeId);
                }
                console.log(typeIds);
            }
            return {
                From: $scope.scopeModel.fromDate,
                To: $scope.scopeModel.toDate,
                Type: typeIds
            };
        };

    };
    return directiveDefinitionObject;
}]);
