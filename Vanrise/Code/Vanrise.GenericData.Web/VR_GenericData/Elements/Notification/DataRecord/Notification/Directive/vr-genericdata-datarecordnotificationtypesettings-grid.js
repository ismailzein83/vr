﻿'use strict';

app.directive('vrGenericdataDatarecordnotificationtypesettingsGrid', ['UtilsService', 'VR_GenericData_DataRecordNotificationTypeSettingsAPIService', 'VR_GenericData_DataRecordNotificationTypeSettingsService', 'VR_Notification_VRNotificationService',
    function (UtilsService, VR_GenericData_DataRecordNotificationTypeSettingsAPIService, VR_GenericData_DataRecordNotificationTypeSettingsService, VR_Notification_VRNotificationService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Notification/Directive/Templates/NotificationTypeSettingsGridTemplate.html'
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
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;

                    var promises = [];

                    var reloadColumns = true;

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

                return VR_GenericData_DataRecordNotificationTypeSettingsAPIService.GetNotificationGridColumnAttributes(notificationTypeId).then(function (response) {
                    var notificationGridColumnAttributes = response;

                    if (notificationGridColumnAttributes != undefined) {
                        for (var index = 0; index < notificationGridColumnAttributes.length; index++) {
                            var notificationGridColumnAttribute = notificationGridColumnAttributes[index];
                            //var column = {
                            //    HeaderText: notificationGridColumnAttribute.Attribute.HeaderText,
                            //    Field: notificationGridColumnAttribute.Attribute.Field,
                            //    Type: notificationGridColumnAttribute.Attribute.Type
                            //};
                            $scope.scopeModel.columns.push(notificationGridColumnAttribute.Attribute);
                        }
                    }
                });
            }
        }
    }]);
