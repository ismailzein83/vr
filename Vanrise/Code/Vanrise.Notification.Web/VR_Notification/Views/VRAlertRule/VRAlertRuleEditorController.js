(function (appControllers) {

    "use strict";

    VRAlertRuleEditorController.$inject = ['$scope', 'VR_Notification_VRAlertRuleAPIService', 'VR_Notification_VRAlertRuleTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function VRAlertRuleEditorController($scope, VR_Notification_VRAlertRuleAPIService, VR_Notification_VRAlertRuleTypeAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var vrAlertRuleId;
        var vrAlertRuleEntity;
        var vrAlertRuleTypeId;
        var vrAlertRuleTypeEntity;

        var vrAlertRuleTypeSelectorAPI;
        var vrAlertRuleTypeSelectoReadyDeferred = UtilsService.createPromiseDeferred();
        var vrAlertRuleTypeSelectoSelectionChangedDeferred;

        var  vrAlertRuleCriteriaDirectiveAPI;
        var  vrAlertRuleCriteriaDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrAlertRuleId = parameters.vrAlertRuleId;
            }

            isEditMode = (vrAlertRuleId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.canDefineCriteria = false;

            $scope.scopeModel.onAlertRuleTypeSelectorReady = function (api) {
                vrAlertRuleTypeSelectorAPI = api;
                vrAlertRuleTypeSelectoReadyDeferred.resolve();
            }
            $scope.scopeModel.onAlertRuleTypeSelectionChanged = function (api) {
                vrAlertRuleTypeId = vrAlertRuleTypeSelectorAPI.getSelectedIds();
                if (vrAlertRuleTypeId != undefined) {

                    getVRAlertRuleType().then(function () {

                        $scope.scopeModel.canDefineCriteria = true;

                        var vrAlertRuleCriteriaDirectivePayload = {};
                        if (vrAlertRuleTypeEntity != undefined && vrAlertRuleTypeEntity.Settings != undefined) {
                            vrAlertRuleCriteriaDirectivePayload.dataAnalysisDefinitionId = vrAlertRuleTypeEntity.Settings.DataAnalysisDefinitionId;
                            vrAlertRuleCriteriaDirectivePayload.criteriaEditor = vrAlertRuleTypeEntity.Settings.CriteriaEditor
                        }
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingAlertRuleTypeSelector = value;
                        };

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, vrAlertRuleCriteriaDirectiveAPI, vrAlertRuleCriteriaDirectivePayload, setLoader, vrAlertRuleTypeSelectoSelectionChangedDeferred);
                    });
                }
            }

            $scope.scopeModel.onVRAlertRuleCriteriaDirectiveReady = function (api) {
                 vrAlertRuleCriteriaDirectiveAPI = api;
                 vrAlertRuleCriteriaDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {

                getVRAlertRule().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getVRAlertRule() {
            return VR_Notification_VRAlertRuleAPIService.GetVRAlertRule(vrAlertRuleId).then(function (response) {
                vrAlertRuleEntity = response;
            });
        }
        function getVRAlertRuleType() {
            return VR_Notification_VRAlertRuleTypeAPIService.GetVRAlertRuleType(vrAlertRuleTypeId).then(function (response) {
                vrAlertRuleTypeEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRAlertRuleTypeSelector, loadVRAlertRuleCriteriaDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var vrAlertRuleName = (vrAlertRuleEntity != undefined) ? vrAlertRuleEntity.Name : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrAlertRuleName, 'Alert Rule');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Alert Rule');
                }
            }
            function loadStaticData() {
                if (vrAlertRuleEntity == undefined)
                    return;
                $scope.scopeModel.name = vrAlertRuleEntity.Name;
            }
            function loadVRAlertRuleTypeSelector() {
                var vrAlertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                vrAlertRuleTypeSelectoReadyDeferred.promise.then(function () {
                    var vrAlertRuleTypeSelectorPayload;

                    if (vrAlertRuleEntity != undefined && vrAlertRuleEntity.RuleTypeId != undefined) {
                        vrAlertRuleTypeSelectorPayload = {
                            selectedIds: vrAlertRuleEntity.RuleTypeId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(vrAlertRuleTypeSelectorAPI, vrAlertRuleTypeSelectorPayload, vrAlertRuleTypeSelectorLoadDeferred);
                });

                return vrAlertRuleTypeSelectorLoadDeferred.promise;  
            }
            function loadVRAlertRuleCriteriaDirective() {
                if (!isEditMode) return;

                var vrAlertRuleCriteriaDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                if (vrAlertRuleTypeSelectoSelectionChangedDeferred == undefined)
                    vrAlertRuleTypeSelectoSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                vrAlertRuleCriteriaDirectiveReadyDeferred.promise.then(function () {

                    vrAlertRuleTypeSelectoSelectionChangedDeferred.promise.then(function () {
                        vrAlertRuleTypeSelectoSelectionChangedDeferred = undefined;

                        var vrAlertRuleCriteriaDirectivePayload = {};
                        if (vrAlertRuleEntity != undefined) {
                            vrAlertRuleCriteriaDirectivePayload.criteria = vrAlertRuleEntity.Settings.Criteria;
                        }
                        if (vrAlertRuleTypeEntity != undefined && vrAlertRuleTypeEntity.Settings != undefined) {
                            vrAlertRuleCriteriaDirectivePayload.dataAnalysisDefinitionId = vrAlertRuleTypeEntity.Settings.DataAnalysisDefinitionId;
                            vrAlertRuleCriteriaDirectivePayload.criteriaEditor = vrAlertRuleTypeEntity.Settings.CriteriaEditor
                        }

                        VRUIUtilsService.callDirectiveLoad(vrAlertRuleCriteriaDirectiveAPI, vrAlertRuleCriteriaDirectivePayload, vrAlertRuleCriteriaDirectiveLoadDeferred);
                    })
                });

                return  vrAlertRuleCriteriaDirectiveLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VR_Notification_VRAlertRuleAPIService.AddVRAlertRule(buildVRAlertRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('VRAlertRule', response, 'Name')) {
                    if ($scope.onVRAlertRuleAdded != undefined)
                        $scope.onVRAlertRuleAdded(response.InsertedObject);
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
            return VR_Notification_VRAlertRuleAPIService.UpdateVRAlertRule(buildVRAlertRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('VRAlertRule', response, 'Name')) {
                    if ($scope.onVRAlertRuleUpdated != undefined) {
                        $scope.onVRAlertRuleUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildCriteriaContext() {

            var context = {
                getDataAnalysisDefinitionId: function () {

                    var dataAnalysisDefinitionId;
                    if (vrAlertRuleTypeEntity != undefined && vrAlertRuleTypeEntity.Settings != undefined)
                        dataAnalysisDefinitionId = vrAlertRuleTypeEntity.Settings.DataAnalysisDefinitionId;
           
                    return dataAnalysisDefinitionId;
                }
            }
            return context;
        }
        function buildVRAlertRuleObjFromScope() {

            return {
                VRAlertRuleId: vrAlertRuleEntity != undefined ? vrAlertRuleEntity.VRAlertRuleId : undefined,
                Name: $scope.scopeModel.name,
                RuleTypeId: vrAlertRuleTypeSelectorAPI.getSelectedIds(),
                Settings: {
                    RuleTypeId: null,
                    Criteria: vrAlertRuleCriteriaDirectiveAPI.getData(),
                    Actions: null,
                    RollbackActions: null
                } 
            };
        }
    }

    appControllers.controller('VR_Notification_VRAlertRuleEditorController', VRAlertRuleEditorController);

})(appControllers);