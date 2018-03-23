'use strict';

app.directive('vrNotificationVralertruleGrid', ['VR_Notification_VRAlertRuleAPIService', 'VR_Notification_VRAlertRuleService', 'VRNotificationService', 'VRUIUtilsService',
    function (VR_Notification_VRAlertRuleAPIService, VR_Notification_VRAlertRuleService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrAlertRuleGrid = new VRAlertRuleGrid($scope, ctrl, $attrs);
                vrAlertRuleGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Notification/Directives/VRAlertRule/Templates/VRAlertRuleGridTemplate.html'
        };

        function VRAlertRuleGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridDrillDownTabsObj;
            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrAlertRule = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Notification_VRAlertRuleService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Notification_VRAlertRuleAPIService.GetFilteredVRAlertRules(dataRetrievalInput).then(function (response) {
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

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onVRAlertRuleAdded = function (addedVRAlertRule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedVRAlertRule);
                    gridAPI.itemAdded(addedVRAlertRule);

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = function (dataItem) {
                    var menu = [];
                    if (dataItem.AllowEdit == true) {
                        menu.push({
                            name: 'Edit',
                            clicked: editVRAlertRule
                        });

                        if (dataItem.Entity.IsDisabled == true) {
                            menu.push({
                                name: 'Enable',
                                clicked: enableVRAlertRule
                            });
                        }
                        else {
                            menu.push({
                                name: 'Disable',
                                clicked: disableVRAlertRule
                            });
                        }
                    }
                    return menu;
                };
            }
            function editVRAlertRule(vrAlertRuleItem) {
                var onVRAlertRuleUpdated = function (updatedVRAlertRule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVRAlertRule);
                    gridAPI.itemUpdated(updatedVRAlertRule);
                    $scope.scopeModel.menuActions(updatedVRAlertRule);
                };

                VR_Notification_VRAlertRuleService.editVRAlertRule(vrAlertRuleItem.Entity.VRAlertRuleId, onVRAlertRuleUpdated);
            }

            function enableVRAlertRule(dataItem) {
                var onAlertRuleEnabled = function (updatedVRAlertRule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVRAlertRule);
                    gridAPI.itemUpdated(updatedVRAlertRule);
                    $scope.scopeModel.menuActions(updatedVRAlertRule);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return VR_Notification_VRAlertRuleAPIService.EnableVRAlertRule({ VRAlertRuleId: dataItem.Entity.VRAlertRuleId, RuleTypeId: dataItem.Entity.RuleTypeId }).then(function (response) {
                            if (onAlertRuleEnabled && typeof onAlertRuleEnabled == 'function') {
                                onAlertRuleEnabled(response.UpdatedObject);
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                });
            }

            function disableVRAlertRule(dataItem) {
                var onAlertRuleDisabled = function (updatedVRAlertRule) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedVRAlertRule);
                    gridAPI.itemUpdated(updatedVRAlertRule);
                    $scope.scopeModel.menuActions(updatedVRAlertRule);
                };

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        return VR_Notification_VRAlertRuleAPIService.DisableVRAlertRule({ VRAlertRuleId: dataItem.Entity.VRAlertRuleId, RuleTypeId: dataItem.Entity.RuleTypeId }).then(function (response) {
                            if (onAlertRuleDisabled && typeof onAlertRuleDisabled == 'function') {
                                onAlertRuleDisabled(response.UpdatedObject);
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
                });
            }
        }
    }]);
