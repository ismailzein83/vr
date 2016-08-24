(function (appControllers) {
    "use strict";

    StatusChargingSetEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_StatusChargingSetAPIService'];

    function StatusChargingSetEditorController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, retailBeStatusChargingSetApiService) {
        var isEditMode;
        var statusChargingSetId;
        var statusChargingEntity;
        var entityTypeAPI;
        var entityTypeSelectorReadyDeferred = utilsService.createPromiseDeferred();

        var chargingSetPeriodDefinitionAPI;
        var chargingSetPeriodDefinitionReadyDeferred = utilsService.createPromiseDeferred();

        var entityTypeSelectedReadyDeferred;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                statusChargingSetId = parameters.statusChargingSetId;
            }
            isEditMode = (statusChargingSetId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.items = [];
            $scope.scopeModel.sectionItems = [];
            $scope.scopeModel.chargingSetPeriodDefinitionDirectiveReady = function(api) {
                chargingSetPeriodDefinitionAPI = api;
                chargingSetPeriodDefinitionReadyDeferred.resolve();
            }
            $scope.scopeModel.save = function() {
                if (isEditMode) {
                    return update();
                } else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function() {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onEntityTypeSelectorReady = function(api) {
                entityTypeAPI = api;
                entityTypeSelectorReadyDeferred.resolve();
            }
            $scope.scopeModel.onEntityTypeSelectionChanged = function () {
                if (entityTypeSelectedReadyDeferred == undefined) {
                    var selectedEntityType = entityTypeAPI.getSelectedIds();
                    if (selectedEntityType != undefined) {
                        $scope.scopeModel.sectionItems.length = 0;
                        getStatusChargeInfos(selectedEntityType).then(function () {
                            for (var j = 0; j < $scope.scopeModel.items.length; j++) {
                                var tempItem = $scope.scopeModel.items[j];
                                addApiToSectionItemObj(tempItem);
                            }
                        });
                    }
                }
            }
        }

        function addApiToSectionItemObj(tempItem) {
            var sectionItem = {
                StatusName: tempItem.StatusName,
                HasInitialCharge: tempItem.HasInitialCharge,
                HasRecurringCharge: tempItem.HasRecurringCharge,
                StatusDefinitionId: tempItem.StatusDefinitionId
            };
            sectionItem.onChargingSetPeriodDefinitionDirectiveReady = function (api) {
                sectionItem.directiveAPI = api;
                var setLoader = function (value) { sectionItem.isLoadingDirective = value };
                vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, sectionItem.directiveAPI, undefined, setLoader);
            };
            $scope.scopeModel.sectionItems.push(sectionItem);
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getStatusChargingSet().then(function () {
                    entityTypeSelectedReadyDeferred = utilsService.createPromiseDeferred();
                    getStatusChargeInfos(statusChargingEntity.Settings.EntityType).then(function () {
                        loadAllControls();
                    });
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function setTitle() {
            if (isEditMode) {
                var statusDefinitionName = (statusChargingEntity != undefined) ? statusChargingEntity.Name : null;
                $scope.title = utilsService.buildTitleForUpdateEditor(statusDefinitionName, 'StatusChargingSet');

            }
            else {
                $scope.title = utilsService.buildTitleForAddEditor('StatusChargingSet');
            }
        }
        function buildStatusChargingSetObjFromScope() {
            var i;
            var statusCharges = [];
            for (i = 0; i < $scope.scopeModel.sectionItems.length; i++) {
                var item = $scope.scopeModel.sectionItems[i];
                var statusCharge = {
                    InitialCharge: item.HasInitialCharge ? item.InitialCharge : 0,
                    StatusDefinitionId: item.StatusDefinitionId,
                    RecurringPeriodSettings: item.directiveAPI.getData()
                }
                statusCharges.push(statusCharge);
            }
            var settings = {
                EntityType: entityTypeAPI.getSelectedIds(),
                StatusCharges: statusCharges
            }

            return {
                StatusChargingSetId: statusChargingEntity != undefined ? statusChargingEntity.StatusChargingSetId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settings
            };
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return retailBeStatusChargingSetApiService.UpdateStatusChargingSet(buildStatusChargingSetObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemUpdated('StatusChargingSet', response, 'Name')) {
                    if ($scope.onStatusChargingSetUpdated != undefined) {
                        $scope.onStatusChargingSetUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function insert() {
            $scope.scopeModel.isLoading = true;
            return retailBeStatusChargingSetApiService.AddStatusChargingSet(buildStatusChargingSetObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemAdded('StatusChargingSet', response, 'Name')) {
                    if ($scope.onStatusChargingSetAdded != undefined)
                        $scope.onStatusChargingSetAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function loadEntityTypeSelector() {

            var entityTypeSelectorLoadDeferred = utilsService.createPromiseDeferred();
            entityTypeSelectorReadyDeferred.promise.then(function () {
                var entityTypeSelectorPayload = null;
                if (isEditMode) {
                    entityTypeSelectorPayload = {
                        selectedIds: statusChargingEntity.Settings.EntityType
                    };
                }
                vruiUtilsService.callDirectiveLoad(entityTypeAPI, entityTypeSelectorPayload, entityTypeSelectorLoadDeferred, chargingSetPeriodDefinitionAPI);
            });
            return entityTypeSelectorLoadDeferred.promise;
        }
        function loadStaticData() {
            if (statusChargingEntity == undefined)
                return;
            $scope.scopeModel.name = statusChargingEntity.Name;
        }

        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadEntityTypeSelector, loadStatusCharges]).then(function() {
                entityTypeSelectedReadyDeferred = undefined;
            }).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadStatusCharges() {
            if (statusChargingEntity == undefined)
                return;
            var promises = [];
            if (statusChargingEntity != undefined && statusChargingEntity.Settings != undefined && statusChargingEntity.Settings.StatusCharges != undefined) {
                for (var i = 0; i < statusChargingEntity.Settings.StatusCharges.length; i++) {
                    var currentStatusChargingEntity = statusChargingEntity.Settings.StatusCharges[i];
                    for (var j = 0; j < $scope.scopeModel.items.length; j++) {
                        var currentItem = $scope.scopeModel.items[j];
                        if (currentStatusChargingEntity.StatusDefinitionId == currentItem.StatusDefinitionId) {
                            var sectionItem = {
                                payload: currentStatusChargingEntity,
                                entityStatusChargeInfo:currentItem,
                                readyPromiseDeferred: utilsService.createPromiseDeferred(),
                                loadPromiseDeferred: utilsService.createPromiseDeferred(),
                            }
                            promises.push(sectionItem.loadPromiseDeferred.promise);
                            AddAPIToSectionItem(sectionItem);

                          //  currentItem.InitialCharge = currentStatusChargingEntity.InitialCharge;

                        }
                    }
                }
            }
            return utilsService.waitMultiplePromises(promises);
        }

        function getStatusChargeInfos(selectedEntityType) {
           return retailBeStatusChargingSetApiService.GetStatusChargeInfos(selectedEntityType).then(function(response) {
                $scope.scopeModel.items = response;
            });
        }

        function AddAPIToSectionItem(sectionItemObj)
        {
            var sectionItem = {
                StatusName: sectionItemObj.entityStatusChargeInfo.StatusName,
                InitialCharge: sectionItemObj.payload.InitialCharge,
                HasInitialCharge: sectionItemObj.entityStatusChargeInfo.HasInitialCharge,
                HasRecurringCharge: sectionItemObj.entityStatusChargeInfo.HasRecurringCharge,
                StatusDefinitionId: sectionItemObj.payload.StatusDefinitionId
            };
            var sectionItemPayload = { RecurringPeriodSettings: sectionItemObj.payload.RecurringPeriodSettings };

            sectionItem.onChargingSetPeriodDefinitionDirectiveReady = function (api) {
                sectionItem.directiveAPI = api;
                sectionItemObj.readyPromiseDeferred.resolve();
            };

            sectionItemObj.readyPromiseDeferred.promise
                .then(function () {
                    vruiUtilsService.callDirectiveLoad(sectionItem.directiveAPI, sectionItemPayload, sectionItemObj.loadPromiseDeferred);
                });
            $scope.scopeModel.sectionItems.push(sectionItem);
        }
        function getStatusChargingSet() {
            return retailBeStatusChargingSetApiService.GetStatusChargingSet(statusChargingSetId).then(function (response) {
                statusChargingEntity = response;
            });
        }
    }
    appControllers.controller('Retail_BE_StatusChargingSetEditorController', StatusChargingSetEditorController);

})(appControllers);