"use strict";

app.directive("vrSecMenuViewGrid", ['VRNotificationService', 'VR_Sec_ViewAPIService', function (VRNotificationService, VR_Sec_ViewAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var viewsGrid = new ViewsGrid($scope, ctrl, $attrs);
            viewsGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/Menu/View/Templates/ViewGridTemplate.html"

    };

    function ViewsGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.views = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_ViewAPIService.GetFilteredViews(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            //$scope.gridMenuActions = [{
            //    name: "Edit",
            //    clicked: editUser,
            //    haspermission: hasUpdateUserPermission
            //}, {
            //    name: "Reset Password",
            //    clicked: resetPassword,
            //    haspermission: hasResetUserPasswordPermission
            //}, {
            //    name: "Assign Permissions",
            //    clicked: assignPermissions,
            //    haspermission: hasUpdateSystemEntityPermissionsPermission // System Entities:Assign Permissions
            //}];
        }

        function editView(viewObj) {
            //var onUserUpdated = function (userObj) {
            //    gridAPI.itemUpdated(userObj);
            //}

            //VR_Sec_UserService.editUser(userObj.Entity.UserId, onUserUpdated);
        }
    }

    return directiveDefinitionObject;

}]);