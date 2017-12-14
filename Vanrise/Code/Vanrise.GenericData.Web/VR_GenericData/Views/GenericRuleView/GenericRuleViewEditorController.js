﻿(function (appControllers) {

    "use strict";

    GenericRuleViewEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_Sec_ViewAPIService', 'VR_Sec_MenuAPIService', 'VRLocalizationService'];

    function GenericRuleViewEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Sec_ViewAPIService, VR_Sec_MenuAPIService, VRLocalizationService) {

        var isEditMode;
        var viewTypeName = "VR_GenericData_GenericView";
        var recordTypeEntity;
        var viewId;

        var reportDirectiveAPI;
        var reportReadyDeferred;

        var menuItems;

        var genericRuleDefinitionAPI;
        var genericRuleDefinitionReadyDeferred = UtilsService.createPromiseDeferred();

        var viewCommonPropertiesAPI;
        var viewCommonPropertiesReadyDeferred = UtilsService.createPromiseDeferred();

        var treeAPI;
        var treeReadyDeferred = UtilsService.createPromiseDeferred();
        var viewEntity;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
            isEditMode = (viewId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGenericRuleDefinitionSelectorDirectiveReady = function (api) {
                genericRuleDefinitionAPI = api;
                genericRuleDefinitionReadyDeferred.resolve();
            };
            $scope.scopeModel.SaveView = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.onViewCommonPropertiesReady = function (api) {
                viewCommonPropertiesAPI = api;
                viewCommonPropertiesReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onTreeReady = function (api) {
                treeAPI = api;
                treeReadyDeferred.resolve();
            };

            $scope.scopeModel.validateMenuLocation = function () {
                return ($scope.scopeModel.selectedMenuItem != undefined) ? null : 'No menu location selected';
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getView().then(function () {
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
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadTree, loadGenericRuleDefinition, loadViewCommonProperties]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

            function setTitle() {
                if (isEditMode && viewEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(viewEntity.Name, 'Generic Rule View Editor');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Generic Rule View Editor');
            }
            function loadViewCommonProperties() {
                    var viewCommmonPropertiesLoadDeferred = UtilsService.createPromiseDeferred();
                    viewCommonPropertiesReadyDeferred.promise.then(function () {
                        var payload = {};
                        if (viewEntity != undefined) {
                            payload.viewEntity = viewEntity;
                        }
                        VRUIUtilsService.callDirectiveLoad(viewCommonPropertiesAPI, payload, viewCommmonPropertiesLoadDeferred);
                    });
                    return viewCommmonPropertiesLoadDeferred.promise;
            }

            function loadStaticData() {
                if (viewEntity != undefined) {
                    $scope.scopeModel.reportName = viewEntity.Name;
                    $scope.scopeModel.reportTitle = viewEntity.Title;                  
                }
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

                function loadMenuItems() {
                    return VR_Sec_MenuAPIService.GetAllMenuItems(true, true).then(function (response) {
                        if (response) {
                            menuItems = [];
                            for (var i = 0; i < response.length; i++) {
                                menuItems.push(response[i]);
                            }
                        }
                    });
                }

                return treeLoadDeferred.promise;


            }

            function loadGenericRuleDefinition() {
                var loadGenericRuleDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                genericRuleDefinitionReadyDeferred.promise.then(function () {
                    var payLoad;
                    if (viewEntity != undefined && viewEntity.Settings != undefined) {
                        payLoad = {
                            selectedIds: viewEntity.Settings.RuleDefinitionIds
                        }
                    }
                    VRUIUtilsService.callDirectiveLoad(genericRuleDefinitionAPI, payLoad, loadGenericRuleDefinitionSelectorPromiseDeferred);
                });
                return loadGenericRuleDefinitionSelectorPromiseDeferred.promise;
            }
            function getView() {
                return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
                    viewEntity = viewEntityObj;
                });
            }

        }

        function buildViewObjectFromScope() {

            var viewSettings = {
                $type: "Vanrise.GenericData.Entities.GenericRuleViewSettings, Vanrise.GenericData.Entities",
                RuleDefinitionIds: genericRuleDefinitionAPI.getSelectedIds()
            };
                viewCommonPropertiesAPI.setCommonProperties(viewSettings);
            var view = {
                ViewId: (viewEntity != undefined) ? viewEntity.ViewId : null,
                Name: $scope.scopeModel.reportName,
                Title: $scope.scopeModel.reportTitle,
                ModuleId: $scope.scopeModel.selectedMenuItem.Id,
                Settings: viewSettings,
                Type: viewEntity != undefined ? viewEntity.Type : undefined,

            };

            return view;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            var viewEntityObj = buildViewObjectFromScope();
            viewEntityObj.ViewTypeName = viewTypeName; 
            return VR_Sec_ViewAPIService.AddView(viewEntityObj).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Generic Rule View', response, 'Name')) {
                    if ($scope.onViewAdded != undefined) {
                        $scope.onViewAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function update() {
            $scope.scopeModel.isLoading = true;
            var viewEntityObj = buildViewObjectFromScope();
            return VR_Sec_ViewAPIService.UpdateView(viewEntityObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Generic Rule View', response, 'Name')) {
                    if ($scope.onViewUpdated != undefined) {
                        $scope.onViewUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

    }

    appControllers.controller('VR_GenericData_GenericRuleViewEditorController', GenericRuleViewEditorController);
})(appControllers);
