(function (appControllers) {

    "use strict";

    supplierIdentificationRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_SupplierIdentificationRuleAPIService','VRValidationService'];

    function supplierIdentificationRuleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_SupplierIdentificationRuleAPIService, VRValidationService) {

        var isEditMode;
        var ruleId;
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierRuleEntity;
        loadParameters();
        defineScope();
        load();
        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                ruleId = parameters.RuleId
            }
            isEditMode = (ruleId != undefined);
        }
        function defineScope() {
            $scope.scopeModal = {};
            $scope.scopeModal.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModal.beginEffectiveDate, $scope.scopeModal.endEffectiveDate);
            }
            $scope.scopeModal.SaveSupplierRule = function () {
                if (isEditMode) {
                    return updateSupplierRule();
                }
                else {
                    return insertSupplierRule();
                }
            };
            $scope.scopeModal.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();

            }
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.scopeModal.outTrunks = [];
            $scope.scopeModal.outCarriers = [];
            $scope.scopeModal.CDPNPrefixes = [];
            $scope.scopeModal.disableOutCarrierAddButton = true;
            $scope.scopeModal.disableAddCDPNPrefixButton = true;
            $scope.scopeModal.disableOutTrunkAddButton = true;
            $scope.scopeModal.addTrunk = function () {
                $scope.scopeModal.outTrunks.push($scope.scopeModal.outTrunk);
                $scope.scopeModal.outTrunk = undefined;
                $scope.scopeModal.disableOutTrunkAddButton = true;
            }
            $scope.scopeModal.addCarrier = function () {
                $scope.scopeModal.outCarriers.push($scope.scopeModal.outCarrier);
                $scope.scopeModal.outCarrier = undefined;
                $scope.scopeModal.disableOutCarrierAddButton = true;
            }
            $scope.scopeModal.addCDPNPrefix = function () {
                $scope.scopeModal.CDPNPrefixes.push($scope.scopeModal.CDPNPrefix);
                $scope.scopeModal.CDPNPrefix = undefined;
                $scope.scopeModal.disableAddCDPNPrefixButton = true;
            }

           
            $scope.scopeModal.onCDPNValueChange = function (value) {
                $scope.scopeModal.disableAddCDPNPrefixButton = value == undefined || UtilsService.contains($scope.scopeModal.CDPNPrefixes, $scope.scopeModal.CDPNPrefix);
            }
            $scope.scopeModal.onOutCarrierValueChange = function (value) {
                $scope.scopeModal.disableOutCarrierAddButton = value == undefined || UtilsService.contains($scope.scopeModal.outCarriers, $scope.scopeModal.outCarrier);
            }
            $scope.scopeModal.onOutTrunkValueChange = function (value) {
                $scope.scopeModal.disableOutTrunkAddButton = value == undefined || UtilsService.contains($scope.scopeModal.outTrunks, $scope.scopeModal.outTrunk);
            }

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Supplier Identification Rule");
                getSupplierRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            customerRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Supplier Identification Rule");
                loadAllControls();
            }

        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadCarrierAccountDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }
        function loadCarrierAccountDirective() {

            var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

            carrierAccountReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: supplierRuleEntity != undefined ? supplierRuleEntity.Settings.SupplierId : undefined
                    }
                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, directivePayload, loadCarrierAccountPromiseDeferred);
                });

            return loadCarrierAccountPromiseDeferred.promise;
        }
        function setDefaultValues() {
        }

        function getSupplierRule() {
            return WhS_CDRProcessing_SupplierIdentificationRuleAPIService.GetRule(ruleId).then(function (supplierRule) {
                supplierRuleEntity = supplierRule;
            });
        }



        function buildSupplierRuleObjectObjFromScope() {

            var settings = {
                SupplierId: $scope.scopeModal.selectedSupplier.CarrierAccountId
            }
            var criteria = {
                OutTrunks: $scope.scopeModal.outTrunks,
                OutCarriers: $scope.scopeModal.outCarriers,
                CDPNPrefixes: $scope.scopeModal.CDPNPrefixes

            }
            var supplierRule = {
                Settings: settings,
                Description: $scope.scopeModal.description,
                Criteria: criteria,
                BeginEffectiveTime: $scope.scopeModal.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModal.endEffectiveDate
            }
            return supplierRule;
        }

        function loadFilterBySection() {
            if (supplierRuleEntity != undefined) {
                $scope.scopeModal.outTrunks = supplierRuleEntity.Criteria.OutTrunks
                $scope.scopeModal.outCarriers = supplierRuleEntity.Criteria.OutCarriers
                $scope.scopeModal.CDPNPrefixes = supplierRuleEntity.Criteria.CDPNPrefixes;
                $scope.scopeModal.beginEffectiveDate = supplierRuleEntity.BeginEffectiveTime;
                $scope.scopeModal.endEffectiveDate = supplierRuleEntity.EndEffectiveTime;
                $scope.scopeModal.description = supplierRuleEntity.Description;
            }
        }

        function insertSupplierRule() {

            var supplierRuleObject = buildSupplierRuleObjectObjFromScope();
            return WhS_CDRProcessing_SupplierIdentificationRuleAPIService.AddRule(supplierRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Supplier Rule", response)) {
                    if ($scope.onSupplierIdentificationRuleAdded != undefined)
                        $scope.onSupplierIdentificationRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateSupplierRule() {
            var supplierRuleObject = buildSupplierRuleObjectObjFromScope();
            supplierRuleObject.RuleId = ruleId;
            WhS_CDRProcessing_SupplierIdentificationRuleAPIService.UpdateRule(supplierRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Supplier Rule", response)) {
                    if ($scope.onSupplierIdentificationRuleUpdated != undefined)
                        $scope.onSupplierIdentificationRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_CDRProcessing_SupplierIdentificationRuleEditorController', supplierIdentificationRuleEditorController);
})(appControllers);
