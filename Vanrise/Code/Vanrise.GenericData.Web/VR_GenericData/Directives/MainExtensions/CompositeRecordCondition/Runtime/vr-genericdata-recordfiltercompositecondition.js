(function (app) {

    'use strict';

    RecordFilterCompositeConditionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService'];

    function RecordFilterCompositeConditionDirective(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecordFilterCompositeConditionDirective($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/CompositeRecordCondition/Runtime/Templates/RecordFilterCompositeConditionTemplate.html'
        };

        function RecordFilterCompositeConditionDirective($scope, ctrl) {
            this.initializeController = initializeController;

            var fields = [];
            var compositeRecordConditionResolvedDataList;
            var compositeRecordConditionDefinitionGroup;
            var dataRecordFieldTypesConfig = [];

            var recordNameSelectorAPI;
            var recordNameSelectionChangedDeferred;

            var groupFilterAPI;
            var groupFilterReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.recordNames = [];

                $scope.scopeModel.onRecordNamesSelectorReady = function (api) {
                    recordNameSelectorAPI = api;
                    recordNameSelectorAPI.selectIfSingleItem();
                };

                $scope.scopeModel.onGroupFilterReady = function (api) {
                    groupFilterAPI = api;
                    groupFilterReadyDeferred.resolve();
                };

                $scope.scopeModel.onRecordNamesSelectionChanged = function (selectedRecordName) {
                    if (selectedRecordName == undefined)
                        return;

                    buildFieldsFromCompositeRecordConditionResolvedDataList(selectedRecordName.Name);

                    if (recordNameSelectionChangedDeferred != undefined) {
                        recordNameSelectionChangedDeferred.resolve();
                    }
                    else {
                        groupFilterReadyDeferred.promise.then(function () {
                            var groupFilterPayload = {
                                context: buildContext()
                            };
                            groupFilterAPI.load(groupFilterPayload);
                        });
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    //$scope.scopeModel.loadingDirective = true;

                    var promises = [];

                    var compositeRecordCondition;

                    if (payload != undefined) {
                        compositeRecordCondition = payload.compositeRecordCondition;
                        compositeRecordConditionDefinitionGroup = payload.compositeRecordConditionDefinitionGroup;
                        compositeRecordConditionResolvedDataList = payload.compositeRecordConditionResolvedDataList;
                    }

                    loadRecordNameSelector();

                    var loadDataRecordFieldTypeConfigPromise = loadDataRecordFieldTypeConfig();
                    promises.push(loadDataRecordFieldTypeConfigPromise);

                    if (payload.compositeRecordCondition != undefined) {
                        var loadGroupFilterDirectiveDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadGroupFilterDirectiveDeferred.promise);

                        recordNameSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([loadDataRecordFieldTypeConfigPromise, recordNameSelectionChangedDeferred.promise]).then(function () {
                            recordNameSelectionChangedDeferred = undefined;

                            loadGroupFilterDirective().then(function () {
                                loadGroupFilterDirectiveDeferred.resolve();
                            });
                        });
                    }

                    function loadRecordNameSelector() {
                        $scope.scopeModel.recordNames = getRecordNames(compositeRecordConditionDefinitionGroup);

                        if (compositeRecordCondition != undefined) {
                            $scope.scopeModel.selectedRecordName = UtilsService.getItemByVal($scope.scopeModel.recordNames, compositeRecordCondition.RecordName, 'Name');
                        }
                    }
                    function loadGroupFilterDirective() {
                        var groupFilterLoadDeferred = UtilsService.createPromiseDeferred();

                        groupFilterReadyDeferred.promise.then(function () {

                            var groupFilterPayload = { context: buildContext() };
                            if (compositeRecordCondition != undefined) {
                                groupFilterPayload.filterObj = compositeRecordCondition.FilterGroup;
                            }
                            VRUIUtilsService.callDirectiveLoad(groupFilterAPI, groupFilterPayload, groupFilterLoadDeferred);
                        });

                        return groupFilterLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        //$scope.scopeModel.loadingDirective = false;
                    });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.CompositeRecordCondition.Runtime.RecordFilterCompositeCondition, Vanrise.GenericData.MainExtensions",
                        ConfigId: "F16BBC6F-F471-4601-B5C9-7C95B88B3ECB",
                        RecordName: $scope.scopeModel.selectedRecordName.Name,
                        FilterGroup: groupFilterAPI != undefined ? groupFilterAPI.getData() : undefined
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadDataRecordFieldTypeConfig() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            dataRecordFieldTypesConfig.push(response[i]);
                        }
                    }
                });
            }

            function getRecordNames(compositeRecordConditionDefinitionGroup) {
                var recordNames = [];
                if (compositeRecordConditionDefinitionGroup && compositeRecordConditionDefinitionGroup.CompositeRecordFilterDefinitions) {
                    var compositeRecordFilterDefinitions = compositeRecordConditionDefinitionGroup.CompositeRecordFilterDefinitions;
                    for (var i = 0; i < compositeRecordFilterDefinitions.length; i++) {
                        var currentCompositeRecordFilterDefinition = compositeRecordFilterDefinitions[i];
                        recordNames.push({
                            Name: currentCompositeRecordFilterDefinition.Name,
                            Title: currentCompositeRecordFilterDefinition.Title
                        });
                    }
                }
                return recordNames;
            }

            function buildFieldsFromCompositeRecordConditionResolvedDataList(recordName) {
                if (compositeRecordConditionResolvedDataList != undefined) {
                    fields = [];
                    for (var i = 0; i < compositeRecordConditionResolvedDataList.length; i++) {
                        var currentCompositeRecordConditionResolvedData = compositeRecordConditionResolvedDataList[i];
                        if (recordName == currentCompositeRecordConditionResolvedData.RecordName) {

                            for (var j = 0; j < currentCompositeRecordConditionResolvedData.Fields.length; j++) {
                                var field = currentCompositeRecordConditionResolvedData.Fields[j];
                                fields.push({
                                    FieldName: field.Name,
                                    FieldTitle: field.Title,
                                    Type: field.Type
                                });
                            }
                        }
                    }
                }
            }

            function buildContext() {
                var context = {
                    getFields: function () {
                        return fields;
                    },
                    getRuleEditor: function (configId) {
                        var dataRecordFieldTypeConfig = UtilsService.getItemByVal(dataRecordFieldTypesConfig, configId, 'ExtensionConfigurationId');
                        if (dataRecordFieldTypeConfig != undefined) {
                            return dataRecordFieldTypeConfig.RuleFilterEditor;
                        }
                    }
                };

                return context;
            }
        }
    }

    app.directive('vrGenericdataRecordfiltercompositecondition', RecordFilterCompositeConditionDirective);
})(app);