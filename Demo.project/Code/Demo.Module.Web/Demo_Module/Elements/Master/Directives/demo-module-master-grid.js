"use strict"
app.directive("demoModuleMasterGrid", ["UtilsService", "VRNotificationService", "Demo_Module_MasterAPIService", "Demo_Module_MasterService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_MasterAPIService, Demo_Module_MasterService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var masterGrid = new MasterGrid($scope, ctrl, $attrs);
            masterGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Elements/Master/Directives/Templates/MasterGridTemplate.html"
    };

    function MasterGrid($scope, ctrl) {

        var gridApi;
        this.initializeController = initializeController;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.masters = [];

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
                    directiveApi.onMasterAdded = function (master) {
                        gridApi.itemAdded(master);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_MasterAPIService.GetFilteredMasters(dataRetrievalInput)
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
                clicked: editMaster,

            }];
        };
        function editMaster(master) {
            var onMasterUpdated = function (master) {
                gridApi.itemUpdated(master);
            };
            Demo_Module_MasterService.editMaster(master.MasterId, onMasterUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
