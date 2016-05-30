(function (appControllers) {

    "use strict";

    MeasureStyleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','VR_Analytic_AnalyticConfigurationAPIService','VR_GenericData_DataRecordFieldTypeConfigAPIService','VR_Analytic_StyleCodeEnum'];

    function MeasureStyleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_Analytic_StyleCodeEnum) {

        var measureStyleEntity;
        var fieldTypeConfigs = [];
        $scope.scopeModel = {};
        $scope.scopeModel.isEditMode = false;
        $scope.scopeModel.measureNames = [];
        $scope.scopeModel.measureStyles = [];

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                $scope.scopeModel.measureFields = parameters.measureFields;
                measureStyleEntity = parameters.measureStyle;
            }
            $scope.scopeModel.isEditMode = (measureStyleEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.measureStyleRuleTemplates = [];

            $scope.scopeModel.removeMeasureStyle = function (measureStyle)
            {
                $scope.scopeModel.measureStyles.splice($scope.scopeModel.measureStyles.indexOf(measureStyle), 1);
            }

            $scope.scopeModel.styleColors = UtilsService.getArrayEnum(VR_Analytic_StyleCodeEnum);


            $scope.scopeModel.isValidMeasureStyles = function()
            {
                if ($scope.scopeModel.measureStyles.length == 0)
                    return "At least one style should be added.";
                return null;
            }
            $scope.scopeModel.addMeasureStyleRule = function()
            {
                var dataItem = {
                    id: $scope.scopeModel.measureStyles.length + 1,
                    configId: $scope.scopeModel.selectedMeasureStyleRuleTemplate.ExtensionConfigurationId,
                    editor: $scope.scopeModel.selectedMeasureStyleRuleTemplate.Editor,
                    name: $scope.scopeModel.selectedMeasureStyleRuleTemplate.Name,
                    onDirectiveReady:function(api)
                    {
                        dataItem.directiveAPI = api;
                        var payload ={context:getContext(),Title:"Compare Value",FieldType:$scope.scopeModel.selectedMeasureName.FieldType};
                        var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, payload, setLoader);
                    }
                }
                $scope.scopeModel.measureStyles.push(dataItem);
            }

            $scope.scopeModel.saveMeasureStyle = function () {
                if ($scope.scopeModel.isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function getContext()
        {
            var context = {
                getFieldTypeEditor: function (fieldTypeConfigId) {
                    var fieldType = UtilsService.getItemByVal(fieldTypeConfigs, fieldTypeConfigId, "DataRecordFieldTypeConfigId");
                    if (fieldType != undefined) {
                        return fieldType.RuntimeEditor;
                    }

                }
            }
            
            return context;
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {

                function setTitle() {
                    if ($scope.scopeModel.isEditMode && measureStyleEntity != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor("Measure Style", measureStyleEntity.MeasureName);
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor("Measure Style");
                }
                function loadSelectedMeasureStyles() {
                    var promises = [];
                    if (measureStyleEntity != undefined && measureStyleEntity.Rules != undefined) {
                        for (var i = 0; i < measureStyleEntity.Rules.length; i++) {
                            var rule = measureStyleEntity.Rules[i];
                            var filterItem = {
                                payload: rule,
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(filterItem.loadPromiseDeferred.promise);
                            addStyleConditionItemToGrid(filterItem)
                        }
                        function addStyleConditionItemToGrid(styleConditionItem) {
                            var matchItem = UtilsService.getItemByVal($scope.scopeModel.measureStyleRuleTemplates, styleConditionItem.payload.Condition.ConfigId, "ExtensionConfigurationId");
                            if (matchItem == null)
                                return;

                            var dataItem = {
                                id: $scope.scopeModel.measureStyles.length + 1,
                                configId: matchItem.ExtensionConfigurationId,
                                editor: matchItem.Editor,
                                name: matchItem.Name,
                                selectedStyleColor: UtilsService.getItemByVal($scope.scopeModel.styleColors, styleConditionItem.payload.StyleCode, 'value')
                            };
                            var dataItemPayload = { context: getContext(), Title: "Compare Value", FieldType: $scope.scopeModel.selectedMeasureName.FieldType,RuleStyle: styleConditionItem.payload.Condition};

                            dataItem.onDirectiveReady = function (api) {
                                dataItem.directiveAPI = api;
                                styleConditionItem.readyPromiseDeferred.resolve();
                            };

                            styleConditionItem.readyPromiseDeferred.promise
                                .then(function () {
                                    VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, styleConditionItem.loadPromiseDeferred);
                                });

                            $scope.scopeModel.measureStyles.push(dataItem);
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                }
                function loadStaticData() {
                    if (measureStyleEntity != undefined) {
                        $scope.scopeModel.selectedMeasureName = UtilsService.getItemByVal($scope.scopeModel.measureFields, measureStyleEntity.MeasureName, "Name");
                    }
                }
                function loadMeasureStyleRuleTemplateConfigs() {
                    return VR_Analytic_AnalyticConfigurationAPIService.GetMeasureStyleRuleTemplateConfigs().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.measureStyleRuleTemplates.push(response[i]);
                            }
                            if (measureStyleEntity != undefined)
                                $scope.scopeModel.selectedMeasureStyleRuleTemplate = UtilsService.getItemByVal($scope.scopeModel.measureStyleRuleTemplates, measureStyleEntity.ConfigId, 'ExtensionConfigurationId');
                            else
                                $scope.scopeModel.selectedMeasureStyleRuleTemplate = $scope.scopeModel.measureStyleRuleTemplates[0];
                        }
                    });
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadMeasureStyleRuleTemplateConfigs, getFieldTypeConfigs]).then(function () {
                    loadSelectedMeasureStyles().finally(function () {
                        $scope.scopeModel.isLoading = false;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }

        function getFieldTypeConfigs() {
            return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                fieldTypeConfigs = [];
                for (var i = 0; i < response.length; i++) {
                    fieldTypeConfigs.push(response[i]);
                }
            });
        }

        function buildMeasuerStyleObjectFromScope() {
            var rules = [];
            for (var i = 0; i < $scope.scopeModel.measureStyles.length; i++)
            {
                var measurestyle = $scope.scopeModel.measureStyles[i];
                var data;
                if (measurestyle.directiveAPI != undefined)
                    data = measurestyle.directiveAPI.getData();
                if (data !=undefined)
                    data.ConfigId = measurestyle.configId;
                rules.push({
                    $type: "Vanrise.Analytic.Entities.StyleRule,Vanrise.Analytic.Entities",
                    Condition: data,
                    StyleCode: measurestyle.selectedStyleColor.value
                });
            }
            var measureStyleRule = {
                MeasureName: $scope.scopeModel.selectedMeasureName.Name,
                Rules: rules
            };
            return measureStyleRule;
        }

        function insert() {
            var measureStyleObj = buildMeasuerStyleObjectFromScope();
            if ($scope.onMeasureStyleAdded != undefined)
                $scope.onMeasureStyleAdded(measureStyleObj);
            $scope.modalContext.closeModal();
        }

        function update() {
            var measureStyleObj = buildMeasuerStyleObjectFromScope();
            if ($scope.onMeasureStyleUpdated != undefined)
                $scope.onMeasureStyleUpdated(measureStyleObj);
            $scope.modalContext.closeModal();
          
        }

    }

    appControllers.controller('VR_Analytic_MeasureStyleEditorController', MeasureStyleEditorController);
})(appControllers);
