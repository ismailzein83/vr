"use strict"
app.directive("demoModuleFamilyGrid", ["UtilsService", "VRNotificationService", "Demo_Module_FamilyAPIService", "Demo_Module_FamilyService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_FamilyAPIService, Demo_Module_FamilyService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var familyGrid = new FamilyGrid($scope, ctrl, $attrs);
            familyGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl:"/Client/Modules/Demo_Module/Directives/Product/templates/FamilyGridTemplate.html"
    };

    function FamilyGrid($scope, ctrl) {

        var gridApi;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.familys = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (query) {
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onFamilyAdded = function (family) {
                        gridApi.itemAdded(family);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_FamilyAPIService.GetFilteredFamilys(dataRetrievalInput)
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
                clicked: editFamily,

            }];
        };
        function editFamily(family) {
            var onFamilyUpdated = function (family) {
                gridApi.itemUpdated(family);
            };
            Demo_Module_FamilyService.editFamily(family.FamilyId, onFamilyUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
