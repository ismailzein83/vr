"use strict"
app.directive("demoModuleInvoiceGrid", ["UtilsService", "VRNotificationService", "Demo_Module_InvoiceAPIService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_InvoiceAPIService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var serviceGrid = new ServiceGrid($scope, ctrl, $attrs);
            serviceGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Elements/CallCenterCustomer/Directives/Templates/InvoiceGridTemplate.html"
    };

    function ServiceGrid($scope, ctrl) {

        var gridApi;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.invoices = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function () {
                        return gridApi.retrieveData();
                    };

                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_InvoiceAPIService.GetInvoices()
                .then(function (response) {
                    if (response != null) {
                        $scope.scopeModel.invoices = response;
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


    };
    return directiveDefinitionObject;
}]);
