'use strict';

app.directive('vrGenericdataDatarecordnotificationtypesettingsGrid', ['UtilsService', 'VR_Notification_VRNotificationsAPIService', 'VRTimerService', 'BusinessProcess_GridMaxSize', 'VR_GenericData_DataRecordNotificationTypeSettingsAPIService', 'VR_GenericData_DataRecordNotificationTypeSettingsService',
    function (UtilsService, VR_Notification_VRNotificationsAPIService, VRTimerService, BusinessProcess_GridMaxSize, VR_GenericData_DataRecordNotificationTypeSettingsAPIService, VR_GenericData_DataRecordNotificationTypeSettingsService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Directive/Templates/NotificationTypeSettingsGridTemplate.html'
        };

        function DataRecordNotificationGridDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            //Variables
            var reloadColumns;
            var isGettingDataFirstTime, minId;
            var notificationTypeId, lastUpdateHandle, nbOfRows, lessThanID, query, extendedQuery; //Input Parameters

            //API
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

                $scope.getStatusColor = function (dataItem) {
                    return VR_GenericData_DataRecordNotificationTypeSettingsService.getStatusColor(dataItem.Entity.Status);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;

                    var promises = [];

                    if (payload != undefined) {
                        reloadColumns = notificationTypeId != payload.notificationTypeId;
                        notificationTypeId = payload.notificationTypeId;
                        query = payload.query;
                        extendedQuery = payload.extendedQuery;
                    }

                    if ($scope.scopeModel.columns.length == 0 || reloadColumns) {
                        $scope.scopeModel.columns.length = 0;
                        var notificationGridColumnsLoadPromise = getNotificationGridColumnsLoadPromise();
                        promises.push(notificationGridColumnsLoadPromise);
                    }

                    //Retrieving Data
                    var gridLoadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises(promises).then(function () {
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
                lessThanID = minId;
                var pageInfo = gridAPI.getPageInfo();
                nbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;

                return VR_Notification_VRNotificationsAPIService.GetBeforeIdVRNotifications(buildVRNotificationBeforeIdInput()).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var vrNotification = response[i];
                            minId = vrNotification.Entity.VRNotificationId;
                            $scope.scopeModel.vrNotifications.push(vrNotification);
                        }
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
                isGettingDataFirstTime = true;
                minId = undefined;
                nbOfRows = undefined;
                lastUpdateHandle = undefined;
                lessThanID = undefined;

                var pageInfo = gridAPI.getPageInfo();
                nbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;

                return VR_Notification_VRNotificationsAPIService.GetFirstPageVRNotifications(buildVRNotificationFirstPageInput()).then(function (response) {

                    console.log(response);

                    manipulateDataUpdated(response);
                    $scope.scopeModel.isLoading = false;
                    createTimer();
                });
            }
            function createTimer() {
                if ($scope.job) {
                    VRTimerService.unregisterJob($scope.job);
                }

                VRTimerService.registerJob(onTimerElapsed, $scope);
            }
            function onTimerElapsed() {
                return VR_Notification_VRNotificationsAPIService.GetUpdatedVRNotifications(buildVRNotificationUpdateInput()).then(function (response) {

                    console.log(response);

                    manipulateDataUpdated(response);
                });
            }
            function manipulateDataUpdated(response) {
                var itemAddedOrUpdatedInThisCall = false;

                if (response != undefined && response.VRNotificationDetails.length > 0) {
                    itemAddedOrUpdatedInThisCall = true;

                    for (var i = 0; i < response.VRNotificationDetails.length; i++) {
                        var vrNotificationDetail = response.VRNotificationDetails[i];

                        if (!isGettingDataFirstTime && vrNotificationDetail.Entity.VRNotificationId < minId)
                            continue;

                        var isVRNotificationDetailUpdated = false;
                        var isItemStatusMatchFilter = isStatusMatched(vrNotificationDetail.Entity.Status);

                        for (var j = 0; j < $scope.scopeModel.vrNotifications.length; j++) {
                            if ($scope.scopeModel.vrNotifications[j].Entity.VRNotificationId == vrNotificationDetail.Entity.VRNotificationId) {
                                if (!isItemStatusMatchFilter) {
                                    $scope.scopeModel.vrNotifications.splice(j, 1);
                                } else {
                                    $scope.scopeModel.vrNotifications[j] = vrNotificationDetail;
                                }
                                isVRNotificationDetailUpdated = true;
                                continue;
                            }
                        }
                        if (!isVRNotificationDetailUpdated) {
                            if (isItemStatusMatchFilter) {
                                $scope.scopeModel.vrNotifications.push(vrNotificationDetail);
                            }
                        }
                    }

                    if (itemAddedOrUpdatedInThisCall) {
                        if ($scope.scopeModel.vrNotifications.length > 0) {
                            $scope.scopeModel.vrNotifications.sort(function (a, b) {
                                return b.Entity.VRNotificationId - a.Entity.VRNotificationId;
                            });

                            if ($scope.scopeModel.vrNotifications.length > BusinessProcess_GridMaxSize.maximumCount) {
                                $scope.scopeModel.vrNotifications.length = BusinessProcess_GridMaxSize.maximumCount;
                            }
                            var minIdIndex = $scope.scopeModel.vrNotifications.length - 1;
                            minId = $scope.scopeModel.vrNotifications[minIdIndex].Entity.VRNotificationId;
                            isGettingDataFirstTime = false;
                        }
                    }
                }
                lastUpdateHandle = response.MaxTimeStamp;
            }

            function isStatusMatched(statusId) {
                if (!isGettingDataFirstTime && query != undefined && query.StatusIds != undefined)
                    return UtilsService.contains(query.StatusIds, statusId);
                return true;
            }

            function buildVRNotificationFirstPageInput() {
                return {
                    $type: " Vanrise.Notification.Entities.VRNotificationFirstPageInput, Vanrise.Notification.Entities",
                    NotificationTypeId: notificationTypeId,
                    NbOfRows: nbOfRows,
                    Query: query,
                    ExtendedQuery: extendedQuery
                };
            }
            function buildVRNotificationUpdateInput() {
                return {
                    $type: " Vanrise.Notification.Entities.VRNotificationUpdateInput, Vanrise.Notification.Entities",
                    NotificationTypeId: notificationTypeId,
                    LastUpdateHandle: lastUpdateHandle,
                    NbOfRows: nbOfRows,
                    Query: query,
                    ExtendedQuery: extendedQuery
                };
            }
            function buildVRNotificationBeforeIdInput() {
                return {
                    $type: " Vanrise.Notification.Entities.VRNotificationBeforeIdInput, Vanrise.Notification.Entities",
                    NotificationTypeId: notificationTypeId,
                    LessThanID: lessThanID,
                    NbOfRows: nbOfRows,
                    Query: query,
                    ExtendedQuery: extendedQuery
                };
            }
        }
    }]);
