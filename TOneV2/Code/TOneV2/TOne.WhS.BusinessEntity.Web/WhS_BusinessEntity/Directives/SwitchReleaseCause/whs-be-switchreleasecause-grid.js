"use strict";

app.directive("whsBeSwitchreleasecauseGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SwitchReleaseCauseAPIService", "WhS_BE_SwitchReleaseCauseService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SwitchReleaseCauseAPIService, WhS_BE_SwitchReleaseCauseService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var switchReleaseCauseGrid = new SwitchReleaseCauseGrid($scope, ctrl, $attrs);
            switchReleaseCauseGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SwitchReleaseCause/Templates/SwitchReleaseCauseGridTemplate.html"
    };

    function SwitchReleaseCauseGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;
        var filterobj = {};
        var gridAPI;
        function initializeController() {
            $scope.switcheReleaseCauses = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onSwitchReleaseCauseAdded = function (switchObject) {
                        gridAPI.itemAdded(switchObject);
                    };

                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SwitchReleaseCauseAPIService.GetFilteredSwitchReleaseCauses(dataRetrievalInput)
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
                clicked: editSwitchReleaseCause,
            }];
        }

        function editSwitchReleaseCause(switchReleaseCauseObject) {
            var onSwitchReleaseCauseUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            var switchReleaseCauseId = switchReleaseCauseObject.SwitchReleaseCauseId;

            WhS_BE_SwitchReleaseCauseService.editSwitchReleaseCause(switchReleaseCauseId, onSwitchReleaseCauseUpdated);
        }
    }
    return directiveDefinitionObject;
}]);