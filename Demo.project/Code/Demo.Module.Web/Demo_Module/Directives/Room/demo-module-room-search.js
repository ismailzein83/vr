"use strict";

app.directive("demoModuleRoomSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Demo_Module_RoomService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Demo_Module_RoomService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new RoomSearch($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/Room/Templates/RoomSearchTemplate.html"
        
    };

    function RoomSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var buildingId;

        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addRoom = function () {
                var buildingIdItem = { BuildingId: buildingId };
                var onRoomAdded = function (obj) {
                    gridAPI.onRoomAdded(obj);
                };
                Demo_Module_RoomService.addRoom(onRoomAdded, buildingIdItem);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    buildingId = payload.buildingId;
                }
                return gridAPI.load(getGridQuery());
            };
            api.onRoomAdded = function (roomObject) {
                gridAPI.onRoomAdded(roomObject);
            };


            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getGridQuery() {
            var payload = {
                query: { BuildingIds: [buildingId] },
                buildingId: buildingId,
                hideBuildingColumn: true
            };
            return payload;
        }
    }

    return directiveDefinitionObject;

}]);
