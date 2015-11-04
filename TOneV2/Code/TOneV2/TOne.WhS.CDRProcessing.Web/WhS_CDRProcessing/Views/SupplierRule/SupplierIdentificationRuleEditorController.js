(function (appControllers) {

    "use strict";

    supplierIdentificationRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_SupplierIdentificationRuleAPIService'];

    function supplierIdentificationRuleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_SupplierIdentificationRuleAPIService) {

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
            $scope.SaveSupplierRule = function () {
                if (isEditMode) {
                    return updateSupplierRule();
                }
                else {
                    return insertSupplierRule();
                }
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();

            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.outTrunks = [];
            $scope.outCarriers = [];
            $scope.CDPNPrefixes = [];
            $scope.addTrunk = function () {
                $scope.outTrunks.push($scope.outTrunk);
                $scope.outTrunk = undefined;
            }
            $scope.addCarrier = function () {
                $scope.outCarriers.push($scope.outCarrier);
                $scope.outCarrier = undefined;
            }
            $scope.addCDPNPrefix = function () {
                $scope.CDPNPrefixes.push($scope.CDPNPrefix);
                $scope.CDPNPrefix = undefined;
            }

            $scope.removeOutTrunk = function (outtrunk) {
                $scope.outTrunks.splice($scope.outTrunks.indexOf(outtrunk), 1);
            }


            $scope.removeOutCarrier = function (outcarrier) {
                $scope.outCarriers.splice($scope.outCarriers.indexOf(outcarrier), 1);
            }


            $scope.removeCDPN = function (cdpn) {
                $scope.CDPNPrefixes.splice($scope.CDPNPrefixes.indexOf(cdpn), 1);
            }
            $scope.onCDPNValueChange = function () {
                $scope.disableAddCDPNPrefixButton = ($scope.CDPNPrefix == null);
            }
            $scope.onOutCarrierValueChange = function () {
                $scope.disableOutCarrierAddButton = ($scope.outCarrier == null);
            }
            $scope.onOutTrunkValueChange = function () {
                $scope.disableOutTrunkAddButton = ($scope.outTrunk == null);
            }

        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Supplier Rule");
                getSupplierRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            customerRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Supplier Rule");
                loadAllControls();
            }

        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadCarrierAccountDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
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
                SupplierId: $scope.selectedSupplier.CarrierAccountId
            }
            var criteria = {
                OUT_Trunks: $scope.outTrunks,
                OUT_Carriers: $scope.outCarriers,
                CDPNPrefixes: $scope.CDPNPrefixes

            }
            var supplierRule = {
                Settings: settings,
                Description: $scope.description,
                Criteria: criteria,
                BeginEffectiveTime: $scope.beginEffectiveDate,
                EndEffectiveTime: $scope.endEffectiveDate
            }

            return supplierRule;
        }

        function loadFilterBySection() {
            if (supplierRuleEntity != undefined) {
                $scope.outTrunks = supplierRuleEntity.Criteria.Out_Trunks
                $scope.outCarriers = supplierRuleEntity.Criteria.Out_Carriers
                $scope.CDPNPrefixes = supplierRuleEntity.Criteria.CDPNPrefixes;
                $scope.beginEffectiveDate = supplierRuleEntity.BeginEffectiveTime;
                $scope.endEffectiveDate = supplierRuleEntity.EndEffectiveTime;
                $scope.description = supplierRuleEntity.Description;
            }
        }

        function insertSupplierRule() {

            var customerRuleObject = buildSupplierRuleObjectObjFromScope();
            return WhS_CDRProcessing_SupplierIdentificationRuleAPIService.AddRule(customerRuleObject)
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
            var customerRuleObject = buildSupplierRuleObjectObjFromScope();
            customerRuleObject.RuleId = ruleId;
            WhS_CDRProcessing_SupplierIdentificationRuleAPIService.UpdateRule(customerRuleObject)
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
