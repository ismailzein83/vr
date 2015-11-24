(function (appControllers) {

    "use strict";

    customerIdentificationRuleEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_CustomerIdentificationRuleAPIService','VRValidationService'];

    function customerIdentificationRuleEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_CustomerIdentificationRuleAPIService, VRValidationService) {

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
            $scope.scopeModal = {};
            $scope.scopeModal.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModal.beginEffectiveDate, $scope.scopeModal.endEffectiveDate);
            }
            $scope.scopeModal.SaveCustomerRule = function () {
                if (isEditMode) {
                    return updateCustomerRule();
                }
                else {
                    return insertCustomerRule();
                }
            };
            $scope.scopeModal.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                carrierAccountReadyPromiseDeferred.resolve();

            }
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal()
            };
            $scope.scopeModal.inTrunks = [];
            $scope.scopeModal.inCarriers = [];
            $scope.scopeModal.CDPNPrefixes = [];
            $scope.scopeModal.disableInCarrierAddButton = true;
            $scope.scopeModal.disableAddCDPNPrefixButton = true;
            $scope.scopeModal.disableInTrunkAddButton = true;

            $scope.scopeModal.addTrunk = function () {
                $scope.scopeModal.inTrunks.push($scope.scopeModal.inTrunk);
                $scope.scopeModal.inTrunk = undefined;
                $scope.scopeModal.disableInTrunkAddButton = true;
            }
            $scope.scopeModal.addCarrier = function () {

                $scope.scopeModal.inCarriers.push($scope.scopeModal.inCarrier);
                $scope.scopeModal.inCarrier = undefined;
                $scope.scopeModal.disableInCarrierAddButton = true;
            }
            $scope.scopeModal.addCDPNPrefix = function () {
              
                    $scope.scopeModal.CDPNPrefixes.push($scope.scopeModal.CDPNPrefix);
                    $scope.scopeModal.CDPNPrefix = undefined;
                    $scope.scopeModal.disableAddCDPNPrefixButton = true;
            }
            $scope.scopeModal.onCDPNValueChange = function (value) {
                $scope.scopeModal.disableAddCDPNPrefixButton = value == undefined|| UtilsService.contains($scope.scopeModal.CDPNPrefixes, $scope.scopeModal.CDPNPrefix);
            }
            $scope.scopeModal.onInCarrierValueChange = function (value) {
                $scope.scopeModal.disableInCarrierAddButton = value == undefined || UtilsService.contains($scope.scopeModal.inCarriers, $scope.scopeModal.inCarrier);
            }
            $scope.scopeModal.onInTrunkValueChange = function (value) {
                $scope.scopeModal.disableInTrunkAddButton = value == undefined || UtilsService.contains($scope.scopeModal.inTrunks, $scope.scopeModal.inTrunk);
            }

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Customer Rule");
                getCustomerRule().then(function () {
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
                   $scope.scopeModal.isLoading = false;
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
                $scope.scopeModal.inTrunks = customerRuleEntity.Criteria.InTrunks
                $scope.scopeModal.inCarriers = customerRuleEntity.Criteria.InCarriers
                $scope.scopeModal.CDPNPrefixes = customerRuleEntity.Criteria.CDPNPrefixes
                $scope.scopeModal.beginEffectiveDate = customerRuleEntity.BeginEffectiveTime;
                $scope.scopeModal.endEffectiveDate = customerRuleEntity.EndEffectiveTime;
                $scope.scopeModal.description = customerRuleEntity.Description;
            }
        }

        function getCustomerRule() {
            return WhS_CDRProcessing_CustomerIdentificationRuleAPIService.GetRule(ruleId).then(function (customerRule) {
                customerRuleEntity = customerRule;
            });
        }

        function buildCustomerRuleObjectObjFromScope() {
            
            var settings = {
                CustomerId: $scope.scopeModal.selectedCustomer.CarrierAccountId
            }
            var criteria = {
                InTrunks: $scope.scopeModal.inTrunks,
                InCarriers: $scope.scopeModal.inCarriers,
                CDPNPrefixes: $scope.scopeModal.CDPNPrefixes

            }
            var customerRule = {
                Settings: settings,
                Description: $scope.scopeModal.description,
                Criteria: criteria,
                BeginEffectiveTime: $scope.scopeModal.beginEffectiveDate,
                EndEffectiveTime: $scope.scopeModal.endEffectiveDate
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
