"use strict";

app.directive("vrGenericdataDatarecordnotificationtypesettingsSearcheditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Notification_VRNotificationTypeAPIService", 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Notification_VRNotificationTypeAPIService, VR_GenericData_DataRecordFieldAPIService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Notification/Directive/Templates/NotificationTypeSettingsSearchEditorTemplate.html"
        };

        function NotificationTypeSettingsSearchEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var notificationTypeId;
            var notificationDataRecordFieldsInfo;
            var context;

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isSearchDirectiveLoading = false;

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
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
                        notificationTypeId = payload.notificationTypeId;
                        context = payload.context;
                    }

                    //Loading RecordFilter Directive
                    var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise();
                    promises.push(recordFilterDirectiveLoadPromise);


                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.isSearchDirectiveLoading = false;
                    });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Notification.DataRecordNotificationExtendedQuery, Vanrise.GenericData.Notification",
                        FilterGroup: recordFilterDirectiveAPI.getData().filterObj
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getRecordFilterDirectiveLoadPromise() {
                var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                recordFilterDirectiveReadyDeferred.promise.then(function () {

                    loadNotificationTypeSettingFields().then(function () {

                        var recordFilterDirectivePayload = {
                            context: buildContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });
                });

                return recordFilterDirectiveLoadDeferred.promise;
            }
            function loadNotificationTypeSettingFields() {
                var loadNotificationTypeSettingFieldsPromiseDeferred = UtilsService.createPromiseDeferred();

                VR_Notification_VRNotificationTypeAPIService.GetNotificationTypeSettings(notificationTypeId).then(function (response) {
                    var notificationTypeSettings = response;
                    var dataRecordTypeId = notificationTypeSettings.ExtendedSettings.DataRecordTypeId;

                    VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                        notificationDataRecordFieldsInfo = response;
                        loadNotificationTypeSettingFieldsPromiseDeferred.resolve();
                    });
                });

                return loadNotificationTypeSettingFieldsPromiseDeferred.promise;
            };

            function buildContext() {
                var context = {
                    getFields: function () {
                        var fields = [];
                        if (notificationDataRecordFieldsInfo != undefined) {
                            for (var i = 0; i < notificationDataRecordFieldsInfo.length; i++) {
                                var notificationDataRecordField = notificationDataRecordFieldsInfo[i].Entity;

                                fields.push({
                                    FieldName: notificationDataRecordField.Name,
                                    FieldTitle: notificationDataRecordField.Title,
                                    Type: notificationDataRecordField.Type
                                });
                            }
                        }
                        return fields;
                    }
                };
                return context;
            }
        }

        return directiveDefinitionObject;
    }
]);