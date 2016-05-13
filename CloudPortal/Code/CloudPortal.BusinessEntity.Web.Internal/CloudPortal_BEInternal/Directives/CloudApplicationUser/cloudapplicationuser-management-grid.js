"use strict";

app.directive("cloudportalBeinternalCloudapplicationUserManagementGrid", ["CloudPortal_BEInternal_CloudApplicationUserAPIService", 'VRNotificationService',
    function (CloudPortal_BEInternal_CloudApplicationUserAPIService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var userGrid = new UsersGrid($scope, ctrl, $attrs);
                userGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/CloudPortal_BEInternal/Directives/CloudApplicationUser/Templates/CloudApplicationUsersGrid.html"

        };

        function UsersGrid($scope, ctrl, $attrs) {

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
                        }

                        return directiveAPI;
                    }
                };

                $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return CloudPortal_BEInternal_CloudApplicationUserAPIService.GetFilteredCloudApplicationUsers(dataRetrievalInput)
                        .then(function (response) {
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                };

                //defineMenuActions();
            }
        }

        return directiveDefinitionObject;

    }]);