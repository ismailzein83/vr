"use strict";

app.directive("vrCommonSettingsGrid", ["UtilsService", "VRNotificationService", "VRCommon_SettingsAPIService", "VRCommon_SettingsService",
function (UtilsService, VRNotificationService, VRCommon_SettingsAPIService, VRCommon_SettingsService) {
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SettingsGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Settings/Templates/SettingsGridTemplate.html"

    };

    function SettingsGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.settings = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_SettingsAPIService.GetFilteredSettings(dataRetrievalInput)
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
            $scope.gridMenuActions = function (dataItem) {
                return [{
                    name: "Edit",
                    clicked: editSettings,
                    haspermission: function (dataItem) {
                        if (dataItem.Entity.IsTechnical == true) {
                            return VRCommon_SettingsAPIService.HasUpdateTechnicalSettingsPermission();

                        }
                        else {
                            return VRCommon_SettingsAPIService.HasUpdateSettingsPermission();
                        }
                    }
                }];
            };
        };

        function editSettings(settings) {
            var onSettingsUpdated = function (settingsObj) {
                gridAPI.itemUpdated(settingsObj);
            };
            VRCommon_SettingsService.editSettings(settings.Entity.SettingId, onSettingsUpdated);
        }
    }

    return directiveDefinitionObject;

}]);
