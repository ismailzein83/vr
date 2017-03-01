"use strict";

app.directive("demoModuleUserGrid", ["UtilsService", "VRNotificationService", "Demo_Module_UserAPIService", "Demo_Module_UserService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_Module_UserAPIService, Demo_Module_UserService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var userGrid = new UserGrid($scope, ctrl, $attrs);
            userGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/User/templates/UserGridTemplate.html"

    };

    function UserGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.users = [];
            $scope.onGridReady = function (api) {

                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onUserAdded = function (userObject) {
                        gridAPI.itemAdded(userObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_Module_UserAPIService.GetFilteredUsers(dataRetrievalInput)
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
                clicked: editUser,

            }];
        }



        function editUser(userObj) {
            var onUserUpdated = function (userObj) {
                gridAPI.itemUpdated(userObj);
            };

            Demo_Module_UserService.editUser(userObj.Entity.Id, onUserUpdated);
        }

    }

    return directiveDefinitionObject;

}]);