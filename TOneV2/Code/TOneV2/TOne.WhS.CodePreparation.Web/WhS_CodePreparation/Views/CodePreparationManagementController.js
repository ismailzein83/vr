(function (appControllers) {

	'use strict';

	CodePreparationManagementController.$inject = ['$window', '$scope', 'WhS_CP_CodePrepAPIService', 'WhS_BP_CreateProcessResultEnum', 'VRUIUtilsService', 'UtilsService', 'VRCommon_CountryAPIService', 'WhS_BE_SaleZoneAPIService', 'VRModalService', 'VRNotificationService', 'WhS_CP_NewCPOutputResultEnum', 'WhS_CP_ZoneItemDraftStatusEnum', 'WhS_CP_ZoneItemStatusEnum', 'WhS_CP_ValidationOutput', 'WhS_CP_CodePrepService', 'WhS_BE_SellingNumberPlanAPIService', 'WhS_CP_NumberingPlanDefinitionEnum', 'BusinessProcess_BPInstanceService', 'VRCommon_VRExclusiveSessionTypeService', 'VRCommon_VRExclusiveSessionTypeAPIService', 'WhS_BE_ExclusiveSessionTypeIdEnum', 'WhS_BE_ExclusiveSessionTargetIdPrefixEnum'];

	function CodePreparationManagementController($window, $scope, WhS_CP_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, VRUIUtilsService, UtilsService, VRCommon_CountryAPIService, WhS_BE_SaleZoneAPIService, VRModalService, VRNotificationService, WhS_CP_NewCPOutputResultEnum, WhS_CP_ZoneItemDraftStatusEnum, WhS_CP_ZoneItemStatusEnum, WhS_CP_ValidationOutput, WhS_CP_CodePrepService, WhS_BE_SellingNumberPlanAPIService, WhS_CP_NumberingPlanDefinitionEnum, BusinessProcess_BPInstanceService, VRCommon_VRExclusiveSessionTypeService, VRCommon_VRExclusiveSessionTypeAPIService, WhS_BE_ExclusiveSessionTypeIdEnum, WhS_BE_ExclusiveSessionTargetIdPrefixEnum) {

		//#region Global Variables

		var sellingNumberPlanDirectiveAPI;
		var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();
		var treeAPI;
		var codesGridAPI;
		var countries = [];
		var filter;
		var incrementalId = 0;

		var exclusiveSessionObject = null;
		//#endregion

		//#region Load

		defineScope();
		loadParameters();
		load();

		//#endregion

		//#region Functions

		function defineScope() {
			$scope.nodes = [];
			$scope.sellingNumberPlans = [];
			$scope.selectedSellingNumberPlan;
			$scope.currentNode;


			hideShowSaleCodes(false);

			$scope.selectedCodes = [];

			$scope.applyCodePreparationState = function () {
				var onCodePreparationApplied = function () {
					onSellingNumberPlanSelectorChanged();
				};
				return WhS_CP_CodePrepAPIService.ApplyCodePreparationState(filter.sellingNumberPlanId, onCodePreparationApplied);
			};

			$scope.uploadCodePreparation = function () {
				var onCodePreparationUpdated = function () {
					onSellingNumberPlanSelectorChanged();
				};
				return WhS_CP_CodePrepAPIService.UploadCodePreparationSheet(filter.sellingNumberPlanId, onCodePreparationUpdated);
			};

			$scope.onSellingNumberPlanSelectorReady = function (api) {
				sellingNumberPlanDirectiveAPI = api;
				sellingNumberPlanReadyPromiseDeferred.resolve();
			};

			$scope.countriesTreeReady = function (api) {
				treeAPI = api;
			};

			$scope.onNodeSelection = function () {
				$scope.isLoadingCurrentNode = true;
				hideShowAddZone(false);

				var promiseDeffered = UtilsService.createPromiseDeferred();
				clearCodesSelection();
				if ($scope.currentNode != undefined) {
					if ($scope.currentNode.type == 'Zone') {
						codesGridAPI.clearUpdatedItems();
						codesGridAPI.loadGrid(getCodesFilterObject()).then(function () {
							setZoneStateVisibility($scope.currentNode.DraftStatus, $scope.currentNode.status);
							promiseDeffered.resolve();
						});
					}
					else {
						setCountryStateVisibility();
						promiseDeffered.resolve();
					}
				}
				else promiseDeffered.resolve();

				return promiseDeffered.promise.then(function () {
					$scope.isLoadingCurrentNode = false;
				});

			};

			$scope.loadEffectiveSaleZones = function (countryNode) {
				var effectiveZonesPromiseDeffered = UtilsService.createPromiseDeferred();
				WhS_CP_CodePrepAPIService.GetZoneItems(filter.sellingNumberPlanId, countryNode.countryId).then(function (response) {
					var effectiveZones = [];
					angular.forEach(response, function (itm) {
						effectiveZones.push(mapZoneToNode(itm));
					});

					var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, countryNode.countryId, 'countryId');
					var currentCountry = $scope.nodes[countryIndex];
					currentCountry.effectiveZones = effectiveZones;

					effectiveZonesPromiseDeffered.resolve(effectiveZones);
				});
				return effectiveZonesPromiseDeffered.promise;
			};

			$scope.onSellingNumberPlanSelectorChanged = function () {

				releaseSession();
				var selectedSellingNumberingPlan = sellingNumberPlanDirectiveAPI.getSelectedIds();
				if (selectedSellingNumberingPlan != undefined) {
					$scope.isLoading = true;
					WhS_CP_CodePrepService.hasRunningProcessesForSellingNumberPlan(selectedSellingNumberingPlan).then(function (response) {
						var targetId = WhS_BE_ExclusiveSessionTargetIdPrefixEnum.NumberingPlan.value + selectedSellingNumberingPlan;
						tryTakeSession(targetId).then(function (response) {
							if (response.IsSucceeded) {
								$scope.isLoading = false;
								onSellingNumberPlanSelectorChanged();
							}
							else
								onTryTakeFailure(response).then(function () {
									$scope.isLoading = false;
								});
						});
					}).catch(function (error) {
						VRNotificationService.notifyException(error, $scope);
					});
				}
				else
					$scope.currentNode = undefined;

			};


			$scope.saleCodesGridReady = function (api) {

				codesGridAPI = api;
			};

			$scope.newZoneClicked = function () {
				var parameters = {
					CountryId: $scope.currentNode.countryId,
					CountryName: $scope.currentNode.nodeName,
					SellingNumberPlanId: filter.sellingNumberPlanId
				};
				var settings = {};
				settings.onScopeReady = function (modalScope) {
					modalScope.onZoneAdded = onZoneAdded;
				};

				VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/NewZoneDialog.html", parameters, settings);
			};

			$scope.newCodeClicked = function () {
				var parameters = {
					ZoneId: $scope.currentNode.zoneId,
					ZoneName: $scope.currentNode.nodeName,
					SellingNumberPlanId: filter.sellingNumberPlanId,
					CountryId: $scope.currentNode.countryId,
					ZoneStatus: $scope.currentNode.status
				};
				var settings = {};
				settings.onScopeReady = function (modalScope) {
					modalScope.onCodeAdded = onCodeAdded;
				};

				VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/NewCodeDialog.html", parameters, settings);
			};

			$scope.moveCodesClicked = function () {
				var codes = codesGridAPI.getSelectedCodes();
				var codesData = [];
				for (var i = 0; i < codes.length; i++) {
					var codeData = {
						Code: codes[i].Code,
						BED: codes[i].BED
					};

					codesData.push(codeData);
				}

				var parameters = {
					ZoneId: $scope.currentNode.zoneId,
					ZoneName: $scope.currentNode.nodeName,
					SellingNumberPlanId: filter.sellingNumberPlanId,
					CountryId: $scope.currentNode.countryId,
					ZoneDataSource: GetCurrentCountryNodeZones(),
					Codes: codesData
				};
				var settings = {};
				settings.onScopeReady = function (modalScope) {
					modalScope.onCodesMoved = onCodesMoved;
				};

				VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/MoveCodeDialog.html", parameters, settings);
			};

			$scope.endClicked = function () {
				return ($scope.selectedCodes.length > 0) ? closeCodes() : closeZone();
			};

			$scope.cancelState = function () {
				return VRNotificationService.showConfirmation().then(function (result) {
					if (result) {
						countries.length = 0;
						return WhS_CP_CodePrepAPIService.CancelCodePreparationState(filter.sellingNumberPlanId).then(function (response) {
							$scope.hasState = !response;
							onSellingNumberPlanSelectorChanged();
						});
					}
				});

			};

			$scope.renameZoneClicked = function () {
				var parameters = {
					ZoneId: $scope.currentNode.zoneId,
					ZoneName: $scope.currentNode.nodeName,
					SellingNumberPlanId: filter.sellingNumberPlanId,
					CountryId: $scope.currentNode.countryId,
				};
				var settings = {};
				settings.onScopeReady = function (modalScope) {
					modalScope.onZoneRenamed = onZoneRenamed;
				};

				VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/RenameZoneDialog.html", parameters, settings);

			};

			$scope.buildTreeId = function (item) {
				return buildTreeId(item);
			};

			$window.onbeforeunload = function () {
				releaseSession();
			};
		}


		function loadParameters() {
		}

		function load() {
			$scope.isLoadingFilter = true;

			UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlans]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.isLoadingFilter = false;
			});
		}



		function loadSellingNumberPlans() {
			var loadSNPPromiseDeferred = UtilsService.createPromiseDeferred();
			sellingNumberPlanReadyPromiseDeferred.promise.then(function () {

				VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, { selectifsingleitem: true }, loadSNPPromiseDeferred);
			});

			return loadSNPPromiseDeferred.promise;
		}

		function onSellingNumberPlanSelectorChanged() {
			var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
			setSellingNumberPlanChangedStateVisibility();
			if (selectedSellingNumberPlanId != undefined) {
				filter = getFilter();
				$scope.isLoadingCountries = true;
				UtilsService.waitMultipleAsyncOperations([getCountries, checkState]).then(function () {
					buildCountriesTree();
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				}).finally(function () {

					$scope.isLoadingCountries = false;
				});
			}
		}

		function onZoneAdded(addedZones) {
			if (addedZones != undefined) {
				hideShowStateAndClearSelection(true);
				var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.countryId, 'countryId');
				var countryNode = $scope.nodes[countryIndex];
				for (var i = 0; i < addedZones.length; i++) {
					var node = mapZoneToNode(addedZones[i]);
					node.isOpened = true;
					treeAPI.createNode(node);
					countryNode.effectiveZones.push(node);
				}
				//  treeAPI.refreshTree($scope.nodes);
			}
		}

		function onZoneClosed() {
			hideShowStateAndClearSelection(true);

			var zoneNode = getCurrentZoneNode();
			var draftStatus = WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value;


			$scope.currentNode.DraftStatus = draftStatus;
			$scope.currentNode.icon = WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.icon;
			zoneNode.DraftStatus = draftStatus;
			zoneNode.icon = WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.icon;

			hideShowRenameZone(draftStatus, zoneNode.Status);
			hideShowEnd(draftStatus, zoneNode.Status);
			hideShowAddCode(draftStatus, zoneNode.Status);

			treeAPI.changeNodeIcon(WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.icon);

			return codesGridAPI.loadGrid(getCodesFilterObject());
		}

		function onZoneRenamed(renamedZone) {
			hideShowStateAndClearSelection(true);

			var zoneNode = getCurrentZoneNode();
			var draftStatus = $scope.currentNode.zoneId != null ? WhS_CP_ZoneItemDraftStatusEnum.Renamed.value : WhS_CP_ZoneItemDraftStatusEnum.New.value;
			var icon = $scope.currentNode.zoneId != null ? WhS_CP_ZoneItemDraftStatusEnum.Renamed.icon : WhS_CP_ZoneItemDraftStatusEnum.New.icon;

			$scope.currentNode.nodeName = renamedZone.NewZoneName;
			$scope.currentNode.DraftStatus = draftStatus;

			zoneNode.DraftStatus = draftStatus;
			zoneNode.icon = icon;
			zoneNode.nodeName = renamedZone.NewZoneName;

			hideShowEnd(draftStatus, zoneNode.Status);

			treeAPI.renameNode(renamedZone.NewZoneName);
			treeAPI.changeNodeIcon(icon);

			return codesGridAPI.loadGrid(getCodesFilterObject());
		}

		function onCodeAdded(addedCodes) {
			if (addedCodes != undefined) {
				hideShowStateAndClearSelection(true);
				for (var i = 0; i < addedCodes.length; i++)
					codesGridAPI.onCodeAdded(addedCodes[i]);

				if ($scope.currentNode != undefined) {
					if ($scope.currentNode.type == 'Zone') {
						return codesGridAPI.loadGrid(getCodesFilterObject());
					}
				}
			}

		}

		function onCodesMoved(movedCodes) {

			if (movedCodes != undefined) {
				hideShowStateAndClearSelection(true);
				for (var i = 0; i < movedCodes.length; i++)
					codesGridAPI.onCodeClosed(movedCodes[i]);

				if ($scope.currentNode != undefined) {
					if ($scope.currentNode.type == 'Zone') {
						return codesGridAPI.loadGrid(getCodesFilterObject()).then(function () {
							setZoneStateVisibility($scope.currentNode.DraftStatus, $scope.currentNode.status);
						});
					}
				}
			}

		}

		function onCodesClosed(closedCodes) {
			if (closedCodes != undefined) {
				hideShowStateAndClearSelection(true);

				for (var i = 0; i < closedCodes.length; i++)
					codesGridAPI.onCodeClosed(closedCodes[i]);

				if ($scope.currentNode != undefined) {
					if ($scope.currentNode.type == 'Zone') {
						return codesGridAPI.loadGrid(getCodesFilterObject()).then(function () {
							setZoneStateVisibility($scope.currentNode.DraftStatus, $scope.currentNode.status);
						});
					}
				}
			}
		}

		function closeCodes() {

			var codes = codesGridAPI.getSelectedCodes();
			var parameters = {
				ZoneId: $scope.currentNode.zoneId,
				ZoneName: $scope.currentNode.nodeName,
				SellingNumberPlanId: filter.sellingNumberPlanId,
				CountryId: $scope.currentNode.countryId,
				Codes: UtilsService.getPropValuesFromArray(codes, 'Code')
			};
			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onCodesClosed = onCodesClosed;
			};

			VRModalService.showModal("/Client/Modules/WhS_CodePreparation/Views/Dialogs/CloseCodeDialog.html", parameters, settings);
		}

		function closeZone() {
			VRNotificationService.showConfirmation("Are you sure you want to close " + $scope.currentNode.nodeName + " zone").then(function (result) {
				if (result) {
					$scope.isLoadingFilter = true;
					var zoneInput = {
						SellingNumberPlanId: filter.sellingNumberPlanId,
						CountryId: $scope.currentNode.countryId,
						ZoneId: $scope.currentNode.zoneId,
						ZoneName: $scope.currentNode.nodeName,
					};
					return WhS_CP_CodePrepAPIService.CloseZone(zoneInput)
						.then(function (response) {
							if (response.Result == WhS_CP_ValidationOutput.Success.value) {
								onZoneClosed();
								VRNotificationService.showSuccess(response.Message);
							}
							else if (response.Result == WhS_CP_ValidationOutput.ValidationError.value) {
								WhS_CP_CodePrepService.NotifyValidationWarning(response.Message);
							}
						}).catch(function (error) {
							VRNotificationService.notifyException(error, $scope);
						})
						.finally(function () {
							$scope.isLoadingFilter = false;
						});

				}
			});
		}

		function clearCodesSelection() {
			$scope.selectedCodes.length = 0;
		}

		function buildTreeId(item) {
			if (item.type == "Country")
				return "Country_" + item.countryId;
			else if (item.zoneId != undefined)
				return "Zone_" + item.zoneId;
			else
				return "Zone_" + incrementalId++;
		}

		//#region "Hide Show"

		function setZoneStateVisibility(draftStatus, status) {
			hideShowRenameZone(draftStatus, status);
			hideShowEnd(draftStatus, status);
			hideShowAddCode(draftStatus, status);
			hideShowSaleCodes(true);
			hideShowAddZone(false);
		}

		function setCountryStateVisibility() {
			hideShowSaleCodes(false);
			$scope.showAddNewCode = false;
			hideShowAddZone(true);
			$scope.showRenameZone = false;
			$scope.showEnd = false;
		}

		function setSellingNumberPlanChangedStateVisibility() {
			hideShowSaleCodes(false);
			$scope.showAddNewCode = false;
			hideShowAddZone(false);
			$scope.showRenameZone = false;
			$scope.showEnd = false;
			hideShowStateAndClearSelection(false);
			clearCodesSelection();
		}

		function hideShowRenameZone(draftStatus, status) {
			if (status != null)
				$scope.showRenameZone = false;
			else
				$scope.showRenameZone = draftStatus == WhS_CP_ZoneItemDraftStatusEnum.New.value;
		}

		function hideShowEnd(draftStatus, status) {
			if (status != null)
				$scope.showEnd = false;
			else
				$scope.showEnd = draftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value && draftStatus != WhS_CP_ZoneItemDraftStatusEnum.New.value && draftStatus != WhS_CP_ZoneItemDraftStatusEnum.Renamed.value;
		}

		function hideShowAddCode(draftStatus, status) {
			if (status != null)
				$scope.showAddNewCode = false;
			else
				$scope.showAddNewCode = draftStatus != WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value;
		}

		function hideShowAddZone(flag) {
			$scope.showAddNewZone = flag;
		}

		function hideShowSaleCodes(flag) {
			$scope.showGrid = flag;
		}

		function hideShowStateAndClearSelection(flag) {
			$scope.hasState = flag;
			clearCodesSelection();
		}


		//#endRegion

		function getCurrentZoneNode() {
			var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.countryId, 'countryId');
			var countryNode = $scope.nodes[countryIndex];

			var zoneIndex = UtilsService.getItemIndexByVal(countryNode.effectiveZones, $scope.currentNode.nodeName, 'nodeName');

			return countryNode.effectiveZones[zoneIndex];
		}

		function GetCurrentCountryNodeZones() {
			var countryIndex = UtilsService.getItemIndexByVal($scope.nodes, $scope.currentNode.countryId, 'countryId');
			var countryNode = $scope.nodes[countryIndex];
			return countryNode.effectiveZones;
		}

		function getZoneInfos(zoneNodes) {
			var zoneInfos = [];
			angular.forEach(zoneNodes, function (itm) {
				zoneInfos.push(mapZoneInfoFromNode(itm));
			});

			return zoneInfos;
		}

		function mapZoneInfoFromNode(zoneItem) {
			return {
				// SaleZoneId: zoneItem.zoneId,
				Name: zoneItem.nodeName
			};
		}

		function getCountries() {
			countries.length = 0;
			return VRCommon_CountryAPIService.GetCountriesInfo().then(function (response) {
				angular.forEach(response, function (itm) {
					countries.push(itm);
				});
			});
		}

		function checkState() {
			countries.length = 0;
			return WhS_CP_CodePrepAPIService.CheckCodePreparationState(filter.sellingNumberPlanId).then(function (response) {
				$scope.hasState = response;
			});
		}

		function buildCountriesTree() {
			$scope.nodes.length = 0;

			for (var i = 0; i < countries.length; i++) {
				var node = mapCountryToNode(countries[i]);
				$scope.nodes.push(node);
			}
			treeAPI.refreshTree($scope.nodes);


		}

		function mapCountryToNode(country) {
			return {
				countryId: country.CountryId,
				nodeName: country.Name,
				effectiveZones: [],
				hasRemoteChildren: true,
				type: 'Country'
			};
		}

		function mapZoneToNode(zoneInfo) {
			var icon = "Client/Modules/WhS_BusinessEntity/Images/Zone.png";
			if ($scope.hasState) {
				switch (zoneInfo.DraftStatus) {
					case WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.value:
						icon = WhS_CP_ZoneItemDraftStatusEnum.ExistingClosed.icon;
						break;
					case WhS_CP_ZoneItemDraftStatusEnum.New.value:
						icon = WhS_CP_ZoneItemDraftStatusEnum.New.icon;
						break;
					case WhS_CP_ZoneItemDraftStatusEnum.Renamed.value:
						icon = WhS_CP_ZoneItemDraftStatusEnum.Renamed.icon;
						break;
				}
			}

			if (zoneInfo.Status != null) {
				switch (zoneInfo.Status) {
					case WhS_CP_ZoneItemStatusEnum.PendingClosed.value:
						icon = WhS_CP_ZoneItemStatusEnum.PendingClosed.icon;
						break;
					case WhS_CP_ZoneItemStatusEnum.PendingEffective.value:
						icon = WhS_CP_ZoneItemStatusEnum.PendingEffective.icon;
						break;
				}
			}

			return {
				zoneId: zoneInfo.ZoneId,
				nodeName: zoneInfo.Name,
				hasRemoteChildren: false,
				effectiveZones: [],
				type: 'Zone',
				status: zoneInfo.Status,
				DraftStatus: zoneInfo.DraftStatus,
				countryId: zoneInfo.CountryId,
				icon: icon
			};
		}


		//#endregion

		//#region Filters
		function getCodesFilterObject() {
			return {
				SellingNumberPlanId: filter.sellingNumberPlanId,
				ZoneId: $scope.currentNode.zoneId,
				ZoneName: $scope.currentNode.nodeName,
				ZoneItemStatus: $scope.currentNode.status,
				CountryId: $scope.currentNode.countryId,
				ShowDraftStatus: $scope.hasState,
				ShowSelectCode: $scope.currentNode.status != WhS_CP_ZoneItemStatusEnum.PendingClosed.value && $scope.currentNode.DraftStatus != WhS_CP_ZoneItemDraftStatusEnum.Renamed.value
			};
		}

		function getFilter() {
			return {
				sellingNumberPlanId: sellingNumberPlanDirectiveAPI.getSelectedIds()
			};
		}


		//#endregion

		function tryTakeSession(targetId) {
			var exclusiveSessionInput = {
				SessionTypeId: WhS_BE_ExclusiveSessionTypeIdEnum.SaleArea.value,
				TargetId: targetId,
			};
			var promiseDeffered = UtilsService.createPromiseDeferred();
			VRCommon_VRExclusiveSessionTypeService.tryTakeSession($scope, exclusiveSessionInput).then(function (response) {
				promiseDeffered.resolve(response);
				if (response.IsSucceeded) {
					response.onTryTakeFailure = onTryTakeFailure;
					$scope.$on("$destroy", function () {
						response.Release();
					});
					exclusiveSessionObject = response;
				}
			}).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			});
			return promiseDeffered.promise;
		}
		function onTryTakeFailure(failureObject) {
			return VRNotificationService.showPromptWarning(failureObject.FailureMessage).then(function () {
				$scope.selectedSellingNumberPlan = null;
			});
		}

		function releaseSession() {
			if (exclusiveSessionObject != null) {
				exclusiveSessionObject.Release();
				exclusiveSessionObject = null;
			}
		}

	}

	appControllers.controller('WhS_CP_CodePreparationManagementController', CodePreparationManagementController);

})(appControllers);
