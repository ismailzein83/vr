'use strict';

app.directive('vrGenericdataDatarecordalertruletypeSettings', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_AdvancedFilterFieldsRelationType',
    function (UtilsService, VRUIUtilsService, VR_GenericData_AdvancedFilterFieldsRelationType) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataRecordAlertRuleTypeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/AlertRule/Directive/Templates/DataRecordAlertRuleTypeSettingsTemplate.html'
        };

        function DataRecordAlertRuleTypeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId;
            var selectedAdvancedFilterFieldIds;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectorSelectionChangedDeferred;

            var vrNotificationTypeSettingsSelectorAPI;
            var vrNotificationTypeSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var advancedFilterFieldsSelectorAPI;
            var advancedFilterFieldsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedDataRecordFields = [];
                $scope.scopeModel.gridDataRecordFields = [];
                $scope.scopeModel.showAdvancedFilterFields = false;
                $scope.scopeModel.advancedFilterFieldsRelationTypeDS = UtilsService.getArrayEnum(VR_GenericData_AdvancedFilterFieldsRelationType);

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAdvancedFilterFieldsSelectorDirectiveReady = function (api) {
                    advancedFilterFieldsSelectorAPI = api;
                    advancedFilterFieldsReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem == undefined)
                        return;

                    dataRecordTypeId = selectedItem.DataRecordTypeId;

                    if (dataRecordTypeSelectorSelectionChangedDeferred != undefined) {
                        dataRecordTypeSelectorSelectionChangedDeferred.resolve();
                    }

                    else {
                        $scope.scopeModel.gridDataRecordFields = [];
                        $scope.scopeModel.selectedAdvancedFilterFieldsRelationType = VR_GenericData_AdvancedFilterFieldsRelationType.AllFields;

                        loadDataRecordTypeFieldsSelector();
                        loadNotificationTypeSelector();
                        loadAdvancedFilterFieldsSelector(); 
                    }

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
                    function loadAdvancedFilterFieldsSelector() {
                        if (advancedFilterFieldsSelectorAPI == undefined)
                            return;

                        var advancedFilterFieldsPayload = {
                            dataRecordTypeId: selectedItem.DataRecordTypeId
                        };
                        var setLoader = function (value) {
                            $scope.isAdvancedFilterDirectiveLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, advancedFilterFieldsSelectorAPI, advancedFilterFieldsPayload, setLoader);
                    }
                };

                $scope.scopeModel.onAdvancedFilterFieldsRelationTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem == undefined)
                        return;

                    if (selectedItem.value == VR_GenericData_AdvancedFilterFieldsRelationType.AllFields.value) {
                        setTimeout(function () {
                            $scope.scopeModel.showAdvancedFilterFields = false;
                            $scope.scopeModel.selectedAdvancedFilterFields = [];
                        });
                    }
                    else {
                        setTimeout(function () {
                            $scope.scopeModel.showAdvancedFilterFields = true;
                        });
                    }
                };

                $scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
                    vrNotificationTypeSettingsSelectorAPI = api;
                    vrNotificationTypeSettingsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordFieldSelected = function (selectedDataRecordField) {
                    $scope.scopeModel.gridDataRecordFields.push({
                        Name: selectedDataRecordField.Name,
                        IsRequired: false,
                        IsSelected: false
                    });
                };

                $scope.scopeModel.onDataRecordFieldDeselected = function (deselectedDataRecordField) {
                    var gridIndex = UtilsService.getItemIndexByVal($scope.scopeModel.gridDataRecordFields, deselectedDataRecordField.Name, 'Name');
                    if (gridIndex > -1) {
                        $scope.scopeModel.gridDataRecordFields.splice(gridIndex, 1);
                    }
                };

                $scope.scopeModel.onDataRecordFieldDeleted = function (deletedDataRecordField) {
                    var gridIndex = UtilsService.getItemIndexByVal($scope.scopeModel.gridDataRecordFields, deletedDataRecordField.Name, 'Name');
                    if (gridIndex > -1) {
                        $scope.scopeModel.gridDataRecordFields.splice(gridIndex, 1);
                    }

                    var selectorIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedDataRecordFields, deletedDataRecordField.Name, 'Name');
                    if (selectorIndex > -1) {
                        $scope.scopeModel.selectedDataRecordFields.splice(selectorIndex, 1);
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var advancedFilters;
                    var identificationFields;
                    var notificationTypeId;

                    if (payload != undefined && payload.settings) {
                        dataRecordTypeId = payload.settings.DataRecordTypeId;
                        identificationFields = payload.settings.IdentificationFields;
                        notificationTypeId = payload.settings.NotificationTypeId;
                        advancedFilters = payload.settings.AdvancedFilters;

                        if (identificationFields != undefined) {
                            $scope.scopeModel.gridDataRecordFields = identificationFields;
                        }
                    }

                    if (advancedFilters != undefined) {
                        $scope.scopeModel.selectedAdvancedFilterFieldsRelationType =
                            UtilsService.getItemByVal($scope.scopeModel.advancedFilterFieldsRelationTypeDS, advancedFilters.FieldsRelationType, "value");

                        if ($scope.scopeModel.selectedAdvancedFilterFieldsRelationType.value == VR_GenericData_AdvancedFilterFieldsRelationType.SpecificFields.value) {
                            selectedAdvancedFilterFieldIds = [];
                            for (var i = 0; i < advancedFilters.AvailableFields.length; i++) {
                                var advancedFilterField = advancedFilters.AvailableFields[i];
                                selectedAdvancedFilterFieldIds.push(advancedFilterField.FieldName);
                            }
                        }
                    }
                    else {
                        $scope.scopeModel.selectedAdvancedFilterFieldsRelationType = VR_GenericData_AdvancedFilterFieldsRelationType.AllFields;
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

                        //Loading AdvancedFilterFields Selector
                        var advancedFilterFieldsSelectorLoadPromise = getAdvancedFilterFieldsSelectorLoadPromise();
                        promises.push(advancedFilterFieldsSelectorLoadPromise);
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

                    function getAdvancedFilterFieldsSelectorLoadPromise() {
                        var loadAdvancedFilterDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        advancedFilterFieldsReadyDeferred.promise.then(function () {

                            var payloadAdvancedFilterDirective = {
                                dataRecordTypeId: dataRecordTypeId,
                                selectedIds: selectedAdvancedFilterFieldIds
                            };
                            VRUIUtilsService.callDirectiveLoad(advancedFilterFieldsSelectorAPI, payloadAdvancedFilterDirective, loadAdvancedFilterDirectivePromiseDeferred);
                        });

                        return loadAdvancedFilterDirectivePromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: promises }).then(function () {
                        dataRecordTypeSelectorSelectionChangedDeferred = undefined;
                    });
                };

                api.getData = function () {

                    var obj = {
                        $type: 'Vanrise.GenericData.Notification.DataRecordAlertRuleTypeSettings, Vanrise.GenericData.Notification',
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        IdentificationFields: buildIdentificationFields(),
                        NotificationTypeId: vrNotificationTypeSettingsSelectorAPI.getSelectedIds(),
                        AdvancedFilters: buildAdvancedFilters()
                    };

                    function buildIdentificationFields() {
                        var identificationFields = [];

                        for (var i = 0; i < $scope.scopeModel.gridDataRecordFields.length; i++) {
                            var gridDataRecordFields = $scope.scopeModel.gridDataRecordFields[i];
                            identificationFields.push({
                                Name: gridDataRecordFields.Name,
                                IsRequired: gridDataRecordFields.IsRequired,
                                IsSelected: gridDataRecordFields.IsSelected
                            });
                        }

                        return identificationFields;
                    }
                    function buildAdvancedFilters() {
                        var advancedFilters = {};
                        advancedFilters.FieldsRelationType = $scope.scopeModel.selectedAdvancedFilterFieldsRelationType.value;

                        if ($scope.scopeModel.showAdvancedFilterFields) {
                            advancedFilters.AvailableFields = [];
                            for (var i = 0; i < $scope.scopeModel.selectedAdvancedFilterFields.length; i++) {
                                var advancedFilterField = $scope.scopeModel.selectedAdvancedFilterFields[i];
                                advancedFilters.AvailableFields.push({ FieldName: advancedFilterField.Name });
                            }
                        }

                        return advancedFilters;
                    }

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);