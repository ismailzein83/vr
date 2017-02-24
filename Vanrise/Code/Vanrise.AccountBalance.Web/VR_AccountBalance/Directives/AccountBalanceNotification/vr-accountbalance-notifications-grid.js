'use strict';

app.directive('vrAccountbalanceNotificationsGrid', ['VR_Notification_VRNotificationsAPIService', 'VRTimerService',
    function (VR_Notification_VRNotificationsAPIService, VRTimerService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cstr = new NotificationNotificationsGridDirective($scope, ctrl, $attrs);
                cstr.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationGridTemplate.html'
        };

        function NotificationNotificationsGridDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var lessThanID, greaterThanId, nbOfRows, vrNotificationTypeId;
            var input = {
                LessThanID: lessThanID,
                GreaterThanID: greaterThanId,
                NbOfRows: nbOfRows,
                NotificationTypeId: vrNotificationTypeId
            };

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrNotifications = [];

                $scope.scopeModel.loadMoreData = function () {
                    return getData();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(defineAPI());

                };

                $scope.scopeModel.searchClicked = function () {
                    onInit();
                };
            }

            function defineAPI() {
                var directiveAPI = {};
                directiveAPI.loadGrid = function (query) {
                    input.vrNotificationTypeId = query.NotificationTypeId;

                    onInit();
                };

                directiveAPI.clearTimer = function () {
                    if ($scope.job) {
                        VRTimerService.unregisterJob($scope.job);
                    }
                };
                return directiveAPI;
            }

            function getData() {

                var pageInfo = gridAPI.getPageInfo();
                input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                return VR_Notification_VRNotificationsAPIService.GetBeforeIdVRNotifications(input).then(function (response) {
                    if (response != undefined && response) {
                        for (var i = 0; i < response.length; i++) {
                            var vrNotification = response[i];
                            $scope.scopeModel.vrNotifications.push(vrNotification);

                        }
                        $scope.scopeModel.vrNotifications.sort(function (a, b) {
                            return b.Entity.Id - a.Entity.Id;
                        });
                        input.LessThanID = $scope.scopeModel.vrNotifications[$scope.scopeModel.vrNotifications.length - 1].Entity.Id;
                        input.GreaterThanID = $scope.scopeModel.vrNotifications[0].Entity.Id;
                    }
                });
            }

            function onInit() {
                $scope.scopeModel.isLoading = true;
                input.LessThanID = undefined;
                input.GreaterThanID = undefined;
                input.NbOfRows = undefined;
                $scope.scopeModel.vrNotifications.length = 0;
                createTimer();
            }

            function manipulateDataUpdated(response) {
                var itemAddedOrUpdatedInThisCall = false;
                if (response != undefined) {
                    for (var i = 0; i < response.length; i++) {
                        var vrNotification = response[i];

                        itemAddedOrUpdatedInThisCall = true;
                        $scope.scopeModel.vrNotifications.push(vrNotification);

                    }

                    if (itemAddedOrUpdatedInThisCall) {
                        if ($scope.scopeModel.vrNotifications.length > 0) {
                            $scope.scopeModel.vrNotifications.sort(function (a, b) {
                                return b.Entity.Id - a.Entity.Id;
                            });

                            if ($scope.scopeModel.vrNotifications.length > BusinessProcess_GridMaxSize.maximumCount) {
                                $scope.scopeModel.vrNotifications.length = BusinessProcess_GridMaxSize.maximumCount;
                            }
                            input.LessThanID = $scope.scopeModel.vrNotifications[$scope.scopeModel.vrNotifications.length - 1].Entity.Id;
                            input.GreaterThanID = $scope.scopeModel.vrNotifications[0].Entity.Id;
                        }
                    }
                }
            }

            function createTimer() {
                if ($scope.job) {
                    VRTimerService.unregisterJob($scope.job);
                }
                var pageInfo = gridAPI.getPageInfo();
                input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                VRTimerService.registerJob(onTimerElapsed, $scope);
            }

            function onTimerElapsed() {
                return VR_Notification_VRNotificationsAPIService.GetUpdatedVRNotifications(input).then(function (response) {
                    manipulateDataUpdated(response);
                    $scope.scopeModel.isLoading = false;
                },
                 function (excpetion) {
                     console.log(excpetion);
                     $scope.scopeModel.isLoading = false;
                 });
            }
        }
    }]);
