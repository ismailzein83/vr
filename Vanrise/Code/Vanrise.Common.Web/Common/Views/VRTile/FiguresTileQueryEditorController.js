(function (appControllers) {

    'use strict';

    FiguresTileQueryEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function FiguresTileQueryEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;
        var settings;

        var directiveAPI;
        var directiveReadyDeferred;

        var selectorAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                settings = parameters.settings;
            }

            isEditMode = (objectVariableEntity != undefined);
        }
        function defineScope() {

            $scope.scopeModal = {};

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

            $scope.scopeModal.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            loadAllControls();
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, getFiguresTilesDefinitionSettingsConfigs, loadDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((objectVariableEntity != undefined) ? objectVariableEntity.ObjectName : null, 'Object Variable') :
                    UtilsService.buildTitleForAddEditor('Object Variable');
            }
            function loadStaticData() {
                if (objectVariableEntity == undefined) {
                    return;
                }
                $scope.scopeModal.objectName = objectVariableEntity.ObjectName;
            }

            function getFiguresTilesDefinitionSettingsConfigs() {
                return VRCommon_VRTileAPIService.GetFiguresTilesDefinitionSettingsConfigs().then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.scopeModel.templateConfigs.push(response[i]);
                        }
                        if (settings != undefined) {
                            $scope.scopeModel.selectedTemplateConfig =
                                UtilsService.getItemByVal($scope.scopeModel.templateConfigs, settings.ConfigId, 'ExtensionConfigurationId');
                        }
                    }
                });
            }
            function loadDirective() {
                directiveReadyDeferred = UtilsService.createPromiseDeferred();
                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                directiveReadyDeferred.promise.then(function () {
                    directiveReadyDeferred = undefined;
                    var directivePayload = {
                        //context: getContext()
                    };
                    if (settings != undefined) {
                        directivePayload.settings = settings;
                    };
                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }

            return UtilsService.waitMultiplePromises(promises);
        };
    



        function insert() {
            var objectVariable = buildObjectVariableFromScope();
            if ($scope.onFigureTileQueryAdded != undefined && typeof ($scope.onFigureTileQueryAdded) == 'function') {
                $scope.onFigureTileQueryAdded(objectVariable);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var objectVariable = buildObjectVariableFromScope();
            if ($scope.onFigureTileQueryUpdated != undefined && typeof ($scope.onFigureTileQueryUpdated) == 'function') {
                $scope.onFigureTileQueryUpdated(objectVariable);
            }
            $scope.modalContext.closeModal();
        }

        function buildObjectVariableFromScope() {
            return {
         
            };
        }
    }

    appControllers.controller('VRCommon_FiguresTileQueryEditorController', FiguresTileQueryEditorController);

})(appControllers);
