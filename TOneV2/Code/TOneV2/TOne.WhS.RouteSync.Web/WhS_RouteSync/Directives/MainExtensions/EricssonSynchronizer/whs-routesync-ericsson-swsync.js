(function (app) {

	'use strict';

	EricssonSWSync.$inject = ["UtilsService", 'VRUIUtilsService'];

	function EricssonSWSync(UtilsService, VRUIUtilsService) {
		return {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new EricssonSWSyncronizerCtor($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "Ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonSWSyncTemplate.html"
		};

		function EricssonSWSyncronizerCtor($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var switchCommunicationAPI;
			var switchCommunicationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var outgoingTrafficCustomerSelectorAPI;
			var outgoingTrafficCustomerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var incomingTrafficSupplierSelectorAPI;
			var incomingTrafficSupplierSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var localSupplierSelectorAPI;
			var localSupplierSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var carrierAccountMappingGridAPI;
			var carrierAccountMappingGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var branchRouteSettingsAPI;
			var branchRouteSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.localSuppliers = [];
				$scope.scopeModel.brList = [];

				//$scope.scopeModel.validateBranchRoutes = function () {
				//	if ($scope.scopeModel.brList == undefined || $scope.scopeModel.brList.length < 1)
				//		return "At least one branch route should be added.";
				//	return null;
				//};

				$scope.scopeModel.isBRValid = function () {
					var brIsValid = true;
					if ($scope.scopeModel.brToAdd == undefined || $scope.scopeModel.brToAdd.length == 0) {
						brIsValid = false;
					}
					else {
						angular.forEach($scope.scopeModel.brList, function (item) {
							if ($scope.scopeModel.brToAdd === item.Name) {
								brIsValid = false;
							}
						});
					}
					return brIsValid;
				};

				$scope.scopeModel.addBR = function () {
					var br = {
						Name: $scope.scopeModel.brToAdd,
						IncludeTrunkAsSwitch: false
					};

					$scope.scopeModel.brList.push(br);
					$scope.scopeModel.brToAdd = undefined;
				};

				$scope.scopeModel.onEricssonSwitchCommunicationReady = function (api) {
					switchCommunicationAPI = api;
					switchCommunicationReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onBranchRouteSettingsReady = function (api) {
					branchRouteSettingsAPI = api;
					branchRouteSettingsReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onOutgoingTrafficCustomerSelectorReady = function (api) {
					outgoingTrafficCustomerSelectorAPI = api;
					outgoingTrafficCustomerSelectorReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onIncomingTrafficSupplierSelectorReady = function (api) {
					incomingTrafficSupplierSelectorAPI = api;
					incomingTrafficSupplierSelectorReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onLocalSuppliersSelectorReady = function (api) {
					localSupplierSelectorAPI = api;
					localSupplierSelectorReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onCarrierAccountMappingGridReady = function (api) {
					carrierAccountMappingGridAPI = api;
					carrierAccountMappingGridReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onSelectLocalSupplier = function (selectedItem) {

					$scope.scopeModel.localSuppliers.push({
						CarrierAccountId: selectedItem.CarrierAccountId,
						Name: selectedItem.Name
					});
				};

				$scope.scopeModel.onDeselectLocalSupplier = function (deselectedItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.localSuppliers, deselectedItem.CarrierAccountId, 'CarrierAccountId');
					$scope.scopeModel.localSuppliers.splice(index, 1);
				};

				$scope.scopeModel.onDeleteLocalSupplierRow = function (deletedItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedLocalSuppliers, deletedItem.CarrierAccountId, 'CarrierAccountId');
					$scope.scopeModel.selectedLocalSuppliers.splice(index, 1);
					$scope.scopeModel.onDeselectLocalSupplier(deletedItem);
				};

				$scope.scopeModel.isLocalSuppliersValid = function () {
					var localSuppliersLength = $scope.scopeModel.localSuppliers.length;
					if (localSuppliersLength == 0)
						return "you should define at least one local supplier";

					return null;
				};

				defineAPI();
			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];

					var ericssonSWSync;
					var outgoingTrafficCustomers;
					var incomingTrafficSuppliers;
					var localSupplierMappings;
					var carrierMappings;
					var sshCommunicationList;
					var switchLoggerList;
					var branchRouteSettings;

					if (payload != undefined) {
						ericssonSWSync = payload.switchSynchronizerSettings;

						if (ericssonSWSync != undefined) {
							$scope.scopeModel.isEditMode = true;
							$scope.scopeModel.firstRCNumber = ericssonSWSync.FirstRCNumber;
							$scope.scopeModel.numberOfMappings = ericssonSWSync.NumberOfMappings;
							$scope.scopeModel.minCodeLength = ericssonSWSync.MinCodeLength;
							$scope.scopeModel.maxCodeLength = ericssonSWSync.MaxCodeLength;
							$scope.scopeModel.localCountryCode = ericssonSWSync.LocalCountryCode;
							$scope.scopeModel.interconnectGeneralPrefix = ericssonSWSync.InterconnectGeneralPrefix;
							$scope.scopeModel.brList = (ericssonSWSync.BranchRoutes != undefined) ? ericssonSWSync.BranchRoutes : [];
							$scope.scopeModel.esr = ericssonSWSync.ESR;
							$scope.scopeModel.cc = ericssonSWSync.CC;
							$scope.scopeModel.percentagePrefix = ericssonSWSync.PercentagePrefix;
							branchRouteSettings = ericssonSWSync.BranchRouteSettings;
							sshCommunicationList = ericssonSWSync.SwitchCommunicationList;
							switchLoggerList = ericssonSWSync.SwitchLoggerList;
							outgoingTrafficCustomers = ericssonSWSync.OutgoingTrafficCustomers;
							incomingTrafficSuppliers = ericssonSWSync.IncomingTrafficSuppliers;
							localSupplierMappings = ericssonSWSync.LocalSupplierMappings;
							carrierMappings = ericssonSWSync.CarrierMappings;
						}
					}
					//Loading Switch Communication
					var switchCommunicationLoadPromise = getSwitchCommunicationLoadPromise();
					promises.push(switchCommunicationLoadPromise);

					//Loading OutgoingTrafficCustomers Selector
					var outgoingTrafficCustomerSelectorLoadPromise = getOutgoingTrafficCustomerSelectorLoadPromise();
					promises.push(outgoingTrafficCustomerSelectorLoadPromise);

					//Loading IncomingTrafficSuppliers Selector
					var incomingTrafficSupplierSelectorLoadPromise = getIncomingTrafficSupplierSelectorLoadPromise();
					promises.push(incomingTrafficSupplierSelectorLoadPromise);

					//Loading LocalSupplierMapping Directive
					var localSupplierMappingDirectiveLoadPromise = getLocalSupplierMappingDirectiveLoadPromise();
					promises.push(localSupplierMappingDirectiveLoadPromise);

					//Loading CarrierAccountMapping Grid
					var carrierAccountMappingGridLoadPromise = getCarrierAccountMappingGridLoadPromise();
					promises.push(carrierAccountMappingGridLoadPromise);

					//LoadingBranchRouteSettings
					var branchRouteSettingsLoadPromise = getBranchRouteSettingsLoadPromise();
					promises.push(branchRouteSettingsLoadPromise);

					function getSwitchCommunicationLoadPromise() {
						var switchCommunicationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

						switchCommunicationReadyPromiseDeferred.promise.then(function () {
							var switchCommunicationPayload;
							if (ericssonSWSync != undefined) {
								switchCommunicationPayload = { sshCommunicationList: sshCommunicationList, switchLoggerList: switchLoggerList };
							}
							VRUIUtilsService.callDirectiveLoad(switchCommunicationAPI, switchCommunicationPayload, switchCommunicationLoadPromiseDeferred);
						});

						return switchCommunicationLoadPromiseDeferred.promise;
					}

					function getOutgoingTrafficCustomerSelectorLoadPromise() {
						var outgoingTrafficCustomerSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

						outgoingTrafficCustomerSelectorReadyPromiseDeferred.promise.then(function () {

							var outgoingTrafficCustomerSelectorPayload;
							if (outgoingTrafficCustomers != undefined) {
								outgoingTrafficCustomerSelectorPayload = { selectedIds: UtilsService.getPropValuesFromArray(outgoingTrafficCustomers, 'CustomerId') };
							}
							VRUIUtilsService.callDirectiveLoad(outgoingTrafficCustomerSelectorAPI, outgoingTrafficCustomerSelectorPayload, outgoingTrafficCustomerSelectorLoadPromiseDeferred);
						});

						return outgoingTrafficCustomerSelectorLoadPromiseDeferred.promise;
					}
					function getIncomingTrafficSupplierSelectorLoadPromise() {
						var incomingTrafficSupplierSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

						incomingTrafficSupplierSelectorReadyPromiseDeferred.promise.then(function () {

							var incomingTrafficSupplierSelectorPayload;
							if (incomingTrafficSuppliers != undefined) {
								incomingTrafficSupplierSelectorPayload = { selectedIds: UtilsService.getPropValuesFromArray(incomingTrafficSuppliers, 'SupplierId') };
							}
							VRUIUtilsService.callDirectiveLoad(incomingTrafficSupplierSelectorAPI, incomingTrafficSupplierSelectorPayload, incomingTrafficSupplierSelectorLoadPromiseDeferred);
						});

						return incomingTrafficSupplierSelectorLoadPromiseDeferred.promise;
					}
					function getLocalSupplierMappingDirectiveLoadPromise() {
						var supplierMappingDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

						localSupplierSelectorReadyPromiseDeferred.promise.then(function () {

							var localSupplierSelectorPayload;
							if (localSupplierMappings != undefined) {
								localSupplierSelectorPayload = { selectedIds: UtilsService.getPropValuesFromArray(localSupplierMappings, 'SupplierId') };
							}
							VRUIUtilsService.callDirectiveLoad(localSupplierSelectorAPI, localSupplierSelectorPayload, supplierMappingDirectiveLoadPromiseDeferred);
						});

						return supplierMappingDirectiveLoadPromiseDeferred.promise.then(function () {

							for (var i = 0; i < $scope.scopeModel.selectedLocalSuppliers.length; i++) {
								var currentLoadSupplier = $scope.scopeModel.selectedLocalSuppliers[i];
								var localSupplierMapping = localSupplierMappings[i];

								$scope.scopeModel.localSuppliers.push({
									CarrierAccountId: currentLoadSupplier.CarrierAccountId,
									Name: currentLoadSupplier.Name,
									BO: localSupplierMapping.BO
								});
							}
						});
					}
					function getCarrierAccountMappingGridLoadPromise() {
						var carrierAccountMappingGridLoadDeferred = UtilsService.createPromiseDeferred();

						carrierAccountMappingGridReadyPromiseDeferred.promise.then(function () {
							var carrierAccountMappingGridPayload = { carrierMappings: carrierMappings };
							VRUIUtilsService.callDirectiveLoad(carrierAccountMappingGridAPI, carrierAccountMappingGridPayload, carrierAccountMappingGridLoadDeferred);
						});

						return carrierAccountMappingGridLoadDeferred.promise;
					}

					function getBranchRouteSettingsLoadPromise() {
						var branchRouteSettingsLoadPromise = UtilsService.createPromiseDeferred();
						branchRouteSettingsReadyPromiseDeferred.promise.then(function () {
							var branchRouteSettingsPayload = { branchRouteSettings: branchRouteSettings };
							VRUIUtilsService.callDirectiveLoad(branchRouteSettingsAPI, branchRouteSettingsPayload, branchRouteSettingsLoadPromise);
						});

						return branchRouteSettingsLoadPromise.promise;
					}

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {

					function getIncomingTrafficSuppliers() {
						var incomingTrafficSupplierIds = incomingTrafficSupplierSelectorAPI.getSelectedIds();
						if (incomingTrafficSupplierIds == undefined || incomingTrafficSupplierIds.length == 0)
							return null;

						var incomingTrafficSuppliers = [];
						for (var index = 0; index < incomingTrafficSupplierIds.length; index++) {
							incomingTrafficSuppliers.push({ SupplierId: incomingTrafficSupplierIds[index] });
						}
						return incomingTrafficSuppliers;
					}
					function getOutgoingTrafficCustomers() {
						var outgoingTrafficCustomerIds = outgoingTrafficCustomerSelectorAPI.getSelectedIds();
						if (outgoingTrafficCustomerIds == undefined || outgoingTrafficCustomerIds.length == 0)
							return null;

						var outgoingTrafficCustomers = [];
						for (var index = 0; index < outgoingTrafficCustomerIds.length; index++) {
							outgoingTrafficCustomers.push({ CustomerId: outgoingTrafficCustomerIds[index] });
						}
						return outgoingTrafficCustomers;
					}
					function getLocalSupplierMappings() {
						var localSupplierMappings = [];
						for (var i = 0; i < $scope.scopeModel.localSuppliers.length; i++) {
							var localSupplier = $scope.scopeModel.localSuppliers[i];
							localSupplierMappings.push({ SupplierId: localSupplier.CarrierAccountId, BO: localSupplier.BO });
						}
						return localSupplierMappings;
					}

					var switchCommunicationData = switchCommunicationAPI.getData();
					var data = {
						$type: "TOne.WhS.RouteSync.Ericsson.EricssonSWSync, TOne.WhS.RouteSync.Ericsson",
						NumberOfMappings: $scope.scopeModel.numberOfMappings,
						MinCodeLength: $scope.scopeModel.minCodeLength,
						MaxCodeLength: $scope.scopeModel.maxCodeLength,
						LocalCountryCode: $scope.scopeModel.localCountryCode,
						InterconnectGeneralPrefix: $scope.scopeModel.interconnectGeneralPrefix,
						IncomingTrafficSuppliers: getIncomingTrafficSuppliers(),
						OutgoingTrafficCustomers: getOutgoingTrafficCustomers(),
						LocalSupplierMappings: getLocalSupplierMappings(),
						CarrierMappings: carrierAccountMappingGridAPI.getData(),
						SwitchCommunicationList: switchCommunicationData != undefined ? switchCommunicationData.sshCommunicationList : undefined,
						SwitchLoggerList: switchCommunicationData != undefined ? switchCommunicationData.switchLoggerList : undefined,
						FirstRCNumber: $scope.scopeModel.firstRCNumber,
						BranchRoutes: $scope.scopeModel.brList,
						BranchRouteSettings: (branchRouteSettingsAPI != undefined) ? branchRouteSettingsAPI.getData() : null,
						ESR: $scope.scopeModel.esr,
						CC: $scope.scopeModel.cc,
						PercentagePrefix: $scope.scopeModel.percentagePrefix
					};
					return data;
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
					ctrl.onReady(api);
				}
			}
		}
	}

	app.directive('whsRoutesyncEricssonSwsync', EricssonSWSync);

})(app);