(function (appControllers) {

    'use strict';

    GenericRuleDefinitionEditorController.$inject = ['$scope', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_Sec_MenuAPIService', 'VR_Sec_ViewAPIService', 'InsertOperationResultEnum', 'VR_Sec_ViewTypeEnum', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function GenericRuleDefinitionEditorController($scope, VR_GenericData_GenericRuleDefinitionAPIService, VR_Sec_MenuAPIService, VR_Sec_ViewAPIService, InsertOperationResultEnum, VR_Sec_ViewTypeEnum, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var isEditMode;
        var menuItems;

        var viewTypeName = "VR_GenericData_GenericRule";
        var viewEntity;

        var settingsTypeName;

        var genericRuleDefinitionId;
        var genericRuleDefinitionEntity;

        var genericRuleDefinitionSettingsAPI;
        var genericRuleDefinitionSettingsReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                genericRuleDefinitionId = parameters.GenericRuleDefinitionId;
                settingsTypeName = parameters.SettingsTypeName;
            }

            isEditMode = (genericRuleDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericRuleDefinfitionSettingsReady = function (api) {
                genericRuleDefinitionSettingsAPI = api;
                genericRuleDefinitionSettingsReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.hasSaveGenericRuleDefinition = function () {
                if (isEditMode) {
                    return VR_GenericData_GenericRuleDefinitionAPIService.HasUpdateGenericRuleDefinition();
                }
                else {
                    return VR_GenericData_GenericRuleDefinitionAPIService.HasAddGenericRuleDefinition();
                }
            };
            $scope.scopeModel.validateMenuLocation = function () {
                return ($scope.scopeModel.selectedMenuItem != undefined) ? null : 'No menu location selected';
            };
        }
        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getGenericRuleDefinition().then(function () {
                    loadAllControls().finally(function () {
                        genericRuleDefinitionEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getGenericRuleDefinition() {
            return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionId).then(function (response) {
                genericRuleDefinitionEntity = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadGenericRuleDefintionSettings]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function setTitle() {
                $scope.title = (isEditMode) ?
                    UtilsService.buildTitleForUpdateEditor((genericRuleDefinitionEntity != undefined) ? genericRuleDefinitionEntity.Name : null, 'Generic Rule Definition') :
                    UtilsService.buildTitleForAddEditor('Generic Rule Definition');
            }
            function loadStaticData() {
                if (genericRuleDefinitionEntity == undefined) {
                    return;
                }
                $scope.scopeModel.name = genericRuleDefinitionEntity.Name;
                $scope.scopeModel.title = genericRuleDefinitionEntity.Title;
            }
           
            function loadGenericRuleDefintionSettings() {
                
                var genericRuleDefinitionSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                genericRuleDefinitionSettingsReadyDeferred.promise.then(function () {
                    var payload;
                    if (genericRuleDefinitionEntity != undefined) {
                        payload = {
                            objects: genericRuleDefinitionEntity.Objects,
                            criteriaDefinition: genericRuleDefinitionEntity.CriteriaDefinition,
                            settingsDefinition: genericRuleDefinitionEntity.SettingsDefinition,
                            security: genericRuleDefinitionEntity.Security,
                            settingsTypeName: settingsTypeName
                        };
                       
                    }

                    VRUIUtilsService.callDirectiveLoad(genericRuleDefinitionSettingsAPI, payload, genericRuleDefinitionSettingsLoadDeferred);
                });

                return genericRuleDefinitionSettingsLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.isLoading = true;

            return VR_GenericData_GenericRuleDefinitionAPIService.AddGenericRuleDefinition(buildGenericRuleDefinitionObjectFromScope())
             .then(function (response) {
                 if (VRNotificationService.notifyOnItemAdded('Generic Rule Definition', response, 'Name')) {
                     if ($scope.onGenericRuleDefinitionAdded != undefined && typeof ($scope.onGenericRuleDefinitionAdded) == 'function') {
                         $scope.onGenericRuleDefinitionAdded(response.InsertedObject);
                     }
                     $scope.modalContext.closeModal();
                 }
             }).catch(function (error) {
                 VRNotificationService.notifyException(error, $scope);
             }).finally(function () {
                 $scope.isLoading = false;

             });
        }

        function update() {
            $scope.isLoading = true;
            return VR_GenericData_GenericRuleDefinitionAPIService.UpdateGenericRuleDefinition(buildGenericRuleDefinitionObjectFromScope())
             .then(function (response) {
                 if (VRNotificationService.notifyOnItemAdded('Generic Rule Definition', response, 'Name')) {
                     if ($scope.onGenericRuleDefinitionUpdated != undefined && typeof ($scope.onGenericRuleDefinitionUpdated) == 'function') {
                         $scope.onGenericRuleDefinitionUpdated(response.UpdatedObject);
                     }
                     $scope.modalContext.closeModal();
                 }
             }).catch(function (error) {
                 VRNotificationService.notifyException(error, $scope);
             }).finally(function () {
                 $scope.isLoading = false;

             });
        }

        function buildContext() {

            var context = {
                getObjectVariables: function () { return objectDirectiveAPI.getData(); }
            };
            return context;
        }
        function buildGenericRuleDefinitionObjectFromScope() {
            var genericRuleDefinitionSettings = genericRuleDefinitionSettingsAPI.getData();
            return {
                GenericRuleDefinitionId: genericRuleDefinitionId,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                CriteriaDefinition: genericRuleDefinitionSettings.criteriaDefinition,
                SettingsDefinition: genericRuleDefinitionSettings.settingsDefinition,
                Objects: genericRuleDefinitionSettings.objects,
                Security:genericRuleDefinitionSettings.security
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionEditorController', GenericRuleDefinitionEditorController);

})(appControllers);