(function (appControllers) {

    "use strict";

    AddBusinessEntityEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

    function AddBusinessEntityEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService) {

       
        var displayName;
        var context;
        var isNew;
        var businessEntityDefinitionId;
        var settings;
        var beDefinitionSelectorApi;
        var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
        var businessEntityDefinitionSelectedPromise;
        var settingsDirectiveAPI;
        var settingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                isNew = parameters.isNew;
                context = parameters.context;
                if (parameters.obj != undefined) {
                    displayName = parameters.obj.DisplayName;
                    businessEntityDefinitionId = parameters.obj.EntityDefinitionId;
                    settings = parameters.obj.Settings;
                }
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isVRWorkflowActivityDisabled = false;
            $scope.scopeModel.selectedBusinessEntity;
            $scope.scopeModel.context = context;

            $scope.modalContext.onModalHide = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
            };
           
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                beDefinitionSelectorApi = api;
                beDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (value) {
                $scope.scopeModel.selectedBusinessEntity = value;
                if (value) {
                    if (businessEntityDefinitionSelectedPromise != undefined)
                        businessEntityDefinitionSelectedPromise.resolve();
                    else {
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingSettingsDirective = value;
                        };
                        settingsReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, settingsDirectiveAPI, {
                                context: context,
                                businessEntityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                            }, setLoader);
                        });
                    }
                }
            };
            $scope.scopeModel.saveActivity = function () {
                return updateActivity();
            };

            $scope.scopeModel.close = function () {
                if ($scope.remove != undefined && isNew == true) {
                    $scope.remove();
                }
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            var promises = [];

            function setTitle() {
                $scope.title = "Edit Add Business Object";
            }

            function loadStaticData() {
                $scope.scopeModel.displayName = displayName;
            }

            function loadBusinessEntityDefinitionSelector() {
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                beDefinitionSelectorPromiseDeferred.promise.then(function () {
                    var payloadSelector = {
                        filter: {
                            Filters: [{
                                $type: "Vanrise.GenericData.Business.WorkflowAddBEActivityFilter, Vanrise.GenericData.Business"
                            }]
                        },
                        selectedIds: businessEntityDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, payloadSelector, businessEntityDefinitionSelectorLoadDeferred);
                });
                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }

            function loadSettingsSelector() {
                var settingsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                if (businessEntityDefinitionSelectedPromise != undefined)
                    promises.push(businessEntityDefinitionSelectedPromise.promise);
                settingsReadyPromiseDeferred.promise.then(function () {
                    var payloadSelector = {
                        context: context,
                        businessEntityDefinitionId: $scope.scopeModel.selectedBusinessEntity.BusinessEntityDefinitionId,
                        settings: settings
                    };
                    VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, payloadSelector, settingsSelectorLoadDeferred);
                    businessEntityDefinitionSelectedPromise = undefined;
                });
                return settingsSelectorLoadDeferred.promise;
            }
            setTitle();
            loadStaticData();
            promises.push(loadBusinessEntityDefinitionSelector());
            if (settings != undefined) {
                businessEntityDefinitionSelectedPromise = UtilsService.createPromiseDeferred();
                promises.push(loadSettingsSelector());
            }
            var rootPromiseNode = { promises: promises };

            return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateActivity() {
            $scope.scopeModel.isLoading = true;
            var updatedObject = {
                displayName: $scope.scopeModel.displayName,
                entityDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                settings: settingsDirectiveAPI.getData()
            };

            if ($scope.onActivityUpdated != undefined) {
                $scope.onActivityUpdated(updatedObject);
            }

            $scope.scopeModel.isLoading = false;
            isNew = false;
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('BusinessProcess_VR_WorkflowActivityAddBusinessEntityController', AddBusinessEntityEditorController);
})(appControllers);