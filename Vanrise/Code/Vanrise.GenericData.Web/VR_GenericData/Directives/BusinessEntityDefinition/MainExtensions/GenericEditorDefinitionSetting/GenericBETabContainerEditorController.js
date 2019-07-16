//(function (appControllers) {

//    "use strict";

//    GenericBETabContainerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_ContainerTypeEnum'];

//    function GenericBETabContainerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_ContainerTypeEnum) {

//        var isEditMode;
//        var tabDefinition;
//        var context;

//        var editorDefinitionAPI;
//        var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//		var localizationTextResourceSelectorAPI;
//		var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);

//            if (parameters != undefined && parameters != null) {
//                tabDefinition = parameters.tabDefinition;
//                context = parameters.context;
//            }
//            isEditMode = (tabDefinition != undefined);
//        }
//        function defineScope() {

//            $scope.scopeModel = {};

//            $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
//                editorDefinitionAPI = api;
//                editorDefinitionReadyPromiseDeferred.resolve();
//            };

//			$scope.scopeModel.onLocalizationTextResourceDirectiveReady = function (api) {
//				localizationTextResourceSelectorAPI = api;
//				localizationTextResourceSelectorReadyPromiseDeferred.resolve();
//			};

//            $scope.scopeModel.saveTabDefinition = function () {
//                if (isEditMode) {
//                    return update();
//                }
//                else {
//                    return insert();
//                }
//            };

//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal();
//            };

//        }
//        function load() {

//            loadAllControls();

//            function loadAllControls() {
//                $scope.scopeModel.isLoading = true;

//                function setTitle() {
//                    if (isEditMode && tabDefinition != undefined)
//                        $scope.title = UtilsService.buildTitleForUpdateEditor(tabDefinition.TabTitle, 'Tab Definition Editor');
//                    else
//                        $scope.title = UtilsService.buildTitleForAddEditor('Tab Definition Editor');
//                }

//                function loadStaticData() {
//                    if (!isEditMode)
//                        return;

//                    $scope.scopeModel.tabTitle = tabDefinition.TabTitle;
//                    $scope.scopeModel.showTab = tabDefinition.ShowTab;
//                }

//                function loadEditorDefinitionDirective() {
//                    var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
//                    editorDefinitionReadyPromiseDeferred.promise.then(function () {
//                        var payload = {
//                            settings: tabDefinition != undefined && tabDefinition.TabSettings || undefined,
//                            context: getContext(),
//                            containerType: VR_GenericData_ContainerTypeEnum.Tab.value
//                        };
//                        VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, payload, loadEditorDefinitionDirectivePromiseDeferred);
//                    });
//                    return loadEditorDefinitionDirectivePromiseDeferred.promise;
//				}

//				function loadLocalizationTextResourceSelector() {
//					var loadSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
//					localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
//						var payload = {
//							selectedValue: tabDefinition != undefined ? tabDefinition.TextResourceKey : undefined
//						};
//						VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, payload, loadSelectorPromiseDeferred);
//					});
//					return loadSelectorPromiseDeferred.promise;
//				}

//				return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadEditorDefinitionDirective,loadLocalizationTextResourceSelector]).then(function () {
//                }).finally(function () {
//                    $scope.scopeModel.isLoading = false;
//                }).catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                });
//            }

//        }

//        function buildTabDefinitionFromScope() {
//            return {
//                TabTitle: $scope.scopeModel.tabTitle,
//                ShowTab: $scope.scopeModel.showTab,
//				TabSettings: editorDefinitionAPI.getData(),
//				TextResourceKey: localizationTextResourceSelectorAPI.getSelectedValues(),
//            };
//        }

//        function insert() {
//            var tabDefinition = buildTabDefinitionFromScope();
//            if ($scope.onTabContainerAdded != undefined) {
//                $scope.onTabContainerAdded(tabDefinition);
//            }
//            $scope.modalContext.closeModal();
//        }

//        function update() {
//            var tabDefinition = buildTabDefinitionFromScope();
//            if ($scope.onTabContainerUpdated != undefined) {
//                $scope.onTabContainerUpdated(tabDefinition);
//            }
//            $scope.modalContext.closeModal();
//        }

//        function getContext() {
//            var currentContext = context;
//            if (currentContext == undefined)
//                currentContext = {};
//            return currentContext;
//        }
//    }

//    appControllers.controller('VR_GenericData_GenericBETabContainerEditorController', GenericBETabContainerEditorController);
//})(appControllers);
