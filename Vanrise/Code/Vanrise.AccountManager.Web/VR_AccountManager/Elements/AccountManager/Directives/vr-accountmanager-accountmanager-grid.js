"use strict";

app.directive("vrAccountmanagerAccountmanagerGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VRCommon_ObjectTrackingService", "VR_AccountManager_AccountManagerAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService, VRCommon_ObjectTrackingService, VR_AccountManager_AccountManagerAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountManagerGrid = new AccountManagerGrid($scope, ctrl, $attrs);
            accountManagerGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerGrid.html'
    };

    function AccountManagerGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;
        
        function initializeController()
        {
            var gridAPI;
            $scope.accountManagers = [];
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

                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_AccountManager_AccountManagerAPIService.GetFilteredAccountManagers(dataRetrievalInput)
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
                clicked: editAccountmanager,
            }];
        }

        function editAccountmanager() {
          
        }
    }
    return directiveDefinitionObject;
}]);