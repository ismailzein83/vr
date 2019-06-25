﻿(function (app) {

    'use strict';

    GenericFilterRuntimeSettingsDirective.$inject = ['UtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_DataRecordFieldAPIService', 'VRUIUtilsService'];

    function GenericFilterRuntimeSettingsDirective(UtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_DataRecordFieldAPIService, VRUIUtilsService) {
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/GenericFilterRuntimeTemplate.html"
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
                        var allFieldValuesByName = payload.allFieldValuesByName;
                        var genericContext = payload.genericContext;

                        var promise = UtilsService.createPromiseDeferred();
                        promises.push(promise.promise);

                        UtilsService.waitMultipleAsyncOperations([loadDataRecordType, loadDataRecordTypeFieldConfigs]).then(function () {
                            var fieldType = UtilsService.getItemByVal(dataRecordType.Fields, definitionSettings.FieldName, 'Name');
                            var filterEditor;
                            var fieldTypeConfig;
                            if (fieldType != undefined) {
                                fieldTypeConfig = UtilsService.getItemByVal(fieldTypeConfigs, fieldType.Type.ConfigId, 'ExtensionConfigurationId');
                            }
                            if (fieldTypeConfig != undefined) {
                                filterEditor = fieldTypeConfig.FilterEditor;
                            }
                            if (filterEditor == null) return;

                            var filter = {};
                            filter.fieldName = definitionSettings.FieldName;
                            filter.isRequired = definitionSettings.IsRequired;
                            filter.directiveEditor = filterEditor;
                            filter.directiveLoadDeferred = UtilsService.createPromiseDeferred();
                            filter.onDirectiveReady = function (api) {
                                filter.directiveAPI = api;
                                var directivePayload = {
                                    fieldTitle: definitionSettings.FieldTitle,
                                    fieldType: fieldType != undefined ? fieldType.Type : undefined,
                                    fieldName: definitionSettings.FieldName,
                                    fieldValue: definitionSettings.DefaultFieldValues,
                                    genericContext: genericContext,
                                    allFieldValuesByName: allFieldValuesByName,
                                    //dataRecordTypeId: dataRecordTypeId
                                    isFromFilterSection: isFromFilterSection
                                };
                                VRUIUtilsService.callDirectiveLoad(filter.directiveAPI, directivePayload, filter.directiveLoadDeferred);
                            };
                            filter.directiveLoadDeferred.promise.then(function () {
                                promise.resolve();
                            }).catch(function (error) {
                                promise.reject(error);
                            });

                            $scope.scopeModel.filter = filter;

                        }).catch(function (error) {
                            promise.reject(error);
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

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var values;
                    if ($scope.scopeModel.filter.directiveAPI != undefined) {
                        values = $scope.scopeModel.filter.directiveAPI.getValuesAsArray();
                    }

                    if (values != undefined) {
                        var finalArray = [];

                        for (var i = 0; i < values.length; i++) {
                            var value = values[i];
                            if (value != undefined) {
                                finalArray.push(value);
                            }
                        }
                    }

                    if (finalArray == undefined || finalArray.length == 0)
                        return;
                    return {
                        Filters: [{
                            FieldName: $scope.scopeModel.filter.fieldName,
                            FilterValues: finalArray
                        }]
                    };
                };

                api.hasFilters = function () {
                    return $scope.scopeModel.filter.directiveAPI != undefined;
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    var _promises = [];
                    var directiveAPI = $scope.scopeModel.filter.directiveAPI;

                    if (directiveAPI != undefined && directiveAPI.onFieldValueChanged != undefined && typeof (directiveAPI.onFieldValueChanged) == "function") {
                        var onFieldValueChangedPromise = directiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                        if (onFieldValueChangedPromise != undefined)
                            _promises.push(onFieldValueChangedPromise);
                    }
                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeGeneric', GenericFilterRuntimeSettingsDirective);

})(app);