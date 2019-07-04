(function (app) {

    'use strict';

    GenericBusinessEntityManagement.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_RecordQueryLogicalOperatorEnum', 'VRCommon_ModalWidthEnum', 'VR_GenericData_GenericBECustomActionService'];

    function GenericBusinessEntityManagement(UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBusinessEntityService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_RecordQueryLogicalOperatorEnum, VRCommon_ModalWidthEnum, VR_GenericData_GenericBECustomActionService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntityManagement($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,

            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Management/Templates/GenericBusinessEntityManagementTemplate.html"
        };

        function GenericBusinessEntityManagement($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var filterRuntimeRootDirectiveAPI;
            var filterRuntimeRootDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var gridDirectiveAPI; 
            var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var businessDefinitionId;
            var genericBEDefinitionSettings;
            var fieldValues;
            var filterValues;

            var showAddFromAccess;
            var showUploadFromAccess;

            var showAddFromDefinitionSettings;
            var showUploadFromDefinitionSettings;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.customActions = [];

                $scope.scopeModel.showAddButton = false;
                $scope.scopeModel.showUploadButton = false;

                $scope.scopeModel.hasFilter = false;

                $scope.scopeModel.onFilterRuntimeRootDirectiveReady = function (api) {
                    filterRuntimeRootDirectiveAPI = api;
                    filterRuntimeRootDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onGridDirectiveReady = function (api) {
                    gridDirectiveAPI = api;
                    gridDirectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.isFilterSectionShow = false;
                $scope.scopeModel.showFilterSection = function () {
                    $scope.scopeModel.isFilterSectionShow = !$scope.scopeModel.isFilterSectionShow;
                };
                $scope.search = function () {
                   return gridDirectiveAPI.load(getGridFilter());
                };

                $scope.addBusinessEntity = function () {
                    var editorEnum = UtilsService.getEnum(VRCommon_ModalWidthEnum, "value", genericBEDefinitionSettings.EditorSize);
                    var editorSize = editorEnum != undefined ? editorEnum.modalAttr : undefined;

                    var onGenericBusinessEntityAdded = function (addedGenericBusinessEntity, reopen) {
                        gridDirectiveAPI.onGenericBEAdded(addedGenericBusinessEntity);
                        if (reopen) {
                            VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBusinessEntityAdded, businessDefinitionId, editorSize, fieldValues);
                        }
                    };
                    VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBusinessEntityAdded, businessDefinitionId, editorSize, fieldValues);
                };

                $scope.scopeModel.uploadBusinessEntity = function () {
                    var editorSize = undefined;
                    VR_GenericData_GenericBusinessEntityService.uploadGenericBusinessEntity(businessDefinitionId, editorSize);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.hasFilter = false;

                    if (payload != undefined) {
                        businessDefinitionId = payload.businessEntityDefinitionId;
                        fieldValues = payload.fieldValues;
                        filterValues = payload.filterValues;
                    }

                    var rootPromiseNode = {
                        promises: [],
                        getChildNode: function () {
                            var promises1 = [];
                            promises1.push(loadCheckDoesUserHaveAddAccess());
                            promises1.push(loadBusinessEntityDefinitionSettings());
                            return {
                                promises: promises1,
                                getChildNode: function () {
                                    var promises2 = [];
                                    if (filterValues != undefined)
                                        promises2.push(getDependentFieldValues());
                                    return {
                                        promises: promises2,
                                        getChildNode: function () {
                                            var promises3 = [];
                                            promises3.push(loadExistingFilter());
                                            return {
                                                promises: promises3,
                                                getChildNode: function () {
                                                    var promises4 = [];
                                                    $scope.scopeModel.customActions = VR_GenericData_GenericBECustomActionService.buildCustomActions(genericBEDefinitionSettings, businessDefinitionId, fieldValues, getContext());
                                                    promises4.push(loadGridDirective());
                                                    return {
                                                        promises: promises4
                                                    };
                                                }
                                            };
                                        }
                                    };
                                }
                            };
                        }
                    };

                    function loadCheckDoesUserHaveAddAccess() {
                        return VR_GenericData_GenericBusinessEntityAPIService.DoesUserHaveAddAccess(businessDefinitionId).then(function (response) {
                            showAddFromAccess = response;
                            showUploadFromAccess = response;
                        });
                    }

                    function loadBusinessEntityDefinitionSettings() {

                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessDefinitionId).then(function (response) {
                            genericBEDefinitionSettings = response;
                            if (genericBEDefinitionSettings != undefined) {
                                if (genericBEDefinitionSettings.FilterDefinition != undefined && genericBEDefinitionSettings.FilterDefinition.Settings != undefined) {
                                    $scope.scopeModel.filterRuntimeEditor = genericBEDefinitionSettings.FilterDefinition.Settings.RuntimeEditor;
                                }

                                showAddFromDefinitionSettings = !genericBEDefinitionSettings.HideAddButton;
                                showUploadFromDefinitionSettings = genericBEDefinitionSettings.ShowUpload;
                            }
                        });
                    }

                    function getDependentFieldValues() {
                        var getDependentFieldValuesPromiseDeferred = UtilsService.createPromiseDeferred();

                        var fieldValues = {};
                        for (var prop in filterValues) {
                            fieldValues[prop] = filterValues[prop].value;
                        }

                        var input = {
                            DataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId,
                            FieldValues: fieldValues
                        };

                        VR_GenericData_GenericBusinessEntityAPIService.GetDependentFieldValues(input).then(function (response) {
                            if (response) {
                                for (var prop in response) {
                                    var dependentFieldValue = response[prop];
                                    if (prop != "$type") {
                                        filterValues[prop] = {
                                            value: dependentFieldValue,
                                            isHidden: true,
                                            isDisabled: true
                                        };
                                    }
                                }
                            }

                            getDependentFieldValuesPromiseDeferred.resolve();
                        });

                        return getDependentFieldValuesPromiseDeferred.promise;
                    }

                    function loadExistingFilter() {
                        return loadFilterRuntimeRootDirective().then(function () {
                            $scope.scopeModel.hasFilter = filterRuntimeRootDirectiveAPI != undefined ? filterRuntimeRootDirectiveAPI.hasFilters() : false;
                        });
                    }

                    function loadFilterRuntimeRootDirective() {
                        var filterRuntimeRootDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        if ($scope.scopeModel.filterRuntimeEditor != undefined) {
                            filterRuntimeRootDirectiveReadyDeferred.promise.then(function () {

                                var filterRuntimeRootDirectivePayload = {
                                    settings: genericBEDefinitionSettings.FilterDefinition.Settings,
                                    dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId,
                                    filterValues: filterValues,
                                    filterRuntimeEditor: $scope.scopeModel.filterRuntimeEditor
                                };
                                VRUIUtilsService.callDirectiveLoad(filterRuntimeRootDirectiveAPI, filterRuntimeRootDirectivePayload, filterRuntimeRootDirectiveLoadDeferred);
                            });
                        } else {
                            filterRuntimeRootDirectiveLoadDeferred.resolve();
                        }

                        return filterRuntimeRootDirectiveLoadDeferred.promise;
                    }

                    function loadGridDirective() {
                        var promiseDeferred = UtilsService.createPromiseDeferred();

                        gridDirectiveReadyDeferred.promise.then(function () {
                            gridDirectiveAPI.load(getGridFilter());
                            promiseDeferred.resolve();
                        });

                        return promiseDeferred.promise;
                    }




                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                        $scope.scopeModel.showAddButton = showAddFromDefinitionSettings && showAddFromAccess;
                        $scope.scopeModel.showUploadButton = showUploadFromDefinitionSettings && showUploadFromAccess;
                    });
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function getContext() {
                return {
                    trigerSearch: function (filterValues) {
                        var filters;
                        if (filterValues != undefined) {
                            if (filters == undefined)
                                filters = [];
                            for (var key in filterValues) {
                                var filterValue = filterValues[key];
                                filters.push({
                                    FieldName: key,
                                    FilterValues: [filterValue]
                                });
                            }
                        }
                        var gridPayload = {
                            query: {
                                FilterGroup: undefined,
                                Filters: filters,
                                fieldValues: fieldValues,
                                OrderType: genericBEDefinitionSettings.OrderType,
                                AdvancedOrderOptions: genericBEDefinitionSettings.AdvancedOrderOptions,
                            },
                            businessEntityDefinitionId: businessDefinitionId,
                            threeSixtyDegreeSettings: genericBEDefinitionSettings.ThreeSixtyDegreeSettings,
                            doNotLoadByDefault: genericBEDefinitionSettings.DoNotLoadByDefault
                        };
                        return gridDirectiveAPI.load(gridPayload);
                    },
                    expendRow: function (genericBEId) {
                       return gridDirectiveAPI.expendRow(genericBEId);
                    },
                    onGenericBEAdded : function (addedGenericBE, errorEntity) {
                        if (errorEntity != undefined && errorEntity.message != undefined) {
                            VR_GenericData_GenericBusinessEntityService.openErrorMessageEditor(errorEntity);
                        }
                        if (gridDirectiveAPI != undefined)
                            gridDirectiveAPI.onGenericBEAdded(addedGenericBE);
                    }
                };
            }

            function getGridFilter() {
                var filterData = filterRuntimeRootDirectiveAPI != undefined ? filterRuntimeRootDirectiveAPI.getData() : undefined;
                var filterGroup;
                var filters;

                if (filterData != undefined) {
                    if (filterData.RecordFilter != undefined) {
                        if (filterData.RecordFilter.$type.indexOf("RecordFilterGroup") != -1) {
                            filterGroup = filterData.RecordFilter;
                        } else {
                            filterGroup = {
                                $type: "Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities",
                                LogicalOperator: VR_GenericData_RecordQueryLogicalOperatorEnum.And.value,
                                Filters: [filterData.RecordFilter]
                            };
                        }
                    }

                    if (filterData.Filters != undefined) {
                        filters = [];
                        for (var i = 0; i < filterData.Filters.length; i++) {
                            filters.push(filterData.Filters[i]);
                        }

                    }
                }

                if (filterValues != undefined) {
                    if (filters == undefined)
                        filters = [];
                    for (var key in filterValues) {
                        var filterValue = filterValues[key].value;
                        filters.push({
                            FieldName: key,
                            FilterValues: [filterValue]
                        });
                    }
                }

                var gridPayload = {
                    query: {
                        FilterGroup: filterGroup,
                        FromTime: filterData != undefined ? filterData.FromTime : undefined,
                        ToTime: filterData != undefined ? filterData.ToTime : undefined,
                        Filters: filters,
                        fieldValues: fieldValues,
                        OrderType: genericBEDefinitionSettings.OrderType,
                        AdvancedOrderOptions: genericBEDefinitionSettings.AdvancedOrderOptions,
                        LimitResult: filterData != undefined ? filterData.LimitResult : undefined
                    },
                    businessEntityDefinitionId: businessDefinitionId,
                    threeSixtyDegreeSettings: genericBEDefinitionSettings.ThreeSixtyDegreeSettings,
                    doNotLoadByDefault: genericBEDefinitionSettings.DoNotLoadByDefault
                };
                return gridPayload;

            }
        }

    }

    app.directive('vrGenericdataGenericbusinessentityManagement', GenericBusinessEntityManagement);


})(app);