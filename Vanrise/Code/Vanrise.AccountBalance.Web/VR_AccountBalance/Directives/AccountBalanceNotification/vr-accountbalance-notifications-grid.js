'use strict';

app.directive('vrAccountbalanceNotificationsGrid', ['UtilsService', 'VR_AccountBalance_AccountBalanceNotificationTypeAPIService', 'VR_GenericData_DataRecordNotificationTypeSettingsService', 'VR_Notification_VRNotificationService', 'VR_Notification_VRAlertRuleService', 'BusinessProcess_BPInstanceService',
    function (UtilsService, VR_AccountBalance_AccountBalanceNotificationTypeAPIService, VR_GenericData_DataRecordNotificationTypeSettingsService, VR_Notification_VRNotificationService, VR_Notification_VRAlertRuleService, BusinessProcess_BPInstanceService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctr = new DataRecordNotificationGridDirective($scope, ctrl, $attrs);
                ctr.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationGridTemplate.html'
        };

        function DataRecordNotificationGridDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            //Variables
            var notificationTypeId;
            var notificationGridCtor;

            //API
            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.columns = [];
                $scope.scopeModel.vrNotifications = [];
                //$scope.scopeModel.menuActions = [];
                $scope.scopeModel.showAccountTypeColumn = false;
                $scope.scopeModel.accountColumnHeader = "";

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.loadMoreData = function () {
                    return notificationGridCtor.getData();
                };

                $scope.scopeModel.getStatusColor = function (dataItem) {
                    return VR_GenericData_DataRecordNotificationTypeSettingsService.getStatusColor(dataItem.Entity.Status);
                };

                $scope.scopeModel.getAlertLevelStyleColor = function (dataItem) {
                    return dataItem.AlertLevelStyle;
                };

                $scope.scopeModel.menuActions = function (dataItem) {
                    var menuActions = [];

                    if (dataItem.Entity.ParentTypes.ParentType2 != undefined) {
                        menuActions.push({
                            name: 'Matching Rule',
                            clicked: viewVRAlertRule
                        });
                    }

                    if (dataItem.Entity.ExecuteBPInstanceID != undefined) {
                        menuActions.push({
                            name: 'Action Process',
                            clicked: viewExecutedBPInstance
                        });
                    }

                    if (dataItem.Entity.ClearBPInstanceID != undefined) {
                        menuActions.push({
                            name: 'Rollback Process',
                            clicked: viewRolledBackBPInstance
                        });
                    }

                    function viewVRAlertRule(vrNotificationItem) {
                        VR_Notification_VRAlertRuleService.viewVRAlertRule(vrNotificationItem.Entity.ParentTypes.ParentType2);
                    }
                    function viewExecutedBPInstance(vrNotificationItem) {
                        BusinessProcess_BPInstanceService.openProcessTracking(vrNotificationItem.Entity.ExecuteBPInstanceID);
                    }
                    function viewRolledBackBPInstance(vrNotificationItem) {
                        BusinessProcess_BPInstanceService.openProcessTracking(vrNotificationItem.Entity.ClearBPInstanceID);
                    }

                    return menuActions;
                };

                //defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;

                    var promises = [];

                    var reloadColumns;

                    if (payload != undefined) {
                        reloadColumns = $scope.scopeModel.columns.length == 0 || notificationTypeId != payload.notificationTypeId;
                        notificationTypeId = payload.notificationTypeId;
                    }

                    if (reloadColumns) {
                        $scope.scopeModel.columns.length = 0;
                        var notificationGridColumnsLoadPromise = getNotificationGridColumnsLoadPromise(notificationTypeId);
                        promises.push(notificationGridColumnsLoadPromise);
                    }

                    notificationGridCtor = new VR_Notification_VRNotificationService.notificationGridCtor($scope, gridAPI, $scope.scopeModel.vrNotifications, payload);

                    //Retrieving Data
                    var gridLoadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        notificationGridCtor.onInitialization().then(function () {
                            $scope.scopeModel.isLoading = false;
                            gridLoadDeferred.resolve();
                        }).catch(function (error) {
                            gridLoadDeferred.reject(error);
                        });
                    }).catch(function (error) {
                        gridLoadDeferred.reject(error);
                    });

                    return gridLoadDeferred.promise;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getNotificationGridColumnsLoadPromise(notificationTypeId) {
                return VR_AccountBalance_AccountBalanceNotificationTypeAPIService.GetAccountBalanceNotificationTypeSettings(notificationTypeId).then(function (response) {
                    var accountBalanceNotificationTypeSettings = response;

                    if (accountBalanceNotificationTypeSettings != undefined) {
                        $scope.scopeModel.accountColumnHeader = accountBalanceNotificationTypeSettings.AccountColumnHeader;
                        $scope.scopeModel.showAccountTypeColumn = accountBalanceNotificationTypeSettings.ShowAccountTypeColumn;
                    }
                });
            }
        }
    }]);
