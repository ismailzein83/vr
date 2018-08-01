//(function (appControllers) {

//	"use strict";

//	CallHttpServiceEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService', 'BusinessProcess_VRWorkflowService', 'VRWorkflowCallHttpServiceMethodEnum', 'VRWorkflowCallHttpServiceMessageFormatEnum'];

//	function CallHttpServiceEditorController($scope, VRNavigationService, VRNotificationService, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService, BusinessProcess_VRWorkflowService, VRWorkflowCallHttpServiceMethodEnum, VRWorkflowCallHttpServiceMessageFormatEnum) {

//		var serviceName;
//		var connectionId;
//		var callHttpServiceMethod;
//		var actionPath;
//		var buildBodyLogic;
//		var callHttpServiceMessageFormat;
//		var responseLogic;
//		var errorLogic;
//		var isSucceeded;
//		var continueWorkflowIfCallFailed;
//		var headers;
//		var urlParameters;

//		var connectionSelectorAPI;
//		var connectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//		var headersGridAPI;
//		var headersGridReadyDeferred = UtilsService.createPromiseDeferred();

//		var urlParametersGridAPI;
//		var urlParametersGridReadyDeferred = UtilsService.createPromiseDeferred();

//		loadParameters();
//		defineScope();
//		load();

//		function loadParameters() {
//			var parameters = VRNavigationService.getParameters($scope);

//			if (parameters != undefined && parameters.obj != undefined) {
//				serviceName = parameters.obj.ServiceName;
//				connectionId = parameters.obj.ConnectionId;
//				callHttpServiceMethod = parameters.obj.CallHttpServiceMethod;
//				actionPath = parameters.obj.ActionPath;
//				buildBodyLogic = parameters.obj.BuildBodyLogic;
//				callHttpServiceMessageFormat = parameters.obj.CallHttpServiceMessageFormat;
//				responseLogic = parameters.obj.ResponseLogic;
//				errorLogic = parameters.obj.ErrorLogic;
//				isSucceeded = parameters.obj.IsSucceeded;
//				continueWorkflowIfCallFailed = parameters.obj.ContinueWorkflowIfCallFailed;
//				headers = parameters.obj.Headers;
//				urlParameters = parameters.obj.URLParameters
//			}
//		}

//		function defineScope() {
//			$scope.scopeModel = {};

//			$scope.scopeModel.onConnectionSelectorReady = function (api) {
//				connectionSelectorAPI = api;
//				connectionSelectorReadyDeferred.resolve();
//			};

//			$scope.scopeModel.onHeadersGridReady = function (api) {
//				headersGridAPI = api;
//				headersGridReadyDeferred.resolve();
//			}

//			$scope.scopeModel.onURLParametersGridReady = function (api) {
//				urlParametersGridAPI = api;
//				urlParametersGridReadyDeferred.resolve();
//			}

//			$scope.scopeModel.saveActivity = function () {
//				return updateActivity();
//			};

//			$scope.scopeModel.close = function () {
//				$scope.modalContext.closeModal();
//			};

//		}

//		function load() {
//			$scope.scopeModel.isLoading = true;
//			loadAllControls();
//		}

//		function loadAllControls() {

//			function setTitle() {
//				$scope.title = "Edit Call Http Service";
//			}

//			function loadCallHttpServiceMethodSelector() {
//				$scope.scopeModel.callHttpServiceMethodEnums = UtilsService.getArrayEnum(VRWorkflowCallHttpServiceMethodEnum);

//				if (callHttpServiceMethod != undefined)
//					$scope.scopeModel.selectedCallHttpServiceMethod = UtilsService.getItemByVal($scope.scopeModel.callHttpServiceMethodEnums, callHttpServiceMethod, "value");
//				else
//					$scope.scopeModel.selectedCallHttpServiceMethod = $scope.scopeModel.callHttpServiceMethodEnums[0];
//			}

//			function loadCallHttpServiceMessageFormatSelector() {
//				$scope.scopeModel.callHttpServiceMessageFormatEnums = UtilsService.getArrayEnum(VRWorkflowCallHttpServiceMessageFormatEnum);

//				if (callHttpServiceMessageFormat != undefined)
//					$scope.scopeModel.selectedCallHttpServiceMessageFormat = UtilsService.getItemByVal($scope.scopeModel.callHttpServiceMessageFormatEnums, callHttpServiceMessageFormat, "value");
//				else
//					$scope.scopeModel.selectedCallHttpServiceMessageFormat = $scope.scopeModel.callHttpServiceMessageFormatEnums[0];
//			}

//			function loadHeadersGrid() {
//				var headersGridLoadDeferred = UtilsService.createPromiseDeferred();
//				headersGridReadyDeferred.promise.then(function () {
//					var headersGridPayload = {
//						headers: headers
//					};
//					VRUIUtilsService.callDirectiveLoad(headersGridAPI, headersGridPayload, headersGridLoadDeferred);
//				});
//				return headersGridLoadDeferred.promise;
//			}

//			function loadURLParametersGrid() {
//				var urlParametersGridLoadDeferred = UtilsService.createPromiseDeferred();
//				urlParametersGridReadyDeferred.promise.then(function () {
//					var urlParametersGridPayload = {
//						urlParameters: urlParameters
//					};
//					VRUIUtilsService.callDirectiveLoad(urlParametersGridAPI, urlParametersGridPayload, urlParametersGridLoadDeferred);
//				});
//				return urlParametersGridLoadDeferred.promise;
//			}

//			function loadConnectionSelector() {
//				var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
//				connectionSelectorReadyDeferred.promise.then(function () {
//					var selectorPayload = {
//						filter: {
//							Filters: [{
//								$type: "Vanrise.Common.Business.VRHttpConnectionFilter, Vanrise.Common.Business"
//							}]
//						},
//						selectedIds: connectionId
//					};
//					VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, selectorPayload, connectionSelectorLoadDeferred);
//				});
//				return connectionSelectorLoadDeferred.promise;
//			}

//			return UtilsService.waitMultipleAsyncOperations([setTitle, loadCallHttpServiceMethodSelector, loadCallHttpServiceMessageFormatSelector, loadConnectionSelector]).then(function () {
//			}).catch(function (error) {
//				VRNotificationService.notifyExceptionWithClose(error, $scope);
//			}).finally(function () {
//				$scope.scopeModel.isLoading = false;
//			});
//		}

//		function updateActivity() {
//			$scope.scopeModel.isLoading = true;
//			var updatedObject = {
//				connectionId: connectionSelectorAPI.getSelectedIds(),
//				callHttpServiceMethod: $scope.scopeModel.selectedCallHttpServiceMethod.value,
//				actionPath: $scope.scopeModel.actionPath,
//				buildBodyLogic: $scope.scopeModel.buildBodyLogic,
//				callHttpServiceMessageFormat: $scope.scopeModel.selectedCallHttpServiceMessageFormat.value,
//				responseLogic: $scope.scopeModel.responseLogic,
//				errorLogic: $scope.scopeModel.errorLogic,
//				isSucceeded: $scope.scopeModel.isSucceeded,
//				continueWorkflowIfCallFailed: $scope.scopeModel.continueWorkflowIfCallFailed,
//				serviceName: $scope.scopeModel.serviceName,
//				headers: (headersGridAPI != undefined) ? headersGridAPI.getData() : undefined,
//				urlParameters: (urlParametersGridAPI != undefined) ? urlParametersGridAPI.getData() : undefined,
//			}
//			if ($scope.onActivityUpdated != undefined) {
//				$scope.onActivityUpdated(updatedObject);
//			}
//			$scope.scopeModel.isLoading = false;
//			$scope.modalContext.closeModal();
//		}
//	}

//	appControllers.controller('BusinessProcess_VR_WorkflowActivityCallHttpServiceController', CallHttpServiceEditorController);
//})(appControllers);