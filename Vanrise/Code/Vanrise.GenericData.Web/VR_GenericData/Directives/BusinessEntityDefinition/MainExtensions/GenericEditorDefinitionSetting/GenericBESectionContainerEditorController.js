//(function (appControllers) {

//    "use strict";

//    GenericBESectionContainerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_GenericData_ContainerTypeEnum'];

//    function GenericBESectionContainerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_GenericData_ContainerTypeEnum) {

//        var isEditMode;
//        var sectionDefinition;
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
//                sectionDefinition = parameters.sectionDefinition;
//                context = parameters.context;
//            }

//            isEditMode = (sectionDefinition != undefined);
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

//            $scope.scopeModel.saveSectionDefinition = function () {
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
//                    if (isEditMode && sectionDefinition != undefined)
//                        $scope.title = UtilsService.buildTitleForUpdateEditor(sectionDefinition.SectionTitle, 'Section Definition Editor');
//                    else
//                        $scope.title = UtilsService.buildTitleForAddEditor('Section Definition Editor');
//                }

//                function loadStaticData() {
//                    if (!isEditMode)
//                        return;

//                    $scope.scopeModel.sectionTitle = sectionDefinition.SectionTitle;
//                    $scope.scopeModel.colNum = sectionDefinition.ColNum;
//                }

//                function loadEditorDefinitionDirective() {
//                    var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

//                    editorDefinitionReadyPromiseDeferred.promise.then(function () {

//                        var payload = {
//                            settings: sectionDefinition != undefined && sectionDefinition.SectionSettings || undefined,
//                            context: getContext(),
//                            containerType: VR_GenericData_ContainerTypeEnum.Section.value
//                        };
//                        VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, payload, loadEditorDefinitionDirectivePromiseDeferred);
//                    });

//                    return loadEditorDefinitionDirectivePromiseDeferred.promise;
//				}

//				function loadLocalizationTextResourceSelector() {
//                    var loadSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//                    localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {

//						var payload = {
//							selectedValue: sectionDefinition != undefined ? sectionDefinition.TextResourceKey : undefined
//						};
//						VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, payload, loadSelectorPromiseDeferred);
//                    });

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

//        function buildSectionDefinitionFromScope() {
//            return {
//                SectionTitle: $scope.scopeModel.sectionTitle,
//                ColNum: $scope.scopeModel.colNum,
//				SectionSettings: editorDefinitionAPI.getData(),
//				TextResourceKey: localizationTextResourceSelectorAPI.getSelectedValues()
//            };
//        }

//        function insert() {
//            var sectionDefinition = buildSectionDefinitionFromScope();
//            if ($scope.onSectionContainerAdded != undefined) {
//                $scope.onSectionContainerAdded(sectionDefinition);
//            }
//            $scope.modalContext.closeModal();
//        }

//        function update() {
//            var sectionDefinition = buildSectionDefinitionFromScope();
//            if ($scope.onSectionContainerUpdated != undefined) {
//                $scope.onSectionContainerUpdated(sectionDefinition);
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

//    appControllers.controller('VR_GenericData_GenericBESectionContainerEditorController', GenericBESectionContainerEditorController);
//})(appControllers);