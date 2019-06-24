"use strict";

app.directive("whsInvoicetypeGenerationcustomsectionSupplier", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_FinancialAccountAPIService", "WhS_BE_CommisssionTypeEnum", "WhS_Invoice_AdjustmentTypeEnum","WhS_Invoice_InvoiceAPIService",
	function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_FinancialAccountAPIService, WhS_BE_CommisssionTypeEnum, WhS_Invoice_AdjustmentTypeEnum, WhS_Invoice_InvoiceAPIService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SupplierGenerationCustomSection($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlSupplierGeneration",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/Extensions/GenerationCustomSection/Templates/SupplierGenerationCustomSectionTemplate.html"
        };

        function SupplierGenerationCustomSection($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;

            var oldCommission;
            var oldAdjustment;

            var timeZoneSelectorAPI;
            var timeZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var emptyTimeZonePromiseDeferred;
            var selectedTimeZoneReadyDeferred;

            var selectedCommissionTypeReadyDeferred;
            function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.hasCommission = ($scope.scopeModel.commission != undefined) ? true : false;
                $scope.scopeModel.onTimeZoneSelectorReady = function (api) {
                    timeZoneSelectorAPI = api;
                    timeZoneSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.commissionTypes = UtilsService.getArrayEnum(WhS_BE_CommisssionTypeEnum);
                $scope.scopeModel.selectedCommissionType = WhS_BE_CommisssionTypeEnum.Display;
                $scope.scopeModel.onTimeZoneSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedTimeZoneReadyDeferred != undefined) {
                            selectedTimeZoneReadyDeferred.resolve();
                        }
                        else {
                            onvaluechanged();
                        }
                    }
                    else {
                        if (emptyTimeZonePromiseDeferred != undefined) {
                            emptyTimeZonePromiseDeferred.resolve();
                        }
                        else {
                            onvaluechanged();
                        }
                    }
				};

				$scope.scopeModel.adjustmentTypes = UtilsService.getArrayEnum(WhS_Invoice_AdjustmentTypeEnum);
				$scope.scopeModel.onCommissionAdjustmentSelectionChanged = function (option) {
                    if (option != undefined) {
                        onvaluechanged();
						$scope.scopeModel.isPercentage = option.value == WhS_Invoice_AdjustmentTypeEnum.Percentage.value;
					}
				};
				$scope.scopeModel.onCommissionFocusChanged = function (value) {
                    if (oldCommission != $scope.scopeModel.commission)
                        onvaluechanged();

					oldCommission != $scope.scopeModel.commission;
					$scope.scopeModel.hasCommission = (value != undefined) ? true : false;
                };

                $scope.scopeModel.onAdjustmentFocusChanged = function () {
                    if (oldAdjustment != $scope.scopeModel.adjustment)
                        onvaluechanged();

                    oldAdjustment != $scope.scopeModel.adjustment;
                };

                $scope.scopeModel.onCommissionTypeSelectionChanged = function (value) {
                    if (selectedCommissionTypeReadyDeferred != undefined) {
                        selectedCommissionTypeReadyDeferred.resolve();
                    }
                    else {
                        onvaluechanged();
                    }
                };

                function onvaluechanged() {
                    if (context != undefined && context.onvaluechanged != undefined && typeof (context.onvaluechanged) == 'function') {
                        context.onvaluechanged();
                    }
                };

                UtilsService.waitMultiplePromises([timeZoneSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

				api.load = function (payload) {
					var promises = [];
                    var invoice;
                    var financialAccountId;
                    var customPayload;
                    if (payload != undefined) {
                        customPayload = payload.customPayload;
                        invoice = payload.invoice;
                        financialAccountId = payload.partnerId;
                        if (financialAccountId == undefined) {
                            financialAccountId = payload.financialAccountId;
                        }
                        if (customPayload != undefined) {
                            selectedCommissionTypeReadyDeferred = UtilsService.createPromiseDeferred();
                            oldAdjustment = $scope.scopeModel.adjustment = customPayload.Adjustment;
                            oldCommission = $scope.scopeModel.commission = customPayload.Commission;
                            $scope.scopeModel.selectedCommissionType = UtilsService.getItemByVal($scope.scopeModel.commissionTypes, customPayload.CommissionType, "value");
                        }
                        if (invoice != undefined && invoice.Details != undefined) {
                            oldAdjustment = $scope.scopeModel.adjustment = invoice.Details.Adjustment;
							oldCommission = $scope.scopeModel.commission = invoice.Details.Commission;
							$scope.scopeModel.adjustment = invoice.Details.Adjustment;
							if (invoice.Details.Adjustment != undefined) {
								$scope.scopeModel.selectedAdjustmentType = WhS_Invoice_AdjustmentTypeEnum.FixedValue;
							}
							else
								$scope.scopeModel.selectedAdjustmentType = WhS_Invoice_AdjustmentTypeEnum.Percentage;
							$scope.scopeModel.hasCommission = ($scope.scopeModel.commission != undefined) ? true : false;
							$scope.scopeModel.selectedCommissionType = UtilsService.getItemByVal($scope.scopeModel.commissionTypes, invoice.Details.CommissionType, "value");
						}
						else {
							$scope.scopeModel.selectedAdjustmentType = WhS_Invoice_AdjustmentTypeEnum.Percentage;
						}
						context = payload.context;
						if (payload.partnerId != undefined) {
							promises.push(loadCarrierCurrency(payload.partnerId));
						}
						if (payload.invoice != undefined && payload.invoice.PartnerId != undefined) {
							promises.push(loadCarrierCurrency(payload.invoice.PartnerId));
						}
                    }
                    if (customPayload == undefined) {
                        if (financialAccountId != undefined) {
                            var loadTimeZonePromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(loadTimeZonePromiseDeferred.promise);
                            selectedTimeZoneReadyDeferred = UtilsService.createPromiseDeferred();


                            WhS_BE_FinancialAccountAPIService.GetSupplierTimeZoneId(financialAccountId).then(function (response) {
                                loadTimeZoneSelector(response).then(function () {
                                    loadTimeZonePromiseDeferred.resolve();
                                }).catch(function (error) {
                                    loadTimeZonePromiseDeferred.reject(error);
                                });
                            }).catch(function (error) {
                                loadTimeZonePromiseDeferred.reject(error);
                            });
                        } else {
                            selectedTimeZoneReadyDeferred = UtilsService.createPromiseDeferred();

                            promises.push(loadTimeZoneSelector());
                        }
                    }
                    else {
                        selectedTimeZoneReadyDeferred = UtilsService.createPromiseDeferred();
                        emptyTimeZonePromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(emptyTimeZonePromiseDeferred.promise);

                        promises.push(loadTimeZoneSelector(customPayload.TimeZoneId));
                    }

                    function loadTimeZoneSelector(selectedIds) {
                        var timeZoneSelectorPayload = {
                            selectedIds: selectedIds
                        };
                        if (invoice != undefined && invoice.Details != undefined) {
                            timeZoneSelectorPayload.selectedIds = invoice.Details.TimeZoneId;
                        }

                        if (timeZoneSelectorPayload.selectedIds != undefined && timeZoneSelectorPayload.selectedIds > 0)
                            promises.push(selectedTimeZoneReadyDeferred.promise);

                        return timeZoneSelectorAPI.load(timeZoneSelectorPayload);
					}
					function loadCarrierCurrency(partnerId) {
						var loadCarrierCurrencyDeferred = UtilsService.createPromiseDeferred();
						WhS_Invoice_InvoiceAPIService.GetFinancialAccountCurrencyDescription(partnerId).then(function (result) {
							$scope.scopeModel.currencyDescription = result;
							loadCarrierCurrencyDeferred.resolve();
						});
						return loadCarrierCurrencyDeferred.promise;
					}

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        selectedTimeZoneReadyDeferred = undefined;
                        emptyTimeZonePromiseDeferred = undefined;
                        selectedCommissionTypeReadyDeferred = undefined;
                    });
                };

				api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Entities.SupplierGenerationCustomSectionPayload,TOne.WhS.Invoice.Entities",
                        TimeZoneId: timeZoneSelectorAPI.getSelectedIds(),
						Commission: ($scope.scopeModel.isPercentage) ? $scope.scopeModel.commission : null,
						CommissionType: ($scope.scopeModel.hasCommission && $scope.scopeModel.selectedCommissionType != undefined && $scope.scopeModel.isPercentage) ? $scope.scopeModel.selectedCommissionType.value : undefined,
						Adjustment: (!$scope.scopeModel.isPercentage) ? $scope.scopeModel.adjustment : null
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;
    }
]);