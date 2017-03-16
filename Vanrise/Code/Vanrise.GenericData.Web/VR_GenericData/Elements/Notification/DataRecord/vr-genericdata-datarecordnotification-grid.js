'use strict';

app.directive('vrGenericdataDatarecordnotificationGrid', ['VR_Notification_VRNotificationsAPIService', 'VRTimerService', 'BusinessProcess_GridMaxSize',
    function (VR_Notification_VRNotificationsAPIService, VRTimerService, BusinessProcess_GridMaxSize) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Templates/DataRecordNotificationGridTemplate.html'
        };

        function DataRecordNotificationGridDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var lessThanID;
            var greaterThanId;
            var nbOfRows;
            var vrNotificationTypeId;

            var input = {
                NotificationTypeId: vrNotificationTypeId,
                NbOfRows: nbOfRows,
                LessThanID: lessThanID,
                GreaterThanID: greaterThanId
            };

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vrNotifications = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.loadMoreData = function () {
                    return getData();
                };

                $scope.scopeModel.searchClicked = function () {
                    onInit();
                };
            }
            function defineAPI() {
                var api = {};

                api.loadGrid = function (query) {

                    console.log(query);

                    //HardCoded
                    input.NotificationTypeId = "6BB06963-AC64-4827-A7FC-EB6892057AD7";
                    onInit();
                };

                api.clearTimer = function () {
                    if ($scope.job) {
                        VRTimerService.unregisterJob($scope.job);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getData() {
                var pageInfo = gridAPI.getPageInfo();
                input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;

                return VR_Notification_VRNotificationsAPIService.GetBeforeIdVRNotifications(input).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var vrNotification = response[i];
                            $scope.scopeModel.vrNotifications.push(vrNotification);
                        }

                        $scope.scopeModel.vrNotifications.sort(function (a, b) {
                            return b.Entity.VRNotificationId - a.Entity.VRNotificationId;
                        });
                        input.LessThanID = $scope.scopeModel.vrNotifications[$scope.scopeModel.vrNotifications.length - 1].Entity.VRNotificationId;
                        input.GreaterThanID = $scope.scopeModel.vrNotifications[0].Entity.VRNotificationId;
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
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
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
                                return b.Entity.VRNotificationId - a.Entity.VRNotificationId;
                            });

                            if ($scope.scopeModel.vrNotifications.length > BusinessProcess_GridMaxSize.maximumCount) {
                                $scope.scopeModel.vrNotifications.length = BusinessProcess_GridMaxSize.maximumCount;
                            }
                            input.LessThanID = $scope.scopeModel.vrNotifications[$scope.scopeModel.vrNotifications.length - 1].Entity.VRNotificationId;
                            input.GreaterThanID = $scope.scopeModel.vrNotifications[0].Entity.VRNotificationId;
                        }
                    }
                }
            }
        }
    }]);
