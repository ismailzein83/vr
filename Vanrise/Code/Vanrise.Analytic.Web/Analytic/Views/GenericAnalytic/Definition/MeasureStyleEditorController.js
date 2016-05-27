(function (appControllers) {

    "use strict";

    MeasureStyleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService','VR_Analytic_AnalyticConfigurationAPIService'];

    function MeasureStyleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService, VR_Analytic_AnalyticConfigurationAPIService) {

        var isEditMode;

        var itemConfigDirectiveAPI;
        var itemConfigDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
   
        var analyticItemConfigId;
        var analyticTableId;
        var itemconfigType;
        $scope.scopeModel = {};
        $scope.scopeModel.measureNames = [];
        $scope.scopeModel.measureStyles = [];
        var measureStyleEntity;
       
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                $scope.scopeModel.measureFields = parameters.measureFields;
                measureStyleEntity = parameters.measureStyle;
            }
            isEditMode = (measureStyleEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.measureStyleRuleTemplates = [];
            $scope.scopeModel.addMeasureStyleRule = function()
            {

            }

            $scope.scopeModel.saveMeasureStyle = function () {
                if (isEditMode) {
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

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadSelectedMeasureStyles, loadStaticData, loadMeasureStyleRuleTemplateConfigs]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

                function setTitle() {
                    if (isEditMode && measureStyleEntity != undefined)
                            $scope.title = UtilsService.buildTitleForUpdateEditor("Measure Style","");
                        else
                            $scope.title = UtilsService.buildTitleForAddEditor("Measure Style");
                }

                function loadSelectedMeasureStyles() {
                   
                }

                function loadStaticData() {
                    if (measureStyleEntity != undefined) {
                        $scope.scopeModel.selectedMeasureNames = UtilsService.getItemByVal($scope.scopeModel.measureFields, measureStyleEntity.MeasureName,"MeasureName");
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
            }

        }


        function buildItemConfigObjectFromScope() {
            var analyticItemConfigdetail = itemConfigDirectiveAPI != undefined ? itemConfigDirectiveAPI.getData() : undefined;
            var analyticItemConfig = {
                AnalyticItemConfigId: analyticItemConfigId,
                TableId: analyticItemConfigEntity != undefined ? analyticItemConfigEntity.TableId : analyticTableId,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                ItemType: itemconfigType,
                Config: analyticItemConfigdetail
            };
            var analyticItemConfigForAdd = {
                ItemType: itemconfigType,
                AnalyticItemConfig: UtilsService.serializetoJson(analyticItemConfig)
            };
            return analyticItemConfigForAdd;
        }


        function insert() {
            $scope.scopeModel.isLoading = true;
            var analyticItemConfigObject = buildItemConfigObjectFromScope();
            return VR_Analytic_AnalyticItemConfigAPIService.AddAnalyticItemConfig(analyticItemConfigObject)
           .then(function (response) {
               if (VRNotificationService.notifyOnItemAdded($scope.scopeModel.selectedItemConfigType.description, response, 'Name')) {
                   if ($scope.onAnalyticItemConfigAdded != undefined)
                       $scope.onAnalyticItemConfigAdded(response.InsertedObject);
                   $scope.modalContext.closeModal();
               }
           }).catch(function (error) {
               VRNotificationService.notifyException(error, $scope);
           }).finally(function () {
               $scope.scopeModel.isLoading = false;
           });

        }

        function update() {
            var analyticItemConfigObject = buildItemConfigObjectFromScope();

            $scope.scopeModel.isLoading = true;

            return VR_Analytic_AnalyticItemConfigAPIService.UpdateAnalyticItemConfig(analyticItemConfigObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated($scope.scopeModel.selectedItemConfigType.description, response, 'Name')) {
                    if ($scope.onAnalyticItemConfigUpdated != undefined)
                        $scope.onAnalyticItemConfigUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

    }

    appControllers.controller('VR_Analytic_MeasureStyleEditorController', MeasureStyleEditorController);
})(appControllers);
