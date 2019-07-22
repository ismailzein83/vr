//(function (appControllers) {

//    "use strict";

//    BeforeInsertHandlerDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

//    function BeforeInsertHandlerDefinitionEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

//        var isEditMode;
//        var entity;
//        var context;

//        var beforeInsertHandlerAPI;
//        var beforeInsertHandlerReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);
//            if (parameters != undefined && parameters != null) {
//                entity = parameters.entity;
//                context = parameters.context;
//            }
//            isEditMode = (entity != undefined);
//        }

//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.onGenericBEBeforeInsertHandlerSettingsReady = function (api) {
//                beforeInsertHandlerAPI = api;
//                beforeInsertHandlerReadyPromiseDeferred.resolve();
//            };


//            $scope.scopeModel.saveBeforeInsertHandler = function () {
//                if (isEditMode) {
//                    return update();
//                }
//                else {
//                    return insert();
//                }
//            };
//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal();
//            };

//        }
//        function load() {

//            loadAllControls();

//            function loadAllControls() {
//                $scope.scopeModel.isLoading = true;
//                function setTitle() {
//                    if (isEditMode && entity != undefined)
//                        $scope.title = UtilsService.buildTitleForUpdateEditor(entity.Name, 'Before Insert Handler');
//                    else
//                        $scope.title = UtilsService.buildTitleForAddEditor('Before Insert Handler');
//                }

//                function loadStaticData() {
//                    if (!isEditMode)
//                        return;

//                    $scope.scopeModel.name = entity != undefined ? entity.Name : undefined;
//                }

//                function loadBeforeInsertHandlerSettings() {
//                    var loadBeforeInsertHandlerSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
//                    beforeInsertHandlerReadyPromiseDeferred.promise.then(function () {
//                        var settingPayload = {
//                            context: context,
//                            settings: entity != undefined ? entity.Settings : undefined
//                        };

//                        VRUIUtilsService.callDirectiveLoad(beforeInsertHandlerAPI, settingPayload, loadBeforeInsertHandlerSettingsPromiseDeferred);
//                    });
//                    return loadBeforeInsertHandlerSettingsPromiseDeferred.promise;
//                }

//                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadBeforeInsertHandlerSettings]).then(function () {
//                }).finally(function () {
//                    $scope.scopeModel.isLoading = false;
//                }).catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                });

//            }

//        }

//        function buildBeforeInsertHandlerFromScope() {
//            return {
//                Name: $scope.scopeModel.name,
//                Settings: beforeInsertHandlerAPI.getData(),
//            };
//        }

//        function insert() {
//            if ($scope.onGenericBEBeforeInsertHandlerAdded != undefined) {
//                $scope.onGenericBEBeforeInsertHandlerAdded(buildBeforeInsertHandlerFromScope());
//            }
//            $scope.modalContext.closeModal();
//        }

//        function update() {
//            if ($scope.onGenericBEBeforeInsertHandlerUpdated != undefined) {
//                $scope.onGenericBEBeforeInsertHandlerUpdated(buildBeforeInsertHandlerFromScope());
//            }
//            $scope.modalContext.closeModal();
//        }
//    }

//    appControllers.controller('VR_GenericData_BeforeInsertHandlerDefinitionEditorController', BeforeInsertHandlerDefinitionEditorController);
//})(appControllers);
