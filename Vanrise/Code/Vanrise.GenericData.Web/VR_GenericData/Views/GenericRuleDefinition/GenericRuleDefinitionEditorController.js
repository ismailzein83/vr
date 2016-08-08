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

        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();

        var objectDirectiveAPI;
        var objectDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var criteriaDirectiveAPI;
        var criteriaDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        
        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();




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

            $scope.scopeModel.onTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

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
            }
            $scope.scopeModel.validateMenuLocation = function () {
                return ($scope.scopeModel.selectedMenuItem != undefined) ? null : 'No menu location selected';
            };
        }
        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getEntities().then(function () {
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

        function getEntities() {
            return UtilsService.waitMultipleAsyncOperations([getGenericRuleDefinition, getGenericRuleDefinitionView]);

            function getGenericRuleDefinition() {
                return VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinition(genericRuleDefinitionId).then(function (response) {
                    genericRuleDefinitionEntity = response;
                });
            }
            function getGenericRuleDefinitionView() {
                VR_GenericData_GenericRuleDefinitionAPIService.GetGenericRuleDefinitionView(genericRuleDefinitionId).then(function (response) {
                    viewEntity = response;
                });
            }
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTree, loadCriteriaDirective, loadSettingsDirective, loadObjectDirective]).catch(function (error) {
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
            function loadTree() {
                var treeLoadDeferred = UtilsService.createPromiseDeferred();

                loadMenuItems().then(function () {
                    treeReadyDeferred.promise.then(function () {
                        if (viewEntity != undefined) {
                            $scope.scopeModel.selectedMenuItem = treeAPI.setSelectedNode(menuItems, viewEntity.ModuleId, "Id", "Childs");
                        }
                        treeAPI.refreshTree(menuItems);
                        treeLoadDeferred.resolve();
                    });
                }).catch(function (error) {
                    treeLoadDeferred.reject(error);
                });

                return treeLoadDeferred.promise;

                function loadMenuItems() {
                    return VR_Sec_MenuAPIService.GetAllMenuItems(false,true).then(function (response) {
                        if (response) {
                            menuItems = [];
                            for (var i = 0; i < response.length; i++) {
                                menuItems.push(response[i]);
                            }
                        }
                    });
                }
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
                    if(genericRuleDefinitionEntity != undefined && genericRuleDefinitionEntity.SettingsDefinition != null)
                        payload = genericRuleDefinitionEntity.SettingsDefinition;
                    else if (settingsTypeName != undefined)
                        payload = { settingsTypeName: settingsTypeName }
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payload, settingsDirectiveLoadDeferred);
                });

                return settingsDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.isLoading = true;
            var promises = [];
            var serverResponse;

            var insertEntityDeferred = UtilsService.createPromiseDeferred();
            promises.push(insertEntityDeferred.promise);
            
            var insertViewDeferred = UtilsService.createPromiseDeferred();
            promises.push(insertViewDeferred.promise);

            insertGenericRuleDefinition().then(function () {
                if (serverResponse.Result == InsertOperationResultEnum.Succeeded.value) {
                    insertEntityDeferred.resolve();
                    insertView().then(function () { insertViewDeferred.resolve(); }).catch(function (error) { insertViewDeferred.reject(error); });
                }else if(serverResponse.Result == InsertOperationResultEnum.SameExists.value)
                {
                    insertEntityDeferred.resolve();
                    insertViewDeferred.resolve();
                }
                else {
                    insertEntityDeferred.reject();
                }
            });

            return UtilsService.waitMultiplePromises(promises).then(function () {
                if (VRNotificationService.notifyOnItemAdded('Generic Rule Definition', serverResponse, 'Name')) {
                    if ($scope.onGenericRuleDefinitionAdded != undefined && typeof ($scope.onGenericRuleDefinitionAdded) == 'function') {
                        $scope.onGenericRuleDefinitionAdded(serverResponse.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function insertGenericRuleDefinition() {
                return VR_GenericData_GenericRuleDefinitionAPIService.AddGenericRuleDefinition(buildGenericRuleDefinitionObjectFromScope()).then(function (response) {
                    serverResponse = response;
                });
            }
            function insertView() {
                var view = buildViewObjectFromScope(serverResponse.InsertedObject.GenericRuleDefinitionId);
                view.ViewTypeName = viewTypeName;
                return VR_Sec_ViewAPIService.AddView(view);
            }
        }
        function update() {
            $scope.isLoading = true;
            var promises = [];
            var ruleResponse;

            return UtilsService.waitMultipleAsyncOperations([updateGenericRuleDefinition, updateView]).then(function () {
                if (VRNotificationService.notifyOnItemUpdated('Generic Rule Definition', ruleResponse, 'Name')) {
                    if ($scope.onGenericRuleDefinitionUpdated != undefined && typeof ($scope.onGenericRuleDefinitionUpdated)) {
                        $scope.onGenericRuleDefinitionUpdated(ruleResponse.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function updateGenericRuleDefinition() {
                return VR_GenericData_GenericRuleDefinitionAPIService.UpdateGenericRuleDefinition(buildGenericRuleDefinitionObjectFromScope()).then(function (response) {
                    ruleResponse = response;
                });
            }
            function updateView() {
                if (viewEntity != null)
                    return VR_Sec_ViewAPIService.UpdateView(buildViewObjectFromScope(genericRuleDefinitionId), viewTypeName);
                else
                {
                    var view = buildViewObjectFromScope(genericRuleDefinitionId);
                    view.ViewTypeName = viewTypeName;
                    return VR_Sec_ViewAPIService.AddView(view);
                }
                    
            }
        }

        function buildContext() {

            var context =  {
                getObjectVariables: function () { return objectDirectiveAPI.getData(); }
            }
            return context;
        }

        function buildGenericRuleDefinitionObjectFromScope() {
            return {
                GenericRuleDefinitionId: genericRuleDefinitionId,
                Name: $scope.scopeModel.name,
                CriteriaDefinition: criteriaDirectiveAPI.getData(),
                SettingsDefinition: settingsDirectiveAPI.getData(),
                Objects: objectDirectiveAPI.getData()
            };
        }
        function buildViewObjectFromScope(genericRuleDefinitionId) {
            return {
                ViewId: (viewEntity != undefined) ? viewEntity.ViewId : null,
                Name: $scope.scopeModel.name,
                Title: $scope.scopeModel.name,
                ModuleId: $scope.scopeModel.selectedMenuItem.Id,
                Settings: {
                    $type: 'Vanrise.GenericData.Entities.GenericRuleViewSettings, Vanrise.GenericData.Entities',
                    RuleDefinitionId: genericRuleDefinitionId
                },
                Type: VR_Sec_ViewTypeEnum.System.value
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericRuleDefinitionEditorController', GenericRuleDefinitionEditorController);

})(appControllers);