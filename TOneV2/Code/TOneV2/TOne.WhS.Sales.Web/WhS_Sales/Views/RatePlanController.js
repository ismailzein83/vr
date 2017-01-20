(function (appControllers) {

	"use strict";

	RatePlanController.$inject = ["$scope", "WhS_Sales_RatePlanService", "WhS_Sales_RatePlanAPIService", "WhS_BE_SalePriceListOwnerTypeEnum", "WhS_Sales_RatePlanStatusEnum", 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPInstanceService', 'WhS_BP_CreateProcessResultEnum', 'VRCommon_CurrencyAPIService', 'WhS_BE_CarrierAccountAPIService', "UtilsService", "VRUIUtilsService", "VRNotificationService"];

	function RatePlanController($scope, WhS_Sales_RatePlanService, WhS_Sales_RatePlanAPIService, WhS_BE_SalePriceListOwnerTypeEnum, WhS_Sales_RatePlanStatusEnum, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, WhS_BP_CreateProcessResultEnum, VRCommon_CurrencyAPIService, WhS_BE_CarrierAccountAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
		var ownerTypeSelectorAPI;
		var ownerTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var sellingProductSelectorAPI;
		var sellingProductSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var carrierAccountSelectorAPI;
		var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var databaseSelectorAPI;
		var databaseSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var policySelectorAPI;

		var currencySelectorAPI;
		var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var systemCurrencyId;
		var draftCurrencyId;
		var defaultCustomerCurrencyId;

		var countrySelectorAPI;
		var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var textFilterAPI;
		var textFilterReadyDeferred = UtilsService.createPromiseDeferred();

		var defaultItem;
		var countryChanges;

		var gridAPI;
		var gridReadyDeferred = UtilsService.createPromiseDeferred();

		var settings = {};
		var pricingSettings;
		var ratePlanSettingsData;
		var saleAreaSettingsData;

		defineScope();
		load();

		function defineScope() {
			/* These vars are reversed with every onOwnerTypeChanged. Therefore, the selling product selector will show when the event first occurs */
			$scope.showSellingProductSelector = false;
			$scope.showCarrierAccountSelector = true;
			/* ***** */

			$scope.onOwnerTypeSelectorReady = function (api) {
				ownerTypeSelectorAPI = api;
				ownerTypeSelectorReadyDeferred.resolve();
			};
			$scope.onOwnerTypeChanged = function (item) {
				resetRatePlan();
				draftCurrencyId = undefined;

				var selectedId = ownerTypeSelectorAPI.getSelectedIds();

				if (selectedId == undefined)
					return;

				$scope.showSellingProductSelector = !$scope.showSellingProductSelector;
				$scope.showCarrierAccountSelector = !$scope.showCarrierAccountSelector;

				if (selectedId == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
					$scope.selectedCustomer = undefined;
				}
				else if (selectedId == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
					$scope.selectedSellingProduct = undefined;
				}
			};

			$scope.onSellingProductSelectorReady = function (api) {
				sellingProductSelectorAPI = api;
				sellingProductSelectorReadyDeferred.resolve();
			};
			$scope.onSellingProductChanged = function () {
				resetRatePlan();
			};

			$scope.onCarrierAccountSelectorReady = function (api) {
				carrierAccountSelectorAPI = api;
				carrierAccountSelectorReadyDeferred.resolve();
			};
			$scope.onCarrierAccountChanged = function () {
				resetRatePlan();
				draftCurrencyId = undefined;

				var selectedId = carrierAccountSelectorAPI.getSelectedIds();

				if (selectedId != undefined) {
					$scope.isLoadingFilterSection = true;
					onCustomerChanged(selectedId).finally(function () {
						$scope.isLoadingFilterSection = false;
					});
				}
			};

			$scope.numberOfOptions = 3;

			$scope.onDatabaseSelectorReady = function (api) {
				databaseSelectorAPI = api;
				databaseSelectorReadyDeferred.resolve();
			};

			$scope.onRoutingDatabaseChanged = function () {
				resetRatePlan();
				draftCurrencyId = undefined;

				var selectedId = databaseSelectorAPI.getSelectedIds();

				if (selectedId == undefined)
					return;

				var policySelectorPayload = {
					filter: {
						RoutingDatabaseId: selectedId
					},
					selectDefaultPolicy: true
				};

				var setLoader = function (value) {
					$scope.isLoadingFilterSection = value;
				};
				VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, policySelectorAPI, policySelectorPayload, setLoader, undefined);
			};

			$scope.onPolicySelectorReady = function (api) {
				policySelectorAPI = api;
			};

			$scope.onCurrencySelectorReady = function (api) {
				currencySelectorAPI = api;
				currencySelectorReadyDeferred.resolve();
			};

			$scope.onCurrencyChanged = function () {
				if (draftCurrencyId == undefined)
					return;

				var selectedId = currencySelectorAPI.getSelectedIds();
				if (selectedId == undefined)
					return;

				if (selectedId == draftCurrencyId)
					return;

				VRNotificationService.showConfirmation('Changing the currency will reset all new rates. Are you sure you want to proceed?').then(function (isConfirmed) {
					if (isConfirmed) {
						var promises = [];

						var saveChangesPromise = saveDraft(false);
						promises.push(saveChangesPromise);

						var deleteChangedRatesDeferred = UtilsService.createPromiseDeferred();
						promises.push(deleteChangedRatesDeferred.promise);

						saveChangesPromise.then(function () {
							WhS_Sales_RatePlanAPIService.DeleteChangedRates(ownerTypeSelectorAPI.getSelectedIds(), getOwnerId(), selectedId).then(function () {
								draftCurrencyId = selectedId;
								deleteChangedRatesDeferred.resolve();
							}).catch(function (error) {
								deleteChangedRatesDeferred.reject(error);
							});
						});

						UtilsService.waitMultiplePromises(promises).then(function () {
							loadGrid();
						});
					}
					else { currencySelectorAPI.selectedCurrency(draftCurrencyId); }
				});
			};

			$scope.onCountrySelectorReady = function (api) {
				countrySelectorAPI = api;
				countrySelectorReadyDeferred.resolve();
			};

			$scope.onTextFilterReady = function (api) {
				textFilterAPI = api;
				textFilterReadyDeferred.resolve();
			};

			$scope.defaultItemTabs = [{
				title: "Default Routing Product",
				directive: "vr-whs-sales-routingproduct-default",
				loadDirective: function (api) {
					var defaultPayload = {};
					defaultPayload.defaultItem = defaultItem;
					defaultPayload.context = {};
					defaultPayload.context.saveDraft = saveDraft;
					return api.load(defaultPayload);
				}
			}];

			$scope.zoneLetters = [];
			$scope.connector = {};
			$scope.connector.selectedZoneLetterIndex = 0;

			$scope.onZoneLetterSelectionChanged = function () {
				return saveDraft(true);
			};

			$scope.onGridReady = function (api) {
				gridAPI = api;
				gridReadyDeferred.resolve();
			};

			$scope.load = function () {
				return loadRatePlan();
			};
			$scope.sellNewCountries = function () {
				var customerId = $scope.selectedCustomer.CarrierAccountId;
				var onCountryChangesUpdated = function (updatedCountryChanges) {
					countryChanges = updatedCountryChanges;

					saveDraft(false).then(function () {
						if (databaseSelectorAPI.getSelectedIds() != null && policySelectorAPI.getSelectedIds() != null)
							loadRatePlan();
					}).catch(function (error) {
						VRNotificationService.notifyException(error, $scope);
					});
				};
				WhS_Sales_RatePlanService.sellNewCountries(customerId, countryChanges, saleAreaSettingsData, ratePlanSettingsData, onCountryChangesUpdated);
			};
			$scope.editSettings = function () {
				var onSettingsUpdated = function (updatedSettings) {
					if (updatedSettings == undefined)
						settings = undefined;
					else {
						settings = {};
						if (updatedSettings.costCalculationMethods != undefined) {
							settings.costCalculationMethods = [];
							for (var i = 0; i < updatedSettings.costCalculationMethods.length; i++) {
								settings.costCalculationMethods.push(updatedSettings.costCalculationMethods[i]);
							}
						}
					}

					pricingSettings = null;
					$scope.showApplyButton = false;

					VRNotificationService.showSuccess("Settings saved");

					var promises = [];

					var saveChangesPromise = saveDraft(false);
					promises.push(saveChangesPromise);

					var loadGridDeferred = UtilsService.createPromiseDeferred();
					promises.push(loadGridDeferred.promise);

					saveChangesPromise.then(function () {
						loadGrid().then(function () {
							loadGridDeferred.resolve();
						}).catch(function (error) {
							loadGridDeferred.reject(error);
						});
					});

					UtilsService.waitMultiplePromises(promises);
				};
				WhS_Sales_RatePlanService.editSettings(settings, onSettingsUpdated);
			};
			$scope.editPricingSettings = function () {
				var onPricingSettingsUpdated = function (updatedPricingSettings) {
					pricingSettings = updatedPricingSettings;

					$scope.showApplyButton = true;
					VRNotificationService.showSuccess("Pricing settings saved");

					var promises = [];

					var saveChangesPromise = saveDraft(false);
					promises.push(saveChangesPromise);

					var loadGridDeferred = UtilsService.createPromiseDeferred();
					promises.push(loadGridDeferred.promise);

					saveChangesPromise.then(function () {
						loadGrid().then(function () {
							loadGridDeferred.resolve();
						}).catch(function (error) {
							loadGridDeferred.reject(error);
						});
					});

					UtilsService.waitMultiplePromises(promises);
				};
				WhS_Sales_RatePlanService.editPricingSettings(settings, pricingSettings, onPricingSettingsUpdated);
			};
			$scope.applyCalculatedRates = function () {
				var promises = [];

				var confirmPromise = VRNotificationService.showConfirmation("Are you sure you want to apply the calculated rates?");
				promises.push(confirmPromise);

				var saveChangesDeferred = UtilsService.createPromiseDeferred();
				promises.push(saveChangesDeferred.promise);

				var applyDeferred = UtilsService.createPromiseDeferred();
				promises.push(applyDeferred.promise);

				var calculatedRates;

				confirmPromise.then(function (confirmed) {
					if (confirmed) {
						saveDraft(false).then(function () {
							saveChangesDeferred.resolve();
							var tryApplyInput = getTryApplyCalculatedRatesInput();
							WhS_Sales_RatePlanAPIService.TryApplyCalculatedRates(tryApplyInput).then(function (response) {
								calculatedRates = response;
								applyDeferred.resolve();
							}).catch(function (error) {
								applyDeferred.reject(error);
							});
						}).catch(function (error) {
							saveChangesDeferred.reject(error);
						});

						UtilsService.waitMultiplePromises([saveChangesDeferred.promise, applyDeferred.promise]).then(function () {
							if (calculatedRates == undefined || calculatedRates == null)
								onRatesApplied();
							else {
								var onSaved = function (validCalculatedRates) {
									var applyInput = getApplyCalculatedRatesInput(validCalculatedRates);
									WhS_Sales_RatePlanAPIService.ApplyCalculatedRates(applyInput).then(function () {
										onRatesApplied();
									}).catch(function (error) {
										VRNotificationService.notifyException(error, $scope);
									});
								};
								WhS_Sales_RatePlanService.viewInvalidRates(calculatedRates, onSaved);
							}
						});
					}
					else {
						saveChangesDeferred.resolve();
						applyDeferred.resolve();
					}
				});

				function getTryApplyCalculatedRatesInput() {
					var input = {
						OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
						OwnerId: getOwnerId(),
						EffectiveOn: new Date(),
						RoutingDatabaseId: databaseSelectorAPI ? databaseSelectorAPI.getSelectedIds() : null,
						PolicyConfigId: policySelectorAPI ? policySelectorAPI.getSelectedIds() : null,
						NumberOfOptions: $scope.numberOfOptions,
						CostCalculationMethods: settings ? settings.costCalculationMethods : null,
						CurrencyId: getCurrencyId(),
						CountryIds: countrySelectorAPI.getSelectedIds()
					};

					if (pricingSettings != undefined) {
						input.RateCalculationMethod = pricingSettings.selectedRateCalculationMethodData;
						if (pricingSettings.selectedCostColumn != undefined)
							input.SelectedCostCalculationMethodConfigId = pricingSettings.selectedCostColumn.ConfigId;
					}

					var textFilterData = textFilterAPI.getData();
					if (textFilterData != undefined) {
						input.ZoneNameFilterType = textFilterData.TextFilterType;
						input.ZoneNameFilter = textFilterData.Text;
					}

					return input;
				}
				function getApplyCalculatedRatesInput(validCalculatedRates) {
					return {
						OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
						OwnerId: getOwnerId(),
						CalculatedRates: validCalculatedRates,
						EffectiveOn: new Date(),
						CurrencyId: getCurrencyId()
					};
				}
				function onRatesApplied() {
					VRNotificationService.showSuccess("Rates applied");
					pricingSettings = null;
					$scope.showApplyButton = false;
					$scope.showCancelButton = true;

					loadGrid().catch(function (error) {
						VRNotificationService.notifyException(error, $scope);
					});
				}

				return UtilsService.waitMultiplePromises(promises);
			};
			$scope.deleteDraft = function () {
				var promises = [];

				var confirmPromise = VRNotificationService.showConfirmation("Are you sure you want to cancel all of your changes?");
				promises.push(confirmPromise);

				var deleteDeferred = UtilsService.createPromiseDeferred();
				promises.push(deleteDeferred.promise);

				confirmPromise.then(function (confirmed) {
					if (confirmed) {
						return WhS_Sales_RatePlanAPIService.DeleteDraft(ownerTypeSelectorAPI.getSelectedIds(), getOwnerId()).then(function (response) {
							if (response) {
								deleteDeferred.resolve();
								VRNotificationService.showSuccess("Draft deleted");
								$scope.showCancelButton = false;

								countryChanges = undefined;
								loadRatePlan();
							}
							else {
								deleteDeferred.reject();
							}
						}).catch(function (error) {
							deleteDeferred.reject();
							VRNotificationService.notifyException(error, $scope);
						});
					}
					else {
						deleteDeferred.resolve();
					}
				});

				return UtilsService.waitMultiplePromises(promises);
			};
			$scope.openBulkActionWizard = function () {
				var ownerType = ownerTypeSelectorAPI.getSelectedIds();
				var ownerId = getOwnerId();
				var ownerSellingNumberPlanId = getOwnerSellingNumberPlanId();
				var gridQuery = getGridQuery(false);
				var onBulkActionAppliedToDraft = function () {
					return loadRatePlan();
				};
				WhS_Sales_RatePlanService.openBulkActionWizard(ownerType, ownerId, ownerSellingNumberPlanId, gridQuery, onBulkActionAppliedToDraft);
			};

			defineSaveButtonMenuActions();
		}
		function load() {
			$scope.isLoadingFilterSection = true;
			loadAllControls();
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([loadOwnerFilterSection, loadRouteOptionsFilterSection, loadCurrencySelector, loadRatePlanSettingsData, loadSaleAreaSettingsData, getSystemCurrencyId, loadCountrySelector, loadTextFilter]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.isLoadingFilterSection = false;
			});
		}
		function loadOwnerFilterSection() {
			var promises = [];

			var ownerTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
			promises.push(ownerTypeSelectorLoadDeferred.promise);

			ownerTypeSelectorReadyDeferred.promise.then(function () {
				var ownerTypeSelectorPayload = { selectedIds: WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value };
				VRUIUtilsService.callDirectiveLoad(ownerTypeSelectorAPI, ownerTypeSelectorPayload, ownerTypeSelectorLoadDeferred);
			});

			var sellingProductSelectorLoadDeferred = UtilsService.createPromiseDeferred();
			promises.push(sellingProductSelectorLoadDeferred.promise);

			sellingProductSelectorReadyDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(sellingProductSelectorAPI, undefined, sellingProductSelectorLoadDeferred);
			});

			var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
			promises.push(carrierAccountSelectorLoadDeferred.promise);

			carrierAccountSelectorReadyDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, undefined, carrierAccountSelectorLoadDeferred);
			});

			return UtilsService.waitMultiplePromises(promises);
		}
		function loadRouteOptionsFilterSection() {
			var promises = [];

			var databaseSelectorLoadDeferred = UtilsService.createPromiseDeferred();
			promises.push(databaseSelectorLoadDeferred.promise);

			databaseSelectorReadyDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(databaseSelectorAPI, undefined, databaseSelectorLoadDeferred);
			});

			return UtilsService.waitMultiplePromises(promises);
		}
		function loadCurrencySelector() {
			var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

			currencySelectorReadyDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, undefined, currencySelectorLoadDeferred);
			});

			return currencySelectorLoadDeferred.promise;
		}
		function loadRatePlanSettingsData() {
			return WhS_Sales_RatePlanAPIService.GetRatePlanSettingsData().then(function (response) {
				ratePlanSettingsData = response;
				settings.costCalculationMethods = [];
				for (var i = 0; i < response.CostCalculationsMethods.length; i++) {
					settings.costCalculationMethods.push(response.CostCalculationsMethods[i]);
				}
			});
		}
		function loadSaleAreaSettingsData() {
			return WhS_Sales_RatePlanAPIService.GetSaleAreaSettingsData().then(function (response) {
				saleAreaSettingsData = response;
			});
		}
		function getSystemCurrencyId() {
			return VRCommon_CurrencyAPIService.GetSystemCurrencyId().then(function (response) {
				systemCurrencyId = response;
			});
		}
		function loadCountrySelector() {
			var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

			countrySelectorReadyDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, undefined, countrySelectorLoadDeferred);
			});

			return countrySelectorLoadDeferred.promise;
		}
		function loadTextFilter() {
			var textFilterLoadDeferred = UtilsService.createPromiseDeferred();

			textFilterReadyDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(textFilterAPI, undefined, textFilterLoadDeferred);
			});

			return textFilterLoadDeferred.promise;
		}

		function loadRatePlan() {

			var promises = [];
			var ownerTypeValue = ownerTypeSelectorAPI.getSelectedIds();

			var getDraftCurrencyIdPromise = getDraftCurrencyId();
			promises.push(getDraftCurrencyIdPromise);

			var checkIfDraftExistsPromise = checkIfDraftExists();
			promises.push(checkIfDraftExistsPromise);

			var loadDefaultItemPromise = loadDefaultItem();
			promises.push(loadDefaultItemPromise);

			var loadGridDeferred = UtilsService.createPromiseDeferred();
			promises.push(loadGridDeferred.promise);

			getDraftCurrencyIdPromise.then(function () {
				var gridQuery = getGridQuery(true);
				gridAPI.load(gridQuery).then(function () {
					loadGridDeferred.resolve();
				}).catch(function (error) {
					loadGridDeferred.reject(error);
				});
			});

			function getDraftCurrencyId() {
				if (ownerTypeSelectorAPI.getSelectedIds() == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
					return WhS_Sales_RatePlanAPIService.GetDraftCurrencyId(WhS_BE_SalePriceListOwnerTypeEnum.Customer.value, getOwnerId()).then(function (response) {
						if (response != null) {
							draftCurrencyId = response;
							currencySelectorAPI.selectedCurrency(response)
						}
						if (draftCurrencyId == undefined)
							draftCurrencyId = defaultCustomerCurrencyId;
					});
				}
				else {
					var deferred = UtilsService.createPromiseDeferred();
					deferred.resolve();
					return deferred.promise;
				}
			}
			function checkIfDraftExists() {
				return WhS_Sales_RatePlanAPIService.CheckIfDraftExists(ownerTypeValue, getOwnerId()).then(function (response) {
					$scope.showCancelButton = response === true;
				}).catch(function (error) {
					VRNotificationService.notifyException(error, $scope); // The user can perform other tasks if CheckIfDraftExists fails
				});
			}
			function loadDefaultItem() {
				var effectiveOn = UtilsService.getDateFromDateTime(new Date());
				return WhS_Sales_RatePlanAPIService.GetDefaultItem(ownerTypeValue, getOwnerId(), effectiveOn).then(function (response) {
					if (response != undefined) {
						defaultItem = response;
						defaultItem.OwnerType = ownerTypeValue;
						defaultItem.OwnerId = getOwnerId();
						for (var i = 0; i < $scope.defaultItemTabs.length; i++) {
							var tab = $scope.defaultItemTabs[i];
							if (tab.directiveAPI)
								tab.loadDirective(tab.directiveAPI);
						}
					}
				});
			}

			return UtilsService.waitMultiplePromises(promises);
		}
		function loadGrid() {
			var gridQuery = getGridQuery();
			return gridAPI.load(gridQuery);
		}
		function getGridQuery(shouldSetFilter) {

			var query = {
				OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
				OwnerId: getOwnerId(),
				CurrencyId: getCurrencyId(),
				RoutingDatabaseId: databaseSelectorAPI.getSelectedIds(),
				PolicyConfigId: policySelectorAPI.getSelectedIds(),
				NumberOfOptions: $scope.numberOfOptions,
				CostCalculationMethods: getCostCalculationMethods(),
				BulkAction: null,
				EffectiveOn: UtilsService.getDateFromDateTime(new Date())
			};

			if (shouldSetFilter === true) {

				query.Filter = {};
				query.Filter.CountryIds = countrySelectorAPI.getSelectedIds();
				query.Filter.BulkActionFilter = null;

				var textFilterData = textFilterAPI.getData();
				if (textFilterData != undefined) {
					query.Filter.ZoneNameFilter = textFilterData.Text;
					query.Filter.ZoneNameFilterType = textFilterData.TextFilterType;
				}
			}

			setGridQueryContext();

			function setGridQueryContext() {
				query.context = {};
				query.context.saveDraft = saveDraft;
				query.context.onZoneLettersLoaded = function () {
					$scope.showSaveButton = true;
					$scope.showSettingsButton = true;
					$scope.showPricingButton = true;
					showRatePlan(true);
				};
				query.context.showRatePlan = showRatePlan;
				query.context.isFilterApplied = isFilterApplied;
			}

			return query;
		}

		function getCostCalculationMethods() {
			var cosCalculationMethods = [];

			if (settings != undefined && settings.costCalculationMethods != undefined) {
				for (var i = 0; i < settings.costCalculationMethods.length ; i++)
					cosCalculationMethods.push(settings.costCalculationMethods[i]);
			}

			return cosCalculationMethods;
		}

		function saveDraft(shouldLoadGrid) {
			var promises = [];

			var saveDraftDeferred = UtilsService.createPromiseDeferred();
			promises.push(saveDraftDeferred.promise);

			var loadGridDeferred = UtilsService.createPromiseDeferred();
			promises.push(loadGridDeferred.promise);

			var newDraft = getNewDraft();

			if (newDraft == undefined)
				saveDraftDeferred.resolve();
			else {
				var parameters = {
					OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
					OwnerId: getOwnerId(),
					NewChanges: newDraft
				};
				WhS_Sales_RatePlanAPIService.SaveChanges(parameters).then(function () {
					$scope.showCancelButton = true;
					saveDraftDeferred.resolve();
				}).catch(function (error) {
					saveDraftDeferred.reject(error);
				});
			}

			saveDraftDeferred.promise.then(function () {
				if (!shouldLoadGrid)
					loadGridDeferred.resolve();
				else {
					loadGrid().then(function () {
						loadGridDeferred.resolve();
					}).catch(function (error) {
						loadGridDeferred.reject(error);
					});
				}
			});

			return UtilsService.waitMultiplePromises(promises);
		}
		function getNewDraft() {
			var newDraft;

			var defaultDraft = getDefaultDraft();
			var zoneDrafts = gridAPI.getZoneDrafts();

			if (defaultDraft != undefined || zoneDrafts != undefined || countryChanges != undefined) {
				newDraft = {
					CurrencyId: getCurrencyId(),
					DefaultChanges: defaultDraft,
					ZoneChanges: zoneDrafts,
					CountryChanges: countryChanges
				};
			}
			return newDraft;
		}
		function getDefaultDraft() {
			if (defaultItem == undefined || !defaultItem.IsDirty)
				return null;

			var defaultDraft = {};
			for (var i = 0; i < $scope.defaultItemTabs.length; i++) {
				var defaultTab = $scope.defaultItemTabs[i];
				if (defaultTab.directiveAPI != undefined)
					defaultTab.directiveAPI.applyChanges(defaultDraft);
			}
			defaultDraft.NewService = defaultItem.NewService;
			defaultDraft.ClosedService = defaultItem.ClosedService;
			defaultDraft.ResetService = defaultItem.ResetService;
			return defaultDraft;
		}

		function onCustomerChanged(customerId) {
			var promises = [];

			var isCustomerValid;
			var customerCurrencyId;

			var validateCustomerPromise = validateCustomer();
			promises.push(validateCustomerPromise);

			var getCustomerCurrencyIdDeferred = UtilsService.createPromiseDeferred();
			promises.push(getCustomerCurrencyIdDeferred.promise);

			var getCountryChangesDeferred = UtilsService.createPromiseDeferred();
			promises.push(getCountryChangesDeferred.promise);

			validateCustomerPromise.then(function () {
				if (isCustomerValid) {
					getCustomerCurrencyId().then(function () {
						getCustomerCurrencyIdDeferred.resolve();
					}).catch(function (error) {
						getCustomerCurrencyIdDeferred.reject(error);
					});
					getCountryChanges().then(function () {
						getCountryChangesDeferred.resolve();
					}).catch(function (error) {
						getCountryChangesDeferred.reject(error, $scope);
					});
				}
				else {
					getCustomerCurrencyIdDeferred.resolve();
					getCountryChangesDeferred.resolve();
					VRNotificationService.showInformation($scope.selectedCustomer.Name + " is not assigned to a selling product");
					$scope.selectedCustomer = undefined;
				}
			});

			function validateCustomer() {
				return WhS_Sales_RatePlanAPIService.ValidateCustomer(customerId, new Date()).then(function (response) {
					isCustomerValid = response;
				});
			}
			function getCustomerCurrencyId() {
				return WhS_BE_CarrierAccountAPIService.GetCarrierAccountCurrencyId(customerId).then(function (response) {
					defaultCustomerCurrencyId = response;
					currencySelectorAPI.selectedCurrency(response);
				});
			}
			function getCountryChanges() {
				return WhS_Sales_RatePlanAPIService.GetCountryChanges(customerId).then(function (response) {
					countryChanges = response;
				});
			}

			return UtilsService.waitMultiplePromises(promises);
		}

		function resetRatePlan() {
			resetZoneLetters();
			showRatePlan(false);
			showActionBarButtons(false);
		}
		function resetZoneLetters() {
			$scope.zoneLetters.length = 0;
			$scope.connector.selectedZoneLetterIndex = 0;
		}
		function showRatePlan(show) {
			$scope.showZoneLetters = show;
			$scope.showDefaultItem = show;
			$scope.showGrid = show;
		}
		function showActionBarButtons(show) {
			$scope.showSaveButton = show;
			$scope.showSettingsButton = show;
			$scope.showPricingButton = show;
			$scope.showCancelButton = show;
			$scope.showApplyButton = show;
		}

		function defineSaveButtonMenuActions() {
			$scope.saveButtonMenuActions = [{
				name: "Draft",
				clicked: function () {
					return saveDraft(false).then(function () {
						VRNotificationService.showSuccess("Draft saved");
					});
				}
			}, {
				name: "Apply Draft",
				clicked: applyDraft
			}];
		}
		function applyDraft() {
			var promises = [];

			var saveChangesPromise = saveDraft(false);
			promises.push(saveChangesPromise);

			var createProcessDeferred = UtilsService.createPromiseDeferred();
			promises.push(createProcessDeferred.promise);

			saveChangesPromise.then(function () {
				var inputArguments = {
					$type: 'TOne.WhS.Sales.BP.Arguments.RatePlanInput, TOne.WhS.Sales.BP.Arguments',
					OwnerType: ownerTypeSelectorAPI.getSelectedIds(),
					OwnerId: getOwnerId(),
					CurrencyId: getCurrencyId(),
					EffectiveDate: UtilsService.getDateFromDateTime(new Date())
				};

				var input = {
					InputArguments: inputArguments
				};

				BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
					createProcessDeferred.resolve();
					if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {

						var processTrackingContext = {
							onClose: function () {
								countryChanges = undefined;
								loadRatePlan();
							}
						};

						BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, processTrackingContext);
					}
				}).catch(function (error) {
					createProcessDeferred.reject(error);
				});
			});

			return UtilsService.waitMultiplePromises(promises);
		}

		function getOwnerId() {
			var ownerId = null;

			if ($scope.showSellingProductSelector)
				ownerId = $scope.selectedSellingProduct.SellingProductId;
			else if ($scope.showCarrierAccountSelector)
				ownerId = $scope.selectedCustomer.CarrierAccountId;

			return ownerId;
		}
		function getOwnerSellingNumberPlanId() {
			var ownerType = ownerTypeSelectorAPI.getSelectedIds();
			if (ownerType == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) {
				if ($scope.selectedSellingProduct != undefined)
					return $scope.selectedSellingProduct.SellingNumberPlanId;
			}
			else {
				if ($scope.selectedCustomer != undefined)
					return $scope.selectedCustomer.SellingNumberPlanId;
			}
		}
		function getCurrencyId() {
			return (ownerTypeSelectorAPI.getSelectedIds() == WhS_BE_SalePriceListOwnerTypeEnum.SellingProduct.value) ? systemCurrencyId : currencySelectorAPI.getSelectedIds();
		}
		function isFilterApplied() {
			var selectedCountryIds = countrySelectorAPI.getSelectedIds();
			if (selectedCountryIds != undefined)
				return true;
			var textFilterData = textFilterAPI.getData();
			if (textFilterData != undefined)
				return true;
			return false;
		}
	}

	appControllers.controller("WhS_Sales_RatePlanController", RatePlanController);

})(appControllers);