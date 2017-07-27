"use strict"
app.directive("demoModuleRoomGrid", ["VRNotificationService", "Demo_Module_RoomAPIService", "Demo_Module_RoomService",
    function (VRNotificationService, Demo_Module_RoomAPIService, Demo_Module_RoomService) {
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
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/Room/templates/RoomGridTemplate.html"
        };
        function RoomGrid($scope, ctrl, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.rooms = [];
                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveAPI());
                    }
                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };
                        directiveAPI.onRoomAdded = function (room) {
                            gridAPI.itemAdded(room);
                        };
                        return directiveAPI;
                    }
                };
                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_RoomAPIService.GetFilteredRooms(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
                defineMenuActions();
            }
            function defineMenuActions() {
                $scope.gridMenuActions = [{
                    name: "Edit",
                    clicked: editRoom,

                }, {
                    name: "Delete",
                    clicked: deleteRoom,
                }];
            }

            function editRoom(room) {
                var onRoomUpdated = function (room) {
                    gridAPI.itemUpdated(room);
                }
                Demo_Module_RoomService.editRoom(room.Entity.RoomId, onRoomUpdated);
            }

            function deleteRoom(room) {
                var onRoomDeleted = function (room) {
                    gridAPI.itemDeleted(room);
                };
                Demo_Module_RoomService.deleteRoom($scope, room, onRoomDeleted)
            }


        }
        return directiveDefinitionObject;
    }]
    );