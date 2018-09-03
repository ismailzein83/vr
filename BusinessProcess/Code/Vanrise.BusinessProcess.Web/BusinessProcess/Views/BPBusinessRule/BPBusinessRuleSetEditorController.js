BusinessProcess_BP_BusinessRuleSetEditorController.$inject = ['$scope', 'BusinessProcess_BPBusinessRuleSetAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

function BusinessProcess_BP_BusinessRuleSetEditorController($scope, BusinessProcess_BPBusinessRuleSetAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {
    var bpDefinitionDirectiveApi;
    var bpDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var bpBusinessRuleSetDetailDirectiveApi;
    var bpBusinessRuleSetDetailReadyPromiseDeferred;

    var bpBusinessRuleSetDirectiveApi;
    var bpBusinessRuleSetReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var isEditMode;
    var bpBusinessRuleSetId;
    var bpBusinessRuleSet;
    var bpBusinessRuleSetParentId;
    var existingBusinessRules;
    var parentRuleSetId;

    var businessProcessSelectedPromiseDeferred;

    var gridAPI;
    var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var ruleSetHasParent;

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
            bpBusinessRuleSetReadyPromiseDeferred.resolve();
            //var setLoader = function (value) { };

            //var businessRuleSetPayload = {
            //    filter: { BPDefinitionId: $scope.bpDefition.BPDefinitionID, CanBeParentOfRuleSetId: bpBusinessRuleSetId },
            //    selectedIds: bpBusinessRuleSetParentId
            //};
            //VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, bpBusinessRuleSetDirectiveApi, businessRuleSetPayload, setLoader, bpBusinessRuleSetReadyPromiseDeferred);

        };
        $scope.onGridReady = function (api) {
            gridAPI = api;
            gridReadyPromiseDeferred.resolve();
        };

        $scope.scopeModel.onBPAccountActionSelectiveReady = function (api) {
            ruleActionsSelectiveAPI = api;
            ruleActionsSelectiveReadyPromiseDeferred.resolve();
        };
        $scope.close = function () {
            $scope.modalContext.closeModal();
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

        //$scope.onBPDefinitionSelectionchanged = function () {
        //    //$scope.isLoading = true;
        //    //gridReadyPromiseDeferred.promise.then(function () {
        //    //    gridAPI.load(getGridPayload()).finally(function () {
        //    //        $scope.isLoading = false;
        //    //    });
        //    //})

        //    if (bpBusinessRuleSetDirectiveApi != undefined && bpBusinessRuleSetDetailDirectiveApi != undefined) {

        //        var bpDefinitionId = bpDefinitionDirectiveApi.getSelectedIds();
        //        var businessRuleSetPayload = {
        //            filter: { BPDefinitionId: bpDefinitionId, CanBeParentOfRuleSetId: bpBusinessRuleSetId },
        //            selectedIds: bpBusinessRuleSetParentId
        //        };

        //        var businessRuleSetDetailPayload = {
        //            BPDefinitionId: bpDefinitionId,
        //            existingBusinessRules: existingBusinessRules
        //        };

        //        if (bpDefinitionId != undefined) {
        //            bpBusinessRuleSetDirectiveApi.load(businessRuleSetPayload);
        //            bpBusinessRuleSetDetailDirectiveApi.load(businessRuleSetDetailPayload);
        //        }

        //    }
        //};

        $scope.onBPDefinitionSelectionchanged = function (value) {
            $scope.bpDefition = value;
            var bpDefinitionId = bpDefinitionDirectiveApi.getSelectedIds();
            if (bpDefinitionId != undefined && $scope.bpDefition != undefined)
            {
                if(businessProcessSelectedPromiseDeferred != undefined)
                {
                    businessProcessSelectedPromiseDeferred.resolve();
                }
                else
                {
                    var parentRuleSetSelectorPayload = {
                        filter: { BPDefinitionId: bpDefinitionId, CanBeParentOfRuleSetId: bpBusinessRuleSetId },
                        selectedIds: bpBusinessRuleSet != undefined ? bpBusinessRuleSetParentId : undefined
                    };
                    var ruleSetGridPayload = getGridPayload();
                    bpBusinessRuleSetDirectiveApi.load(parentRuleSetSelectorPayload);
                    gridAPI.load(ruleSetGridPayload);
                }
            }
        };
        $scope.onParentRuleSetChanged = function (value) {
            if (!isEditMode && value != undefined) {
                parentRuleSetId = bpBusinessRuleSetDirectiveApi.getSelectedIds();
                gridReadyPromiseDeferred.promise.then(function () {
                    $scope.isLoading = true;
                    gridAPI.load(getGridPayload()).finally(function () {
                        $scope.isLoading = false;
                    });
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
        instance.Details = { ActionDetails: []};
        for (var z = 0; z < businessRules.length; z++) {
            var currentItem = businessRules[z];
            var effectiveAction = gridAPI.getData();
            var dataItem = { BPBusinessRuleDefinitionId: currentItem.RuleDefinitionId, Settings: {} };
            if (currentItem.Entity.Action != undefined) {
                dataItem.Settings.Disabled = currentItem.Entity.Disabled;
                if (currentItem.Entity.Action.action != undefined)
                    dataItem.Settings.Action = currentItem.Entity.Action.action;
                else dataItem.Settings.Action = currentItem.Entity.Action;
            }
            instance.Details.ActionDetails.push(dataItem);
        }
        return instance;
    }
    function loadRuleSetGrid() {
        if (isEditMode) {
            var ruleSetLoadDeferred = UtilsService.createPromiseDeferred();
            gridReadyPromiseDeferred.promise.then(function () {
                var payload = getGridPayload();
                VRUIUtilsService.callDirectiveLoad(gridAPI, payload, ruleSetLoadDeferred);
            });
            return ruleSetLoadDeferred.promise;
        }
    }

    function getGridPayload() {
        return obj = {
            query: {
                BusinessProcessId: $scope.bpDefition.BPDefinitionID,
                BusinessRuleSetDefinitionId: bpBusinessRuleSetId != undefined ? bpBusinessRuleSetId : undefined,
                ParentBusinessRuleSetId: parentRuleSetId
            },
            isEditMode: isEditMode,
            bpBusinessRuleSetId: bpBusinessRuleSetId,
            parentRuleSetId: parentRuleSetId,
        };
    }
    function load() {
        $scope.isLoading = true;

        if (isEditMode) {
            getBPBusinessRuleSet().then(function () {
                loadAllControls()
                    .finally(function () {
                        bpBusinessRuleSet = undefined;
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

    function getBPBusinessRuleSet() {
        return BusinessProcess_BPBusinessRuleSetAPIService.GetBusinessRuleSetsByID(bpBusinessRuleSetId).then(function (response) {
            bpBusinessRuleSet = response;
            bpBusinessRuleSetParentId = response.ParentId;
            existingBusinessRules = response.Details;
        });

    }

    function loadAllControls() {
        $scope.isLoading = true;
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadData, loadBPSection])
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
    function loadBPSection ()
    {
        var loadBusinessProcessPromiseDeferred = UtilsService.createPromiseDeferred();

        var promises = [];
        promises.push(loadBusinessProcessPromiseDeferred.promise);

        var payload = {
            filter: {
                $type: "Vanrise.BusinessProcess.Entities.BPDefinitionInfoFilter,Vanrise.BusinessProcess.Entities",
                Filters: [{
                    $type: "Vanrise.BusinessProcess.Business.BPDefinitionRuleSetFilter,Vanrise.BusinessProcess.Business"
                }]
            }
        };
        if (bpBusinessRuleSet != undefined && bpBusinessRuleSet.BPDefinitionId != undefined) {
            payload.selectedIds = bpBusinessRuleSet != undefined ? bpBusinessRuleSet.BPDefinitionId : undefined;
            businessProcessSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
        }

        bpDefinitionReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, payload, loadBusinessProcessPromiseDeferred);
        });
        if (bpBusinessRuleSet != undefined && bpBusinessRuleSet.BPDefinitionId != undefined) {
            var loadParentRuleSetPromiseDeferred = UtilsService.createPromiseDeferred();
            var loadRuleSetGridPromiseDeferref = UtilsService.createPromiseDeferred();

            promises.push(loadParentRuleSetPromiseDeferred.promise);
            promises.push(loadRuleSetGridPromiseDeferref.promise);
            UtilsService.waitMultiplePromises([bpBusinessRuleSetReadyPromiseDeferred.promise, gridReadyPromiseDeferred.promise, businessProcessSelectedPromiseDeferred.promise]).then(function () {
                var bpDefinitionId = bpDefinitionDirectiveApi.getSelectedIds();
                var parentRuleSetSelectorPayload = {
                    filter: { BPDefinitionId: bpDefinitionId, CanBeParentOfRuleSetId: bpBusinessRuleSetId },
                    selectedIds:bpBusinessRuleSet != undefined ? bpBusinessRuleSetParentId:undefined
                };
                var ruleSetGridPayload = getGridPayload();
                VRUIUtilsService.callDirectiveLoad(bpBusinessRuleSetDirectiveApi, parentRuleSetSelectorPayload, loadParentRuleSetPromiseDeferred);
                VRUIUtilsService.callDirectiveLoad(gridAPI, ruleSetGridPayload, loadRuleSetGridPromiseDeferref);
               
                businessProcessSelectedPromiseDeferred = undefined;
            });
        }

        return UtilsService.waitMultiplePromises(promises);
    }
    function loadBPDefinitions() {
        var loadBPDefinitionsPromiseDeferred = UtilsService.createPromiseDeferred();
        bpDefinitionReadyPromiseDeferred.promise.then(function () {
            var payload = {
                filter: {
                    $type: "Vanrise.BusinessProcess.Entities.BPDefinitionInfoFilter,Vanrise.BusinessProcess.Entities",
                    Filters: [{
                        $type: "Vanrise.BusinessProcess.Business.BPDefinitionRuleSetFilter,Vanrise.BusinessProcess.Business"
                    }]
                }
            };
            if (isEditMode) {
                payload.selectedIds = bpBusinessRuleSet.BPDefinitionId;
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
