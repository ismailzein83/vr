'use strict';

app.directive('whsRoutesyncCarrieraccountmappingEricssonGrid', ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'WhS_BE_CarrierAccountAPIService', 'WhS_BE_CarrierAccountTypeEnum',
	function (VRNotificationService, VRUIUtilsService, UtilsService, WhS_BE_CarrierAccountAPIService, WhS_BE_CarrierAccountTypeEnum) {
		return {
			restrict: 'E',
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new EricssonCarrierAccountMappingGridCtor($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/WhS_RouteSync/Directives/CarrierAccountMapping/Templates/CarrierAccountMappingEricssonGridTemplate.html'
		};

		function EricssonCarrierAccountMappingGridCtor($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var carrierMappings;
			var context;

			var gridAPI;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.carrierAccountMappings = [];
				$scope.scopeModel.carrierAccountMappingsGridDS = [];

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.loadMoreData = function () {
					return loadMoreCarrierMappings();
				};

				$scope.scopeModel.showExpandIcon = function (dataItem) {
					if (showCustomerMapping(dataItem))
						return true;

					if (showSupplierMapping(dataItem))
						return true;

					return false;
				};
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {

					if (payload != undefined) {
						carrierMappings = payload.carrierMappings;
						context = payload.context;
					}

					var promises = [];

					var ericssonCarrierAccountMappingsGridLoadPromise = getEricssonCarrierAccountMappingsGridLoadPromise();
					promises.push(ericssonCarrierAccountMappingsGridLoadPromise);

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {

					var results = {};
					for (var i = 0; i < $scope.scopeModel.carrierAccountMappings.length; i++) {
						var carrierAccountMapping = $scope.scopeModel.carrierAccountMappings[i];
						if (carrierAccountMapping == undefined)
							continue;

						if (carrierAccountMapping.customerMappingGridAPI != undefined || carrierAccountMapping.supplierMappingGridAPI != undefined) {
							results[carrierAccountMapping.CarrierAccountId] = {
								CarrierId: carrierAccountMapping.CarrierAccountId,
								CustomerMapping: getCustomerMapping(carrierAccountMapping.customerMappingGridAPI, carrierAccountMapping.CarrierAccountId),
								SupplierMapping: getSupplierMapping(carrierAccountMapping.supplierMappingGridAPI, carrierAccountMapping.CarrierAccountId)
							};
						} else {
							results[carrierAccountMapping.CarrierAccountId] = carrierMappings != undefined ? carrierMappings[carrierAccountMapping.CarrierAccountId] : undefined;
						}
					}

					function getCustomerMapping(customerMappingGridAPI, carrierAccountId) {
						if (customerMappingGridAPI != undefined)
							return customerMappingGridAPI.getData();

						if (carrierMappings != undefined && carrierMappings[carrierAccountId] != undefined)
							return carrierMappings[carrierAccountId].CustomerMapping;

						return null;
					}
					function getSupplierMapping(supplierMappingGridAPI, carrierAccountId) {
						if (supplierMappingGridAPI != undefined)
							return supplierMappingGridAPI.getData();

						if (carrierMappings != undefined && carrierMappings[carrierAccountId] != undefined)
							return carrierMappings[carrierAccountId].SupplierMapping;

						return null;
					}

					return results;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function getEricssonCarrierAccountMappingsGridLoadPromise() {

				var loadEricssonCarrierAccountMappingsGridPromiseDeferred = UtilsService.createPromiseDeferred();

				var serializedFilter = {};
				WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serializedFilter).then(function (response) {

					if (response != undefined) {
						var mappingSeparator = context != undefined ? context.getMappingSeparator() : undefined;

						for (var i = 0; i < response.length; i++) {
							var currentCarrierAccountInfo = response[i];

							var carrierMapping = carrierMappings != undefined ? carrierMappings[response[i].CarrierAccountId] : undefined;
							var carrierAccountMapping = {
								CarrierAccountId: currentCarrierAccountInfo.CarrierAccountId,
								CarrierAccountType: currentCarrierAccountInfo.AccountType,
								CarrierAccountName: currentCarrierAccountInfo.Name,
								CustomerMapping: (carrierMapping && carrierMapping.CustomerMapping) ? carrierMapping.CustomerMapping : undefined,
								SupplierMapping: (carrierMapping && carrierMapping.SupplierMapping) ? carrierMapping.SupplierMapping : undefined
							};
							extendCarrierAccountMapping(carrierAccountMapping);
							$scope.scopeModel.carrierAccountMappings.push(carrierAccountMapping);
						}
					}

					loadMoreCarrierMappings();
					loadEricssonCarrierAccountMappingsGridPromiseDeferred.resolve();
				}).catch(function (error) {
					loadEricssonCarrierAccountMappingsGridPromiseDeferred.reject(error);
				});

				return loadEricssonCarrierAccountMappingsGridPromiseDeferred.promise;
			}
			function loadMoreCarrierMappings() {

				var pageInfo = gridAPI.getPageInfo();
				var itemsLength = pageInfo.toRow;

				if (pageInfo.toRow > $scope.scopeModel.carrierAccountMappings.length) {
					if (pageInfo.fromRow < $scope.scopeModel.carrierAccountMappings.length)
						itemsLength = $scope.scopeModel.carrierAccountMappings.length;
					else
						return;
				}

				var items = [];

				for (var i = pageInfo.fromRow - 1; i < itemsLength; i++) {
					var currentCarrierAccountMapping = $scope.scopeModel.carrierAccountMappings[i];
					defineCarrierAccountMappingTabs(currentCarrierAccountMapping);
					items.push(currentCarrierAccountMapping);
				}
				gridAPI.addItemsToSource(items);
			}
			function defineCarrierAccountMappingTabs(carrierAccountMapping) {

				var drillDownTabs = [];

				if (showCustomerMapping(carrierAccountMapping)) {
					drillDownTabs.push(buildCustomerMappingDrillDownTab());
				}

				if (showSupplierMapping(carrierAccountMapping)) {
					drillDownTabs.push(buildSupplierMappingDrillDownTab());
				}

				setDrillDownTabs();

				function buildCustomerMappingDrillDownTab() {
					var drillDownTab = {};
					drillDownTab.title = "In";
					drillDownTab.directive = "whs-routesync-ericsson-customermapping";

					drillDownTab.loadDirective = function (customerMappingGridAPI, carrierAccountMapping) {
						carrierAccountMapping.customerMappingGridAPI = customerMappingGridAPI;
						return carrierAccountMapping.customerMappingGridAPI.load(buildCustomerMappingPayload(carrierAccountMapping));
					};

					function buildCustomerMappingPayload(carrierAccountMapping) {
						var customerMappingPayload = {};
						//customerMappingPayload.carrierAccountId = carrierAccountMapping.CarrierAccountId;
						customerMappingPayload.customerMapping = carrierAccountMapping.CustomerMapping;
						customerMappingPayload.context = buildCustomerMappingDirectiveContext(carrierAccountMapping);
						return customerMappingPayload;
					}

					return drillDownTab;
				}
				function buildSupplierMappingDrillDownTab() {
					var drillDownTab = {};
					drillDownTab.title = "Out";
					drillDownTab.directive = "whs-routesync-ericsson-suppliermapping";

					drillDownTab.loadDirective = function (supplierMappingGridAPI, carrierAccountMapping) {
						carrierAccountMapping.supplierMappingGridAPI = supplierMappingGridAPI;
						return carrierAccountMapping.supplierMappingGridAPI.load(buildSupplierMappingQuery(carrierAccountMapping));
					};

					function buildSupplierMappingQuery(carrierAccountMapping) {
						var supplierMappingQuery = {};
						//supplierMappingQuery.carrierAccountId = carrierAccountMapping.CarrierAccountId;
						supplierMappingQuery.supplierMapping = carrierAccountMapping.SupplierMapping;
						supplierMappingQuery.context = buildSupplierMappingDirectiveContext(carrierAccountMapping);
						return supplierMappingQuery;
					}

					return drillDownTab;
				}
				function setDrillDownTabs() {
					var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
					drillDownManager.setDrillDownExtensionObject(carrierAccountMapping);
				}
			}
			function showCustomerMapping(carrierMappingItem) {
				if (WhS_BE_CarrierAccountTypeEnum.Exchange.value == carrierMappingItem.CarrierAccountType || WhS_BE_CarrierAccountTypeEnum.Customer.value == carrierMappingItem.CarrierAccountType)
					return true;
				return false;
			}
			function showSupplierMapping(carrierMappingItem) {
				if (WhS_BE_CarrierAccountTypeEnum.Exchange.value == carrierMappingItem.CarrierAccountType || WhS_BE_CarrierAccountTypeEnum.Supplier.value == carrierMappingItem.CarrierAccountType)
					return true;
				return false;
			}

			function extendCarrierAccountMapping(carrierAccountMapping) {
				if (carrierAccountMapping == undefined)
					return;

				if (carrierAccountMapping.CustomerMapping != undefined) {
					var customerMapping = carrierAccountMapping.CustomerMapping;

					var isCustomerMappingExists = customerMapping != undefined && customerMapping.BO != undefined && customerMapping.BO != "";
					if (isCustomerMappingExists) {
						carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMapping);
					} else {
						carrierAccountMapping.CustomerMappingDescription = "";
					}
				}

				if (carrierAccountMapping.SupplierMapping != undefined) {
					var supplierMapping = carrierAccountMapping.SupplierMapping;

					var isSupplierMappingExists = supplierMapping != undefined;
					if (isSupplierMappingExists) {
						carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMapping);
					} else {
						carrierAccountMapping.SupplierMappingDescription = "";
					}
				}
			}
			function buildCustomerMappingDescription(customerMapping) {
				if (customerMapping == undefined)
					return "";

				var boDesciption = customerMapping.BO != undefined ? customerMapping.BO : "";
				var nationalOBA = customerMapping.NationalOBA != undefined ? customerMapping.NationalOBA : "";
				var internationalOBA = customerMapping.InternationalOBA != undefined ? customerMapping.InternationalOBA : "";
				//var numberOfInTrunks = customerMapping.InTrunks != undefined && customerMapping.InTrunks.length > 0 ? customerMapping.InTrunks.length : 0;
				//return "BO: '" + boDesciption + "'; National OBA: '" + nationalOBA + "'; International OBA: '" + internationalOBA + "'; # of InTrunks: " + numberOfInTrunks;

				return "BO: '" + boDesciption + "'; National OBA: '" + nationalOBA + "'; International OBA: '" + internationalOBA;
			}
			function buildCustomerMappingDirectiveContext(carrierAccountMapping) {
				var context = {
					updateErrorDescription: function (isValid, fromCustomerMapping) {
						updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping);
					},
					updateCustomerMappingDescription: function (customerMapping) {
						carrierAccountMapping.CustomerMappingDescription = buildCustomerMappingDescription(customerMapping);
					}
				};
				return context;
			}
			function buildSupplierMappingDescription(supplierMapping) {
				if (supplierMapping == undefined || supplierMapping.OutTrunks == undefined)
					return "";

				var numberOfOutTrunks = supplierMapping.OutTrunks != undefined && supplierMapping.OutTrunks.length > 0 ? supplierMapping.OutTrunks.length : 0;
				var numberOfTrunkGroups = supplierMapping.TrunkGroups != undefined && supplierMapping.TrunkGroups.length > 0 ? supplierMapping.TrunkGroups.length : 0;

				return "# of OutTrunks: " + numberOfOutTrunks + "; # of TrunkGroups: " + numberOfTrunkGroups;
			}
			function buildSupplierMappingDirectiveContext(carrierAccountMapping) {
				var context = {
					updateErrorDescription: function (isValid, fromCustomerMapping) {
						updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping);
					},
					updateSupplierMappingDescription: function (supplierMapping) {
						carrierAccountMapping.SupplierMappingDescription = buildSupplierMappingDescription(supplierMapping);
					}
				};
				return context;
			}
			function updateErrorDescription(carrierAccountMapping, isValid, fromCustomerMapping) {

				if (fromCustomerMapping) {
					carrierAccountMapping.isCustomerMappingInvalid = !isValid;
				} else {
					carrierAccountMapping.isSupplierMappingInvalid = !isValid;
				}
			}
		}
	}]);