(function (appControllers) {

    "use strict";

    MeasureStyleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_Analytic_StyleCodeEnum'];

    function MeasureStyleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService, VR_GenericData_DataRecordFieldAPIService, VR_Analytic_StyleCodeEnum) {

        var measureStyleEntity;
        var fieldTypeConfigs = [];
        var selectedMeasure;
        $scope.scopeModel = {};
        $scope.scopeModel.isEditMode = false;
        $scope.scopeModel.measureNames = [];
        $scope.scopeModel.measureStyles = [];
        var countId = 0;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                selectedMeasure = parameters.selectedMeasure;
                measureStyleEntity = parameters.measureStyle;
            }
            $scope.scopeModel.isEditMode = (measureStyleEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel.removeMeasureStyle = function (measureStyle) {
                $scope.scopeModel.measureStyles.splice($scope.scopeModel.measureStyles.indexOf(measureStyle), 1);
            };

          
            $scope.scopeModel.onMeasureSelectionChanged = function () {
                $scope.scopeModel.measureStyles.length = 0;
            };

            $scope.scopeModel.styleColors = UtilsService.getArrayEnum(VR_Analytic_StyleCodeEnum);

            $scope.scopeModel.isValidMeasureStyles = function () {
                if ($scope.scopeModel.measureStyles.length == 0)
                    return "At least one style should be added.";
                return null;
            };

            $scope.scopeModel.addMeasureStyleRule = function () {
                addMeasureStyleRule();
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
                $scope.modalContext.closeModal()
            };

        }

        function addMeasureStyleRule() {
            var fieldType = UtilsService.getItemByVal(fieldTypeConfigs, selectedMeasure.FieldType.ConfigId, "ExtensionConfigurationId");

            var dataItem = {
                id: countId++,
                configId: fieldType != undefined ? fieldType.ExtensionConfigurationId : undefined,
                editor: fieldType != undefined ? fieldType.RuleFilterEditor : undefined,
                name: fieldType != undefined ? fieldType.Name : undefined,
                selectedStyleColor: VR_Analytic_StyleCodeEnum.Red,
                onDirectiveReady: function (api) {
                    dataItem.directiveAPI = api;
                    var payload = getPayload();
                    var setLoader = function (value) {
                        dataItem.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, payload, setLoader);
                }
            };
            $scope.scopeModel.measureStyles.push(dataItem);
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
                            var matchItem = UtilsService.getItemByVal(fieldTypeConfigs, styleConditionItem.payload.RecordFilter.FieldName, "Name");
                            if (matchItem == null)
                                return;

                            var dataItem = {
                                id: countId++,
                                configId: matchItem.ExtensionConfigurationId,
                                editor: matchItem.RuleFilterEditor,
                                name: matchItem.Name,
                                selectedStyleColor: UtilsService.getItemByVal($scope.scopeModel.styleColors, styleConditionItem.payload.StyleCode, 'value')
                            };
                            var dataItemPayload = getPayload();
                            dataItemPayload.filterObj = styleConditionItem.payload.RecordFilter;

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
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, getFieldTypeConfigs]).then(function () {
                    loadSelectedMeasureStyles().then(function () {
                        if (!$scope.scopeModel.isEditMode)
                            addMeasureStyleRule();
                    } ).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }

        function getPayload()
        {
            var payload = {
                dataRecordTypeField: { Type: selectedMeasure.FieldType },
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
            for (var i = 0; i < $scope.scopeModel.measureStyles.length; i++)
            {
                var measurestyle = $scope.scopeModel.measureStyles[i];
                var data;
                if (measurestyle.directiveAPI != undefined)
                    data = measurestyle.directiveAPI.getData();
                if (data !=undefined)
                    data.FieldName = measurestyle.name;
                rules.push({
                    $type: "Vanrise.Analytic.Entities.StyleRule,Vanrise.Analytic.Entities",
                    RecordFilter: data,
                    StyleCode: measurestyle.selectedStyleColor.value
                });
            }
            var measureStyleRule = {
                MeasureName: selectedMeasure.Name,
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
