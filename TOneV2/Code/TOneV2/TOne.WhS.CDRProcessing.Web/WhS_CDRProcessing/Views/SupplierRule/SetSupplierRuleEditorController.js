(function (appControllers) {

    "use strict";

    setSupplierRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_SetSupplierRuleAPIService'];

    function setSupplierRuleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_SetSupplierRuleAPIService) {

        var editMode;
        var ruleId;
        var carrierAccountDirectiveAPI;
        var supplierRuleData;
        loadParameters();
        defineScope();
        load();
        function loadParameters() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                ruleId = parameters.RuleId
            }
            editMode = (ruleId != undefined);
        }
        function defineScope() {
            $scope.SaveSupplierRule = function () {
                if (editMode) {
                    return updateSupplierRule();
                }
                else {
                    return insertSupplierRule();
                }
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                if (supplierRuleData != undefined)
                    fillScopeFromSupplierRuleObj(supplierRuleData);
                load();
            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.outTrunks = [];
            $scope.outCarriers = [];
            $scope.CDPNs = [];
            $scope.addTrunk = function () {
                $scope.outTrunks.push($scope.outTrunk);
                $scope.outTrunk = undefined;
            }
            $scope.addCarrier = function () {
                $scope.outCarriers.push($scope.outCarrier);
                $scope.outCarrier = undefined;
            }
            $scope.addCDPN = function () {
                $scope.CDPNs.push($scope.CDPN);
                $scope.CDPN = undefined;
            }

            $scope.removeOutTrunk = function (outtrunk) {
                $scope.outTrunks.splice($scope.outTrunks.indexOf(outtrunk), 1);
            }


            $scope.removeOutCarrier = function (outcarrier) {
                $scope.outCarriers.splice($scope.outCarriers.indexOf(outcarrier), 1);
            }


            $scope.removeCDPN = function (cdpn) {
                $scope.CDPNs.splice($scope.CDPNs.indexOf(cdpn), 1);
            }

        }

        function load() {
            $scope.isGettingData = true;
            if (carrierAccountDirectiveAPI == undefined)
                return;
            carrierAccountDirectiveAPI.load();

            if (editMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Edit Supplier Rule");
                getSupplierRule();
            }
            else {
                $scope.title = UtilsService.buildTitleForUpdateEditor("New Supplier Rule");
                $scope.isGettingData = false;
                setDefaultValues();
            }


        }
        function setDefaultValues() {
        }

        function getSupplierRule() {

            return WhS_CDRProcessing_SetSupplierRuleAPIService.GetRule(ruleId).then(function (supplierRule) {
                supplierRuleData = supplierRule;
                fillScopeFromSupplierRuleObj(supplierRule);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
        }



        function buildSupplierRuleObjectObjFromScope() {

            var settings = {
                $type: "TOne.WhS.CDRProcessing.Entities.SetSupplierRuleSettings,TOne.WhS.CDRProcessing.Entities",
                SupplierId: $scope.selectedSupplier.CarrierAccountId
            }
            var criteria = {
                $type: "TOne.WhS.CDRProcessing.Entities.SetSupplierRuleCriteria,TOne.WhS.CDRProcessing.Entities",
                OUT_Trunk: $scope.outTrunks,
                OUT_Carrier: $scope.outCarriers,
                CDPN: $scope.CDPNs

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

        function fillScopeFromSupplierRuleObj(supplierRuleObject) {
            if (carrierAccountDirectiveAPI == undefined)
                return;
            $scope.outTrunks = supplierRuleObject.Criteria.Out_Trunk
            $scope.outCarriers = supplierRuleObject.Criteria.Out_Carrier
            $scope.CDPNs = supplierRuleObject.Criteria.CDPN
            carrierAccountDirectiveAPI.setData(supplierRuleObject.Settings.SupplierId);
            $scope.beginEffectiveDate = supplierRuleObject.BeginEffectiveTime;
            $scope.endEffectiveDate = supplierRuleObject.EndEffectiveTime;
            $scope.description = supplierRuleObject.Description;
            $scope.isGettingData = false;
        }

        function insertSupplierRule() {

            var customerRuleObject = buildSupplierRuleObjectObjFromScope();
            return WhS_CDRProcessing_SetSupplierRuleAPIService.AddRule(customerRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Supplier Rule", response)) {
                    if ($scope.onSetSupplierRuleAdded != undefined)
                        $scope.onSetSupplierRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateSupplierRule() {
            var customerRuleObject = buildSupplierRuleObjectObjFromScope();
            customerRuleObject.RuleId = ruleId;
            WhS_CDRProcessing_SetSupplierRuleAPIService.UpdateRule(customerRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Supplier Rule", response)) {
                    if ($scope.onSetSupplierRuleUpdated != undefined)
                        $scope.onSetSupplierRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_CDRProcessing_SetSupplierRuleEditorController', setSupplierRuleEditorController);
})(appControllers);
