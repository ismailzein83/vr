(function (appControllers) {

    "use strict";

    RatePlanPricingSettingsController.$inject = ["$scope", "WhS_Sales_RatePlanAPIService", "UtilsService", "VRUIUtilsService", "VRNavigationService", "VRNotificationService"];

    function RatePlanPricingSettingsController($scope, WhS_Sales_RatePlanAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var ratePlanSettings;
        var pricingSettings;
        var directiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                ratePlanSettings = parameters.ratePlanSettings;
                pricingSettings = UtilsService.cloneObject(parameters.pricingSettings, true); // parameters.pricingSettings is cloned to prevent the pricingSettings global var of RatePlan.js from changing
            }
        }

        function defineScope() {
            $scope.title = "Edit Pricing Settings";

            $scope.costColumns = [];
            $scope.selectedCostColumn;
            $scope.templates = [];
            $scope.selectedTemplate;

            $scope.onDirectiveReady = function (api) {
                directiveAPI = api;

                var directivePayload = pricingSettings ? pricingSettings.selectedRateCalculationMethodData : null;
                var loadDirectiveDeferred = UtilsService.createPromiseDeferred();

                loadDirectiveDeferred.promise.catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.isLoadingDirective = false;

                    if (pricingSettings)
                        pricingSettings.selectedRateCalculationMethodData = null;
                });

                $scope.isLoadingDirective = true;
                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadDirectiveDeferred);
            };

            $scope.save = function () {
                pricingSettings = {};

                pricingSettings.selectedCostColumn = $scope.selectedCostColumn;
                pricingSettings.selectedRateCalculationMethod = $scope.selectedTemplate;
                pricingSettings.selectedRateCalculationMethodData = directiveAPI.getData();
                pricingSettings.selectedRateCalculationMethodData.ConfigId = $scope.selectedCostColumn.ConfigId;

                if ($scope.onPricingSettingsUpdated && typeof ($scope.onPricingSettingsUpdated) == "function")
                    $scope.onPricingSettingsUpdated(pricingSettings);
                
                $scope.modalContext.closeModal();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;

            UtilsService.waitMultipleAsyncOperations([loadCostColumns, loadTemplates]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadCostColumns() {
                if (ratePlanSettings && ratePlanSettings.CostCalculationMethods) {
                    for (var i = 0; i < ratePlanSettings.CostCalculationMethods.length; i++) {
                        $scope.costColumns.push(ratePlanSettings.CostCalculationMethods[i]);
                    }

                    if (pricingSettings)
                        $scope.selectedCostColumn = pricingSettings.selectedCostColumn;
                }
            }

            function loadTemplates() {
                return WhS_Sales_RatePlanAPIService.GetRateCalculationMethodTemplates().then(function (response) {
                    if (response) {
                        for (var i = 0; i < response.length; i++) {
                            $scope.templates.push(response[i]);
                        }

                        if (pricingSettings)
                            $scope.selectedTemplate = pricingSettings.selectedRateCalculationMethod;
                    }
                });
            }
        }
    }

    appControllers.controller("WhS_Sales_RatePlanPricingSettingsController", RatePlanPricingSettingsController);

})(appControllers);
