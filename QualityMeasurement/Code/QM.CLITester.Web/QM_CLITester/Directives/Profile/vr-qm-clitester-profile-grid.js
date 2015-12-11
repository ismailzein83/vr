"use strict";

app.directive("vrQmClitesterProfileGrid", ["UtilsService", "VRNotificationService", "QM_CLITester_ProfileAPIService", "QM_CLITester_ProfileService",
function (UtilsService, VRNotificationService, QM_CLITester_ProfileAPIService, QM_CLITester_ProfileService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
            //= (means function), @ (means attribute)
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var profileGrid = new ProfileGrid($scope, ctrl, $attrs);
            profileGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/QM_CLITester/Directives/Profile/Templates/ProfileGridTemplate.html"

    };

    function ProfileGrid($scope, ctrl, $attrs) {
        var gridAPI;

        function initializeController() {
            $scope.profiles = [];

            $scope.onGridReady = function (api) {

                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onProfileUpdated = function (profileObject) {
                        gridAPI.itemUpdated(profileObject);
                    }

                    directiveAPI.onProfileAdded = function (profileObject) {
                        gridAPI.itemAdded(profileObject);
                    }

                    return directiveAPI;
                }

            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return QM_CLITester_ProfileAPIService.GetFilteredProfiles(dataRetrievalInput)
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
                clicked: editProfile,
            }
            ];
        }

        function editProfile(profile) {
            var onProfileUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            QM_CLITester_ProfileService.editProfile(profile.Entity.ProfileId, onProfileUpdated);
        }


        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;

}]);
