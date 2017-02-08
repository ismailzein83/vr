(function (appControllers) {

    "use strict";

    HandlerEditorController.$inject = ['$scope',  'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function HandlerEditorController($scope,  UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var outPutHandlerEntity;
        var parsedDataRecordTypeId;
        var dataTransformationDefinitionId;

        var dataTransformationSelectorAPI;
        var dataTransformationSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var handlerTypeSelectorAPI;
        var handlerTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var handlerTypeEditorAPI;
        var handlerTypeEditorReadyDeferred;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                outPutHandlerEntity = parameters.outPutHandler;
                dataTransformationDefinitionId = parameters.dataTransformationDefinitionId;

            }
            isEditMode = (outPutHandlerEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.saveHandler = function () {
                $scope.scopeModel.isLoading = true;
                if (isEditMode) {
                    return updateHandler();
                }
                else {
                    return insertHandler();
                }
            };
            $scope.scopeModel.onDataTransformationDefinitionParsedRecordReady = function (api) {
                dataTransformationSelectorAPI = api;
                dataTransformationSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onHandlerTypeSelectorReady = function (api) {
                handlerTypeSelectorAPI = api;
                handlerTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onHandlerTypeEditorReady = function (api) {
                handlerTypeEditorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingEditor = value; };
                var payloadDirective;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, handlerTypeEditorAPI, payloadDirective, setLoader, handlerTypeEditorReadyDeferred);
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls().finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadRecordNameSelector, loadHandlerTypeSelector, loadHandlerEditor])
                 .catch(function (error) {
                     VRNotificationService.notifyExceptionWithClose(error, $scope);
                 })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function setTitle() {
            if (isEditMode && outPutHandlerEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(outPutHandlerEntity.OutputRecordName, 'Handler');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Handler');
        }
       
        function loadRecordNameSelector() {

            var loadDataTransformationSelectorDeferred = UtilsService.createPromiseDeferred();
            dataTransformationSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    dataTransformationDefinitionId: dataTransformationDefinitionId,
                    filter: {  IsArray: true },
                    selectedIds:outPutHandlerEntity && outPutHandlerEntity.OutputRecordName || undefined
                };
                VRUIUtilsService.callDirectiveLoad(dataTransformationSelectorAPI, payload, loadDataTransformationSelectorDeferred);
            });
            return loadDataTransformationSelectorDeferred.promise;

          
        }

        function loadHandlerTypeSelector() {
            
            var loadHandlerTypePromiseDeferred = UtilsService.createPromiseDeferred();
            handlerTypeSelectorReadyDeferred.promise.then(function () {
                var payloadDirective = {
                    selectedIds: outPutHandlerEntity && outPutHandlerEntity.Handler.ConfigId || undefined
                };
                VRUIUtilsService.callDirectiveLoad(handlerTypeSelectorAPI, payloadDirective, loadHandlerTypePromiseDeferred);
            });
            return loadHandlerTypePromiseDeferred.promise;
        }

        function loadHandlerEditor() {
            if (isEditMode) {
                handlerTypeEditorReadyDeferred = UtilsService.createPromiseDeferred();
                var loadHandlerEditorPromiseDeferred = UtilsService.createPromiseDeferred();
                handlerTypeEditorReadyDeferred.promise.then(function () {
                    handlerTypeEditorReadyDeferred = undefined;
                    var payloadDirective = {
                        data: outPutHandlerEntity && outPutHandlerEntity.Handler || undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(handlerTypeEditorAPI, payloadDirective, loadHandlerEditorPromiseDeferred);
                });
                return loadHandlerEditorPromiseDeferred.promise;
           }
        }
        function insertHandler() {
            var object = buildHandlerObjFromScope();          
            if ($scope.onOutPutHandlerAdded != undefined)
                $scope.onOutPutHandlerAdded(object);
            $scope.modalContext.closeModal();

        }

        function updateHandler() {
            var object = buildHandlerObjFromScope();
            if ($scope.onOutPutHandlerUpdated != undefined)
                $scope.onOutPutHandlerUpdated(object);
                $scope.modalContext.closeModal();
       }

        function buildHandlerObjFromScope() {
            var settings = handlerTypeEditorAPI.getData();
            settings.ConfigId = handlerTypeSelectorAPI.getSelectedIds()
            var item = {
                OutputRecordName: dataTransformationSelectorAPI.getSelectedIds(),
                Handler: settings
            }
            return item;
        }
    }

    appControllers.controller('Mediation_Generic_HandlerEditorController', HandlerEditorController);
})(appControllers);
