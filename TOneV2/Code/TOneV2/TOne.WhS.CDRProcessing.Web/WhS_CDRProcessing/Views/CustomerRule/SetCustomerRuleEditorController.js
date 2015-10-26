(function (appControllers) {

    "use strict";

    setCustomerRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_SetCustomerRuleAPIService'];

    function setCustomerRuleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_SetCustomerRuleAPIService) {

        var editMode;
        var ruleId;
        var carrierAccountDirectiveAPI;
       var customerRuleData;
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
            $scope.SaveCustomerRule = function () {
                if (editMode) {
                    return updateCustomerRule();
                }
                else {
                    return insertCustomerRule();
                }
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                if (customerRuleData != undefined)
                    fillScopeFromCustomerRuleObj(customerRuleData);
                load();
            }
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.inTrunks = [];
            $scope.inCarriers = [];
            $scope.CDPNs = [];
            $scope.addTrunk = function () {
                $scope.inTrunks.push($scope.inTrunk);
                $scope.inTrunk = undefined;
            }
            $scope.addCarrier = function () {
                $scope.inCarriers.push($scope.inCarrier);
                $scope.inCarrier = undefined;
            }
            $scope.addCDPN = function () {
                $scope.CDPNs.push($scope.CDPN);
                $scope.CDPN = undefined;
            }

            $scope.removeInTrunk = function (intrunk) {
                $scope.inTrunks.splice($scope.inTrunks.indexOf(intrunk), 1);
            }


            $scope.removeInCarrier = function (incarrier) {
                $scope.inCarriers.splice($scope.inCarriers.indexOf(incarrier), 1);
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
                $scope.title = UtilsService.buildTitleForUpdateEditor("Edit Customer Rule");
                getCustomerRule();
            }
            else {
                $scope.title = UtilsService.buildTitleForUpdateEditor("New Customer Rule");
                $scope.isGettingData = false;
                setDefaultValues();
            }


        }
        function setDefaultValues() {
        }

        function getCustomerRule() {

            return WhS_CDRProcessing_SetCustomerRuleAPIService.GetRule(ruleId).then(function (customerRule) {
                customerRuleData = customerRule;
                fillScopeFromCustomerRuleObj(customerRule);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
        }

       

        function buildCustomerRuleObjectObjFromScope() {
            
            var settings = {
                $type: "TOne.WhS.CDRProcessing.Entities.SetCustomerRuleSettings,TOne.WhS.CDRProcessing.Entities",
                CustomerId: $scope.selectedCustomer.CarrierAccountId
            }
            var criteria = {
                $type: "TOne.WhS.CDRProcessing.Entities.SetCustomerRuleCriteria,TOne.WhS.CDRProcessing.Entities",
                IN_Trunk: $scope.inTrunks,
                IN_Carrier:$scope.inCarriers,
                CDPN:$scope.CDPNs

            }
            var customerRule = {
                Settings: settings,
                Description: $scope.description,
                Criteria: criteria,
                BeginEffectiveTime: $scope.beginEffectiveDate,
                EndEffectiveTime: $scope.endEffectiveDate
            }
           
            return customerRule;
        }

        function fillScopeFromCustomerRuleObj(customerRuleObject) {
            if (carrierAccountDirectiveAPI == undefined)
                return;
            $scope.inTrunks = customerRuleObject.Criteria.IN_Trunk
            $scope.inCarriers = customerRuleObject.Criteria.IN_Carrier
            $scope.CDPNs = customerRuleObject.Criteria.CDPN
            carrierAccountDirectiveAPI.setData(customerRuleObject.Settings.CustomerId);
            $scope.beginEffectiveDate = customerRuleObject.BeginEffectiveTime;
            $scope.endEffectiveDate = customerRuleObject.EndEffectiveTime;
            $scope.description = customerRuleObject.Description;
            $scope.isGettingData = false;
        }

        function insertCustomerRule() {

            var customerRuleObject = buildCustomerRuleObjectObjFromScope();
            return WhS_CDRProcessing_SetCustomerRuleAPIService.AddRule(customerRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Customer Rule", response)) {
                    if ($scope.onSetCustomerRuleAdded != undefined)
                        $scope.onSetCustomerRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateCustomerRule() {
            var customerRuleObject = buildCustomerRuleObjectObjFromScope();
            customerRuleObject.RuleId = ruleId;
            WhS_CDRProcessing_SetCustomerRuleAPIService.UpdateRule(customerRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Customer Rule", response)) {
                    if ($scope.onSetCustomerRuleUpdated != undefined)
                        $scope.onSetCustomerRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_CDRProcessing_SetCustomerRuleEditorController', setCustomerRuleEditorController);
})(appControllers);
