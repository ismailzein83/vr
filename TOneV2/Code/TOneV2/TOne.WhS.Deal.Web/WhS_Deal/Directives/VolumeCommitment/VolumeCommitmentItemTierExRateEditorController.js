(function (app) {

    'use strict';

    VolumeCommitmentItemTierExRateEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function VolumeCommitmentItemTierExRateEditorController($scope, VRNavigationService, UtilsService, VRNotificationService, VRUIUtilsService) {

        var exRateEntity;
        var tiers;
        var context;
        var isEditMode;
        var zoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var zoneDirectiveAPI;

        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parametersObj = VRNavigationService.getParameters($scope);
            if (parametersObj != undefined) {
                exRateEntity = parametersObj.exRateEntity;
                context = parametersObj.context;
                if (context != undefined) 
                    $scope.scopeModel.zoneSelector = context.getZoneSelector();
            }
            isEditMode = (exRateEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateExRateItem() : addExRateItem();
            };
            $scope.scopeModel.onZoneSelectorReady = function (api) {
                zoneDirectiveAPI = api;
                zoneReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadZoneSection]).then(function () {
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }



      
        function setTitle() {
            if (isEditMode && exRateEntity != undefined)
                $scope.title = 'Edit Exception Rate';
            else
                $scope.title = 'Add Exception Rate';
        }

        function loadStaticData() {
            if (exRateEntity == undefined)
                return;
            $scope.scopeModel.rate = exRateEntity.Rate;
        }
    
        function loadZoneSection() {
            var loadZonePromiseDeferred = UtilsService.createPromiseDeferred();
            zoneReadyPromiseDeferred.promise.then(function () {
                var payload = context != undefined ? context.getZoneSelectorPayload(exRateEntity) : undefined;
                var excZoneIds = exRateEntity != undefined ? exRateEntity.ZoneIds : undefined;
                payload.filter = {
                    AvailableZoneIds: context.getSelectedZonesIds(),
                    ExcludedZoneIds: context.getExceptionsZoneIds(excZoneIds),
                    CountryIds: [context.getCountryId()]
                };
                VRUIUtilsService.callDirectiveLoad(zoneDirectiveAPI, payload, loadZonePromiseDeferred);
            });
            return loadZonePromiseDeferred.promise;
        }

        function builObjFromScope() {
            return {
                ZoneIds: zoneDirectiveAPI.getSelectedIds(),
                ZoneNames: context.getZonesNames(zoneDirectiveAPI.getSelectedIds()),
                Rate: $scope.scopeModel.rate
            };
        }

        function addExRateItem() {
            var parameterObj = builObjFromScope();
            if ($scope.onVolumeCommitmentItemTierExRateAdded != undefined) {
                $scope.onVolumeCommitmentItemTierExRateAdded(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

        function updateExRateItem() {
            var parameterObj = builObjFromScope();
            if ($scope.onVolumeCommitmentItemTierExRateUpdated != undefined) {
                $scope.onVolumeCommitmentItemTierExRateUpdated(parameterObj);
            }
            $scope.modalContext.closeModal();
        }

    }
    app.controller('WhS_Deal_VolumeCommitmentItemTierExRateEditorController', VolumeCommitmentItemTierExRateEditorController);

})(app);