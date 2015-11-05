(function (appControllers) {

    "use strict";

    customerIdentificationRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_CustomerIdentificationRuleAPIService'];

    function customerIdentificationRuleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_CustomerIdentificationRuleAPIService) {

        var isEditMode;
        var ruleId;
        var carrierAccountDirectiveAPI;
        var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var customerRuleEntity;
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
            $scope.SaveCustomerRule = function () {
                if (isEditMode) {
                    return updateCustomerRule();
                }
                else {
                    return insertCustomerRule();
                }
            };
            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();

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
            $scope.isLoading = true;
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Customer Rule");
                getCustomerRule().then(function () {
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
                $scope.title = UtilsService.buildTitleForAddEditor("Customer Rule");
                loadAllControls();
            }

        }
        function setDefaultValues() {
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
                        selectedIds: customerRuleEntity != undefined ? customerRuleEntity.Settings.CustomerId : undefined
                    }
                    VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, directivePayload, loadCarrierAccountPromiseDeferred);
                });

            return loadCarrierAccountPromiseDeferred.promise;
        }
        function loadFilterBySection() {
            if (customerRuleEntity != undefined) {
                $scope.inTrunks = customerRuleEntity.Criteria.InTrunks
                $scope.inCarriers = customerRuleEntity.Criteria.InCarriers
                $scope.CDPNPrefixes = customerRuleEntity.Criteria.CDPNPrefixes
                $scope.beginEffectiveDate = customerRuleEntity.BeginEffectiveTime;
                $scope.endEffectiveDate = customerRuleEntity.EndEffectiveTime;
                $scope.description = customerRuleEntity.Description;
            }
        }

        function getCustomerRule() {
            return WhS_CDRProcessing_CustomerIdentificationRuleAPIService.GetRule(ruleId).then(function (customerRule) {
                customerRuleEntity = customerRule;
            });
        }

        function buildCustomerRuleObjectObjFromScope() {
            
            var settings = {
                CustomerId: $scope.selectedCustomer.CarrierAccountId
            }
            var criteria = {
                InTrunks: $scope.inTrunks,
                InCarriers: $scope.inCarriers,
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
