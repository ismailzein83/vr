
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
        templateUrl: "/Client/Modules/Demo_Module/Directives/Family/Template/FamilyGridTemplate.html"
    };

    function FamilyGrid($scope, ctrl) {

        var gridApi;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.families = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                var drillDownDefinitions = [];
                AddMemberDrillDown();
                function AddMemberDrillDown() {
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Member";
                    drillDownDefinition.directive = "demo-module-member-search";

                    drillDownDefinition.loadDirective = function (directiveAPI, familyItem) {
                        familyItem.memberGridAPI = directiveAPI;

                        var payload = {
                            familyId: familyItem.FamilyId
                        };

                        return familyItem.memberGridAPI.load(payload);
                    };
                    drillDownDefinitions.push(drillDownDefinition);
                }

                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (query) {
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onFamilyAdded = function (family) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(family);
                        gridApi.itemAdded(family);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_FamilyAPIService.GetFilteredFamilies(dataRetrievalInput)
                .then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                        }
                    }
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
                gridDrillDownTabsObj.setDrillDownExtensionObject(family);
                gridApi.itemUpdated(family);
            };
            Demo_Module_FamilyService.editFamily(family.FamilyId, onFamilyUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
