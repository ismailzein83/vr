(function (appControllers) {
	"use strict";
	routeTableRouteEditorController.$inject = ['$scope', 'NP_IVSwitch_RouteTableRouteAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RouteRuleCriteriaTypeEnum', 'WhS_Routing_RouteRuleAPIService', 'NP_IVSwitch_RouteTableViewTypeEnum', 'NP_IVSwitch_RouteTableAPIService'];

	function routeTableRouteEditorController($scope, NP_IVSwitch_RouteTableRouteAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Routing_RouteRuleCriteriaTypeEnum, WhS_Routing_RouteRuleAPIService, NP_IVSwitch_RouteTableViewTypeEnum, NP_IVSwitch_RouteTableAPIService) {
		var routeTableId;
		var routeTableViewType;
		var routeTableRouteName;
		var isEditMode;
		var routeTableRouteOptions;
		$scope.scopeModel = {};
		var excludedCarrierAccountIds;
		var codeListDirectiveAPI;
		var codeListDirectiveDefferedReady = UtilsService.createPromiseDeferred();

		var supplierRouteGridAPI;
		var supplierRouteGridDefferedReady = UtilsService.createPromiseDeferred();
		var isBlockedAccount;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);
			if (parameters != undefined && parameters != null) {
				routeTableViewType = parameters.RouteTableViewType;
				switch (routeTableViewType) {
					case NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value:
						$scope.scopeModel.labelName = NP_IVSwitch_RouteTableViewTypeEnum.ANumber.description;

						break;
					case NP_IVSwitch_RouteTableViewTypeEnum.Whitelist.value:
						$scope.scopeModel.labelName = NP_IVSwitch_RouteTableViewTypeEnum.Whitelist.description;

						break;
					case NP_IVSwitch_RouteTableViewTypeEnum.BNumber.value:
						$scope.scopeModel.labelName = NP_IVSwitch_RouteTableViewTypeEnum.BNumber.description;

						break;
				}
				routeTableId = parameters.RouteTableId;
				$scope.scopeModel.routeTableRouteName = parameters.Destination;
				routeTableRouteName = parameters.Destination;
				$scope.scopeModel.isANumber = (NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value == parameters.RouteTableViewType) ? true : false;
				$scope.scopeModel.isBNumber = (NP_IVSwitch_RouteTableViewTypeEnum.BNumber.value == parameters.RouteTableViewType) ? true : false;
			}
			isEditMode = $scope.scopeModel.routeTableRouteName != undefined ? true : false;
		}

		function defineScope() {

			$scope.onCodeListDirectiveReady = function (api) {
				codeListDirectiveAPI = api;
				codeListDirectiveDefferedReady.resolve();
			};

			$scope.onSupplierRouteGridDirectiveReady = function (api) {
				supplierRouteGridAPI = api;
				supplierRouteGridDefferedReady.resolve();

			};

			$scope.scopeModel.hasSaveRouteTableRoutePermission = function () {
				if (isEditMode)
					return NP_IVSwitch_RouteTableRouteAPIService.HasUpdateRouteTableRoutePermission();
				else
					return NP_IVSwitch_RouteTableRouteAPIService.HasAddRouteTableRoutesPermission();
			};

			$scope.scopeModel.saveRouteTableRT = function () {
				if (isEditMode)
					return updateRouteTableRT();
				else {
					var objToAdd = buildParentObjectFromScopeForAdd();
					NP_IVSwitch_RouteTableRouteAPIService.CheckIfCodesExist(objToAdd).then(function (response) {
						if (response)
							VRNotificationService.showConfirmation("An existing route on one or more codes will be overriden").then(function (result)
							{
								if(result)
									return insertRouteTableRT();

							});
						else
							return insertRouteTableRT();


					});


				}

			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};

		}

		function load() {
			if (isEditMode) {
				$scope.scopeModel.isLoading = true;

				$scope.scopeModel.addMode = false;

				UtilsService.waitMultiplePromises([getExcludedCarrierAccountIds(), getRouteTableRouteOptions()]).then(function () {
					loadAllControls().finally(function () {
						routeTableEntity = undefined;
					});
				}).catch(function (error) {
					$scope.scopeModel.isLoading = false;
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				});
			}
			else {
				$scope.scopeModel.addMode = true;
				getExcludedCarrierAccountIds().then(function () {
					loadAllControls();
				});
			}
		}
		function getExcludedCarrierAccountIds() {
			return NP_IVSwitch_RouteTableAPIService.GetCarrierAccountIds(routeTableId, routeTableViewType).then(function (response) {
				if (response != undefined)
					excludedCarrierAccountIds = response;
			});
		}
		function getRouteTableRouteOptions() {
			return NP_IVSwitch_RouteTableRouteAPIService.GetRouteTableRoutesOptions(routeTableId, $scope.scopeModel.routeTableRouteName).then(function (response) {
				routeTableRouteOptions = response;
				if ($scope.scopeModel.isBNumber == true)
					$scope.scopeModel.techPrefix = routeTableRouteOptions.TechPrefix;
				else
					$scope.scopeModel.bNumber = routeTableRouteOptions.TechPrefix;
			});
		}

		function loadAllControls() {
			var promises = [];
			function codeListDirective() {
				return codeListDirectiveDefferedReady.promise.then(function () {
					var directivePayload = {
						context: {
							AllowTextCode: true,
						}
					};
					VRUIUtilsService.callDirectiveLoad(codeListDirectiveAPI, directivePayload, undefined);
				});
			}

			function setTitle() {
				if (isEditMode)
					$scope.title = UtilsService.buildTitleForUpdateEditor($scope.scopeModel.routeTableRouteName, " Route Table Route");
				else
					$scope.title = UtilsService.buildTitleForAddEditor("Route Table Route");
			}

			function loadSupplierRouteGrid()
			{
				var supplierRouteGridAPILoadDeferred = UtilsService.createPromiseDeferred();
				supplierRouteGridDefferedReady.promise.then(function () {
					if (routeTableRouteOptions == undefined)
						routeTableRouteOptions = {};
					routeTableRouteOptions.excludedCarrierAccountIds = excludedCarrierAccountIds;
					VRUIUtilsService.callDirectiveLoad(supplierRouteGridAPI, routeTableRouteOptions, supplierRouteGridAPILoadDeferred);
				});
				return supplierRouteGridAPILoadDeferred.promise;
			}


			return UtilsService.waitMultipleAsyncOperations([codeListDirective, setTitle, loadSupplierRouteGrid]).then(function () {
			}).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			})
				.finally(function () {
					$scope.scopeModel.isLoading = false;
				});
		}

		function insertRouteTableRT() {

			$scope.scopeModel.isLoading = true;
			var routeTableObject = buildParentObjectFromScopeForAdd();
			return NP_IVSwitch_RouteTableRouteAPIService.AddRouteTableRoutes(routeTableObject)
				.then(function (response) {
					if (VRNotificationService.notifyOnItemAdded("Route Table Route", response, "Name")) {
						if ($scope.onRouteTableRTAdded != undefined) {
							$scope.onRouteTableRTAdded();
						}
						$scope.modalContext.closeModal();
					}
				}).catch(function (error) {
					$scope.scopeModel.isLoading = false;
					VRNotificationService.notifyException(error, $scope);
				}).finally(function () {
					$scope.scopeModel.isLoading = false;
				});

		}

		function updateRouteTableRT() {
			$scope.scopeModel.isLoading = true;
			var routeTableObject = buildParentObjectFromScopeForEdit();
			NP_IVSwitch_RouteTableRouteAPIService.UpdateRouteTableRoute(routeTableObject).then(function (response) {
				if (VRNotificationService.notifyOnItemUpdated("Route Table Route", response, "Name")) {
					if ($scope.onRouteTableUpdated != undefined) {
						$scope.onRouteTableUpdated(response.UpdatedObject);
					}
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				$scope.scopeModel.isLoading = false;
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;

			});
		}

		function buildParentObjectFromScopeForEdit() {
			var routeOptions = supplierRouteGridAPI.getData();
			isBlockedAccount = routeOptions.IsBlockedAccount;
			var supplierRouteGridData = routeOptions.RouteOptions;
			var routeOptionsToEdit = [];
			if (supplierRouteGridData != undefined) {
				for (var i = 0; i < supplierRouteGridData.length; i++) {
					var item =supplierRouteGridData[i];
					var routeOption = {
						RouteId: item.RouteId,
						Percentage: item.Percentage
					};
					if (item.BackupRouteIds != undefined)
						routeOption.BackupOptions = item.BackupRouteIds;

					routeOptionsToEdit.push(routeOption);
				}
			}
			var objectScopeForEdit = {
				RouteTableId: routeTableId,
				Destination: routeTableRouteName,
				RouteOptionsToEdit: routeOptionsToEdit,
				IsBlockedAccount: isBlockedAccount,
				TechPrefix: ($scope.scopeModel.bNumber != undefined) ? $scope.scopeModel.bNumber : $scope.scopeModel.techPrefix
			};
			return objectScopeForEdit;
		}

		function buildParentObjectFromScopeForAdd() {
			var routeOptions = supplierRouteGridAPI.getData();
			isBlockedAccount = routeOptions.IsBlockedAccount;
			var supplierRouteGridData = routeOptions.RouteOptions;
			var routeOptionsToAdd = [];
			if (supplierRouteGridData != undefined)
				for (var i = 0; i < supplierRouteGridData.length; i++) {
					var item = supplierRouteGridData[i];

					var routeOption = {
						RouteId: item.RouteId,
						Percentage: item.Percentage
					};
					if (item.BackupRouteIds !=undefined)
						routeOption.BackupOptions = item.BackupRouteIds;
					routeOptionsToAdd.push(routeOption);
				}
			var objectScopeForAdd = {
				CodeListResolver: {
					Settings: codeListDirectiveAPI.getData()
				},
				IsBlockedAccount: isBlockedAccount,
				RouteOptionsToAdd: routeOptionsToAdd,
				RouteTableId: routeTableId,
				TechPrefix: ($scope.scopeModel.bNumber != undefined) ? $scope.scopeModel.bNumber : $scope.scopeModel.techPrefix
			};
			return objectScopeForAdd;
		}

	}
	appControllers.controller('NP_IVSwitch_RouteTableRouteEditorController', routeTableRouteEditorController);
})(appControllers);