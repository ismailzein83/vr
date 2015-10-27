(function (appControllers) {

    "use strict";

    customerIdentificationRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_CustomerIdentificationRuleAPIService'];

    function customerIdentificationRuleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_CustomerIdentificationRuleAPIService) {

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
            $scope.CDPNPrefixes = [];
            $scope.addTrunk = function () {
                $scope.inTrunks.push($scope.inTrunk);
                $scope.inTrunk = undefined;
            }
            $scope.addCarrier = function () {
                $scope.inCarriers.push($scope.inCarrier);
                $scope.inCarrier = null;
            }
            $scope.addCDPNPrefix = function () {
                $scope.CDPNPrefixes.push($scope.CDPNPrefix);
                $scope.CDPNPrefix = undefined;
            }

            $scope.removeInTrunk = function (intrunk) {
                $scope.inTrunks.splice($scope.inTrunks.indexOf(intrunk), 1);
            }


            $scope.removeInCarrier = function (incarrier) {
                $scope.inCarriers.splice($scope.inCarriers.indexOf(incarrier), 1);
            }


            $scope.removeCDPN = function (cdpn) {
                $scope.CDPNPrefixes.splice($scope.CDPNPrefixes.indexOf(cdpn), 1);
            }
            $scope.onCDPNValueChange = function () {
                $scope.disableAddCDPNPrefixButton = ($scope.CDPNPrefix == null);
            }
            $scope.onInCarrierValueChange = function () {
                $scope.disableInCarrierAddButton = ($scope.inCarrier == null || $scope.inCarrier == undefined);
            }
            $scope.onInTrunkValueChange = function () {
                $scope.disableInTrunkAddButton = ($scope.inTrunk == null);
            }

        }

        function load() {
           $scope.isGettingData = true;
            if (carrierAccountDirectiveAPI == undefined)
                return;
            carrierAccountDirectiveAPI.load();
            
            if (editMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Customer Rule");
                getCustomerRule();
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Customer Rule");
                $scope.isGettingData = false;
                setDefaultValues();
            }


        }
        function setDefaultValues() {
        }

        function getCustomerRule() {

            return WhS_CDRProcessing_CustomerIdentificationRuleAPIService.GetRule(ruleId).then(function (customerRule) {
                customerRuleData = customerRule;
                fillScopeFromCustomerRuleObj(customerRule);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isGettingData = false;
            });
        }

       

        function buildCustomerRuleObjectObjFromScope() {
            
            var settings = {
                CustomerId: $scope.selectedCustomer.CarrierAccountId
            }
            var criteria = {
                IN_Trunks: $scope.inTrunks,
                IN_Carriers:$scope.inCarriers,
                CDPNPrefixes: $scope.CDPNPrefixes

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
            $scope.inTrunks = customerRuleObject.Criteria.IN_Trunks
            $scope.inCarriers = customerRuleObject.Criteria.IN_Carriers
            $scope.CDPNPrefixes = customerRuleObject.Criteria.CDPNPrefixes
            carrierAccountDirectiveAPI.setData(customerRuleObject.Settings.CustomerId);
            $scope.beginEffectiveDate = customerRuleObject.BeginEffectiveTime;
            $scope.endEffectiveDate = customerRuleObject.EndEffectiveTime;
            $scope.description = customerRuleObject.Description;
            $scope.isGettingData = false;
        }

        function insertCustomerRule() {

            var customerRuleObject = buildCustomerRuleObjectObjFromScope();
            return WhS_CDRProcessing_CustomerIdentificationRuleAPIService.AddRule(customerRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Customer Rule", response)) {
                    if ($scope.onCustomerIdentificationRuleAdded != undefined)
                        $scope.onCustomerIdentificationRuleAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateCustomerRule() {
            var customerRuleObject = buildCustomerRuleObjectObjFromScope();
            customerRuleObject.RuleId = ruleId;
            WhS_CDRProcessing_CustomerIdentificationRuleAPIService.UpdateRule(customerRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Customer Rule", response)) {
                    if ($scope.onCustomerIdentificationRuleUpdated != undefined)
                        $scope.onCustomerIdentificationRuleUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_CDRProcessing_CustomerIdentificationRuleEditorController', customerIdentificationRuleEditorController);
})(appControllers);
