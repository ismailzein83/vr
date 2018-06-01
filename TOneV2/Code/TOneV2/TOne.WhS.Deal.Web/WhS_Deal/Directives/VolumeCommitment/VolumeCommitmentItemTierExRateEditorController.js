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

        var rateEvaluatorSelectiveDirectiveAPI;
        var rateEvaluatorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parametersObj = VRNavigationService.getParameters($scope);
            if (parametersObj != undefined) {
                exRateEntity = parametersObj.exRateEntity;
                context = parametersObj.context;
                if (context != undefined) {
                    $scope.scopeModel.zoneSelector = context.getZoneSelector();
                    $scope.scopeModel.rateEvaluatorSelective = context.getRateEvaluatorSelective();
                }
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
            $scope.scopeModel.onrateEvaluatorSelectiveReady = function (api) {
                rateEvaluatorSelectiveDirectiveAPI = api;
                rateEvaluatorReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadZoneSection, loadRateEvaluatorSelectiveDirective]).then(function () {
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadRateEvaluatorSelectiveDirective() {
            var loadREWSelectiveDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            rateEvaluatorReadyPromiseDeferred.promise.then(function () {

                var payload = undefined;
                if (exRateEntity != undefined && exRateEntity.EvaluatedRate != undefined) {
                    payload =
                    {
                        evaluatedRate: exRateEntity.EvaluatedRate
                    };
                }
                VRUIUtilsService.callDirectiveLoad(rateEvaluatorSelectiveDirectiveAPI, payload, loadREWSelectiveDirectivePromiseDeferred);
            });
            return loadREWSelectiveDirectivePromiseDeferred.promise;
        }


        function setTitle() {
            if (isEditMode && exRateEntity != undefined)
                $scope.title = 'Edit Exception Rate';
            else
                $scope.title = 'Add Exception Rate';
        }


        function loadZoneSection() {
            var loadZonePromiseDeferred = UtilsService.createPromiseDeferred();
            zoneReadyPromiseDeferred.promise.then(function () {
                var payload = context != undefined ? context.getZoneSelectorPayload(exRateEntity) : undefined;
                var excZoneIds = exRateEntity != undefined ? UtilsService.getPropValuesFromArray(exRateEntity.Zones, "ZoneId") : undefined;
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
            var zones = [];
            var zoneIds = zoneDirectiveAPI.getSelectedIds();
            for (var j = 0; j < zoneIds.length; j++) {
                zones.push(
                {
                    ZoneId: zoneIds[j]
                });
            }
            return {
                Zones: zones,
                ZoneNames: context.getZonesNames(zoneDirectiveAPI.getSelectedIds()),
                Rate: $scope.scopeModel.rate,
                EvaluatedRate: rateEvaluatorSelectiveDirectiveAPI.getData(),
                Description: rateEvaluatorSelectiveDirectiveAPI.getDescription()
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