(function (appControllers) {

    "use strict";

    MeasureStyleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VR_GenericData_DataRecordFieldAPIService','VR_Analytic_MeasureStyleRuleAPIService'];

    function MeasureStyleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService, VR_GenericData_DataRecordFieldAPIService, VR_Analytic_MeasureStyleRuleAPIService) {

        var measureStyleEntity;
        var fieldTypeConfigs = [];

        var measure;
        $scope.scopeModel = {};
        $scope.scopeModel.isEditMode = false;
        $scope.scopeModel.measureNames = [];
        $scope.scopeModel.measureStyles = [];
        $scope.scopeModel.recommendedMeasureStyles = [];

        var countId = 0;
        var context;
        var analyticTableId;
        var measureName;
        var statusDefinitionBeId;
        var recommendedId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                measureStyleEntity = parameters.measureStyle;
                context = parameters.context;
                analyticTableId = parameters.analyticTableId;
                measureName = parameters.measureName;
                statusDefinitionBeId = parameters.statusDefinitionBeId;
                recommendedId = parameters.recommendedId;
            }
            $scope.scopeModel.isEditMode = parameters.isEditMode;
        }

        function defineScope() {

            $scope.scopeModel.removeMeasureStyle = function (measureStyle) {
                $scope.scopeModel.measureStyles.splice($scope.scopeModel.measureStyles.indexOf(measureStyle), 1);
            };
            $scope.scopeModel.removeRecommendedMeasureStyle = function (recommendedMeasureStyle) {
                $scope.scopeModel.recommendedMeasureStyles.splice($scope.scopeModel.recommendedMeasureStyles.indexOf(recommendedMeasureStyle), 1);
            };

            $scope.scopeModel.onMeasureSelectionChanged = function () {
                $scope.scopeModel.measureStyles.length = 0;
            };

            $scope.scopeModel.isValidMeasureStyles = function () {
                if ($scope.scopeModel.measureStyles.length == 0)
                    return "At least one style should be added.";
                return null;
            };
            $scope.scopeModel.isValidRecommendedMeasureStyles = function () {
                if ($scope.scopeModel.recommendedMeasureStyles.length == 0)
                    return "At least one style should be added.";
                return null;
            };

            $scope.scopeModel.addMeasureStyleRule = function () {
                addMeasureStyleRule();
            };
            $scope.scopeModel.addRecommendedMeasureStyleRule = function () {
                addRecommendedMeasureStyleRule();
            };

            $scope.scopeModel.saveMeasureStyle = function () {
                if ($scope.scopeModel.isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function addMeasureStyleRule() {

            measure = context.getMeasure(measureName);
            var fieldType = UtilsService.getItemByVal(fieldTypeConfigs, measure.FieldType.ConfigId, "ExtensionConfigurationId");

            var dataItem = {
                id: countId++,
                configId: fieldType != undefined ? fieldType.ExtensionConfigurationId : undefined,
                editor: fieldType != undefined ? fieldType.RuleFilterEditor : undefined,
                name: measure.Name,
                onDirectiveReady: function (api) {
                    dataItem.directiveAPI = api;
                    var payload = getPayload(measure);
                    var setLoader = function (value) {
                        setTimeout(function () {
                            dataItem.isLoadingDirective = value;
                            UtilsService.safeApply($scope);
                        });
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, payload, setLoader);
                },
                statusDefinitionLoadDeferred: UtilsService.createPromiseDeferred(),
                onStatusDefinitionSelectorReady: function (api) {
                    dataItem.statusDefinitionSelectorAPI = api;
                    var payload = {
                        filter: {
                            BusinessEntityDefinitionId: statusDefinitionBeId,
                            ExcludedIds: [recommendedId]
                        },
                    };
                    VRUIUtilsService.callDirectiveLoad(api, payload, dataItem.statusDefinitionLoadDeferred);
                }
            };
            $scope.scopeModel.measureStyles.push(dataItem);
        }
        function addRecommendedMeasureStyleRule() {
            measure = context.getMeasure(measureName);
            var fieldType = UtilsService.getItemByVal(fieldTypeConfigs, measure.FieldType.ConfigId, "ExtensionConfigurationId");

            var dataItem = {
                id: countId++,
                configId: fieldType != undefined ? fieldType.ExtensionConfigurationId : undefined,
                editor: fieldType != undefined ? fieldType.RuleFilterEditor : undefined,
                name: measure.Name,
                onRecommendedDirectiveReady: function (api) {
                    dataItem.recommendedDirectiveAPI = api;
                    var payload = getPayload(measure);
                    var setLoader = function (value) {
                        setTimeout(function () {
                            dataItem.isLoadingRecommendedDirective = value;
                            UtilsService.safeApply($scope);
                        });
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.recommendedDirectiveAPI, payload, setLoader);
                }
            };
            $scope.scopeModel.recommendedMeasureStyles.push(dataItem);
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if ($scope.scopeModel.isEditMode && measureStyleEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor("Measure Style", measureStyleEntity.Entity.MeasureStyleRuleDetail.MeasureName);
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor("Measure Style");
                }
                function loadSelectedMeasureStyles() {
                    var promises = [];
                    if (measureStyleEntity != undefined && measureStyleEntity.Entity.MeasureStyleRuleDetail.Rules != undefined) {
                        promises.push(loadRecommendedGrid(measureStyleEntity.Entity.MeasureStyleRule));
                        for (var i = 0; i < measureStyleEntity.Entity.MeasureStyleRuleDetail.Rules.length; i++) {
                            var rule = measureStyleEntity.Entity.MeasureStyleRuleDetail.Rules[i];
                            var filterItem = {
                                payload: rule,
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(filterItem.loadPromiseDeferred.promise);
                            addStyleConditionItemToGrid(filterItem);
                        }

                    }
                    return UtilsService.waitMultiplePromises(promises);
                }
                function loadRecommendedGrid(measureStyleRule) {
                    var promises = [];
                    measure = context.getMeasure(measureName);
                    var fieldType = UtilsService.getItemByVal(fieldTypeConfigs, measure.FieldType.ConfigId, "ExtensionConfigurationId");
                    var editorObject= fieldType != undefined ? fieldType.RuleFilterEditor : undefined;

                    for (var i = 0; i < measureStyleRule.RecommendedStyleRule.RecordFilters.length; i++) {
                        var recordFilter = measureStyleRule.RecommendedStyleRule.RecordFilters[i];
                        var dataItem = {
                            editor: editorObject
                        };
                        promises.push(extendRecommendedGridDataItem(dataItem,recordFilter));
                        $scope.scopeModel.recommendedMeasureStyles.push(dataItem);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                }
                function extendRecommendedGridDataItem(dataItem, recordFilter) {
                    var dataItemPayload = {};
                    dataItemPayload.filterObj = recordFilter;
                    dataItem.loadDirectivePromise = UtilsService.createPromiseDeferred();
                    dataItem.onRecommendedDirectiveReady = function (api) {
                        dataItem.recommendedDirectiveAPI = api;
                        VRUIUtilsService.callDirectiveLoad(dataItem.recommendedDirectiveAPI, dataItemPayload, dataItem.loadDirectivePromise);
                    };
                    return dataItem.loadDirectivePromise.promise;
                }
                function addStyleConditionItemToGrid(styleConditionItem) {
                    measure = context.getMeasure(styleConditionItem.payload.RecordFilter.FieldName);
                    var matchItem = UtilsService.getItemByVal(fieldTypeConfigs, measure.FieldType.ConfigId, "ExtensionConfigurationId");
                    if (matchItem == null)
                        return;

                    var dataItem = {
                        id: countId++,
                        configId: matchItem.ExtensionConfigurationId,
                        editor: matchItem.RuleFilterEditor,
                        name: measure.Name,
                    };
                    var dataItemPayload = getPayload(measure);
                    dataItemPayload.filterObj = styleConditionItem.payload.RecordFilter;

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        styleConditionItem.readyPromiseDeferred.resolve();
                    };
                    styleConditionItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, styleConditionItem.loadPromiseDeferred);
                        });
                    dataItem.statusDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

                    dataItem.onStatusDefinitionSelectorReady = function (api) {
                        dataItem.statusDefinitionSelectorAPI = api;
                        var payload = {
                            filter: {
                                BusinessEntityDefinitionId: statusDefinitionBeId,
                                ExcludedIds: [recommendedId]
                            },
                            selectedIds: styleConditionItem.payload.StatusDefinitionId
                        };
                        VRUIUtilsService.callDirectiveLoad(api, payload, dataItem.statusDefinitionLoadDeferred);
                    };
                    dataItem.statusDefinitionLoadDeferred.promise.then(function () {
                        dataItem.isLoading = false;

                    });
                    $scope.scopeModel.measureStyles.push(dataItem);
                }

                function loadStaticData() {
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, getFieldTypeConfigs]).then(function () {
                    loadSelectedMeasureStyles().then(function () {
                        if (!$scope.scopeModel.isEditMode) {
                            addMeasureStyleRule();
                            addRecommendedMeasureStyleRule();
                        }
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }

        function getPayload(measure) {
            var payload = {
                dataRecordTypeField: { Type: measure.FieldType },
            };
            return payload;
        }

        function getFieldTypeConfigs() {
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                fieldTypeConfigs = [];
                for (var i = 0; i < response.length; i++) {
                    fieldTypeConfigs.push(response[i]);
                }
            });
        }

        function buildMeasuerStyleObjectFromScope() {
            var rules = [];
            for (var i = 0; i < $scope.scopeModel.measureStyles.length; i++) {
                var measurestyle = $scope.scopeModel.measureStyles[i];
                var data;
                if (measurestyle.directiveAPI != undefined)
                    data = measurestyle.directiveAPI.getData();
                if (data != undefined)
                    data.FieldName = measurestyle.name;

               

                rules.push({
                    $type: "Vanrise.Analytic.Entities.StyleRule,Vanrise.Analytic.Entities",
                    RecordFilter: data,
                    StatusDefinitionId: measurestyle.statusDefinitionSelectorAPI.getSelectedIds()
                });

            }
            var recordFilters = [];
            for (var i = 0; i < $scope.scopeModel.recommendedMeasureStyles.length; i++) {
                var recommendedMeasurestyle = $scope.scopeModel.recommendedMeasureStyles[i];
                var recommendedData;
                if (recommendedMeasurestyle.recommendedDirectiveAPI != undefined)
                    recommendedData = recommendedMeasurestyle.recommendedDirectiveAPI.getData();
                if (recommendedData != undefined)
                    recommendedData.FieldName = measurestyle.name;
                recordFilters.push(recommendedData);
            };
            var measureStyleRule = {
                MeasureName: measure.Name,
                RecommendedStyleRule: { RecordFilters:recordFilters},
                Rules: rules
            };
            return measureStyleRule;
        }

        function insert() {
            var measureStyleObj = buildMeasuerStyleObjectFromScope();
            var measureStyleRuleInput = {
                AnalyticTableId: analyticTableId,
                MeasureStyleRule: measureStyleObj
            };
            return VR_Analytic_MeasureStyleRuleAPIService.GetMeasureStyleRuleDetail(measureStyleRuleInput).then(function (response) {
                console.log(response);
                var measureStyleDetail = response;
                if ($scope.onMeasureStyleAdded != undefined) {
                    $scope.onMeasureStyleAdded(measureStyleDetail);
                }
                $scope.modalContext.closeModal();
            });
        }

        function update() {
            var measureStyleObj = buildMeasuerStyleObjectFromScope();
            var measureStyleRuleInput = {
                AnalyticTableId: analyticTableId,
                MeasureStyleRule: measureStyleObj
            };
            return VR_Analytic_MeasureStyleRuleAPIService.GetMeasureStyleRuleDetail(measureStyleRuleInput).then(function (response) {
                var measureStyleDetail = response;
                if ($scope.onMeasureStyleUpdated != undefined)
                    $scope.onMeasureStyleUpdated(measureStyleDetail);
                $scope.modalContext.closeModal();
            });


        }

    }

    appControllers.controller('VR_Analytic_MeasureStyleEditorController', MeasureStyleEditorController);
})(appControllers);
