'use strict';

app.directive('vrAnalyticReporttypedefintionSettings', ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_ReportTypeService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_Analytic_ReportTypeService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRReportTypeDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/MainExtensions/ReportType/Templates/VRReportTypeDefinitionSettingsTemplate.html'
        };

        function VRReportTypeDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var vrReportTypeDefinitionSettings;
            var dataStorageId;
            var filterFields = [];
            var attachements = [];
            var dataRecordTypeId;

            var dataRecordStorageAPI;
            var dataRecordStorageReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordStorageSelectedDeferred;

            var filterFieldsGridAPI;
            var filterFieldsGridReadyDeferred = UtilsService.createPromiseDeferred();

            var attachementsGridAPI;
            var attachementsGridReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.filterFields = [];
                $scope.scopeModel.attachements = [];

                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageAPI = api;
                    dataRecordStorageReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFilterFieldsGridReady = function (api) {
                    filterFieldsGridAPI = api;
                    filterFieldsGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onAttachementsGridReady = function (api) {
                    attachementsGridAPI = api;
                    attachementsGridReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataStorageChanged = function (item) {
                    if (item != undefined) {
                        dataStorageId = item.DataRecordStorageId;
                        dataRecordTypeId = item.DataRecordTypeId;

                        if (dataRecordStorageSelectedDeferred != undefined) {
                            dataRecordStorageSelectedDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.filterFields.length = 0;
                            $scope.scopeModel.attachements.length = 0;
                        }
                    }
                };

                $scope.scopeModel.addFilterField = function () {
                    addFilterField();
                };

                $scope.scopeModel.addAttachement = function () {
                    var onAttachementAdded = function (attachement) {
                        $scope.scopeModel.attachements.push(attachement);
                    };
                    VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                        var dataRecordFieldsInfo = response;
                        VR_Analytic_ReportTypeService.addAttachement(onAttachementAdded, buildContext(dataRecordFieldsInfo));
                    });
                };

                $scope.scopeModel.disableAddFilterFields = function () {
                    return $scope.scopeModel.selectedDataRecordStorage != undefined ? false : true;
                };

                $scope.scopeModel.disableAddAttachement = function () {
                    return $scope.scopeModel.selectedDataRecordStorage != undefined ? false : true;
                };

                $scope.scopeModel.removeFilterFields = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.filterFields, dataItem.id, 'id');
                    if (index > -1) {
                        $scope.scopeModel.filterFields.splice(index, 1);
                    }
                };

                $scope.scopeModel.removeAttachement = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.attachements, dataItem.Name, 'Name');
                    if (index > -1) {
                        $scope.scopeModel.attachements.splice(index, 1);
                    }
                };

                $scope.scopeModel.validateFilterGrid = function () {
                    if ($scope.scopeModel.filterFields.length > 0 && checkDuplicateFilterFieldName())
                        return "Every field name can be selected once";

                    if ($scope.scopeModel.filterFields.length == 0)
                        return "At least one filter must be added";

                    return null;
                };

                $scope.scopeModel.validateAttachementGrid = function () {
                    if ($scope.scopeModel.attachements.length == 0)
                        return "At least one attachement must be added";

                    return null;
                };

                UtilsService.waitMultiplePromises([dataRecordStorageReadyPromiseDeferred.promise, filterFieldsGridReadyDeferred.promise, attachementsGridReadyDeferred.promise]).then(function () {
                    defineAPI();
                });

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        var vrReportTypeDefinitionEntity = payload.componentType;

                        if (vrReportTypeDefinitionEntity != undefined) {
                            $scope.scopeModel.reportTypeName = vrReportTypeDefinitionEntity.Name;
                            vrReportTypeDefinitionSettings = vrReportTypeDefinitionEntity.Settings;
                        }

                        if (vrReportTypeDefinitionSettings != undefined) {
                            dataStorageId = vrReportTypeDefinitionSettings.DataStorageId;
                            filterFields = vrReportTypeDefinitionSettings.FilterFields;
                            attachements = vrReportTypeDefinitionSettings.Attachements;
                        }
                    }

                    initialPromises.push(loadDataRecordStorageSelector());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var dataItemsLoadPromises = [];

                            if (dataStorageId != undefined) {
                                dataRecordStorageSelectedDeferred = UtilsService.createPromiseDeferred();

                                if (filterFields.length > 0) {
                                    $scope.scopeModel.isFilterGridLoading = true;
                                    for (var i = 0; i < filterFields.length; i++) {
                                        var dataItem = {
                                            payload: filterFields[i]
                                        };
                                        addItemToFilterFieldsGrid(dataItem, dataItemsLoadPromises);
                                    }
                                }

                                if (attachements.length > 0) {
                                    for (var i = 0; i < attachements.length; i++) {
                                        var attachement = attachements[i];
                                        $scope.scopeModel.attachements.push(attachement);
                                    }
                                }
                            }

                            return {
                                promises: dataItemsLoadPromises
                            }
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                        dataRecordStorageSelectedDeferred = undefined;
                        $scope.scopeModel.isFilterGridLoading = false;
                        $scope.scopeModel.isAttachementsGridLoading = false;
                    });
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.reportTypeName,
                        Settings: {
                            $type: "Vanrise.Analytic.Entities.VRReportTypeDefinitionSettings, Vanrise.Analytic.Entities",
                            DataStorageId: dataRecordStorageAPI.getSelectedIds(),
                            FilterFields: getFilterFields(),
                            Attachements: $scope.scopeModel.attachements
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadDataRecordStorageSelector() {
                var dataRecordStorageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordStorageReadyPromiseDeferred.promise.then(function () {
                    var dataRecordStorageSelectorPayload = {};

                    if (dataStorageId != undefined)
                        dataRecordStorageSelectorPayload.selectedIds = dataStorageId;

                    VRUIUtilsService.callDirectiveLoad(dataRecordStorageAPI, dataRecordStorageSelectorPayload, dataRecordStorageSelectorLoadDeferred);
                });

                return dataRecordStorageSelectorLoadDeferred.promise;
            }

            function addFilterField() {
                $scope.scopeModel.isFilterGridLoading = true;
                addItemToFilterFieldsGridFromAdd();
            }

            function addItemToFilterFieldsGridFromAdd() {
                var dataItem = {
                    id: $scope.scopeModel.filterFields.length + 1,
                };

                dataItem.onFilterFieldNameSelectorReady = function (api) {
                    dataItem.filterFieldNameDirectiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isFilterGridLoading = value;
                    };
                    var context = buildContext();
                    var filterFieldNameDirectivePayload = {
                        dataRecordTypeId: context.getDataRecordTypeId(),
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.filterFieldNameDirectiveAPI, filterFieldNameDirectivePayload, setLoader);
                };

                $scope.scopeModel.isFilterGridLoading = false;
                $scope.scopeModel.filterFields.push(dataItem);
            }

            function addItemToFilterFieldsGrid(gridItem, dataItemsLoadPromises) {
                var dataItem = {
                    id: $scope.scopeModel.filterFields.length + 1,
                    FieldName: gridItem.payload.FieldName,
                    isFilterFieldNameRequired: gridItem.payload.IsRequired,
                    filterFieldNameSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                    loadFilterFieldNameSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                };

                dataItem.onFilterFieldNameSelectorReady = function (api) {
                    dataItem.filterFieldNameDirectiveAPI = api;
                    dataItem.filterFieldNameSelectorReadyDeferred.resolve();
                };

                dataItem.filterFieldNameSelectorReadyDeferred.promise.then(function () {
                    var context = buildContext();
                    var filterFieldNameDirectivePayload = {
                        dataRecordTypeId: context.getDataRecordTypeId(),
                        selectedIds: gridItem.payload.FieldName
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.filterFieldNameDirectiveAPI, filterFieldNameDirectivePayload, dataItem.loadFilterFieldNameSelectorPromiseDeferred);
                });

                dataItemsLoadPromises.push(dataItem.loadFilterFieldNameSelectorPromiseDeferred.promise);

                $scope.scopeModel.filterFields.push(dataItem);
            }

            function getFilterFields() {
                var columns = [];
                for (var i = 0; i < $scope.scopeModel.filterFields.length; i++) {
                    var column = $scope.scopeModel.filterFields[i];
                    columns.push({
                        FieldName: column.filterFieldNameDirectiveAPI.getSelectedIds(),
                        IsRequired: column.isFilterFieldNameRequired,
                    });
                }
                return columns;
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editAttachement,
                }];
            }

            function editAttachement(attachementEntity) {
                var onAttachementUpdated = function (updatedAttachementObj) {
                    var index = $scope.scopeModel.attachements.indexOf(attachementEntity);
                    $scope.scopeModel.attachements[index] = updatedAttachementObj;
                };
                VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                    var dataRecordFieldsInfo = response;
                    VR_Analytic_ReportTypeService.editAttachement(attachementEntity, buildContext(dataRecordFieldsInfo), onAttachementUpdated);
                });
            }

            function buildContext(dataRecordFieldsInfo) {
                var context = {
                    getDataRecordStroage: function () {
                        return dataStorageId;
                    },
                    getDataRecordTypeId: function () {
                        return dataRecordTypeId;
                    },
                    getFields: function () {
                        var fields = [];
                        if (dataRecordFieldsInfo != undefined) {
                            for (var i = 0; i < dataRecordFieldsInfo.length; i++) {
                                var dataRecordField = dataRecordFieldsInfo[i].Entity;

                                fields.push({
                                    FieldName: dataRecordField.Name,
                                    FieldTitle: dataRecordField.Title,
                                    Type: dataRecordField.Type
                                });
                            }
                        }
                        return fields;
                    }
                };
                return context;
            }

            function checkDuplicateFilterFieldName() {
                var fieldLength = $scope.scopeModel.filterFields.length;
                for (var i = 0; i < fieldLength; i++) {
                    var currentItem = $scope.scopeModel.filterFields[i];
                    var currentItemFieldName;

                    if (currentItem.filterFieldNameDirectiveAPI != undefined) {
                        currentItemFieldName = currentItem.filterFieldNameDirectiveAPI.getSelectedIds();
                    }

                    for (var j = i + 1; j < fieldLength; j++) {
                        var item = $scope.scopeModel.filterFields[j];
                        if (item.filterFieldNameDirectiveAPI != undefined && item.filterFieldNameDirectiveAPI.getSelectedIds() == currentItemFieldName && currentItemFieldName != undefined)
                            return true;
                    }
                }
                return false;
            }
        }
    }]);
