(function (appControllers) {

    "use strict";

    AnalyticItemConfigEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','VR_Analytic_AnalyticTypeEnum','VR_Analytic_AnalyticItemConfigAPIService'];

    function AnalyticItemConfigEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {

        var isEditMode;
        var itemConfigDirectiveAPI;
        var itemConfigDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var analyticItemConfigEntity;
        var analyticItemConfigId;
        var analyticTableId;
        var itemconfigType;
        var localizationTextResourceSelectorAPI;
        var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                analyticItemConfigId = parameters.analyticItemConfigId;
                itemconfigType = parameters.itemconfigType;
                analyticTableId = parameters.analyticTableId;
            }
            isEditMode = (analyticItemConfigId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            getItemConfigType();
            $scope.scopeModel.onItemConfigTypeDirectiveReady = function (api) {
                itemConfigDirectiveAPI = api;
                itemConfigDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.saveItemConfig = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.onLocalizationTextResourceSelectorReady = function (api) {
                localizationTextResourceSelectorAPI = api;
                localizationTextResourceSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getItemConfig().then(function () {
                    loadAllControls();
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([setTitle, loadItemConfigTypeDirective, loadStaticData, loadLocalizationTextResourceSelector]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

                function setTitle() {
                    if ($scope.scopeModel.selectedItemConfigType != undefined)
                    {
                        if (isEditMode && analyticItemConfigEntity != undefined)
                            $scope.title = UtilsService.buildTitleForUpdateEditor(analyticItemConfigEntity.Name, $scope.scopeModel.selectedItemConfigType.description);
                        else
                            $scope.title = UtilsService.buildTitleForAddEditor($scope.scopeModel.selectedItemConfigType.description);
                    }
                   
                }

                function loadItemConfigTypeDirective() {
                    var loadItemConfigTypeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    itemConfigDirectiveReadyDeferred.promise.then(function () {
                        var payLoad = {
                            tableId: analyticTableId,
                            ConfigEntity: analyticItemConfigEntity!=undefined? analyticItemConfigEntity.Config:undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(itemConfigDirectiveAPI, payLoad, loadItemConfigTypeDirectivePromiseDeferred);
                    });
                    return loadItemConfigTypeDirectivePromiseDeferred.promise;
                }
                function loadLocalizationTextResourceSelector() {
                    var localizationTextResourceSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var localizationTextResourcePayload = (analyticItemConfigEntity != undefined && analyticItemConfigEntity.Config != undefined) ? { selectedValue: analyticItemConfigEntity.Config.TitleResourceKey } : undefined;

                    localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, localizationTextResourcePayload, localizationTextResourceSelectorLoadPromiseDeferred);
                    });
                    return localizationTextResourceSelectorLoadPromiseDeferred.promise;
                }
                function loadStaticData()
                {
                    if(analyticItemConfigEntity !=undefined)
                    {
                        $scope.scopeModel.name = analyticItemConfigEntity.Name;
                        $scope.scopeModel.title = analyticItemConfigEntity.Title;
                    }
                }
            }

        }

        function getItemConfig()
        {
            return VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigsById(analyticTableId, itemconfigType, analyticItemConfigId).then(function (analyticItemConfigEntityObj) {
                analyticItemConfigEntity = analyticItemConfigEntityObj;
            });

        }

        function getItemConfigType()
        {
            for(var prop in VR_Analytic_AnalyticTypeEnum)
            {
                if(VR_Analytic_AnalyticTypeEnum[prop].value == itemconfigType)
                {
                    $scope.scopeModel.selectedItemConfigType =  VR_Analytic_AnalyticTypeEnum[prop];
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
                ItemType:itemconfigType,
                Config: analyticItemConfigdetail
            };
            if (analyticItemConfig.Config == undefined)
                analyticItemConfig.Config = {};
            analyticItemConfig.Config.TitleResourceKey = localizationTextResourceSelectorAPI != undefined ? localizationTextResourceSelectorAPI.getSelectedValues() : undefined;
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

    appControllers.controller('VR_Analytic_AnalyticItemConfigEditorController', AnalyticItemConfigEditorController);
})(appControllers);
