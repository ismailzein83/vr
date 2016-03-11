"use strict";

app.directive("vrSecMenuViewGrid", ['VRNotificationService', 'VR_Sec_ViewAPIService','VR_Sec_ViewService', function (VRNotificationService, VR_Sec_ViewAPIService, VR_Sec_ViewService) {

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
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editView,
                haspermission: hasUpdateViewPermission // System Entities:Assign Permissions
            }];
        }
        function hasUpdateViewPermission() {
            return VR_Sec_ViewAPIService.HasUpdateViewPermission();
        }
        function editView(viewObj) {
            var onViewUpdated = function (viewObj) {
                gridAPI.itemUpdated(viewObj);
            }

            VR_Sec_ViewService.editView(viewObj.Entity.ViewId, onViewUpdated);
        }
    }

    return directiveDefinitionObject;

}]);