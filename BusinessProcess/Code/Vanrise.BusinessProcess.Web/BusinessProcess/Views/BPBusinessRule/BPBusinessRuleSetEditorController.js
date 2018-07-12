BusinessProcess_BP_BusinessRuleSetEditorController.$inject = ['$scope', 'BusinessProcess_BPBusinessRuleSetAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

function BusinessProcess_BP_BusinessRuleSetEditorController($scope, BusinessProcess_BPBusinessRuleSetAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {
    var bpDefinitionDirectiveApi;
    var bpDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var bpBusinessRuleSetDetailDirectiveApi;
    var bpBusinessRuleSetDetailReadyPromiseDeferred;

    var bpBusinessRuleSetDirectiveApi;
    var bpBusinessRuleSetReadyPromiseDeferred;

    var isEditMode;
    var bpBusinessRuleSetId;
    var bpBusinessRuleSet;
    var bpBusinessRuleSetParentId;
    var existingBusinessRules;
    var parentRuleSetId;

    var gridAPI;

    var ruleActionsSelectiveAPI;
    var ruleActionsSelectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    loadParameters();
    load();

    function defineScope() {
        $scope.scopeModel = { isReadOnly: undefined };

        $scope.onBPDefinitionDirectiveReady = function (api) {
            bpDefinitionDirectiveApi = api;
            bpDefinitionReadyPromiseDeferred.resolve();
        };

        $scope.onBPBusinessRuleSetDirectiveReady = function (api) {
            bpBusinessRuleSetDirectiveApi = api;
          
                var setLoader = function (value) { };

                var businessRuleSetPayload = {
                    filter: { BPDefinitionId: $scope.bpDefition.BPDefinitionID, CanBeParentOfRuleSetId: bpBusinessRuleSetId },
                    selectedIds: bpBusinessRuleSetParentId
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, bpBusinessRuleSetDirectiveApi, businessRuleSetPayload, setLoader, bpBusinessRuleSetReadyPromiseDeferred);
            
        };
        $scope.onGridReady = function (api) {
            $scope.isLoading = true;
            gridAPI = api;
            gridAPI.load(getGridPayload()).finally(function () {
                $scope.isLoading = false;
            });
        };

        $scope.scopeModel.onBPAccountActionSelectiveReady = function (api) {
            ruleActionsSelectiveAPI = api;
            ruleActionsSelectiveReadyPromiseDeferred.resolve();
        };
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.createNewBusinessRuleSet = function () {
            var businessRuleSetObj = buildObjectFromScope();

            if (isEditMode) {
                BusinessProcess_BPBusinessRuleSetAPIService.UpdateBusinessRuleSet(businessRuleSetObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Business Rule Set", response)) {
                        if ($scope.onBusinessRuleSetUpdated != undefined) {
                            $scope.onBusinessRuleSetUpdated(response.UpdatedObject);
                            gridAPI.onBusinessRuleSetUpdated(response.UpdatedObject);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
            else {
                BusinessProcess_BPBusinessRuleSetAPIService.AddBusinessRuleSet(businessRuleSetObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Business Rule Set", response)) {
                        if ($scope.onBusinessRuleSetAdded != undefined && response.InsertedObject != undefined) {
                            //businessRuleSetObj.BPBusinessRuleSetId = response.InsertedObject.Entity.BPBusinessRuleSetId;
                            $scope.onBusinessRuleSetAdded(response.InsertedObject);
                            gridAPI.onBusinessRuleSetAdded(response.InsertedObject);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
        };

        $scope.hasSaveBusinessRuleSetPermission = function () {
            if (isEditMode) {
                return BusinessProcess_BPBusinessRuleSetAPIService.HasEditBusinessRuleSet();
            }
            else {
                return BusinessProcess_BPBusinessRuleSetAPIService.HasAddBusinessRuleSet();
            }
        };

        $scope.onBPDefinitionSelectionchanged = function () {

            if (bpBusinessRuleSetDirectiveApi != undefined && bpBusinessRuleSetDetailDirectiveApi != undefined) {

                var bpDefinitionId = bpDefinitionDirectiveApi.getSelectedIds();
                var businessRuleSetPayload = {
                    filter: { BPDefinitionId: bpDefinitionId, CanBeParentOfRuleSetId: bpBusinessRuleSetId },
                    selectedIds: bpBusinessRuleSetParentId
                };

                var businessRuleSetDetailPayload = {
                    BPDefinitionId: bpDefinitionId,
                    existingBusinessRules: existingBusinessRules
                };

                if (bpDefinitionId != undefined) {
                    bpBusinessRuleSetDirectiveApi.load(businessRuleSetPayload);
                    bpBusinessRuleSetDetailDirectiveApi.load(businessRuleSetDetailPayload);
                }
            }
        };

        $scope.onParentRuleSetChanged = function () {
            parentRuleSetId = bpBusinessRuleSetDirectiveApi.getSelectedIds();
            if (gridAPI != undefined) {
                $scope.isLoading = true;
                gridAPI.load(getGridPayload()).finally(function () {
                    $scope.isLoading = false;
                });
            }
        };
    }

    function buildObjectFromScope() {
        var businessRules = gridAPI.getData();
        var instance = {};
        instance.BPBusinessRuleSetId = bpBusinessRuleSetId;
        instance.Name = $scope.name;
        instance.BPDefinitionId = $scope.bpDefition.BPDefinitionID;
        instance.ParentId = $scope.scopeModel.selectedbusinessRuleSet != undefined ? $scope.scopeModel.selectedbusinessRuleSet.BPBusinessRuleSetId : null;
        instance.Details = { ActionDetails: [] };
        for (var z = 0; z < businessRules.length; z++) {
            var currentItem = businessRules[z];
            var effectiveAction = gridAPI.getData();
            var dataItem = { BPBusinessRuleDefinitionId: currentItem.RuleDefinitionId, Settings: { } };
            if (currentItem.Entity.Action != undefined) {
                if (currentItem.Entity.Action.action != undefined)
                    dataItem.Settings.Action = currentItem.Entity.Action.action;
                else dataItem.Settings.Action = currentItem.Entity.Action;
            }
            instance.Details.ActionDetails.push(dataItem);
        }
        return instance;
    }

    function getGridPayload() {
        return obj = {
            query: {
                BusinessProcessId: $scope.bpDefition.BPDefinitionID,
                BusinessRuleSetDefinitionId: bpBusinessRuleSetId != undefined ? bpBusinessRuleSetId : bpBusinessRuleSetDirectiveApi.getSelectedIds()
            },
            isEditMode: isEditMode,
            bpBusinessRuleSetId: bpBusinessRuleSetId,
            parentRuleSetId: parentRuleSetId
        }



    }
    function load() {
        $scope.isLoading = true;

        if (isEditMode) {
            getBPBusinessRuleSet().then(function () {
                loadAllControls()
                    .finally(function () {
                        bpBusinessRuleSet = undefined;
                    });
            }).catch(function () {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }
        else {
            loadAllControls();
        }
    }

    function getBPBusinessRuleSet() {
        return BusinessProcess_BPBusinessRuleSetAPIService.GetBusinessRuleSetsByID(bpBusinessRuleSetId).then(function (response) {
            bpBusinessRuleSet = response;
            bpBusinessRuleSetParentId = response.ParentId;
            existingBusinessRules = response.Details;
        });

    }

    function loadAllControls() {
        $scope.isLoading = true;
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadData, loadBPDefinitions])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isLoading = false;
          });
    }

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            bpBusinessRuleSetId = parameters.bpBusinessRuleSetId;
        }
        if (bpBusinessRuleSetId != undefined) {
            isEditMode = true;
            $scope.scopeModel.isReadOnly = true;
        }
    }

    function loadRuleActionsSelective() {
        var ruleActionsLoadDeferred = UtilsService.createPromiseDeferred();
        ruleActionsSelectiveReadyPromiseDeferred.promise.then(function () {
            var payload;
            VRUIUtilsService.callDirectiveLoad(ruleActionsSelectiveAPI, payload, ruleActionsLoadDeferred);
        });

        return ruleActionsLoadDeferred.promise;
    }
    function loadData() {
        if (bpBusinessRuleSet == undefined)
            return;
        $scope.name = bpBusinessRuleSet.Name;
    }

    function loadBPDefinitions() {
        var loadBPDefinitionsPromiseDeferred = UtilsService.createPromiseDeferred();
        bpDefinitionReadyPromiseDeferred.promise.then(function () {
            var payload = undefined;
            if (isEditMode) {
                payload = { selectedIds: bpBusinessRuleSet.BPDefinitionId };
            }
            VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, payload, loadBPDefinitionsPromiseDeferred);
        });

        return loadBPDefinitionsPromiseDeferred.promise;
    }

    function setTitle() {
        if (isEditMode)
            $scope.title = 'Edit Business Rule Set: ' + bpBusinessRuleSet.Name;
        else
            $scope.title = 'Add Business Rule Set';
    }


}
appControllers.controller('BusinessProcess_BP_BusinessRuleSetEditorController', BusinessProcess_BP_BusinessRuleSetEditorController);
