"use strict";

app.directive("vrGenericdataGenericbeFilterruntimeRootcontainer", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericBusinessEntityService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntityRootEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBEFilterRuntimeRootContainerEditor.html"
        };

        function GenericBusinessEntityRootEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var settings;
            var dataRecordTypeId;
            var filterValues;
            var searchManagementFunc;
            var allFieldValuesByFieldNames = {};

            var filterDirectiveAPI;
            var filterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFilterEditorRuntimeDirectiveReady = function (api) {
                    filterDirectiveAPI = api;
                    filterDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    if (payload != undefined) {
                        settings = payload.settings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        filterValues = payload.filterValues;
                        searchManagementFunc = payload.searchManagementFunc;
                        $scope.scopeModel.filterRuntimeEditor = payload.filterRuntimeEditor;
                    }

                    if ($scope.scopeModel.filterRuntimeEditor != undefined) {
                        var filterDirectiveLoadPromise = loadFilterDirective();
                        promises.push(filterDirectiveLoadPromise);
                    }

                    function loadFilterDirective() {
                        var filterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        filterDirectiveReadyDeferred.promise.then(function () {
                            var allFieldValuesByName;
                            if (filterValues != undefined) {
                                allFieldValuesByName = {};
                                for (var key in filterValues) {
                                    var filterValue = filterValues[key].value;
                                    allFieldValuesByName[key] = typeof (filterValue) != "object" ? [filterValue] : filterValue;
                                }
                            }

                            var filterDirectivePayload = {
                                settings: settings,
                                dataRecordTypeId: dataRecordTypeId,
                                filterValues: filterValues,
                                allFieldValuesByName: allFieldValuesByName,
                                genericContext: buildGenericContext(),
                                isFromFilterSection: true,
                                searchManagementFunc: searchManagementFunc
                            };
                            VRUIUtilsService.callDirectiveLoad(filterDirectiveAPI, filterDirectivePayload, filterDirectiveLoadDeferred);
                        });

                        return filterDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return filterDirectiveAPI != undefined ? filterDirectiveAPI.getData() : undefined;
                };

                api.hasFilters = function () {
                    return filterDirectiveAPI != undefined ? filterDirectiveAPI.hasFilters() : false;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }

            function buildGenericContext() {
                var context = {
                    notifyFieldValueChanged: function (changedField) {  //changedField = {fieldName : 'name', fieldValues : ['value1', 'value2'...] }
                        var fieldName = changedField.fieldName;
                        var fieldValues = changedField.fieldValues;

                        var _promises = [];

                        if (!VR_GenericData_GenericBusinessEntityService.tryUpdateAllFieldValuesByFieldName(fieldName, fieldValues, allFieldValuesByFieldNames)) {
                            return UtilsService.waitMultiplePromises(_promises);
                        }

                        if (filterDirectiveAPI.onFieldValueChanged != undefined && typeof (filterDirectiveAPI.onFieldValueChanged) == "function") {
                            var promise = filterDirectiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                            if (promise != undefined) {
                                _promises.push(promise);
                            }
                        }

                        return UtilsService.waitMultiplePromises(_promises);
                    }
                };

                return context;
            }
        }

        return directiveDefinitionObject;
    }
]);