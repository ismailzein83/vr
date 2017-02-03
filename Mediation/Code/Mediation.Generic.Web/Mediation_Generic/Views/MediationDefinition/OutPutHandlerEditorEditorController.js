(function (appControllers) {

    "use strict";

    HandlerEditorController.$inject = ['$scope',  'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function HandlerEditorController($scope,  UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var outPutHandlerEntity;

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

            $scope.scopeModel.onHandlerTypeSelectorReady = function (api) {
                handlerTypeSelectorAPI = api;
                handlerTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onHandlerTypeEditorReady = function (api) {
                handlerTypeEditorAPI = api;
                var setLoader = function (value) { $scope.scopeModel.isLoadingEditor = value; };
                var payloadDirective;
                //if (outPutHandlerEntity!=undefined){
                //    payloadDirective = { data: outPutHandlerEntity };
                //}
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadHandlerTypeSelector, loadHandlerEditor])
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
        function loadStaticData() {
            if (outPutHandlerEntity != undefined) {
                $scope.scopeModel.outputRecordName = outPutHandlerEntity.OutputRecordName;
            }
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
                OutputRecordName: $scope.scopeModel.outputRecordName,
                Handler: settings
            }
            return item;
        }
    }

    appControllers.controller('Mediation_Generic_HandlerEditorController', HandlerEditorController);
})(appControllers);
