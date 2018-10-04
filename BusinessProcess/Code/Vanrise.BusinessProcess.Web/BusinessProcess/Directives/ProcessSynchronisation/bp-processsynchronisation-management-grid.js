"use strict";

app.directive("businessprocessProcesssynchronisationManagementGrid", ["UtilsService", "VRNotificationService", "BusinessProcess_ProcessSynchronisationAPIService", "BusinessProcess_ProcessSynchronisationService", "VRUIUtilsService", "VRTimerService",
    function (UtilsService, VRNotificationService, BusinessProcess_ProcessSynchronisationAPIService, BusinessProcess_ProcessSynchronisationService, VRUIUtilsService, VRTimerService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var processSynchronisationManagementGrid = new ProcesssSynchronisationManagementGrid($scope, ctrl, $attrs);
                processSynchronisationManagementGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/ProcessSynchronisation/Templates/ProcessSynchronisationManagementGridTemplate.html"
        };

        function ProcesssSynchronisationManagementGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var gridDrillDownTabsObj;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.processesSynchronisation = [];
                $scope.scopeModel.gridMenuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = BusinessProcess_ProcessSynchronisationService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.scopeModel.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return BusinessProcess_ProcessSynchronisationAPIService.GetFilteredProcessesSynchronisations(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onProcessSynchronisationAdded = function (addedProcessSynchronisation) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedProcessSynchronisation);
                    gridAPI.itemAdded(addedProcessSynchronisation);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = function (processSynchronisationItem) {
                    var menuActions = [];

                    menuActions.push({
                        name: 'Edit',
                        clicked: editProcessSynchronisation,
                        haspermission: HasUpdateProcessSynchronisationPermission
                    });
                    if (processSynchronisationItem.IsEnabled) {
                        menuActions.push({
                            name: 'Disable',
                            clicked: disableProcessSynchronisation
                        });
                    }
                    else {
                        menuActions.push({
                            name: 'Enable',
                            clicked: enableProcessSynchronisation
                        });
                    }

                    return menuActions;
                };
            }

            function editProcessSynchronisation(processSynchronisationItem) {
                var onProcessSynchronisationUpdated = function (updatedProcessSynchronisation) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedProcessSynchronisation);
                    gridAPI.itemUpdated(updatedProcessSynchronisation);
                };


                BusinessProcess_ProcessSynchronisationService.editProcessSynchronisation(processSynchronisationItem.ProcessSynchronisationId, onProcessSynchronisationUpdated);
            }
            function HasUpdateProcessSynchronisationPermission() {
                return BusinessProcess_ProcessSynchronisationAPIService.HasUpdateProcessSynchronisationPermission();
            }

            function enableProcessSynchronisation(processSynchronisationItem) {
                var onProcessSynchronisationEnabled = function (updatedProcessSynchronisation) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedProcessSynchronisation);
                    gridAPI.itemUpdated(updatedProcessSynchronisation);
                    $scope.scopeModel.gridMenuActions(updatedProcessSynchronisation);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return BusinessProcess_ProcessSynchronisationAPIService.EnableProcessSynchronisation(processSynchronisationItem.ProcessSynchronisationId).then(function (response) {
                            if (onProcessSynchronisationEnabled && typeof onProcessSynchronisationEnabled == 'function') {
                                onProcessSynchronisationEnabled(response.UpdatedObject);
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                });
            }

            function disableProcessSynchronisation(processSynchronisationItem) {
                var onProcessSynchronisationDisabled = function (updatedProcessSynchronisation) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedProcessSynchronisation);
                    gridAPI.itemUpdated(updatedProcessSynchronisation);
                    $scope.scopeModel.gridMenuActions(updatedProcessSynchronisation);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return BusinessProcess_ProcessSynchronisationAPIService.DisableProcessSynchronisation(processSynchronisationItem.ProcessSynchronisationId).then(function (response) {
                            if (onProcessSynchronisationDisabled && typeof onProcessSynchronisationDisabled == 'function') {
                                onProcessSynchronisationDisabled(response.UpdatedObject);
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                });
            }
        }

        return directiveDefinitionObject;
    }]);