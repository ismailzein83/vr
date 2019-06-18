//(function (appControllers) {

//	"use strict";

//	GenericBECustomActionDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

//    function GenericBECustomActionDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

//		var isEditMode;
//        var customActionDefinition;
//		var context;


//        var customActionSettingsDirectiveAPI;
//        var customActionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//        var buttonTypeDirectiveAPI;
//        var buttonTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//		loadParameters();
//		defineScope();
//		load();

//		function loadParameters() {
//			var parameters = VRNavigationService.getParameters($scope);
//			if (parameters != undefined && parameters != null) {
//                customActionDefinition = parameters.customActionDefinition;
//				if (parameters.context != undefined)
//					context = parameters.context;
//			}
//            isEditMode = (customActionDefinition != undefined);
//		}
//		function defineScope() {
//			$scope.scopeModel = {};

//			$scope.scopeModel.onCustomActionSettingDirectiveReady = function (api) {
//                customActionSettingsDirectiveAPI = api;
//                customActionSettingsReadyPromiseDeferred.resolve();
//			};

//			$scope.scopeModel.onButtonTypeDirectiveReady = function (api) {
//                buttonTypeDirectiveAPI = api;
//                buttonTypeDirectiveReadyPromiseDeferred.resolve();
//			};


//			$scope.scopeModel.saveCustomActionDefinition = function () {
//				if (isEditMode) {
//					return update();
//				}
//				else {
//					return insert();
//				}
//			};

//			$scope.scopeModel.close = function () {
//				$scope.modalContext.closeModal();
//			};

//		}
//		function load() {

//			loadAllControls();

//			function loadAllControls() {
//				$scope.scopeModel.isLoading = true;
//				function setTitle() {
//                    if (isEditMode && customActionDefinition != undefined)
//                        $scope.title = UtilsService.buildTitleForUpdateEditor(customActionDefinition.Title, 'Custom Action Definition Editor');
//					else
//                        $scope.title = UtilsService.buildTitleForAddEditor('Custom Action Definition Editor');
//				}

//				function loadStaticData() {
//					if (!isEditMode)
//						return;

//                    $scope.scopeModel.title = customActionDefinition.Title;
//				}

//				function loadSettingDirective() {
//					var loadCustomActionSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
//                    customActionSettingsReadyPromiseDeferred.promise.then(function () {
//						var payload = { context: getContext() };

//                        payload.settings = customActionDefinition != undefined && customActionDefinition.Settings != undefined ? customActionDefinition.Settings : undefined;


//                        VRUIUtilsService.callDirectiveLoad(customActionSettingsDirectiveAPI, payload, loadCustomActionSettingsPromiseDeferred);
//					});
//                    return loadCustomActionSettingsPromiseDeferred.promise;
//				}

//				function loadButtonTypeDirective() {
//                    var loadButtonTypePromiseDeferred = UtilsService.createPromiseDeferred();
//                    buttonTypeDirectiveReadyPromiseDeferred.promise.then(function () {
//                        var payload;
//                        if (customActionDefinition != undefined) {
//							payload = {
//                                selectedIds: customActionDefinition.ButtonType
//							};
//						}
//                        VRUIUtilsService.callDirectiveLoad(buttonTypeDirectiveAPI, payload, loadButtonTypePromiseDeferred);

//					});
//                    return loadButtonTypePromiseDeferred.promise;
//				}

//                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadButtonTypeDirective, loadSettingDirective]).then(function () {
//				}).finally(function () {
//					$scope.scopeModel.isLoading = false;
//				}).catch(function (error) {
//					VRNotificationService.notifyExceptionWithClose(error, $scope);
//				});

//			}

//		}

//        function buildCustomActionDefinitionFromScope() {
//			return {
//                GenericBECustomActionId: customActionDefinition != undefined ? customActionDefinition.GenericBECustomActionId : UtilsService.guid(),
//                Title: $scope.scopeModel.title,
//                ButtonType: buttonTypeDirectiveAPI.getData().value,
//                Settings: customActionSettingsDirectiveAPI.getData()
//			};
//		}

//		function insert() {
//            var customActionDefinition = buildCustomActionDefinitionFromScope();
//			if ($scope.onGenericBECustomActionDefinitionAdded != undefined) {
//                $scope.onGenericBECustomActionDefinitionAdded(customActionDefinition);
//			}
//			$scope.modalContext.closeModal();
//		}

//		function update() {
//            var customActionDefinition = buildCustomActionDefinitionFromScope();
//            if ($scope.onGenericBECustomActionDefinitionUpdated != undefined) {
//                $scope.onGenericBECustomActionDefinitionUpdated(customActionDefinition);
//			}
//			$scope.modalContext.closeModal();
//		}

//		function getContext() {
//			var currentContext = context;
//			if (currentContext == undefined)
//				currentContext = {};

//			return currentContext;
//		}

//	}

//    appControllers.controller('VR_GenericData_GenericBECustomActionDefintionController', GenericBECustomActionDefintionController);
//})(appControllers);
