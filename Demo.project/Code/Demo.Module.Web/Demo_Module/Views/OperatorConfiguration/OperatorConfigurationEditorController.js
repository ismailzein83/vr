(function (appControllers) {

    "use strict";

    operatorConfigurationEditorController.$inject = ['$scope', 'Demo_OperatorConfigurationAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRValidationService'];

    function operatorConfigurationEditorController($scope, Demo_OperatorConfigurationAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRValidationService) {
        var isEditMode;
        var operatorConfigurationId;
        var operatorConfigurationEntity;

        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var cdrDirectionDirectiveAPI;
        var cdrDirectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencyDirectiveAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sourceTemplateDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred;

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

            $scope.sourceTypeTemplates = [];
            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTemplateDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTemplateDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
            }

            $scope.selectedCurrency;

            $scope.scopeModal.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();

            }

            $scope.validateDateRange = function () {
                return VRValidationService.validateTimeRange($scope.scopeModal.fromDate, $scope.scopeModal.toDate);
            }

            $scope.onCurrencyDirectiveReady = function (api) {
                currencyDirectiveAPI = api;
                currencyReadyPromiseDeferred.resolve();
            }

            $scope.onCDRDirectionReady = function (api) {
                cdrDirectionDirectiveAPI = api;
                cdrDirectionReadyPromiseDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadOperatorProfileDirective, loadCDRDirections, loadCurrencies, loadServiceSubTypes])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadServiceSubTypes() {
            var promises = [];
            var sourceConfigId;
            if (operatorConfigurationEntity != undefined && operatorConfigurationEntity.ServiceSubTypeSettings ) {
                sourceConfigId = operatorConfigurationEntity.ServiceSubTypeSettings.ConfigId;
            }

            var loadServiceSubTypePromise = Demo_OperatorConfigurationAPIService.GetServiceSubTypeTemplates().then(function (response) {

                angular.forEach(response, function (item) {
                    $scope.sourceTypeTemplates.push(item);
                });

                if (sourceConfigId != undefined)
                    $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

            });
            promises.push(loadServiceSubTypePromise);

            if (sourceConfigId != undefined) {
                sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                    var serviceSubTypePayload;

                    if (operatorConfigurationEntity != undefined && operatorConfigurationEntity.ServiceSubTypeSettings) {
                        serviceSubTypePayload = {
                            selectedIds: [operatorConfigurationEntity.ServiceSubTypeSettings.SelectedId]
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(sourceTemplateDirectiveAPI, serviceSubTypePayload, loadSourceTemplatePromiseDeferred);
                });

                promises.push(loadSourceTemplatePromiseDeferred.promise);
            }

            return UtilsService.waitMultiplePromises(promises);
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
                $scope.scopeModal.notes = operatorConfigurationEntity.Notes;
                $scope.scopeModal.fromDate = operatorConfigurationEntity.FromDate;
                $scope.scopeModal.toDate = operatorConfigurationEntity.ToDate;
            }
        }

        function buildOperatorConfigurationObjFromScope() {
            var obj = {
                OperatorConfigurationId: (operatorConfigurationId != null) ? operatorConfigurationId : 0,
                OperatorId: operatorProfileDirectiveAPI.getSelectedIds(),
                Volume: $scope.scopeModal.volume,
                Percentage: $scope.scopeModal.percentage,
                FromDate: $scope.scopeModal.fromDate,
                ToDate: $scope.scopeModal.toDate,
                Amount: $scope.scopeModal.amount,
                CDRDirection: cdrDirectionDirectiveAPI.getSelectedIds(),
                Notes: $scope.scopeModal.notes,
                Currency: currencyDirectiveAPI.getSelectedIds(),
                ServiceSubTypeSettings: { $type: "Demo.Module.MainExtension.ServiceSubTypeVoice, Demo.Module.MainExtension", SelectedId: sourceTemplateDirectiveAPI.getSelectedIds(), ConfigId: $scope.selectedSourceTypeTemplate.TemplateConfigID }
            };
            console.log(obj)
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
