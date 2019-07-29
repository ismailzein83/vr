(function (app) {

    'use strict';

    ListGenericFilterRuntimeSettingsDirective.$inject = ['UtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_DataRecordFieldAPIService', 'VRUIUtilsService'];

    function ListGenericFilterRuntimeSettingsDirective(UtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_DataRecordFieldAPIService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericFilterRuntimeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/GenericListFilterRuntimeTemplate.html"
        };

        function GenericFilterRuntimeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var definitionSettings;
            var dataRecordTypeId;
            var dataRecordType;
            var fieldTypeConfigs;
            var isFromFilterSection;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.filters = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        dataRecordTypeId = payload.dataRecordTypeId;
                        definitionSettings = payload.settings;
                        isFromFilterSection = payload.isFromFilterSection;
                        var searchManagementFunc = payload.searchManagementFunc;
                        var allFieldValuesByName = payload.allFieldValuesByName;
                        var genericContext = payload.genericContext;

                        var loadPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadPromiseDeferred.promise);

                        UtilsService.waitMultipleAsyncOperations([loadDataRecordType, loadDataRecordTypeFieldConfigs]).then(function () {
                            if (definitionSettings != undefined) {
                                var genericFilters = definitionSettings.Filters;
                                if (genericFilters != undefined) {
                                    for (var i = 0; i < genericFilters.length; i++) {
                                        var genericFilter = genericFilters[i];
                                        addFilter(genericFilter);
                                    }
                                }
                            }
                        }).catch(function (error) {
                            loadPromiseDeferred.reject(error);
                        });
                    }

                    function loadDataRecordType() {
                        return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                            dataRecordType = response;
                        });
                    }

                    function loadDataRecordTypeFieldConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            fieldTypeConfigs = response;
                        });
                    }

                    function addFilter(genericFilter) {
                        var filterEditor;

                        var fieldType = UtilsService.getItemByVal(dataRecordType.Fields, genericFilter.FieldName, 'Name');
                        if (fieldType != undefined) {
                            var fieldTypeConfig = UtilsService.getItemByVal(fieldTypeConfigs, fieldType.Type.ConfigId, 'ExtensionConfigurationId');
                            if (fieldTypeConfig != undefined) {
                                filterEditor = fieldTypeConfig.FilterEditor;
                            }
                        }
                        if (filterEditor == null)
                            return;
                        var filter = {
                            fieldName: genericFilter.FieldName,
                            isRequired: genericFilter.IsRequired,
                            directiveEditor: filterEditor,
                            directiveLoadDeferred: UtilsService.createPromiseDeferred()
                        };
                        filter.onDirectiveReady = function (api) {
                            filter.directiveAPI = api;
                            var directivePayload = {
                                fieldTitle: genericFilter.FieldTitle,
                                fieldType: fieldType != undefined ? fieldType.Type : undefined,
                                fieldName: genericFilter.FieldName,
                                fieldValue: genericFilter.DefaultFieldValues,
                                genericContext: genericContext,
                                allFieldValuesByName: allFieldValuesByName,
                                //dataRecordTypeId: dataRecordTypeId
                                isFromFilterSection: isFromFilterSection,
                                triggerSearch: genericFilter.TriggerSearch,
                                searchManagementFunc: searchManagementFunc
                            };

                            VRUIUtilsService.callDirectiveLoad(filter.directiveAPI, directivePayload, filter.directiveLoadDeferred);
                        };
                        filter.directiveLoadDeferred.promise.then(function () {
                            loadPromiseDeferred.resolve();
                        }).catch(function (error) {
                            loadPromiseDeferred.reject(error);
                        });
                        $scope.scopeModel.filters.push(filter);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var array = [];
                    var filters = $scope.scopeModel.filters;
                    if (filters != undefined) {
                        for (var i = 0; i < filters.length; i++) {
                            var filter = filters[i];
                            var finalArray = [];
                            var values;
                            if (filter.directiveAPI != undefined) {
                                values = filter.directiveAPI.getValuesAsArray();
                            }

                            if (values != undefined) {
                                for (var j = 0; j < values.length; j++) {
                                    var value = values[j];
                                    if (value != undefined) {
                                        finalArray.push(value);
                                    }
                                }
                            }

                            if (finalArray != undefined && finalArray.length > 0) {
                                array.push({
                                    FieldName: filter.fieldName,
                                    FilterValues: finalArray
                                });
                            }
                        }
                    }
                    return {
                        Filters: array
                    };
                };

                api.hasFilters = function () {
                    return $scope.scopeModel.filters.length > 0;
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    var _promises = [];
                    var filters = $scope.scopeModel.filters;
                    if (filters != undefined) {
                        for (var i = 0; i <filters.length; i++) {
                            var directiveAPI = filters[i].directiveAPI;
                            if (directiveAPI != undefined && directiveAPI.onFieldValueChanged != undefined && typeof (directiveAPI.onFieldValueChanged) == "function") {
                                var onFieldValueChangedPromise = directiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                                if (onFieldValueChangedPromise != undefined)
                                    _promises.push(onFieldValueChangedPromise);
                            }
                        }
                    }
                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeListfilterruntimeGeneric', ListGenericFilterRuntimeSettingsDirective);
})(app);