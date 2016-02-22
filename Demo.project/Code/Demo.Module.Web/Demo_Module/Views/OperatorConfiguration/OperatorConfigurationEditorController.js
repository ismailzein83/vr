(function (appControllers) {

    "use strict";

    operatorConfigurationEditorController.$inject = ['$scope', 'Demo_OperatorConfigurationAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService'];

    function operatorConfigurationEditorController($scope, Demo_OperatorConfigurationAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService) {
        var isEditMode;
        var operatorConfigurationId;
        var operatorConfigurationEntity;

        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var serviceTypeDirectiveAPI;
        var serviceTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var unitTypeDirectiveAPI;
        var unitTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cdrDirectionDirectiveAPI;
        var cdrDirectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cdrTypeDirectiveAPI;
        var cdrTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        var currencyDirectiveAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                operatorConfigurationId = parameters.OperatorConfigurationId;
            }

            isEditMode = (operatorConfigurationId != undefined);

        }

        function defineScope() {

            $scope.scopeModal = {
            };


            $scope.selectedCurrency;

            $scope.scopeModal.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();

            }


            $scope.onCurrencyDirectiveReady = function (api) {
                currencyDirectiveAPI = api;
                currencyReadyPromiseDeferred.resolve();
            }


            $scope.onServiceTypeReady = function (api) {
                serviceTypeDirectiveAPI = api;
                serviceTypeReadyPromiseDeferred.resolve();
            }

            $scope.onUnitTypeReady = function (api) {
                unitTypeDirectiveAPI = api;
                unitTypeReadyPromiseDeferred.resolve();
            }

            $scope.onCDRDirectionReady = function (api) {
                cdrDirectionDirectiveAPI = api;
                cdrDirectionReadyPromiseDeferred.resolve();
            }

            $scope.onCDRTypeReady = function (api) {
                cdrTypeDirectiveAPI = api;
                cdrTypeReadyPromiseDeferred.resolve();
            }

            $scope.SaveOperatorConfiguration = function () {
                if (isEditMode) {
                    return updateOperatorConfiguration();
                }
                else {
                    return insertOperatorConfiguration();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
        }

        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getOperatorConfiguration().then(function () {
                    loadAllControls()
                        .finally(function () {
                            operatorConfigurationEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getOperatorConfiguration() {
            return Demo_OperatorConfigurationAPIService.GetOperatorConfiguration(operatorConfigurationId).then(function (operatorconfiguration) {
                operatorConfigurationEntity = operatorconfiguration;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadOperatorProfileDirective, loadServiceTypes, loadUnitTypes, loadCDRTypes, loadCDRDirections, loadCurrencies])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadServiceTypes() {
            var loadServiceTypesPromiseDeferred = UtilsService.createPromiseDeferred();
            serviceTypeReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: (operatorConfigurationEntity != undefined ? (operatorConfigurationEntity.AmountType != undefined ? [operatorConfigurationEntity.AmountType] : undefined) : (operatorConfigurationId != undefined ? operatorConfigurationId : undefined))
                }
                VRUIUtilsService.callDirectiveLoad(serviceTypeDirectiveAPI, directivePayload, loadServiceTypesPromiseDeferred);
            });
            return loadServiceTypesPromiseDeferred.promise;
        }


        function loadUnitTypes() {
            var loadUnitTypesPromiseDeferred = UtilsService.createPromiseDeferred();
            unitTypeReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: (operatorConfigurationEntity != undefined ? (operatorConfigurationEntity.UnitType != undefined ? [operatorConfigurationEntity.UnitType] : undefined) : (operatorConfigurationId != undefined ? operatorConfigurationId : undefined))
                }
                VRUIUtilsService.callDirectiveLoad(unitTypeDirectiveAPI, directivePayload, loadUnitTypesPromiseDeferred);
            });
            return loadUnitTypesPromiseDeferred.promise;
        }


        function loadCDRTypes() {
            var loadCDRTypesPromiseDeferred = UtilsService.createPromiseDeferred();
            cdrTypeReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: (operatorConfigurationEntity != undefined ? (operatorConfigurationEntity.CDRType != undefined ? [operatorConfigurationEntity.CDRType] : undefined) : (operatorConfigurationId != undefined ? operatorConfigurationId : undefined))
                }
                VRUIUtilsService.callDirectiveLoad(cdrTypeDirectiveAPI, directivePayload, loadCDRTypesPromiseDeferred);
            });
            return loadCDRTypesPromiseDeferred.promise;
        }

        function loadCDRDirections() {
            var loadCDRDirectionsPromiseDeferred = UtilsService.createPromiseDeferred();
            cdrDirectionReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: (operatorConfigurationEntity != undefined ? (operatorConfigurationEntity.CDRDirection != undefined ? [operatorConfigurationEntity.CDRDirection] : undefined) : (operatorConfigurationId != undefined ? operatorConfigurationId : undefined))
                }
                VRUIUtilsService.callDirectiveLoad(cdrDirectionDirectiveAPI, directivePayload, loadCDRDirectionsPromiseDeferred);
            });
            return loadCDRDirectionsPromiseDeferred.promise;
        }


        function setTitle() {
            $scope.title = isEditMode ? UtilsService.buildTitleForUpdateEditor(operatorConfigurationEntity ? '' : null, 'Operator Declared Information') : UtilsService.buildTitleForAddEditor('Operator Declared Information');
        }

        function loadCurrencies() {
            var loadCurrencyPromiseDeferred = UtilsService.createPromiseDeferred();
            currencyReadyPromiseDeferred.promise.then(function () {
                var directivePayload = {
                    selectedIds: (operatorConfigurationEntity != undefined ? (operatorConfigurationEntity.Currency != undefined ? [operatorConfigurationEntity.Currency] : undefined) : (operatorConfigurationId != undefined ? operatorConfigurationId : undefined))
                }
                VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, directivePayload, loadCurrencyPromiseDeferred);
            });
        }

        function loadOperatorProfileDirective() {

            var loadOperatorProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            operatorProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = {
                        selectedIds: (operatorConfigurationEntity != undefined ? operatorConfigurationEntity.OperatorId : (operatorConfigurationId != undefined ? operatorConfigurationId : undefined))
                    }
                    VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveAPI, directivePayload, loadOperatorProfilePromiseDeferred);
                });

            return loadOperatorProfilePromiseDeferred.promise;
        }


        function loadStaticSection() {
            if (operatorConfigurationEntity != undefined) {
                $scope.scopeModal.volume = operatorConfigurationEntity.Volume;
                $scope.scopeModal.amount = operatorConfigurationEntity.Amount;
                $scope.scopeModal.percentage = operatorConfigurationEntity.Percentage;
            }
        }

        function buildOperatorConfigurationObjFromScope() {
            console.log($scope.scopeModal.percentage)
            var obj = {
                OperatorConfigurationId: (operatorConfigurationId != null) ? operatorConfigurationId : 0,
                OperatorId: operatorProfileDirectiveAPI.getSelectedIds(),
                Volume: $scope.scopeModal.volume,
                Percentage: $scope.scopeModal.percentage,
                Amount: $scope.scopeModal.amount,
                AmountType: serviceTypeDirectiveAPI.getSelectedIds(),
                UnitType: unitTypeDirectiveAPI.getSelectedIds(),
                CDRDirection: cdrDirectionDirectiveAPI.getSelectedIds(),
                CDRType: cdrTypeDirectiveAPI.getSelectedIds(),
                Currency: currencyDirectiveAPI.getSelectedIds()
            };

            return obj;
        }

        function insertOperatorConfiguration() {
            $scope.isLoading = true;

            var operatorconfigurationObject = buildOperatorConfigurationObjFromScope();

            return Demo_OperatorConfigurationAPIService.AddOperatorConfiguration(operatorconfigurationObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Operator Configuration", response, undefined)) {
                    if ($scope.onOperatorConfigurationAdded != undefined)
                        $scope.onOperatorConfigurationAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateOperatorConfiguration() {
            $scope.isLoading = true;

            var operatorconfigurationObject = buildOperatorConfigurationObjFromScope();

            Demo_OperatorConfigurationAPIService.UpdateOperatorConfiguration(operatorconfigurationObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Operator Configuration", response, undefined)) {
                    if ($scope.onOperatorConfigurationUpdated != undefined)
                        $scope.onOperatorConfigurationUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('Demo_OperatorConfigurationEditorController', operatorConfigurationEditorController);
})(appControllers);
