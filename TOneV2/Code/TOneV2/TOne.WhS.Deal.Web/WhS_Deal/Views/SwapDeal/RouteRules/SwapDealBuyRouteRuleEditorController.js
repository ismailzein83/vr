(function (appControllers) {

    "use strict";

    SwapDealBuyRouteRuleEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'WhS_Deal_SwapDealBuyRouteRuleAPIService'];

    function SwapDealBuyRouteRuleEditorController($scope, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService, WhS_Deal_SwapDealBuyRouteRuleAPIService) {

        var isEditMode;

        var swapDealBuyRouteRuleId;
        var swapDealBuyRouteRuleEntity;
        var swapDealId;

        var swapDealBuyRouteRuleSettingsDirectiveAPI;
        var swapDealBuyRouteRuleSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var minBED;
        var maxEED;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                swapDealBuyRouteRuleId = parameters.swapDealBuyRouteRuleId;
                swapDealId = parameters.swapDealId;
            }

            isEditMode = (swapDealBuyRouteRuleId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSwapDealBuyRouteRuleSettingsDirectiveReady = function (api) {
                swapDealBuyRouteRuleSettingsDirectiveAPI = api;
                swapDealBuyRouteRuleSettingsDirectiveReadyDeferred.resolve();
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
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.validateBED = function () {
                if ($scope.scopeModel.beginEffectiveDate != undefined && $scope.scopeModel.endEffectiveDate != undefined) {
                    var validationResult = UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
                    if (validationResult != null)
                        return validationResult;
                }

                var bed = $scope.scopeModel.beginEffectiveDate;
                if (bed != undefined && !(bed instanceof Date))
                    bed = UtilsService.createDateFromString(bed);

                if (minBED != undefined && bed != undefined && bed < minBED) {
                    return 'BED should be greater than ' + UtilsService.dateToServerFormat(minBED);
                }
                return null;
            };

            $scope.scopeModel.validateEED = function () {
                if ($scope.scopeModel.beginEffectiveDate != undefined && $scope.scopeModel.endEffectiveDate != undefined) {
                    var validationResult = UtilsService.validateDates($scope.scopeModel.beginEffectiveDate, $scope.scopeModel.endEffectiveDate);
                    if (validationResult != null)
                        return validationResult;
                }

                var eed = $scope.scopeModel.endEffectiveDate;
                if (eed != undefined && !(eed instanceof Date))
                    eed = UtilsService.createDateFromString(eed);

                if (maxEED != undefined && (eed == undefined || eed > maxEED)) {
                    return 'EED should be less than ' + UtilsService.dateToServerFormat(maxEED);
                }
                return null;
            };

        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getSwapDealBuyRouteRule().then(function () {
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

        function getSwapDealBuyRouteRule() {
            return WhS_Deal_SwapDealBuyRouteRuleAPIService.GetSwapDealBuyRouteRule(swapDealBuyRouteRuleId).then(function (response) {
                swapDealBuyRouteRuleEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSwapDealBuyRouteRuleSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var swapDealBuyRouteRuleName = (swapDealBuyRouteRuleEntity && swapDealBuyRouteRuleEntity.Settings) ? swapDealBuyRouteRuleEntity.Settings.Description : undefined;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(swapDealBuyRouteRuleName, 'Swap Deal Buy Route Rule');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Swap Deal Buy Route Rule');
                }
            }
            function loadStaticData() {
                if (swapDealBuyRouteRuleEntity == undefined)
                    return;

                if (swapDealBuyRouteRuleEntity.Settings != undefined) {
                    $scope.scopeModel.description = swapDealBuyRouteRuleEntity.Settings.Description;
                    $scope.scopeModel.beginEffectiveDate = swapDealBuyRouteRuleEntity.Settings.BED;
                    $scope.scopeModel.endEffectiveDate = swapDealBuyRouteRuleEntity.Settings.EED;
                }
            }
            function loadSwapDealBuyRouteRuleSettingsDirective() {
                var swapDealBuyRouteRuleSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                swapDealBuyRouteRuleSettingsDirectiveReadyDeferred.promise.then(function () {

                    var swapDealBuyRouteRuleSettingsPayload = {
                        swapDealId: swapDealId,
                        swapDealBuyRouteRuleContext: buildSwapDealBuyRouteRuleContext()
                    };
                    if (swapDealBuyRouteRuleEntity && swapDealBuyRouteRuleEntity.Settings) {
                        swapDealBuyRouteRuleSettingsPayload.swapDealBuyRouteRuleSettings = swapDealBuyRouteRuleEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(swapDealBuyRouteRuleSettingsDirectiveAPI, swapDealBuyRouteRuleSettingsPayload, swapDealBuyRouteRuleSettingsLoadDeferred);
                });

                return swapDealBuyRouteRuleSettingsLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;

            return WhS_Deal_SwapDealBuyRouteRuleAPIService.AddSwapDealBuyRouteRule(buildSwapDealBuyRouteRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('SwapDealBuyRouteRule', response, 'Settings.Description')) {
                    if ($scope.onSwapDealBuyRouteRuleAdded != undefined)
                        $scope.onSwapDealBuyRouteRuleAdded(response.InsertedObject);
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

            return WhS_Deal_SwapDealBuyRouteRuleAPIService.UpdateSwapDealBuyRouteRule(buildSwapDealBuyRouteRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('SwapDealBuyRouteRule', response, 'Settings.Description')) {
                    if ($scope.onSwapDealBuyRouteRuleUpdated != undefined) {
                        $scope.onSwapDealBuyRouteRuleUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSwapDealBuyRouteRuleObjFromScope() {

            var swapDealBuyRouteRuleSettings = swapDealBuyRouteRuleSettingsDirectiveAPI.getData();
            if (swapDealBuyRouteRuleSettings == undefined)
                swapDealBuyRouteRuleSettings = {};

            swapDealBuyRouteRuleSettings.SwapDealId = swapDealId;
            swapDealBuyRouteRuleSettings.Description = $scope.scopeModel.description;
            swapDealBuyRouteRuleSettings.BED = $scope.scopeModel.beginEffectiveDate;
            swapDealBuyRouteRuleSettings.EED = $scope.scopeModel.endEffectiveDate;

            return {
                VRRuleId: swapDealBuyRouteRuleEntity != undefined ? swapDealBuyRouteRuleEntity.VRRuleId : undefined,
                Settings: swapDealBuyRouteRuleSettings
            };
        }

        function buildSwapDealBuyRouteRuleContext() {
            var swapDealBuyRouteRuleContext = {
                setTimeSettings: function (bed, eed) {
                    if (bed != undefined) {
                        minBED = UtilsService.createDateFromString(bed);
                        if (!isEditMode) {
                            $scope.scopeModel.beginEffectiveDate = minBED;
                        }
                    }

                    if (eed != undefined) {
                        maxEED = UtilsService.createDateFromString(eed);
                        if (!isEditMode) {
                            $scope.scopeModel.endEffectiveDate = maxEED;
                        }
                    }
                }
            };

            return swapDealBuyRouteRuleContext;
        }
    }

    appControllers.controller('WhS_Deal_SwapDealBuyRouteRuleEditorController', SwapDealBuyRouteRuleEditorController);

})(appControllers);