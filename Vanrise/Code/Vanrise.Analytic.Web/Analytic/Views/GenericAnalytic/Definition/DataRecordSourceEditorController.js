(function (appControllers) {

    'use strict';

    DataRecordSourceEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldAPIService', 'ColumnWidthEnum', 'VR_Analytic_OrderDirectionEnum', 'VRCommon_GridWidthFactorEnum', 'Analytic_RecordSearchService', 'VRLocalizationService', 'VR_Analytic_AdvancedFilterFieldsRelationType'];

    function DataRecordSourceEditorController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService, ColumnWidthEnum, VR_Analytic_OrderDirectionEnum, VRCommon_GridWidthFactorEnum, Analytic_RecordSearchService, VRLocalizationService, VR_Analytic_AdvancedFilterFieldsRelationType) {

        var isEditMode;
        var dataRecordSource;
        var existingSources;
        var selectedAdvancedFilterFieldNames;

        var dataRecordTypeSelectorAPI;
        var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var dataRecordTypeSelectionChangedDeferred;

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyDeferred;

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var subviewDefinitionGridAPI;
        var subviewDefinitionGridReadyDeferred = UtilsService.createPromiseDeferred();

        var localizationTextResourceSelectorAPI;
        var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var advancedFilterFieldsSelectorAPI;
        var advancedFilterFieldsReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                dataRecordSource = parameters.DataRecordSource;
                existingSources = parameters.ExistingSources;
            }

            isEditMode = (dataRecordSource != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.detailWidths = UtilsService.getArrayEnum(ColumnWidthEnum);
            $scope.scopeModel.orderDirectionList = UtilsService.getArrayEnum(VR_Analytic_OrderDirectionEnum);
            $scope.scopeModel.showAdvancedFilterFields = false;
            $scope.scopeModel.advancedFilterFieldsRelationTypeDS = UtilsService.getArrayEnum(VR_Analytic_AdvancedFilterFieldsRelationType);

            $scope.dataRecordTypeFields = [];
            $scope.selectedFields = [];
            $scope.selectedFilters = [];
            $scope.selectedDetails = [];
            $scope.selectedSortColumns = [];

            $scope.selectedFieldsGrid = [];
            $scope.selectedFiltersGrid = [];
            $scope.selectedDetailsGrid = [];
            $scope.selectedSubviewDefinitionsGrid = [];
            $scope.selectedSortColumnsGrid = [];
            $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

            $scope.scopeModel.onAdvancedFilterFieldsSelectorDirectiveReady = function (api) {
                advancedFilterFieldsSelectorAPI = api;
                advancedFilterFieldsReadyDeferred.resolve();
            };
            $scope.onDataRecordTypeSelectorReady = function (api) {
                dataRecordTypeSelectorAPI = api;
                dataRecordTypeSelectorReadyDeferred.resolve();
            };
            $scope.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageSelectorAPI = api;
            };
            $scope.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };
            $scope.onSubviewGridReady = function (api) {
                subviewDefinitionGridAPI = api;
                subviewDefinitionGridReadyDeferred.resolve();
            };
            $scope.onLocalizationTextResourceSelectorReady = function (api) {
                localizationTextResourceSelectorAPI = api;
                localizationTextResourceSelectorReadyPromiseDeferred.resolve();
            };
            $scope.onBeforeDataRecordTypeSelectionChanged = function () {

                var selectedDataRecordType = $scope.scopeModel.selectedDataRecordType;
                if (selectedDataRecordType == undefined)
                    return true;

                return VRNotificationService.showConfirmation("Below Tabs Data will be deleted. Are you sure you want to continue?").then(function (response) {
                    if (response) {
                        $scope.selectedFields = [];
                        $scope.selectedFilters = [];
                        $scope.selectedDetails = [];
                        $scope.selectedSortColumns = [];

                        $scope.selectedFieldsGrid = [];
                        $scope.selectedFiltersGrid = [];
                        $scope.selectedDetailsGrid = [];
                        $scope.selectedSubviewDefinitionsGrid = [];
                        $scope.selectedSortColumnsGrid = [];
                    }

                    return response;
                });
            };
            $scope.onDataRecordTypeSelectionChanged = function (option) {
                if (option != undefined) {
                    if (dataRecordStorageSelectorAPI != undefined) {
                        var setLoader = function (value) { $scope.isRecordStorageLoading = value; };
                        var payload = {
                            DataRecordTypeId: option != undefined ? option.DataRecordTypeId : 0,
                            selectedIds: dataRecordSource != undefined ? dataRecordSource.RecordStorageIds : undefined
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordStorageSelectorAPI, payload, setLoader, dataRecordStorageSelectorReadyDeferred);
                    }

                    $scope.isRecordTypeFieldsLoading = true;

                    loadDataRecord(option.DataRecordTypeId).finally(function (response) {
                        $scope.isRecordTypeFieldsLoading = false;

                        //Loading RecordFilter
                        var recordFilterDirectivePayload = {};
                        recordFilterDirectivePayload.context = buildContext();
                        var setLoader = function (value) {
                            $scope.scopeModel.isRecordFilterDirectiveLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterDirectivePayload, setLoader, dataRecordTypeSelectionChangedDeferred);
                    });

                    if (dataRecordTypeSelectionChangedDeferred == undefined) {
                        $scope.scopeModel.selectedAdvancedFilterFieldsRelationType = VR_Analytic_AdvancedFilterFieldsRelationType.AllFields;

                        if (advancedFilterFieldsSelectorAPI != undefined) {

                            var advancedFilterFieldsPayload = {
                                dataRecordTypeId: option.DataRecordTypeId
                            };
                            var setLoader = function (value) {
                                $scope.isAdvancedFilterDirectiveLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, advancedFilterFieldsSelectorAPI, advancedFilterFieldsPayload, setLoader);
                        }
                    }
                }
            };

            $scope.scopeModel.onAdvancedFilterFieldsRelationTypeSelectionChanged = function (selectedItem) {
                if (selectedItem == undefined)
                    return;

                if (selectedItem.value == VR_Analytic_AdvancedFilterFieldsRelationType.AllFields.value) {
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

            $scope.onSelectedField = function (selectedField) {
                var dataItem = {
                    FieldName: selectedField.Name,
                    FieldTitle: selectedField.Title,
                    TitleResourceKey: selectedField.TitleResourceKey
                };

                dataItem.onGridWidthFactorSelectorReady = function (api) {
                    dataItem.gridWidthFactorAPI = api;
                    var dataItemPayload = {
                        data: {
                            Width: VRCommon_GridWidthFactorEnum.Normal.value
                        }
                    };
                    var setLoader = function (value) { $scope.isLoadingDirective = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.gridWidthFactorAPI, dataItemPayload, setLoader);
                };

                dataItem.onDataRecordGridStyleDefinitionReady = function (api) {
                    dataItem.dataRecordGridStyleAPI = api;
                    var dataItemPayload;
                    var setLoader = function (value) { $scope.isLoadingDirective = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.dataRecordGridStyleAPI, dataItemPayload, setLoader);
                };

                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    var setLoader = function (value) { dataItem.isFieldTextResourceSelectorLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                };

                $scope.selectedFieldsGrid.push(dataItem);
            };

            $scope.onSelectedFilter = function (selectedFilter) {
                var dataItem = {
                    FieldName: selectedFilter.Name,
                    FieldTitle: selectedFilter.Title,
                    TitleResourceKey: selectedFilter.TitleResourceKey
                };
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    var setLoader = function (value) { dataItem.isFilterTextResourceSelectorLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                };
                $scope.selectedFiltersGrid.push(dataItem);
            };
            $scope.onSelectedDetailItem = function (detailItem) {
                var dataItem = {
                    FieldName: detailItem.Name,
                    FieldTitle: detailItem.Title,
                    SelectedDetailWidth: ColumnWidthEnum.QuarterRow,
                    TitleResourceKey: detailItem.TitleResourceKey
                };
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    var setLoader = function (value) { dataItem.isFieldDetailTextResourceSelectorLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                };
                $scope.selectedDetailsGrid.push(dataItem);
            };
            $scope.onSelectedSortColumn = function (sortColumn) {
                var dataItem = {
                    FieldName: sortColumn.Name,
                    FieldTitle: sortColumn.Title,
                    SelectedOrderDirection: VR_Analytic_OrderDirectionEnum.Ascending
                };
                $scope.selectedSortColumnsGrid.push(dataItem);
            };

            $scope.onDeSelectedField = function (selectedField) {
                var index = UtilsService.getItemIndexByVal($scope.selectedFieldsGrid, selectedField.Name, "FieldName");
                $scope.selectedFieldsGrid.splice(index, 1);
            };
            $scope.onDeSelectedFilter = function (selectedFilter) {
                var index = UtilsService.getItemIndexByVal($scope.selectedFiltersGrid, selectedFilter.Name, "FieldName");
                $scope.selectedFiltersGrid.splice(index, 1);
            };
            $scope.onDeSelectedDetailItem = function (detailItem) {
                $scope.selectedDetailsGrid.splice(UtilsService.getItemIndexByVal($scope.selectedDetailsGrid, detailItem.Name, "FieldName"), 1);
            };
            $scope.onDeSelectedSortColumn = function (sortColumn) {
                $scope.selectedSortColumnsGrid.splice(UtilsService.getItemIndexByVal($scope.selectedSortColumnsGrid, sortColumn.Name, "FieldName"), 1);
            };

            $scope.removeField = function (field) {
                $scope.selectedFieldsGrid.splice($scope.selectedFieldsGrid.indexOf(field), 1);
                $scope.selectedFields.splice(UtilsService.getItemIndexByVal($scope.selectedFields, field.FieldName, "Name"), 1);

            };
            $scope.removeFilter = function (filter) {
                $scope.selectedFiltersGrid.splice($scope.selectedFiltersGrid.indexOf(filter), 1);
                $scope.selectedFilters.splice(UtilsService.getItemIndexByVal($scope.selectedFilters, filter.FieldName, "Name"), 1);
            };
            $scope.removeDetail = function (detail) {
                $scope.selectedDetailsGrid.splice($scope.selectedDetailsGrid.indexOf(detail), 1);
                $scope.selectedDetails.splice(UtilsService.getItemIndexByVal($scope.selectedDetails, detail.FieldName, "Name"), 1);
            };
            $scope.removeSubviewDefinition = function (subviewDefinition) {
                $scope.selectedSubviewDefinitionsGrid.splice(UtilsService.getItemIndexByVal($scope.selectedSubviewDefinitionsGrid, subviewDefinition.Name, "Name"), 1);
            };
            $scope.removeSortColumn = function (sortColumn) {
                $scope.selectedSortColumnsGrid.splice($scope.selectedSortColumnsGrid.indexOf(sortColumn), 1);
                $scope.selectedSortColumns.splice(UtilsService.getItemIndexByVal($scope.selectedSortColumns, sortColumn.FieldName, "Name"), 1);
            };

            $scope.onAddSubviewDefinition = function () {
                var onSubviewDefinitionAdded = function (subviewDefinition) {
                    $scope.selectedSubviewDefinitionsGrid.push(subviewDefinition);
                };

                var dataRecordTypeId;
                if ($scope.scopeModel.selectedDataRecordType != undefined) {
                    dataRecordTypeId = $scope.scopeModel.selectedDataRecordType.DataRecordTypeId;
                }

                Analytic_RecordSearchService.addDRSearchPageSubviewDefinition(onSubviewDefinitionAdded, dataRecordTypeId);
            };
            $scope.subviewDefinitionMenuActions = function () {
                function editSubviewDefinition(subviewDefinition) {
                    var onSubviewDefinitionUpdated = function (updatedSubviewDefinition) {
                        var index = UtilsService.getItemIndexByVal($scope.selectedSubviewDefinitionsGrid, subviewDefinition.Name, 'Name');
                        $scope.selectedSubviewDefinitionsGrid[index] = updatedSubviewDefinition;
                    };

                    var dataRecordTypeId;
                    if ($scope.scopeModel.selectedDataRecordType != undefined) {
                        dataRecordTypeId = $scope.scopeModel.selectedDataRecordType.DataRecordTypeId;
                    }

                    Analytic_RecordSearchService.editDRSearchPageSubviewDefinition(onSubviewDefinitionUpdated, subviewDefinition, dataRecordTypeId);
                }

                return [{
                    name: 'Edit',
                    clicked: editSubviewDefinition
                }];
            };

            $scope.isFieldGridValid = function () {
                if ($scope.selectedFieldsGrid.length == 0) {
                    return 'At least one Field must be added.';
                }
                return null;
            };

            $scope.saveDataRecordSource = function () {
                if (validateData()) {
                    if (isEditMode) {
                        return updateDataRecordSource();
                    }
                    else {
                        return saveDataRecordSource();
                    }
                }
                else {
                    VRNotificationService.showWarning('Same Source Title Exists');
                }
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.validateDataRecordStorageSelector = function () {
                var _selectedDataRecordStorages = $scope.selectedDataRecordStorages;
                if (_selectedDataRecordStorages == undefined)
                    return null;

                for (var index = 0; index < _selectedDataRecordStorages.length; index++) {
                    var currentItem = _selectedDataRecordStorages[index];
                    if (currentItem.IsRemoteRecordStorage && _selectedDataRecordStorages.length > 1)
                        return "Remote Data Storage should be uniquely selected";
                }

                return null;
            };

            function validateData() {
                if (!existingSources || existingSources.length == 0) {
                    return true;
                }

                if (existingSources.indexOf($scope.scopeModel.title.toLowerCase()) > -1) {
                    return false;
                } else {
                    return true;
                }
            }
        }
        function load() {
            $scope.isLoading = true;

            if (isEditMode)
                dataRecordTypeSelectionChangedDeferred = UtilsService.createPromiseDeferred();

            loadAllControls().then(function () {
                dataRecordTypeSelectionChangedDeferred = undefined;
            });
        }

        function loadAllControls() {

            setTitle();
            setData();

            return UtilsService.waitPromiseNode({
                promises: [loadDataRecordTypeSelector(), loadDataRecordFields(), loadRecordFilterDirectiveLoadPromise(), loadSubviewGrid(), loadLocalizationTextResourceSelector()],
                getChildNode: function () {

                    var advancedFilters;
                    var dataRecordTypeId;

                    if (dataRecordSource != undefined) {
                        dataRecordTypeId = dataRecordSource.DataRecordTypeId;
                        advancedFilters = dataRecordSource.AdvancedFilters;
                    }

                    if (advancedFilters != undefined) {
                        $scope.scopeModel.selectedAdvancedFilterFieldsRelationType =
                            UtilsService.getItemByVal($scope.scopeModel.advancedFilterFieldsRelationTypeDS, advancedFilters.FieldsRelationType, "value");

                        if ($scope.scopeModel.selectedAdvancedFilterFieldsRelationType.value == VR_Analytic_AdvancedFilterFieldsRelationType.SpecificFields.value) {
                            selectedAdvancedFilterFieldNames = [];
                            for (var i = 0; i < dataRecordSource.AdvancedFilters.AvailableFields.length; i++) {
                                var advancedFilterField = dataRecordSource.AdvancedFilters.AvailableFields[i];
                                selectedAdvancedFilterFieldNames.push(advancedFilterField.FieldName);
                            }
                        }
                    }
                    else {
                        $scope.scopeModel.selectedAdvancedFilterFieldsRelationType = VR_Analytic_AdvancedFilterFieldsRelationType.AllFields;
                    }

                    var childPromises = [];

                    if (dataRecordTypeId != undefined) {
                        childPromises.push(loadAdvancedFilterSelector(dataRecordTypeId));
                    }

                    return { promises: childPromises };
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode && dataRecordSource != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(dataRecordSource.Title, 'Record Source');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Record Source');
        }
        function setData() {
            if (dataRecordSource != undefined) {
                $scope.scopeModel.title = dataRecordSource.Title;
                $scope.scopeModel.sourceName = dataRecordSource.Name;
                $scope.scopeModel.hideTimeRange = dataRecordSource.HideTimeRange;
            }
        }
        function loadDataRecordTypeSelector() {
            var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            dataRecordTypeSelectorReadyDeferred.promise.then(function () {

                var payload;
                if (isEditMode) {
                    payload = { selectedIds: dataRecordSource.DataRecordTypeId };
                }
                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
            });

            return dataRecordTypeSelectorLoadDeferred.promise;
        }
        function loadDataRecordFields() {

            if (!isEditMode)
                return UtilsService.waitPromiseNode({ promises: [] });

            return UtilsService.waitPromiseNode({
                promises: [dataRecordTypeSelectionChangedDeferred.promise],
                getChildNode: function () {
                    var promises = [];

                    $scope.selectedFields.length = 0;
                    $scope.selectedDetails.length = 0;

                    if (dataRecordSource != undefined && dataRecordSource.GridColumns) {
                        for (var x = 0; x < dataRecordSource.GridColumns.length; x++) {
                            var currentColumn = dataRecordSource.GridColumns[x];
                            var selectedField = UtilsService.getItemByVal($scope.dataRecordTypeFields, currentColumn.FieldName, "Name");
                            if (selectedField != undefined) {

                                $scope.selectedFields.push(selectedField);

                                var gridColumnItem = {
                                    payload: selectedField,
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    styleReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    styleLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(gridColumnItem.loadPromiseDeferred.promise);
                                promises.push(gridColumnItem.styleLoadPromiseDeferred.promise);
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(gridColumnItem.textResourceLoadPromiseDeferred.promise);
                                addGridColumnAPI(gridColumnItem, currentColumn);
                            }
                        }
                    }
                    if (dataRecordSource != undefined && dataRecordSource.ItemDetails) {
                        for (var y = 0; y < dataRecordSource.ItemDetails.length; y++) {
                            var currentDetail = dataRecordSource.ItemDetails[y];
                            var selectedDetail = UtilsService.getItemByVal($scope.dataRecordTypeFields, currentDetail.FieldName, "Name");
                            if (selectedDetail != undefined) {
                                $scope.selectedDetails.push(selectedDetail);
                                var itemDetail = {
                                    payload: selectedDetail,
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(itemDetail.textResourceLoadPromiseDeferred.promise);

                                addGridItemDetailAPI(itemDetail, currentDetail);
                            }
                        }
                    }
                    if (dataRecordSource != undefined && dataRecordSource.SortColumns) {
                        for (var z = 0; z < dataRecordSource.SortColumns.length; z++) {
                            var currentSortColumn = dataRecordSource.SortColumns[z];
                            var selectedSortColumn = UtilsService.getItemByVal($scope.dataRecordTypeFields, currentSortColumn.FieldName, "Name");
                            if (selectedSortColumn != undefined) {
                                $scope.selectedSortColumns.push(selectedSortColumn);

                                var sortColumn = {
                                    payload: selectedSortColumn,
                                };
                                addSortColumnAPI(sortColumn, currentSortColumn);
                            }
                        }
                    }
                    if (dataRecordSource != undefined && dataRecordSource.Filters) {
                        for (var z = 0; z < dataRecordSource.Filters.length; z++) {
                            var currentFilter = dataRecordSource.Filters[z];
                            var selectedFilter = UtilsService.getItemByVal($scope.dataRecordTypeFields, currentFilter.FieldName, "Name");
                            if (selectedFilter != undefined) {
                                $scope.selectedFilters.push(selectedFilter);

                                var filter = {
                                    payload: selectedFilter,
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(filter.textResourceLoadPromiseDeferred.promise);

                                addFilterAPI(filter, currentFilter);
                            }
                        }
                    }
                    return { promises: promises };
                }
            });
        }
        function loadRecordFilterDirectiveLoadPromise() {
            if (!isEditMode)
                return UtilsService.waitPromiseNode({ promises: [] });

            return UtilsService.waitPromiseNode({
                promises: [recordFilterDirectiveReadyDeferred.promise, dataRecordTypeSelectionChangedDeferred.promise],
                getChildNode: function () {
                    var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    recordFilterDirectiveReadyDeferred.promise.then(function () {
                        var recordFilterDirectivePayload = {};
                        recordFilterDirectivePayload.context = buildContext();
                        if (dataRecordSource != undefined && dataRecordSource.RecordFilter) {
                            recordFilterDirectivePayload.FilterGroup = dataRecordSource.RecordFilter;
                        }
                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });

                    return { promises: [recordFilterDirectiveLoadDeferred.promise] };
                }
            });
        }

        function loadAdvancedFilterSelector(dataRecordTypeId) {
            var loadAdvancedFilterDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            advancedFilterFieldsReadyDeferred.promise.then(function () {

                var payloadAdvancedFilterDirective = {
                    dataRecordTypeId: dataRecordTypeId,
                    selectedIds: selectedAdvancedFilterFieldNames
                };
                VRUIUtilsService.callDirectiveLoad(advancedFilterFieldsSelectorAPI, payloadAdvancedFilterDirective, loadAdvancedFilterDirectivePromiseDeferred);
            });

            return loadAdvancedFilterDirectivePromiseDeferred.promise;
        }

        function loadSubviewGrid() {

            if (!isEditMode)
                return UtilsService.waitPromiseNode({ promises: [] });

            var subviewGridLoadDeferred = UtilsService.createPromiseDeferred();

            subviewDefinitionGridReadyDeferred.promise.then(function () {
                if (dataRecordSource != undefined && dataRecordSource.SubviewDefinitions) {
                    for (var index = 0; index < dataRecordSource.SubviewDefinitions.length; index++) {
                        var currentSubviewDefinition = dataRecordSource.SubviewDefinitions[index];
                        $scope.selectedSubviewDefinitionsGrid.push(currentSubviewDefinition);
                    }
                }
                subviewGridLoadDeferred.resolve();
            });

            return subviewGridLoadDeferred.promise;
        }

        function loadDataRecord(dataRecordTypeId) {
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                $scope.dataRecordTypeFields.length = 0;
                if (response != undefined) {
                    angular.forEach(response, function (item) {
                        $scope.dataRecordTypeFields.push(item.Entity);
                    });
                }
            });
        }
        function loadLocalizationTextResourceSelector() {
            var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            var localizationTextResourcePayload = { selectedValue: dataRecordSource != undefined ? dataRecordSource.TitleResourceKey : undefined };

            localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
            });
            return localizationTextResourceSelectorLoadPromiseDeferred.promise;
        }
        function addGridColumnAPI(gridField, payload) {

            var dataItemPayload = {
                data: {
                    Width: VRCommon_GridWidthFactorEnum.Normal.value
                }
            };

            var dataItem = {
                FieldName: gridField.payload.Name,
                FieldTitle: gridField.payload.Title
            };

            var stylePayload;
            var textResourcePayload;
            if (payload) {
                dataItem.FieldTitle = payload.FieldTitle;
                dataItem.FieldDescription = payload.FieldDescription;
                dataItemPayload.data = payload.ColumnSettings;
                dataItem.isHidden = payload.IsHidden;
                dataItem.oldTitleResourceKey = payload.TitleResourceKey;
                stylePayload = { selectedIds: payload.ColumnStyleId };
                textResourcePayload = { selectedValue: payload.TitleResourceKey };
            }

            dataItem.onGridWidthFactorSelectorReady = function (api) {
                dataItem.gridWidthFactorAPI = api;
                gridField.readyPromiseDeferred.resolve();
            };
            dataItem.onDataRecordGridStyleDefinitionReady = function (api) {
                dataItem.dataRecordGridStyleAPI = api;
                gridField.styleReadyPromiseDeferred.resolve();
            };
            dataItem.onTextResourceSelectorReady = function (api) {
                dataItem.textResourceSeletorAPI = api;
                gridField.textResourceReadyPromiseDeferred.resolve();
            };

            gridField.readyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataItem.gridWidthFactorAPI, dataItemPayload, gridField.loadPromiseDeferred);
            });
            gridField.styleReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataItem.dataRecordGridStyleAPI, stylePayload, gridField.styleLoadPromiseDeferred);
            });
            gridField.textResourceReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, gridField.textResourceLoadPromiseDeferred);
            });

            $scope.selectedFieldsGrid.push(dataItem);
        }
        function addGridItemDetailAPI(gridField, payload) {
            var dataItem = {
                FieldName: gridField.payload.Name,
                FieldTitle: gridField.payload.Title,
                SelectedDetailWidth: payload != undefined ? UtilsService.getItemByVal($scope.scopeModel.detailWidths, payload.ColumnWidth, 'value') : ColumnWidthEnum.QuarterRow
            };
            var textResourcePayload = { selectedValue: payload != undefined ? payload.TitleResourceKey : undefined };

            if (payload) {
                dataItem.FieldTitle = payload.FieldTitle;
                dataItem.oldTitleResourceKey = payload.TitleResourceKey;

            }
            dataItem.onTextResourceSelectorReady = function (api) {
                dataItem.textResourceSeletorAPI = api;
                gridField.textResourceReadyPromiseDeferred.resolve();
            };

            gridField.textResourceReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, gridField.textResourceLoadPromiseDeferred);
                });

            $scope.selectedDetailsGrid.push(dataItem);
        }
        function addSortColumnAPI(gridField, payload) {
            var dataItem = {
                FieldName: gridField.payload.Name,
                FieldTitle: gridField.payload.Title,
                SelectedOrderDirection: payload != undefined ? UtilsService.getItemByVal($scope.scopeModel.orderDirectionList, payload.IsDescending, 'value') : undefined
            };
            if (payload) {
                dataItem.FieldTitle = payload.FieldTitle;
            }
            $scope.selectedSortColumnsGrid.push(dataItem);
        }
        function addFilterAPI(filter, payload) {
            var dataItem = {
                FieldName: filter.payload.Name,
                FieldTitle: filter.payload.Title,
                IsRequired: filter.payload.IsRequired
            };
            var textResourcePayload;

            if (payload) {
                dataItem.FieldTitle = payload.FieldTitle;
                dataItem.IsRequired = payload.IsRequired;
                dataItem.oldTitleResourceKey = payload.TitleResourceKey;
                textResourcePayload = { selectedValue: payload.TitleResourceKey };
            }
            dataItem.onTextResourceSelectorReady = function (api) {
                dataItem.textResourceSeletorAPI = api;
                filter.textResourceReadyPromiseDeferred.resolve();
            };
            filter.textResourceReadyPromiseDeferred.promise
                .then(function () {
                    VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, filter.textResourceLoadPromiseDeferred);
                });
            $scope.selectedFiltersGrid.push(dataItem);
        }

        function saveDataRecordSource() {
            var sourceObj = buildSourceObj();
            if ($scope.onDataRecordSourceAdded != undefined && typeof ($scope.onDataRecordSourceAdded) == 'function') {
                $scope.onDataRecordSourceAdded(sourceObj);
            }
            $scope.modalContext.closeModal();
        }
        function updateDataRecordSource() {
            var sourceObj = buildSourceObj();
            if ($scope.onDataRecordSourceUpdated != undefined && typeof ($scope.onDataRecordSourceUpdated) == 'function') {
                $scope.onDataRecordSourceUpdated(sourceObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildSourceObj() {

            var columns = [];
            for (var x = 0; x < $scope.selectedFieldsGrid.length; x++) {
                var currentItem = $scope.selectedFieldsGrid[x];
                columns.push({
                    FieldName: currentItem.FieldName,
                    FieldTitle: currentItem.FieldTitle,
                    FieldDescription: currentItem.FieldDescription,
                    ColumnSettings: currentItem.gridWidthFactorAPI.getData(),
                    IsHidden: currentItem.isHidden,
                    ColumnStyleId: currentItem.dataRecordGridStyleAPI.getSelectedIds(),
                    TitleResourceKey: currentItem.textResourceSeletorAPI != undefined ? currentItem.textResourceSeletorAPI.getSelectedValues() : currentItem.oldTitleResourceKey
                });
            }

            var filters = [];
            for (var x = 0; x < $scope.selectedFiltersGrid.length; x++) {
                var currentFilter = $scope.selectedFiltersGrid[x];
                filters.push({
                    FieldName: currentFilter.FieldName,
                    FieldTitle: currentFilter.FieldTitle,
                    IsRequired: currentFilter.IsRequired,
                    TitleResourceKey: currentFilter.textResourceSeletorAPI != undefined ? currentFilter.textResourceSeletorAPI.getSelectedValues() : currentFilter.oldTitleResourceKey
                });
            }

            var details = [];
            for (var y = 0; y < $scope.selectedDetailsGrid.length; y++) {
                var currentDetail = $scope.selectedDetailsGrid[y];
                details.push({
                    FieldName: currentDetail.FieldName,
                    FieldTitle: currentDetail.FieldTitle,
                    ColumnWidth: currentDetail.SelectedDetailWidth.value,
                    TitleResourceKey: currentDetail.textResourceSeletorAPI != undefined ? currentDetail.textResourceSeletorAPI.getSelectedValues() : currentDetail.oldTitleResourceKey
                });
            }

            var subviewDefinitions = [];
            for (var t = 0; t < $scope.selectedSubviewDefinitionsGrid.length; t++) {
                var currentSubviewDefinition = $scope.selectedSubviewDefinitionsGrid[t];
                subviewDefinitions.push(currentSubviewDefinition);
            }

            var sortColumns = [];
            for (var z = 0; z < $scope.selectedSortColumnsGrid.length; z++) {
                var currentSortColumn = $scope.selectedSortColumnsGrid[z];
                sortColumns.push({ FieldName: currentSortColumn.FieldName, IsDescending: currentSortColumn.SelectedOrderDirection.value });
            }

            var advancedFilters = {};
            advancedFilters.FieldsRelationType = $scope.scopeModel.selectedAdvancedFilterFieldsRelationType.value;

            if ($scope.scopeModel.showAdvancedFilterFields) {
                advancedFilters.AvailableFields = [];

                for (var i = 0; i < $scope.scopeModel.selectedAdvancedFilterFields.length; i++) {
                    var advancedFilterField = $scope.scopeModel.selectedAdvancedFilterFields[i];
                    advancedFilters.AvailableFields.push({ FieldName: advancedFilterField.Name });
                }
            }

            var obj = {
                Title: $scope.scopeModel.title,
                Name: $scope.scopeModel.sourceName,
                DataRecordTypeId: $scope.scopeModel.selectedDataRecordType.DataRecordTypeId,
                RecordStorageIds: dataRecordStorageSelectorAPI.getSelectedIds(),
                GridColumns: columns,
                ItemDetails: details,
                SubviewDefinitions: subviewDefinitions,
                SortColumns: sortColumns,
                Filters: filters,
                RecordFilter: recordFilterDirectiveAPI.getData().filterObj,
                TitleResourceKey: localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined,
                HideTimeRange: $scope.scopeModel.hideTimeRange,
                AdvancedFilters: advancedFilters,
            };
            return obj;
        }
        function buildContext() {
            var context = {
                getFields: function () {
                    var fields = [];
                    if ($scope.dataRecordTypeFields != undefined) {
                        for (var i = 0; i < $scope.dataRecordTypeFields.length; i++) {
                            var currentItem = $scope.dataRecordTypeFields[i];

                            fields.push({
                                FieldName: currentItem.Name,
                                FieldTitle: currentItem.Title,
                                Type: currentItem.Type
                            });
                        }
                    }
                    return fields;
                }
            };
            return context;
        }
    }

    appControllers.controller('Analytic_DataRecordSourceEditorController', DataRecordSourceEditorController);
})(appControllers);