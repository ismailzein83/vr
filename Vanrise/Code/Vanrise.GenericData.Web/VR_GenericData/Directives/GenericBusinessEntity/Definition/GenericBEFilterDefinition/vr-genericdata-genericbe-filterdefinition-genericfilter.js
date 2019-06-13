(function (app) {

    'use strict';

    FilterDefinitionGenericFilterDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService'];

    function FilterDefinitionGenericFilterDirective(UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericFilterCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/GenericFilterDefinitionTemplate.html'
        };

        function GenericFilterCtor($scope, ctrl) {

            var context;
            var settings;
            var dataRecordFieldTypes = [];
            var fieldType;
            var previousFieldTypeRuntimeDirective;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypePromiseDeferredSelectionChanged;

            var localizationTextResourceSelectorAPI;
            var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var fieldTypeRuntimeDirectiveAPI;
            var fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onLocalizationTextResourceDirectiveReady = function (api) {
                    localizationTextResourceSelectorAPI = api;
                    localizationTextResourceSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFieldTypeRumtimeDirectiveReady = function (api) {
                    fieldTypeRuntimeDirectiveAPI = api;
                    fieldTypeRuntimeReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeFieldsSelectionChanged = function (item) {
                    if (item != undefined) {
                        if (dataRecordTypePromiseDeferredSelectionChanged != undefined) {
                            dataRecordTypePromiseDeferredSelectionChanged.resolve();
                        } else {
                            $scope.scopeModel.fieldTitle = item.Title;
                            fieldType = context.getFieldType(item.Name);
                            var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");

                            if (dataRecordFieldType != undefined) {
                                $scope.scopeModel.fieldTypeRuntimeDirective = dataRecordFieldType.FilterEditor;

                                if (previousFieldTypeRuntimeDirective != $scope.scopeModel.fieldTypeRuntimeDirective) {
                                    fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    previousFieldTypeRuntimeDirective = $scope.scopeModel.fieldTypeRuntimeDirective;
                                }

                                fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
                                    var setLoader = function (value) { $scope.scopeModel.isFieldTypeRumtimeDirectiveLoading = value; };
                                    var directivePayload = {
                                        fieldTitle: "Default value",
                                        fieldType: fieldType,
                                        fieldName: item.Name,
                                        fieldValue: undefined
                                    };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldTypeRuntimeDirectiveAPI, directivePayload, setLoader);
                                });
                            }
                        }
                    }
                    else {
                        $scope.scopeModel.fieldTitle = undefined;
                        $scope.scopeModel.fieldTypeRuntimeDirective = undefined;
                    }
                };

                defineAPI();
            };
            function defineAPI() {
                var api = {};
                api.getData = function () {
                    var filterValues;
                    if (fieldTypeRuntimeDirectiveAPI != undefined) {
                        filterValues = fieldTypeRuntimeDirectiveAPI.getValuesAsArray();
                    }
                    var finalArray = [];
                    if (filterValues != undefined) {
                        for (var i = 0; i < filterValues.length; i++) {
                            var filterValue = filterValues[i];
                            if (filterValue != undefined) {
                                finalArray.push(filterValue);
                            }
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        FieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                        FieldTitle: $scope.scopeModel.fieldTitle,
                        IsRequired: $scope.scopeModel.isRequired,
                        TextResourceKey: localizationTextResourceSelectorAPI.getSelectedValues(),
                        DefaultFieldValues: (finalArray != undefined && finalArray.length != 0) ? finalArray : undefined
                    };
                };

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        settings = payload.settings;
                    }

                    function loadDataRecordTitleFieldsSelector() {
                        if (payload.settings != undefined && payload.settings.FieldName != undefined) {
                            dataRecordTypePromiseDeferredSelectionChanged = UtilsService.createPromiseDeferred();
                        }

                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var fieldsPayload = {
                                dataRecordTypeId: context.getDataRecordTypeId(),
                            };
                            if (payload.settings != undefined && payload.settings.FieldName != undefined) {
                                fieldsPayload.selectedIds = payload.settings.FieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
                    }

                    function loadStaticData() {
                        if (payload != undefined && payload.settings != undefined) {
                            $scope.scopeModel.fieldTitle = payload.settings.FieldTitle;
                            $scope.scopeModel.isRequired = payload.settings.IsRequired;
                        }
                    }

                    function loadLocalizationTextResourceSelector() {
                        var loadSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                selectedValue: (payload != undefined && payload.settings != undefined) ? payload.settings.TextResourceKey : undefined

                            };
                            VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, directivePayload, loadSelectorPromiseDeferred);
                        });
                        return loadSelectorPromiseDeferred.promise;
                    }

                    function getDataRecordFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            for (var i = 0; i < response.length; i++) {
                                var dataRecordFieldType = response[i];
                                dataRecordFieldTypes.push(dataRecordFieldType);
                            }
                        });
                    }

                    function loadFieldTypeRuntimeEditor() {
                        var loadRuntimeEditorPromiseDeferred = UtilsService.createPromiseDeferred();
                        var readyPromises = [];
                        readyPromises.push(fieldTypeRuntimeReadyPromiseDeferred.promise);
                        if (payload.settings != undefined && payload.settings.FieldName != undefined) {
                            readyPromises.push(dataRecordTypePromiseDeferredSelectionChanged.promise);
                        }
                        UtilsService.waitMultiplePromises(readyPromises).then(function () {
                            var directivePayload = {
                                fieldTitle: "Default Value",
                                fieldType: fieldType,
                                fieldName: settings.FieldName,
                                fieldValue: settings != undefined ? settings.DefaultFieldValues : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(fieldTypeRuntimeDirectiveAPI, directivePayload, loadRuntimeEditorPromiseDeferred);
                        });
                        return loadRuntimeEditorPromiseDeferred.promise;
                    }

                    loadStaticData();
                    initialPromises.push(getDataRecordFieldTypeConfigs());
                    initialPromises.push(loadDataRecordTitleFieldsSelector());
                    initialPromises.push(loadLocalizationTextResourceSelector());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            if (settings != undefined) {
                                fieldType = context.getFieldType(settings.FieldName);
                                var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");
                                if (dataRecordFieldType != undefined && dataRecordFieldType.FilterEditor != undefined) {
                                    $scope.scopeModel.fieldTypeRuntimeDirective = dataRecordFieldType.FilterEditor;
                                    previousFieldTypeRuntimeDirective = $scope.scopeModel.fieldTypeRuntimeDirective;
                                }
                            }
                            var directivePromises = [];

                            if ($scope.scopeModel.fieldTypeRuntimeDirective != undefined) {
                                directivePromises.push(loadFieldTypeRuntimeEditor());
                            }
                            return {
                                promises: directivePromises,
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                        if (payload != undefined && payload.settings != undefined) {
                            $scope.scopeModel.fieldTitle = payload.settings.FieldTitle;
                        }
                        dataRecordTypePromiseDeferredSelectionChanged = undefined;
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                return context;
            }
        }
    }
    app.directive('vrGenericdataGenericbeFilterdefinitionGenericfilter', FilterDefinitionGenericFilterDirective);
})(app);