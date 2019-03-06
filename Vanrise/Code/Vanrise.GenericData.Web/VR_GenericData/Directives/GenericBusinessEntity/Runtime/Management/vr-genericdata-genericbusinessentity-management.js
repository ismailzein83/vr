﻿(function (app) {

    'use strict';

    GenericBusinessEntityManagement.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_RecordQueryLogicalOperatorEnum'];

    function GenericBusinessEntityManagement(UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBusinessEntityService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_RecordQueryLogicalOperatorEnum) {
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

            var filterDirectiveAPI;
            var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var gridDirectiveAPI;
            var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var businessDefinitionId;
            var genericBEDefinitionSettings;
            var businessEntityDefinitionSettingsLoadDeferred = UtilsService.createPromiseDeferred();
            var fieldValues;
            var filterValues;
            var defaultValues;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showAddButton = true;
                $scope.scopeModel.showUploadButton = true;

                $scope.scopeModel.hasFilter = false;

                $scope.scopeModel.onFilterDirectiveReady = function (api) {
                    filterDirectiveAPI = api;
                    filterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onGridDirectiveReady = function (api) {
                    gridDirectiveAPI = api;
                    gridDirectiveReadyDeferred.resolve();
                };

                $scope.search = function () {
                    return gridDirectiveAPI.load(getGridFilter());
                };

                $scope.addBusinessEntity = function () {
                    var onGenericBusinessEntityAdded = function (addedGenericBusinessEntity) {
                        gridDirectiveAPI.onGenericBEAdded(addedGenericBusinessEntity);
                    };
                    var editorSize = undefined;//genericBEDefinitionSettings != undefined ? genericBEDefinitionSettings.editorSize : undefined;
                    VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(onGenericBusinessEntityAdded, businessDefinitionId, editorSize, fieldValues, defaultValues);
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

                    var promises = [];

                    if (payload != undefined) {
                        businessDefinitionId = payload.businessEntityDefinitionId;
                        fieldValues = payload.fieldValues;
                        filterValues = payload.filterValues;
                        defaultValues = payload.defaultValues;
                    }

                    promises.push(loadBusinessEntityDefinitionSettings());
                    promises.push(loadExistingFilter());
                    promises.push(loadGridDirective());
                    promises.push(loadCheckDoesUserHaveAddAccess());

                    function loadCheckDoesUserHaveAddAccess() {
                        return VR_GenericData_GenericBusinessEntityAPIService.DoesUserHaveAddAccess(businessDefinitionId).then(function (response) {
                            $scope.scopeModel.showAddButton = $scope.scopeModel.showAddButton && response;
                            $scope.scopeModel.showUploadButton = $scope.scopeModel.showAddButton && $scope.scopeModel.showUploadButton;
                        });
                    }


                    function loadBusinessEntityDefinitionSettings() {

                        return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessDefinitionId).then(function (response) {
                            genericBEDefinitionSettings = response;
                            if (genericBEDefinitionSettings != undefined) {
                                if (genericBEDefinitionSettings.FilterDefinition != undefined && genericBEDefinitionSettings.FilterDefinition.Settings != undefined) {
                                    $scope.scopeModel.filterDirective = genericBEDefinitionSettings.FilterDefinition.Settings.RuntimeEditor;
                                }

                                $scope.scopeModel.showAddButton = $scope.scopeModel.showAddButton && !genericBEDefinitionSettings.HideAddButton;
                                $scope.scopeModel.showUploadButton = genericBEDefinitionSettings.ShowUpload && $scope.scopeModel.showAddButton;
                            }
                        });
                    }

                    function loadExistingFilter() {
                        return loadFilterDirective().then(function () {
                            $scope.scopeModel.hasFilter = filterDirectiveAPI != undefined ? filterDirectiveAPI.hasFilters() : false;
                        });
                    }

                    function loadFilterDirective() {
                        var filterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([loadBusinessEntityDefinitionSettings()]).then(function () {
                            if ($scope.scopeModel.filterDirective != undefined) {
                                filterDirectiveReadyDeferred.promise.then(function () {

                                    var filterDirectivePayload = {
                                        settings: genericBEDefinitionSettings.FilterDefinition.Settings,
                                        dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId,
                                        filterValues: filterValues
                                    };
                                    VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, filterDirectivePayload, filterDirectiveLoadDeferred);
                                });
                            } else {
                                filterDirectiveLoadDeferred.resolve();
                            }
                        });
                        return filterDirectiveLoadDeferred.promise;
                    }


                    function loadGridDirective() {
                        var promiseDeferred = UtilsService.createPromiseDeferred();

                        gridDirectiveReadyDeferred.promise.then(function () {
                            gridDirectiveAPI.load(getGridFilter());
                            promiseDeferred.resolve();
                        });

                        return promiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }



            function getGridFilter() {
                var filterData = filterDirectiveAPI != undefined ? filterDirectiveAPI.getData() : undefined;
                var filterGroup;
                var filters;

                if (filterData != undefined) {
                    if (filterData.RecordFilter != undefined) {
                        if (filterData.RecordFilter.$type.indexOf("RecordFilterGroup") < 0) {
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
                        filters.push({
                            FieldName: key,
                            FilterValues: [filterValues[key]]
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
                        LimitResult: filterData != undefined ? filterData.LimitResult : undefined
                    },
                    businessEntityDefinitionId: businessDefinitionId
                };
                return gridPayload;
            }
        }

    }

    app.directive('vrGenericdataGenericbusinessentityManagement', GenericBusinessEntityManagement);


})(app);