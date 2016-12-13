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

        var objectDirectiveAPI;
        var objectDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var criteriaDirectiveAPI;
        var criteriaDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        
        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var viewPermissionAPI;
        var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var addPermissionAPI;
        var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var editPermissionAPI;
        var editPermissionReadyDeferred = UtilsService.createPromiseDeferred();


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

            $scope.scopeModel.onObjectDirectiveReady = function (api) {
                objectDirectiveAPI = api;
                objectDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onCriteriaDirectiveReady = function (api) {
                criteriaDirectiveAPI = api;
                criteriaDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                viewPermissionAPI = api;
                viewPermissionReadyDeferred.resolve();
            };
            $scope.scopeModel.onAddRequiredPermissionReady = function (api) {
                addPermissionAPI = api;
                addPermissionReadyDeferred.resolve();
            };
            $scope.scopeModel.onEditRequiredPermissionReady = function (api) {
                editPermissionAPI = api;
                editPermissionReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCriteriaDirective, loadSettingsDirective, loadObjectDirective, loadViewRequiredPermission, loadAddRequiredPermission, loadEditRequiredPermission]).catch(function (error) {
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
            }          
            function loadObjectDirective() {
                var objectDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                objectDirectiveReadyDeferred.promise.then(function () {
                    var objectDirectivePayload;

                    if (genericRuleDefinitionEntity != undefined && genericRuleDefinitionEntity.Objects != null) {

                        var objects = [];
                        for (var key in genericRuleDefinitionEntity.Objects) {
                            if (key != "$type")
                                objects.push(genericRuleDefinitionEntity.Objects[key]);
                        }

                        objectDirectivePayload = {
                            objects: objects
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(objectDirectiveAPI, objectDirectivePayload, objectDirectiveLoadDeferred);
                });

                return objectDirectiveLoadDeferred.promise;
            }
            function loadCriteriaDirective() {
                var criteriaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                criteriaDirectiveReadyDeferred.promise.then(function () {
                    var criteriaDirectivePayload = { context: buildContext() };

                    if (genericRuleDefinitionEntity != undefined && genericRuleDefinitionEntity.CriteriaDefinition != null) {
                        criteriaDirectivePayload.GenericRuleDefinitionCriteriaFields = genericRuleDefinitionEntity.CriteriaDefinition.Fields
                    }

                    VRUIUtilsService.callDirectiveLoad(criteriaDirectiveAPI, criteriaDirectivePayload, criteriaDirectiveLoadDeferred);
                });
                
                return criteriaDirectiveLoadDeferred.promise;
            }
            function loadSettingsDirective() {
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                settingsDirectiveReadyDeferred.promise.then(function () {
                    var payload;
                    if (genericRuleDefinitionEntity != undefined && genericRuleDefinitionEntity.SettingsDefinition != null)
                        payload = genericRuleDefinitionEntity.SettingsDefinition;
                    else if (settingsTypeName != undefined)
                        payload = { settingsTypeName: settingsTypeName };
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payload, settingsDirectiveLoadDeferred);
                });

                return settingsDirectiveLoadDeferred.promise;
            }
            function loadViewRequiredPermission() {
                var viewPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                viewPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (genericRuleDefinitionEntity != undefined && genericRuleDefinitionEntity.Security != undefined && genericRuleDefinitionEntity.Security.ViewRequiredPermission != null) {
                        payload = {
                            data: genericRuleDefinitionEntity.Security.ViewRequiredPermission
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, payload, viewPermissionLoadDeferred);
                });

                return viewPermissionLoadDeferred.promise;
            }
            function loadAddRequiredPermission() {
                var addPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                addPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (genericRuleDefinitionEntity != undefined && genericRuleDefinitionEntity.Security != undefined && genericRuleDefinitionEntity.Security.AddRequiredPermission != null) {
                        payload = {
                            data: genericRuleDefinitionEntity.Security.AddRequiredPermission
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(addPermissionAPI, payload, addPermissionLoadDeferred);
                });

                return addPermissionLoadDeferred.promise;
            }
            function loadEditRequiredPermission() {
                var editPermissionLoadDeferred = UtilsService.createPromiseDeferred();

                editPermissionReadyDeferred.promise.then(function () {
                    var payload;

                    if (genericRuleDefinitionEntity != undefined && genericRuleDefinitionEntity.Security != undefined && genericRuleDefinitionEntity.Security.EditRequiredPermission != null) {
                        payload = {
                            data: genericRuleDefinitionEntity.Security.EditRequiredPermission
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(editPermissionAPI, payload, editPermissionLoadDeferred);
                });

                return editPermissionLoadDeferred.promise;
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
            console.log(buildGenericRuleDefinitionObjectFromScope())
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
            return {
                GenericRuleDefinitionId: genericRuleDefinitionId,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.title,
                CriteriaDefinition: criteriaDirectiveAPI.getData(),
                SettingsDefinition: settingsDirectiveAPI.getData(),
                Objects: objectDirectiveAPI.getData(),
                Security: {
                    ViewRequiredPermission: viewPermissionAPI.getData(),
                    AddRequiredPermission:  addPermissionAPI.getData(),
                    EditRequiredPermission: editPermissionAPI.getData()
                }
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionEditorController', GenericRuleDefinitionEditorController);

})(appControllers);