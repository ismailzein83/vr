'use strict';

app.directive('vrGenericdataDatarecordalertruletypeSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataRecordAlertRuleTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/AlertRule/Directive/Templates/DataRecordAlertRuleTypeSettingsTemplate.html'
        };

        function DataRecordAlertRuleTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectorSelectionChangedDeferred;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var vrNotificationTypeSettingsSelectorAPI;
            var vrNotificationTypeSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onDataRecordFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
                    vrNotificationTypeSettingsSelectorAPI = api;
                    vrNotificationTypeSettingsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined) {
                        dataRecordTypeId = selectedItem.DataRecordTypeId;

                        if (dataRecordTypeSelectorSelectionChangedDeferred != undefined) {
                            dataRecordTypeSelectorSelectionChangedDeferred.resolve();
                        }
                        else {
                            loadDataRecordTypeFieldsSelector();
                            loadNotificationTypeSelector();

                            function loadDataRecordTypeFieldsSelector() {

                                var dataRecordTypeFieldsSelectorPayload = {
                                    dataRecordTypeId: dataRecordTypeId
                                };

                                var setLoader = function (value) {
                                    $scope.scopeModel.isDataRecordtypeFieldsSelectorLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, setLoader, undefined);
                            }
                            function loadNotificationTypeSelector() {

                                var vrNotificationSelectorPayload = {
                                    filter: {
                                        Filters: [{
                                            $type: "Vanrise.GenericData.Notification.DataRecordNotificationTypeFilter, Vanrise.GenericData.Notification",
                                            DataRecordTypeId: dataRecordTypeId
                                        }]
                                    }
                                };

                                var setLoader = function (value) {
                                    $scope.scopeModel.isNotificationTypeSettingsSelectorLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, vrNotificationTypeSettingsSelectorAPI, vrNotificationSelectorPayload, setLoader, undefined);
                            }
                        }
                    }
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var identificationFields;
                    var notificationTypeId;

                    if (payload != undefined && payload.settings) {
                        dataRecordTypeId = payload.settings.DataRecordTypeId;
                        identificationFields = payload.settings.IdentificationFields;
                        notificationTypeId = payload.settings.NotificationTypeId;
                    }

                    //Loading DataRecordType Selector
                    var dataRecordTypeSelectorLoadPromise = getDataRecordTypeSelectorLoadPromise();
                    promises.push(dataRecordTypeSelectorLoadPromise);

                    if (dataRecordTypeId != undefined) {
                        //Loading DataRecordTypeFields Selector
                        var dataRecordTypeFieldsSelectorLoadPromise = getDataRecordTypeFieldsSelectorLoadPromise();
                        promises.push(dataRecordTypeFieldsSelectorLoadPromise);

                        //Loading NotificationType Selector
                        var notificationTypeSelectorPromise = getNotificationTypeSelectorPromise();
                        promises.push(notificationTypeSelectorPromise);

                        UtilsService.waitMultiplePromises([dataRecordTypeFieldsSelectorLoadPromise, notificationTypeSelectorPromise]).then(function () {
                            dataRecordTypeSelectorSelectionChangedDeferred = undefined;
                        });
                    }

                    function getDataRecordTypeSelectorLoadPromise() {
                        if (dataRecordTypeId != undefined)
                            dataRecordTypeSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var dataRecordTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorPromiseDeferred.promise.then(function () {
                            var dataRecordTypeSelectorPayload;
                            if (dataRecordTypeId != undefined) {
                                dataRecordTypeSelectorPayload = { selectedIds: dataRecordTypeId };
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadPromiseDeferred);
                        });

                        return dataRecordTypeSelectorLoadPromiseDeferred.promise;
                    }
                    function getDataRecordTypeFieldsSelectorLoadPromise() {
                        var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([dataRecordTypeFieldsSelectorReadyDeferred.promise, dataRecordTypeSelectorSelectionChangedDeferred.promise]).then(function () {

                            var dataRecordTypeFieldsSelectorPayload = {
                                dataRecordTypeId: dataRecordTypeId
                            };
                            if (identificationFields != undefined) {
                                dataRecordTypeFieldsSelectorPayload.selectedIds = UtilsService.getPropValuesFromArray(identificationFields, 'Name');
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                        });

                        return dataRecordTypeFieldsSelectorLoadDeferred.promise;
                    }
                    function getNotificationTypeSelectorPromise() {
                        var vrNotificationSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([vrNotificationTypeSettingsSelectorReadyDeferred.promise, dataRecordTypeSelectorSelectionChangedDeferred.promise]).then(function () {

                            var vrNotificationSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.GenericData.Notification.DataRecordNotificationTypeFilter, Vanrise.GenericData.Notification",
                                        DataRecordTypeId: dataRecordTypeId
                                    }]
                                }
                            };
                            if (notificationTypeId != undefined) {
                                vrNotificationSelectorPayload.selectedIds = notificationTypeId;
                            }
                            VRUIUtilsService.callDirectiveLoad(vrNotificationTypeSettingsSelectorAPI, vrNotificationSelectorPayload, vrNotificationSelectorLoadDeferred);
                        });

                        return vrNotificationSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var obj = {
                        $type: 'Vanrise.GenericData.Notification.DataRecordAlertRuleTypeSettings, Vanrise.GenericData.Notification',
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        IdentificationFields: buildIdentificationFields(),
                        NotificationTypeId: vrNotificationTypeSettingsSelectorAPI.getSelectedIds()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildIdentificationFields() {
                var alertRuleTypeRecordFields = [];
                var identificationFields = dataRecordTypeFieldsSelectorAPI.getSelectedIds();

                if (identificationFields != undefined) {
                    for (var index = 0; index < identificationFields.length; index++) {
                        alertRuleTypeRecordFields.push({
                            Name: identificationFields[index]
                        });
                    }
                }
                return alertRuleTypeRecordFields;
            }
        }
    }]);