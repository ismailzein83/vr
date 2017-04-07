(function (appControllers) {

    "use strict";

    VRNotificationService.$inject = ['UtilsService', 'VR_Notification_VRNotificationsAPIService', 'VRTimerService', 'VR_Notification_GridMaxSize'];

    function VRNotificationService(UtilsService, VR_Notification_VRNotificationsAPIService, VRTimerService, VR_Notification_GridMaxSize) {

        function notificationGridCtor(scope, gridAPI, gridDataSource, gridPayload) {

            var notificationTypeId, query, extendedQuery;
            var isGettingDataFirstTime, minId, nbOfRows, lastUpdateHandle, lessThanID;

            if (gridPayload != undefined) {
                notificationTypeId = gridPayload.notificationTypeId;
                query = gridPayload.query;
                extendedQuery = gridPayload.extendedQuery;
            };

            function onInitialization() {
                gridDataSource.length = 0;
                isGettingDataFirstTime = true;
                minId = undefined;
                nbOfRows = undefined;
                lastUpdateHandle = undefined;
                lessThanID = undefined;

                var pageInfo = gridAPI.getPageInfo();
                nbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;

                return createTimer();
            };

            function createTimer() {
                if (scope.job) {
                    VRTimerService.unregisterJob(scope.job);
                }

                return VR_Notification_VRNotificationsAPIService.GetFirstPageVRNotifications(buildVRNotificationFirstPageInput()).then(function (response) {
                    manipulateDataUpdated(response);
                    isGettingDataFirstTime = false;
                    VRTimerService.registerJob(onTimerElapsed, scope);
                });
            };

            function onTimerElapsed(jobId) {
                return VR_Notification_VRNotificationsAPIService.GetUpdatedVRNotifications(buildVRNotificationUpdateInput()).then(function (response) {
                    if (scope.job.id == jobId)
                        manipulateDataUpdated(response);
                });
            };

            function manipulateDataUpdated(response) {
                var itemAddedOrUpdatedInThisCall = false;

                if (response != undefined && response.VRNotificationDetails != undefined && response.VRNotificationDetails.length > 0) {
                    itemAddedOrUpdatedInThisCall = true;

                    for (var i = 0; i < response.VRNotificationDetails.length; i++) {
                        var vrNotificationDetail = response.VRNotificationDetails[i];

                        if (!isGettingDataFirstTime && vrNotificationDetail.Entity.VRNotificationId < minId)
                            continue;

                        var isVRNotificationDetailUpdated = false;
                        var isItemStatusMatchFilter = isStatusMatched(vrNotificationDetail.Entity.Status);

                        for (var j = 0; j < gridDataSource.length; j++) {
                            if (gridDataSource[j].Entity.VRNotificationId == vrNotificationDetail.Entity.VRNotificationId) {
                                if (!isItemStatusMatchFilter) {
                                    gridDataSource.splice(j, 1);
                                } else {
                                    gridDataSource[j] = vrNotificationDetail;
                                }
                                isVRNotificationDetailUpdated = true;
                                continue;
                            }
                        }
                        if (!isVRNotificationDetailUpdated) {
                            if (isItemStatusMatchFilter) {
                                gridDataSource.push(vrNotificationDetail);
                            }
                        }
                    }

                    if (itemAddedOrUpdatedInThisCall) {
                        if (gridDataSource.length > 0) {
                            gridDataSource.sort(function (a, b) {
                                return b.Entity.VRNotificationId - a.Entity.VRNotificationId;
                            });

                            if (gridDataSource.length > VR_Notification_GridMaxSize.maximumCount) {
                                gridDataSource.length = VR_Notification_GridMaxSize.maximumCount;
                            }
                            var minIdIndex = gridDataSource.length - 1;
                            minId = gridDataSource[minIdIndex].Entity.VRNotificationId;
                        }
                    }
                }
                lastUpdateHandle = response.MaxTimeStamp;
            };

            function isStatusMatched(statusId) {
                if (!isGettingDataFirstTime && query != undefined && query.StatusIds != undefined)
                    return UtilsService.contains(query.StatusIds, statusId);
                return true;
            };

            function buildVRNotificationFirstPageInput() {
                return {
                    $type: "Vanrise.Notification.Entities.VRNotificationFirstPageInput, Vanrise.Notification.Entities",
                    NotificationTypeId: notificationTypeId,
                    NbOfRows: nbOfRows,
                    Query: query,
                    ExtendedQuery: extendedQuery
                };
            };

            function buildVRNotificationUpdateInput() {
                return {
                    $type: "Vanrise.Notification.Entities.VRNotificationUpdateInput, Vanrise.Notification.Entities",
                    NotificationTypeId: notificationTypeId,
                    LastUpdateHandle: lastUpdateHandle,
                    NbOfRows: nbOfRows,
                    Query: query,
                    ExtendedQuery: extendedQuery
                };
            };

            function buildVRNotificationBeforeIdInput() {
                return {
                    $type: "Vanrise.Notification.Entities.VRNotificationBeforeIdInput, Vanrise.Notification.Entities",
                    NotificationTypeId: notificationTypeId,
                    LessThanID: lessThanID,
                    NbOfRows: nbOfRows,
                    Query: query,
                    ExtendedQuery: extendedQuery
                };
            };

            function getData() {
                lessThanID = minId;

                return VR_Notification_VRNotificationsAPIService.GetBeforeIdVRNotifications(buildVRNotificationBeforeIdInput()).then(function (response) {
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var vrNotification = response[i];
                            minId = vrNotification.Entity.VRNotificationId;
                            gridDataSource.push(vrNotification);
                        }
                    }
                });
            };

            return {
                onInitialization: onInitialization,
                getData: getData
            };
        };

        return {
            notificationGridCtor: notificationGridCtor
        };
    }

    appControllers.service('VR_Notification_VRNotificationService', VRNotificationService);

})(appControllers);