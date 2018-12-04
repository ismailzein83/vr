(function (appControllers) {

    "use strict";

    DealBuyRouteRuleEditorController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'WhS_Deal_BuyRouteRuleAPIService', 'VRDateTimeService'];

    function DealBuyRouteRuleEditorController($scope, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService, WhS_Deal_BuyRouteRuleAPIService, VRDateTimeService) {

        var isEditMode;

        var dealBuyRouteRuleId;
        var dealBuyRouteRuleEntity;
        var dealId;
        var dealBED;

        var dealBuyRouteRuleSettingsDirectiveAPI;
        var dealBuyRouteRuleSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var minBED;
        var maxEED;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                dealBuyRouteRuleId = parameters.dealBuyRouteRuleId;
                dealId = parameters.dealId;
                dealBED = parameters.dealBED
            }

            isEditMode = (dealBuyRouteRuleId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.beginEffectiveDate = VRDateTimeService.getCurrentDateWithoutMilliseconds();

            $scope.scopeModel.onDealBuyRouteRuleSettingsDirectiveReady = function (api) {
                dealBuyRouteRuleSettingsDirectiveAPI = api;
                dealBuyRouteRuleSettingsDirectiveReadyDeferred.resolve();
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
                getDealBuyRouteRule().then(function () {
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

        function getDealBuyRouteRule() {
            return WhS_Deal_BuyRouteRuleAPIService.GetDealBuyRouteRule(dealBuyRouteRuleId).then(function (response) {
                dealBuyRouteRuleEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadDealBuyRouteRuleSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var dealBuyRouteRuleName = (dealBuyRouteRuleEntity && dealBuyRouteRuleEntity.Settings) ? dealBuyRouteRuleEntity.Settings.Description : undefined;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(dealBuyRouteRuleName, 'Deal Buy Route Rule');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Deal Buy Route Rule');
                }
            }
            function loadStaticData() {
                if (dealBuyRouteRuleEntity == undefined)
                    return;

                if (dealBuyRouteRuleEntity.Settings != undefined) {
                    $scope.scopeModel.description = dealBuyRouteRuleEntity.Settings.Description;
                    $scope.scopeModel.beginEffectiveDate = dealBuyRouteRuleEntity.Settings.BED;
                    $scope.scopeModel.endEffectiveDate = dealBuyRouteRuleEntity.Settings.EED;
                }
            }
            function loadDealBuyRouteRuleSettingsDirective() {
                var dealBuyRouteRuleSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                dealBuyRouteRuleSettingsDirectiveReadyDeferred.promise.then(function () {

                    var dealBuyRouteRuleSettingsPayload = {
                        dealId: dealId,
                        dealBED: dealBED,
                        dealBuyRouteRuleContext: buildDealBuyRouteRuleContext()
                    };
                    if (dealBuyRouteRuleEntity && dealBuyRouteRuleEntity.Settings) {
                        dealBuyRouteRuleSettingsPayload.dealBuyRouteRuleSettings = dealBuyRouteRuleEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(dealBuyRouteRuleSettingsDirectiveAPI, dealBuyRouteRuleSettingsPayload, dealBuyRouteRuleSettingsLoadDeferred);
                });

                return dealBuyRouteRuleSettingsLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;

            return WhS_Deal_BuyRouteRuleAPIService.AddDealBuyRouteRule(buildDealBuyRouteRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('DealBuyRouteRule', response, 'Settings.Description')) {
                    if ($scope.onDealBuyRouteRuleAdded != undefined)
                        $scope.onDealBuyRouteRuleAdded(response.InsertedObject);
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

            return WhS_Deal_BuyRouteRuleAPIService.UpdateDealBuyRouteRule(buildDealBuyRouteRuleObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('DealBuyRouteRule', response, 'Settings.Description')) {
                    if ($scope.onDealBuyRouteRuleUpdated != undefined) {
                        $scope.onDealBuyRouteRuleUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildDealBuyRouteRuleObjFromScope() {

            var dealBuyRouteRuleSettings = dealBuyRouteRuleSettingsDirectiveAPI.getData();
            if (dealBuyRouteRuleSettings == undefined)
                dealBuyRouteRuleSettings = {};

            dealBuyRouteRuleSettings.DealId = dealId;
            dealBuyRouteRuleSettings.Description = $scope.scopeModel.description;
            dealBuyRouteRuleSettings.BED = $scope.scopeModel.beginEffectiveDate;
            dealBuyRouteRuleSettings.EED = $scope.scopeModel.endEffectiveDate;

            return {
                VRRuleId: dealBuyRouteRuleEntity != undefined ? dealBuyRouteRuleEntity.VRRuleId : undefined,
                Settings: dealBuyRouteRuleSettings
            };
        }

        function buildDealBuyRouteRuleContext() {
            var dealBuyRouteRuleContext = {
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

            return dealBuyRouteRuleContext;
        }
    }

    appControllers.controller('WhS_Deal_BuyRouteRuleEditorController', DealBuyRouteRuleEditorController);
})(appControllers);