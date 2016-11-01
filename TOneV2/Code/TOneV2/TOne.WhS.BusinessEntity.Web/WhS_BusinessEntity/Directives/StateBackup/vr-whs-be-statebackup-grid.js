"use strict";

app.directive("vrWhsBeStatebackupGrid", ["UtilsService", "VRNotificationService", "WhS_BE_StateBackupAPIService",
function (UtilsService, VRNotificationService, WhS_BE_StateBackupAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new StateBackupsGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/StateBackup/Templates/StateBackupGridTemplate.html"

    };

    function StateBackupsGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gquery;
        this.initializeController = initializeController;

        function initializeController() {
           
            $scope.stateBackups = [];

            defineMenuActions();

            $scope.onGridReady = function (api) {
                gridAPI = api;
                
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                   
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        gquery = query;
                        return gridAPI.retrieveData(query);
                    }
                   
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_StateBackupAPIService.GetFilteredStateBackups(dataRetrievalInput)
                    .then(function (response) {
                         onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }


        function defineMenuActions() {
            var menuActions = [{
                name: "Restore",
                clicked: restoreStateBackup,
                haspermission: hasRestorePermission
            }];

            $scope.gridMenuActions = function (dataItem) {
                if (dataItem.Entity.RestoreDate == null)
                    return menuActions;
            };
        }

        function hasRestorePermission() {
            return WhS_BE_StateBackupAPIService.HasRestoreDataPermission();
        }
        function restoreStateBackup(stateBackupObject) {
            var onStateBackupRestored = function (stateBackup) {
                return gridAPI.retrieveData(gquery);
            }
            return VRNotificationService.showConfirmation("Are you sure you want to restore the selected backup").then(function (result) {
                if (result) {
                    ctrl.isRestoring = true;
                    return WhS_BE_StateBackupAPIService.RestoreData(stateBackupObject.Entity.StateBackupId).then(function (response) {
                        if (VRNotificationService.notifyOnItemUpdated("State Backup", response, ""))
                            onStateBackupRestored(response.UpdatedObject);
                            ctrl.isRestoring = false;
                    });
                }
            });

        }


    }

    return directiveDefinitionObject;

}]);
