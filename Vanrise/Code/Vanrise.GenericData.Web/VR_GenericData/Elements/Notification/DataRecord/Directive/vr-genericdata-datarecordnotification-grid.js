'use strict';

app.directive('vrGenericdataDatarecordnotificationGrid', ['UtilsService', 'VR_Notification_VRNotificationsAPIService', 'VRTimerService', 'BusinessProcess_GridMaxSize', 'VR_GenericData_DataRecordNotificationTypeSettingsAPIService',
    function (UtilsService, VR_Notification_VRNotificationsAPIService, VRTimerService, BusinessProcess_GridMaxSize, VR_GenericData_DataRecordNotificationTypeSettingsAPIService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Directive/Templates/DataRecordNotificationGridTemplate.html'
        };

        function DataRecordNotificationGridDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var reloadColumns;
            var notificationTypeId;
            var searchQuery;

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
                $scope.scopeModel.columns = [];
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

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        reloadColumns = notificationTypeId != payload.notificationTypeId;
                        notificationTypeId = payload.notificationTypeId;
                        searchQuery = payload.query;
                    }

                    if ($scope.scopeModel.columns.length == 0 || reloadColumns) {
                        $scope.scopeModel.columns.length = 0;
                        var notificationGridColumnsLoadPromise = getNotificationGridColumnsLoadPromise();
                        promises.push(notificationGridColumnsLoadPromise);
                    }

                    //Retrieving Data
                    var gridLoadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises(promises).then(function () {
                        input.NotificationTypeId = notificationTypeId;
                        onInit();
                        gridLoadDeferred.resolve();
                    }).catch(function (error) {
                        gridLoadDeferred.reject(error);
                    });

                    return gridLoadDeferred.promise;
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

            function getNotificationGridColumnsLoadPromise() {
                var notificationGridColumnAttributesLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                VR_GenericData_DataRecordNotificationTypeSettingsAPIService.GetNotificationGridColumnAttributes(notificationTypeId).then(function (response) {
                    var notificationGridColumnAttributes = response;

                    if (notificationGridColumnAttributes != undefined) {
                        for (var index = 0; index < notificationGridColumnAttributes.length; index++) {
                            var notificationGridColumnAttribute = notificationGridColumnAttributes[index];
                            var column = {
                                HeaderText: notificationGridColumnAttribute.Attribute.HeaderText,
                                Field: notificationGridColumnAttribute.Attribute.Field,
                                Type: notificationGridColumnAttribute.Attribute.Type
                            };
                            $scope.scopeModel.columns.push(column);
                        }
                    }
                    notificationGridColumnAttributesLoadPromiseDeferred.resolve();
                }).catch(function (error) {
                    notificationGridColumnAttributesLoadPromiseDeferred.reject(error);
                });

                return notificationGridColumnAttributesLoadPromiseDeferred.promise;
            }

            function onInit() {
                $scope.scopeModel.vrNotifications.length = 0;
                $scope.scopeModel.isLoading = true;
                input.GreaterThanID = undefined;
                input.LessThanID = undefined;
                input.NbOfRows = undefined;

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
