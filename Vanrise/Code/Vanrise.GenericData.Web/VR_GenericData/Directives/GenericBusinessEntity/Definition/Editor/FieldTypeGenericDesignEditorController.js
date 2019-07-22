//(function (appControllers) {

//    "use strict";

//    FieldTypeGenericDesignEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

//    function FieldTypeGenericDesignEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

//        var fieldTypeEntitySettings;
//        var context;

//        var fieldTypeRuntimeDirectiveAPI;
//        var fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);
//            if (parameters != undefined) {
//                fieldTypeEntitySettings = parameters.fieldTypeEntity;
//                context = parameters.context;
//            }
//        }

//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.onFieldTypeRumtimeDirectiveReady = function (api) {
//                fieldTypeRuntimeDirectiveAPI = api;
//                fieldTypeRuntimeReadyPromiseDeferred.resolve();
//            };

//            $scope.scopeModel.saveFieldTypeSettings = function () {
              
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
//                    $scope.title = UtilsService.buildTitleForUpdateEditor(fieldTypeEntitySettings.FieldPath, 'Generic Field Type Settings');
//                }

//                function loadStaticData() {
//                  if
//                }

//                function loadAdditionalSettingsSelective() {
//                    var additionalSettingsSelectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
//                    additionalSettingsSelectiveReadyPromiseDeferred.promise.then(function () {
//                        var additionalSettingsPayload = additionalSettings != undefined ? additionalSettings.Settings : undefined;
//                        VRUIUtilsService.callDirectiveLoad(additionalSettingsSelectiveAPI, additionalSettingsPayload, additionalSettingsSelectiveLoadPromiseDeferred);
//                    });
//                    return additionalSettingsSelectiveLoadPromiseDeferred.promise;
//                }


//                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadAdditionalSettingsSelective]).then(function () {
//                }).finally(function () {
//                    $scope.scopeModel.isLoading = false;
//                }).catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                });

//            }

//        }
     

//        function loadFieldTypeRuntimeDirective() {
//            fieldTypeRuntimeLoadPromiseDeferred == UtilsService.createPromiseDeferred();
//            fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
//                var directivePayload = {
//                    fieldTitle: "Default Value",
//                    fieldType: fieldTypeEntitySettings.fieldType,
//                    fieldName: fieldTypeEntitySettings.FieldPath,
//                    fieldValue: fieldTypeEntitySettings.DefaultFieldValue
//                };
//                VRUIUtilsService.callDirectiveLoad(fieldTypeRuntimeDirectiveAPI, directivePayload, fieldTypeRuntimeLoadPromiseDeferred);
//            });
//            return fieldTypeRuntimeLoadPromiseDeferred.promise;
//        }

//        function getContext() {
//            var currentContext = context;
//            if (currentContext == undefined)
//                currentContext = {};
//            return currentContext;
//        }

//    }

//    appControllers.controller('VR_GenericData_FieldTypeGenericDesignEditorController', FieldTypeGenericDesignEditorController);
//})(appControllers);
