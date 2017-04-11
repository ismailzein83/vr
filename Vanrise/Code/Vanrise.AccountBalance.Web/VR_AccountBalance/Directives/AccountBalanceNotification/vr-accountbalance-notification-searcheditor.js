"use strict";

app.directive("vrAccountbalanceNotificationSearcheditor", ["UtilsService", "VRUIUtilsService", "VR_Notification_VRNotificationTypeAPIService",
    function (UtilsService, VRUIUtilsService, VR_Notification_VRNotificationTypeAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new NotificationTypeSettingsSearchEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationSearchEditor.html"
        };

        function NotificationTypeSettingsSearchEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var notificationTypeId;
            var accountBalanceNotificationTypeExtendedSettings;

            var directiveAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    var directivePayload = {
                        accountBalanceNotificationTypeExtendedSettings: accountBalanceNotificationTypeExtendedSettings
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, undefined);
                };

                $scope.scopeModel.showBasicTab = function () {
                    if (context == undefined)
                        return false;

                    return context.isNotificationTypeSettingSelected();
                };
                $scope.scopeModel.showAdvancedTab = function () {
                    if (context == undefined)
                        return false;

                    return context.isNotificationTypeSettingSelected() && context.isAdvancedTabSelected();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isSearchDirectiveLoading = true;

                    var promises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        notificationTypeId = payload.notificationTypeId;
                    }

                    var loadNotificationTypeSettings = getNotificationTypeSettings(notificationTypeId);
                    promises.push(loadNotificationTypeSettings);

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.isSearchDirectiveLoading = false;
                    });
                };

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.AccountBalance.Business.AccountBalanceNotificationQuery, Vanrise.AccountBalance.Business",
                        //CurrentBalance: undefined,
                        AccountBalanceNotificationExtendedQuery: directiveAPI != undefined ? directiveAPI.getData(): undefined
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getNotificationTypeSettings(notificationTypeId) {
                return VR_Notification_VRNotificationTypeAPIService.GetNotificationTypeSettings(notificationTypeId).then(function (response) {
                    var notificationTypeEntity = response;

                    if (notificationTypeEntity != undefined && notificationTypeEntity.ExtendedSettings != undefined) {
                        accountBalanceNotificationTypeExtendedSettings =  notificationTypeEntity.ExtendedSettings.AccountBalanceNotificationTypeExtendedSettings
                    }

                    if (accountBalanceNotificationTypeExtendedSettings != undefined) {
                        $scope.scopeModel.notificationQueryEditor = accountBalanceNotificationTypeExtendedSettings.NotificationQueryEditor;
                    }
                });
            }
        }

        return directiveDefinitionObject;
    }
]);