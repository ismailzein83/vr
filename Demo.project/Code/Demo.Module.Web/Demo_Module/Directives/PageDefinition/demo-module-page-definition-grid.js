"use strict"
app.directive("demoModulePageDefinitionGrid", ["UtilsService", "VRNotificationService", "Demo_Module_PageDefinitionAPIService", "Demo_Module_PageDefinitionService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_PageDefinitionAPIService, Demo_Module_PageDefinitionService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var pageDefinitionGrid = new PageDefinitionGrid($scope, ctrl, $attrs);
            pageDefinitionGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/PageDefinition/Templates/PageDefinitionGridTemplate.html"
    };

    function PageDefinitionGrid($scope, ctrl) {

        var gridApi;
        $scope.scopeModel = {};
        this.initializeController = initializeController;

        function initializeController() {
           

            $scope.scopeModel.pageDefinitions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        var query = payload.query;

                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onPageDefinitionAdded = function (pageDefinition) {
                        gridApi.itemAdded(pageDefinition);
                    };
                    return directiveApi;
                };
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return VR_Tools_ColumnsAPIService.GetFilteredPageDefinitions(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editPageDefinition,

            }];
        }

        function editPageDefinition(pageDefinition) {
            var onPageDefinitionUpdated = function (pageDefinition) {
                gridApi.itemUpdated(pageDefinition);
            };
            Demo_Module_PageDefinitionService.editPageDefinition(pageDefinition.PageDefinitionId, onPageDefinitionUpdated);
        };

    };

    return directiveDefinitionObject;
}]);
