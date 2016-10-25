(function (appControllers) {

	'use strict';

	OutboundRateCalcMethodEditorController.$inject = ['$scope', 'WhS_Deal_SwapDealAnalysisAPIService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

	function OutboundRateCalcMethodEditorController($scope, WhS_Deal_SwapDealAnalysisAPIService, VRNavigationService, UtilsService, VRUIUtilsService, VRNotificationService) {

		var isEditMode;

		var calculationMethodId;
		var rateCalcMethod;

		var rateCalcMethodSelectorAPI;
		var rateCalcMethodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var directiveAPI;
		var directiveReadyDeferred;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				calculationMethodId = parameters.calculationMethodId;
				rateCalcMethod = parameters.outboundRateCalcMethod;
			}

			isEditMode = (rateCalcMethod != undefined);
		}
		function defineScope() {

			$scope.scopeModel = {};

			$scope.scopeModel.rateCalcMethodExtensionConfigs = [];

			$scope.scopeModel.onRateCalcMethodSelectorReady = function (api) {
				rateCalcMethodSelectorAPI = api;
				rateCalcMethodSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onDirectiveReady = function (api) {
				directiveAPI = api;
				var setLoader = function (value) {
					$scope.scopeModel.isLoading = value;
				};
				VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, rateCalcMethod, setLoader, directiveReadyDeferred);
			};

			$scope.scopeModel.save = function () {
				return (isEditMode) ? updateRateCalcMethod() : insertRateCalcMethod();
			};

			$scope.scopeModel.close = function () {
				$scope.modalContext.closeModal();
			};
		}
		function load() {

			$scope.scopeModel.isLoading = true;

			if (isEditMode) {
				directiveReadyDeferred = UtilsService.createPromiseDeferred();
			}

			loadAllControls();
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadRateCalcMethodSelector, loadDirective]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
		function setTitle() {
			if (isEditMode) {
				var rateCalcMethodTitle;
				if (rateCalcMethod != undefined)
					rateCalcMethodTitle = rateCalcMethod.Title;
				$scope.title = UtilsService.buildTitleForUpdateEditor(rateCalcMethodTitle, 'Outbound Rate Calc Method');
			}
			else
				$scope.title = UtilsService.buildTitleForAddEditor('Outbound Rate Calc Method');
		}
		function loadRateCalcMethodSelector() {
			return WhS_Deal_SwapDealAnalysisAPIService.GetOutboundRateCalcMethodExtensionConfigs().then(function (response) {
				if (response != null) {
					for (var i = 0; i < response.length; i++)
						$scope.scopeModel.rateCalcMethodExtensionConfigs.push(response[i]);
				}
				if (rateCalcMethod != undefined) {
					$scope.scopeModel.selectedRateCalcMethodExtensionConfig =
						UtilsService.getItemByVal($scope.scopeModel.rateCalcMethodExtensionConfigs, rateCalcMethod.ConfigId, 'ExtensionConfigurationId');
				}
			});
		}
		function loadDirective() {

			if (rateCalcMethod == undefined)
				return;

			var directiveLoadDeferred = UtilsService.createPromiseDeferred();

			directiveReadyDeferred.promise.then(function () {
				directiveReadyDeferred = undefined;
				VRUIUtilsService.callDirectiveLoad(directiveAPI, rateCalcMethod, directiveLoadDeferred);
			});

			return directiveLoadDeferred.promise;
		}

		function insertRateCalcMethod() {
			var rateCalcMethod = buildRateCalcMethodFromScope();
			if ($scope.onOutboundRateCalcMethodAdded != undefined)
				$scope.onOutboundRateCalcMethodAdded(rateCalcMethod);
			$scope.modalContext.closeModal();
		}
		function updateRateCalcMethod() {
			var rateCalcMethod = buildRateCalcMethodFromScope();
			if ($scope.onOutboundRateCalcMethodUpdated != undefined)
				$scope.onOutboundRateCalcMethodUpdated(rateCalcMethod);
			$scope.modalContext.closeModal();
		}
		function buildRateCalcMethodFromScope() {
			var rateCalcMethod = directiveAPI.getData();
			rateCalcMethod.CalculationMethodId = calculationMethodId;
			rateCalcMethod.ConfigId = $scope.scopeModel.selectedRateCalcMethodExtensionConfig.ExtensionConfigurationId;
			return rateCalcMethod;
		}
	}

	appControllers.controller('WhS_Deal_OutboundRateCalcMethodEditorController', OutboundRateCalcMethodEditorController);

})(appControllers);