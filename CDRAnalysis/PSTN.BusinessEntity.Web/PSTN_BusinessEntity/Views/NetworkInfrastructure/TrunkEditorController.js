TrunkEditorController.$inject = ["$scope", "CDRAnalysis_PSTN_TrunkAPIService", "TrunkTypeEnum", "TrunkDirectionEnum", "UtilsService", "VRNavigationService", "VRNotificationService", "VRUIUtilsService"];

function TrunkEditorController($scope, CDRAnalysis_PSTN_TrunkAPIService, TrunkTypeEnum, TrunkDirectionEnum, UtilsService, VRNavigationService, VRNotificationService, VRUIUtilsService) {

    var trunkId;
    var switchId;
    var isEditMode;
    var trunkEntity;
    var switchDirectiveApi;
    var switchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var typeDirectiveApi;
    var typeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var directionDirectiveApi;
    var directionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var linkSwitchDirectiveApi;
    var linkSwitchReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var linkTrunkDirectiveApi;
    var linkTrunkReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var currentlyLinkedToTrunkId;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            trunkId = parameters.TrunkId;
            switchId = parameters.SwitchId;
        }
        $scope.disableSwitchMenu = switchId != undefined;
        isEditMode = (trunkId != undefined);
    }

    function defineScope() {

        $scope.hasSaveTrunkPermission = function () {
            if (isEditMode)
                return CDRAnalysis_PSTN_TrunkAPIService.HasUpdateTrunkPermission();
            else
                return CDRAnalysis_PSTN_TrunkAPIService.HasAddTrunkPermission();
        };

        $scope.selectedSwitch;
        $scope.selectedSwitchToLinkTo;
        $scope.selectedTrunkToLinkTo;
        $scope.disableSwitchMenu = switchId != undefined;
        $scope.onReadySwicth = function (api) {
            switchDirectiveApi = api;
            switchReadyPromiseDeferred.resolve();
        };

        $scope.onReadyTrunkType = function (api) {
            typeDirectiveApi = api;
            typeReadyPromiseDeferred.resolve();
        };

        $scope.onReadyTrunkDirection = function (api) {
            directionDirectiveApi = api;
            directionReadyPromiseDeferred.resolve();
        };

        $scope.onReadyLinkSwicth = function (api) {
            linkSwitchDirectiveApi = api;
            linkSwitchReadyPromiseDeferred.resolve();
        };

        $scope.onReadyLinkTrunk = function (api) {
            linkTrunkDirectiveApi = api;
            linkTrunkReadyPromiseDeferred.resolve();
        };

        $scope.onSwitchChanged = function () {

            $scope.selectedSwitchToLinkTo = undefined;
            if ($scope.selectedSwitch != undefined && $scope.selectedSwitch != null) {
                var directivePayload = {};
                directivePayload.filter = { ExcludedIds: [$scope.selectedSwitch.SwitchId] };
                linkSwitchDirectiveApi.load(directivePayload)
            }

        };

        $scope.onSwitchToLinkToChanged = function () {
            $scope.selectedTrunkToLinkTo = undefined;
            if ($scope.selectedSwitchToLinkTo != undefined && $scope.selectedSwitchToLinkTo != null) {
                var directivePayload = {};
                directivePayload.filter = {
                    SwitchIds: [$scope.selectedSwitchToLinkTo.SwitchId],
                    TrunkNameFilter: null
                };
                directivePayload.excludedId = trunkId;
                linkTrunkDirectiveApi.load(directivePayload)
            }
        };

        $scope.saveTrunk = function () {
            if (isEditMode)
                return updateTrunk();
            else
                return insertTrunk();
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        $scope.isGettingData = true;

        if (isEditMode) {
            getTrunk().then(function () {
                loadAllControls().then(function () {
                    trunkEntity = undefined;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isGettingData = false;
                });
            });
        }
        else {
            loadAllControls();
        }

    }

    function getTrunk() {
        return CDRAnalysis_PSTN_TrunkAPIService.GetTrunkById(trunkId).then(function (trunk) {
            trunkEntity = trunk;
        });
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSwitchSelector, loadTypeSelector, loadDirectionSelector, loadLinkSwitchSelector, loadLinkTrunkSelector])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isGettingData = false;
          });
    }



    function setTitle() {
        if (isEditMode && trunkEntity != undefined)
            $scope.title = UtilsService.buildTitleForUpdateEditor(trunkEntity.Name, "Trunk");
        else
            $scope.title = UtilsService.buildTitleForAddEditor("Trunk");
    }

    function loadStaticData() {

        if (trunkEntity == undefined)
            return;

        $scope.name = trunkEntity.Name;
        $scope.symbol = trunkEntity.Symbol;
    }


    function loadSwitchSelector() {
        var switchLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        switchReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload = {
                    selectedIds: trunkEntity != undefined ? trunkEntity.SwitchId : (switchId != undefined) ? switchId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(switchDirectiveApi, directivePayload, switchLoadPromiseDeferred);
            });
        return switchLoadPromiseDeferred.promise;
    }

    function loadTypeSelector() {
        var typeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        typeReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload = {
                    selectedIds: trunkEntity != undefined ? trunkEntity.Type : undefined

                };
                VRUIUtilsService.callDirectiveLoad(typeDirectiveApi, directivePayload, typeLoadPromiseDeferred);
            });
        return typeLoadPromiseDeferred.promise;
    }

    function loadDirectionSelector() {
        var directionLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        directionReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload = {
                    selectedIds: trunkEntity != undefined ? trunkEntity.Direction : undefined

                };
                VRUIUtilsService.callDirectiveLoad(directionDirectiveApi, directivePayload, directionLoadPromiseDeferred);
            });
        return directionLoadPromiseDeferred.promise;
    }

    function loadLinkSwitchSelector() {
        var linkSwitchLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        linkSwitchReadyPromiseDeferred.promise
            .then(function () {
                var directivePayload = {};
                if (trunkEntity && trunkEntity.SwitchId)
                    directivePayload.filter = { ExcludedIds: [trunkEntity.SwitchId] };

                VRUIUtilsService.callDirectiveLoad(linkSwitchDirectiveApi, directivePayload, linkSwitchLoadPromiseDeferred);
            });
        return linkSwitchLoadPromiseDeferred.promise;
    }

    function loadLinkTrunkSelector() {
        var linkTrunkLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        linkTrunkReadyPromiseDeferred.promise
        .then(function () {
            if (trunkEntity && trunkEntity.LinkedToTrunkId != null) {
                currentlyLinkedToTrunkId = trunkEntity.LinkedToTrunkId;
                return CDRAnalysis_PSTN_TrunkAPIService.GetTrunkById(trunkEntity.LinkedToTrunkId)
                   .then(function (response) {
                       var payload = {
                           selectedIds: response != null && response != undefined ? response.SwitchId : undefined
                       };
                       payload.filter = {
                           ExcludedIds: trunkEntity != null && trunkEntity != undefined ? [trunkEntity.SwitchId] : undefined
                       };

                       linkSwitchDirectiveApi.load(payload).then(function () {
                           var directivePayload = {
                               selectedIds: trunkEntity != undefined ? trunkEntity.LinkedToTrunkId : undefined
                           };
                           directivePayload.filter = {
                               SwitchIds: response != null && response != undefined ? [response.SwitchId] : undefined,
                               TrunkNameFilter: null
                           };
                           directivePayload.excludedId = trunkId;
                           VRUIUtilsService.callDirectiveLoad(linkTrunkDirectiveApi, directivePayload, linkTrunkLoadPromiseDeferred);
                       });

                   })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isGettingData = false;
            });
            }
            else {
                VRUIUtilsService.callDirectiveLoad(linkTrunkDirectiveApi, undefined, linkTrunkLoadPromiseDeferred);
            }
        });
        return linkTrunkLoadPromiseDeferred.promise;
    }
    function updateTrunk() {
        var trunkObj = buildTrunkObjFromScope();
        $scope.isGettingData = true;
        return CDRAnalysis_PSTN_TrunkAPIService.UpdateTrunk(trunkObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch Trunk", response, "Name or Symbol")) {
                    if ($scope.onTrunkUpdated != undefined) {
                        $scope.onTrunkUpdated(response.UpdatedObject, currentlyLinkedToTrunkId, trunkObj.LinkedToTrunkId);
                    }

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
    }

    function insertTrunk() {
        var trunkObj = buildTrunkObjFromScope();
        $scope.isGettingData = true;
        return CDRAnalysis_PSTN_TrunkAPIService.AddTrunk(trunkObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch Trunk", response, "Name or Symbol")) {
                    if ($scope.onTrunkAdded != undefined)
                        $scope.onTrunkAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isGettingData = false;
            });
    }

    function buildTrunkObjFromScope() {
        return {
            TrunkId: trunkId,
            Name: $scope.name,
            Symbol: $scope.symbol,
            SwitchId: switchDirectiveApi.getSelectedIds(),
            Type: typeDirectiveApi.getSelectedIds(),
            Direction: directionDirectiveApi.getSelectedIds(),
            LinkedToTrunkId: linkTrunkDirectiveApi.getSelectedIds()
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_TrunkEditorController", TrunkEditorController);
