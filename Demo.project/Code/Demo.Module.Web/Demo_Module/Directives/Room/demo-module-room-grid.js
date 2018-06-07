"use strict"
app.directive("demoModuleRoomGrid", ["UtilsService", "VRNotificationService", "Demo_Module_RoomAPIService", "Demo_Module_RoomService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_RoomAPIService, Demo_Module_RoomService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var roomGrid = new RoomGrid($scope, ctrl, $attrs);
            roomGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Room/Templates/RoomGridTemplate.html"
    };

    function RoomGrid($scope, ctrl) {

        var gridApi;
        var buildingId;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.rooms = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        var query = payload.query;
                        buildingId = payload.buildingId;
                        if (payload.hideBuildingColumn)
                            $scope.scopeModel.hideBuildingColumn = payload.hideBuildingColumn;
                        return gridApi.retrieveData(query);
                        
                    };

                    directiveApi.onRoomAdded = function (room) {
                        gridApi.itemAdded(room);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_RoomAPIService.GetFilteredRooms(dataRetrievalInput)
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
                clicked: editRoom,

            }];
        };
        function editRoom(room) {
            var onRoomUpdated = function (room) {
                gridApi.itemUpdated(room);
            };
            var buildingIdItem = buildingId != undefined ? { BuildingId: buildingId } : undefined;
            Demo_Module_RoomService.editRoom(room.RoomId, onRoomUpdated, buildingIdItem);
        };


    };
    return directiveDefinitionObject;
}]);
