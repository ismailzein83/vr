'use strict';

app.directive('vrGenericdataDatarecordalertruleExtendedsettings', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_DataRecordAlertRuleService', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_DataRecordAlertRuleService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataRecordAlertRuleExtendedSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/AlertRule/Directive/Templates/DataRecordAlertRuleExtendedSettingsTemplate.html'
        };

        function DataRecordAlertRuleExtendedSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId, identificationFields, notificationTypeId; //assigned from alertTypeSettings
            var dataRecordFieldsInfo;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeFieldsSelectionChangedDeferred;

            var dataRecordAlertRuleSettingsAPI;
            var dataRecordAlertRuleSettingReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {}; 
                $scope.scopeModel.tabObject = { showTab: false };

                $scope.scopeModel.onDataRecordFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onDataRecordAlertRuleSettingsReady = function (api) {
                    dataRecordAlertRuleSettingsAPI = api;
                    dataRecordAlertRuleSettingReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordFieldsSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined && selectedItem.length > 0) {
                        $scope.scopeModel.tabObject.showTab = true;

                        if (dataRecordTypeFieldsSelectionChangedDeferred != undefined) {
                            dataRecordTypeFieldsSelectionChangedDeferred.resolve();
                        }
                    }
                };

                $scope.scopeModel.onBeforeDataRecordFieldsSelectionChanged = function () {
                    var isSettingsDirectiveHasData = dataRecordAlertRuleSettingsAPI.hasData();
                    if (isSettingsDirectiveHasData == true) {
                        return VRNotificationService.showConfirmation("Settings Data will be deleted. Are you sure you want to continue?").then(function (response) {
                            return response;
                        });
                    }

                    return;
                };
                $scope.scopeModel.onSelectDataRecordField = function (selectedItem) {
                    $scope.scopeModel.tabObject.showTab = true;

                    if (selectedItem != undefined) {
                        loadDataRecordAlertRuleSettingsDirective();
                    }
                };
                $scope.scopeModel.onDeselectDataRecordField = function (deselectedItem) {
                    if ($scope.scopeModel.selectedDataRecordFields.length == 1)
                        $scope.scopeModel.tabObject.showTab = false;

                    if (deselectedItem != undefined) {
                        loadDataRecordAlertRuleSettingsDirective();
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var availableIdentificationFields, minNotificationInterval, settings; //assigned from alertExtendedSettings

                    if (payload != undefined) {
                        var alertTypeSettings = payload.alertTypeSettings;
                        var alertExtendedSettings = payload.alertExtendedSettings;

                        if (alertTypeSettings != undefined) {
                            dataRecordTypeId = alertTypeSettings.DataRecordTypeId;
                            identificationFields = alertTypeSettings.IdentificationFields;
                            notificationTypeId = alertTypeSettings.NotificationTypeId;
                        }

                        if (alertExtendedSettings != undefined) {
                            availableIdentificationFields = alertExtendedSettings.AvailableIdentificationFields;
                            minNotificationInterval = alertExtendedSettings.MinNotificationInterval;
                            settings = alertExtendedSettings.Settings;
                        }
                    }

                    //Loading DataRecordTypeField Selector
                    var dataRecordTypeFieldsSelectorLoadPromise = getDataRecordTypeFieldsSelectorLoadPromise();
                    promises.push(dataRecordTypeFieldsSelectorLoadPromise);

                    //Loading MinNotificationInterval 
                    if (minNotificationInterval != undefined)
                        $scope.scopeModel.minNotificationInterval = minNotificationInterval;
                    else
                        $scope.scopeModel.minNotificationInterval = '0.01:00:00';


                    //Loading DataRecordAlertRuleSettings Directive
                    var dataRecordAlertRuleSettingsLoadPromise = getDataRecordAlertRuleSettingsLoadPromise();
                    promises.push(dataRecordAlertRuleSettingsLoadPromise);


                    function getDataRecordTypeFieldsSelectorLoadPromise() {
                        dataRecordTypeFieldsSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {
                            var _includedFieldNames = UtilsService.getPropValuesFromArray(identificationFields, 'Name');

                            var dataRecordTypeFieldsSelectorPayload = {
                                dataRecordTypeId: dataRecordTypeId,
                                filter: {
                                    IncludedFieldNames: _includedFieldNames
                                }
                            };

                            if (availableIdentificationFields != undefined) {
                                dataRecordTypeFieldsSelectorPayload.selectedIds = UtilsService.getPropValuesFromArray(availableIdentificationFields, 'Name');
                            } else {
                                dataRecordTypeFieldsSelectorPayload.selectedIds = _includedFieldNames; //by default Select all Identification Fields
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                        });

                        return dataRecordTypeFieldsSelectorLoadDeferred.promise;
                    }
                    function getDataRecordAlertRuleSettingsLoadPromise() {
                        var dataRecordAlertRuleSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([dataRecordAlertRuleSettingReadyDeferred.promise, dataRecordTypeFieldsSelectionChangedDeferred.promise]).then(function () {
                            dataRecordAlertRuleSettingsLoadPromise = undefined;

                            getDataRecordFieldsInfo().then(function (response) {
                                var dataRecordAlertRuleSettingsPayload = {
                                    settings: settings,
                                    context: buildDataRecordAlertRuleSettingsContext()
                                };
                                VRUIUtilsService.callDirectiveLoad(dataRecordAlertRuleSettingsAPI, dataRecordAlertRuleSettingsPayload, dataRecordAlertRuleSettingsLoadDeferred);
                            });
                        });

                        return dataRecordAlertRuleSettingsLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    function buildAvailableIdentificationFields() {
                        var alertRuleTypeRecordFields = [];
                        var availableIdentificationFields = dataRecordTypeFieldsSelectorAPI.getSelectedIds();

                        if (availableIdentificationFields != undefined) {
                            for (var index = 0; index < availableIdentificationFields.length; index++) {
                                alertRuleTypeRecordFields.push({
                                    Name: availableIdentificationFields[index]
                                });
                            }
                        }
                        return alertRuleTypeRecordFields;
                    }

                    return {
                        $type: 'Vanrise.GenericData.Notification.DataRecordAlertRuleExtendedSettings, Vanrise.GenericData.Notification',
                        AvailableIdentificationFields: buildAvailableIdentificationFields(),
                        MinNotificationInterval: $scope.scopeModel.minNotificationInterval,
                        Settings: dataRecordAlertRuleSettingsAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getDataRecordFieldsInfo() {
                var loadDataRecordFieldsInfoPromise;

                if (dataRecordFieldsInfo == undefined) {
                    loadDataRecordFieldsInfoPromise = VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                        dataRecordFieldsInfo = response;
                    });
                } else {
                    var loadDataRecordFieldsInfoPromiseDeferred = UtilsService.createPromiseDeferred();
                    loadDataRecordFieldsInfoPromiseDeferred.resolve();
                    loadDataRecordFieldsInfoPromise = loadDataRecordFieldsInfoPromiseDeferred.promise;
                }

                return loadDataRecordFieldsInfoPromise;
            }

            function loadDataRecordAlertRuleSettingsDirective() {

                UtilsService.waitMultiplePromises([dataRecordAlertRuleSettingReadyDeferred.promise]).then(function () {
                    getDataRecordFieldsInfo().then(function () {

                        var dataRecordAlertRuleSettingsPayload = {
                            context: buildDataRecordAlertRuleSettingsContext()
                        };

                        var setLoader = function (value) {
                            $scope.scopeModel.isDataRecordAlertRuleSettingsDirectiveLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordAlertRuleSettingsAPI, dataRecordAlertRuleSettingsPayload, setLoader, undefined);
                    });
                });
            };

            function buildDataRecordAlertRuleSettingsContext() {
                return {
                    recordfields: buildRecordFields(),
                    vrActionTargetType: buildDAProfCalcTargetType(),
                    notificationTypeId: notificationTypeId
                };
            }

            function buildRecordFields() {
                var recordFields = [];
                for (var i = 0; i < dataRecordFieldsInfo.length; i++) {
                    var field = dataRecordFieldsInfo[i].Entity;
                    recordFields.push({
                        Name: field.Name,
                        Title: field.Title,
                        Type: field.Type
                    });
                };
                return recordFields;
            }

            function buildDAProfCalcTargetType() {
                return {
                    $type: "Vanrise.GenericData.Notification.DataRecordActionTargetType, Vanrise.GenericData.Notification",
                    DataRecordTypeId: dataRecordTypeId
                };
            }
        }
    }]);