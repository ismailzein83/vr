(function (appControllers) {

    'use strict';

    FiguresTileQueryEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VRCommon_VRTileAPIService'];

    function FiguresTileQueryEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VRCommon_VRTileAPIService) {

        var isEditMode;
        var settings;

        var directiveAPI;
        var directiveReadyDeferred;

        var selectorAPI;

        var figureTileQueries;

        var figureTileEntity;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                settings = parameters.settings;
                figureTileEntity = parameters.figuresTileQueryEntity;
                figureTileQueries = parameters.figureTileQueries;
            }
            isEditMode = (figureTileEntity != undefined);
        }
        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.templateConfigs = [];
            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var directivepPayload = {
                    //context: getContext()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivepPayload, setLoader, directiveReadyDeferred);
            };
            $scope.scopeModel.onExtensionConfigurationSelectionChaged = function (value) {
                if (value != undefined) {
                    $scope.scopeModel.isDirectiveLoading = true;
                    loadDirective().then(function () {
                        $scope.scopeModel.isDirectiveLoading = false;
                    });
                }
                   
            };
            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.scopeModel.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, getFiguresTilesDefinitionSettingsConfigs]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((figureTileEntity != undefined) ? figureTileEntity.ObjectName : null, 'Figure Tile Query') :
                    UtilsService.buildTitleForAddEditor('Figure Tile Query');
            }
            function loadStaticData() {
                if (figureTileEntity == undefined)
                    return;
                else {
                    $scope.scopeModel.name = figureTileEntity.Name;
                }

                $scope.scopeModel.objectName = figureTileEntity.ObjectName;
            }

            function getFiguresTilesDefinitionSettingsConfigs() {
                return VRCommon_VRTileAPIService.GetFiguresTilesDefinitionSettingsConfigs().then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }
                        if (figureTileEntity != undefined) {
                            $scope.scopeModel.selectedTemplateConfig =
                                UtilsService.getItemByVal($scope.scopeModel.templateConfigs, figureTileEntity.Settings.ConfigId, 'ExtensionConfigurationId');
                        }
                    }
                });
            }
            
            return UtilsService.waitMultiplePromises(promises);
        };



        function loadDirective() {
            directiveReadyDeferred = UtilsService.createPromiseDeferred();
            var directiveLoadDeferred = UtilsService.createPromiseDeferred();
            directiveReadyDeferred.promise.then(function () {
                directiveReadyDeferred = undefined;
                var directivePayload = {
                    configId: $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId
                };
                if (figureTileEntity != undefined && figureTileEntity.Settings != undefined) {
                    var settings = figureTileEntity.Settings;
                    directivePayload.analyticTableId = settings.AnalyticTableId;
                    directivePayload.measures = settings.Measures;
                    directivePayload.timePeriod = settings.TimePeriod;
                    directivePayload.filterObj = settings.RecordFilter;
                    directivePayload.dimensionId = settings.DimensionId;
                    directivePayload.advancedOrderOptions = settings.AdvancedOrderOptions;

                };
                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
            });
            return directiveLoadDeferred.promise;
        }
        function insert() {
            var query = buildObjectFromScope();
            if ($scope.onFigureTileQueryAdded != undefined && typeof ($scope.onFigureTileQueryAdded) == 'function') {
                $scope.onFigureTileQueryAdded(query);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            
            var query = buildObjectFromScope();
            if (query.Settings.AnalyticTableId != figureTileEntity.Settings.AnalyticTableId)
                $scope.clearItemsToDisplayDataSource();
            if ($scope.onFigureTileQueryUpdated != undefined && typeof ($scope.onFigureTileQueryUpdated) == 'function') {
                $scope.onFigureTileQueryUpdated(query);
            }
            $scope.modalContext.closeModal();
        }

        function buildObjectFromScope() {
            return {
                FiguresTileQueryId: isEditMode ? figureTileEntity.FiguresTileQueryId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Settings: directiveAPI.getData()
            };
        }
    }

    appControllers.controller('VRCommon_FiguresTileQueryEditorController', FiguresTileQueryEditorController);

})(appControllers);
