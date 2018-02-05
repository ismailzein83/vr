(function (appControllers) {

    "use strict";

    GenericBETabContainerEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBETabContainerEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var tabDefinition;
        var context;

        var editorDefinitionAPI;
        var editorDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                tabDefinition = parameters.tabDefinition;
                context = parameters.context;
            }
            isEditMode = (tabDefinition != undefined);
        }
        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onGenericBEEditorDefinitionDirectiveReady = function (api) {
                editorDefinitionAPI = api;
                editorDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveTabDefinition = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }
        function load() {

            loadAllControls();

            function loadAllControls() {
                $scope.scopeModel.isLoading = true;

                function setTitle() {
                    if (isEditMode && tabDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(tabDefinition.TabTitle, 'Tab Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Tab Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.tabTitle = tabDefinition.TabTitle;
                    $scope.scopeModel.showTab = tabDefinition.ShowTab;
                }

                function loadEditorDefinitionDirective() {
                    var loadEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    editorDefinitionReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            settings: tabDefinition != undefined && tabDefinition.TabSettings || undefined,
                            context: getContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(editorDefinitionAPI, payload, loadEditorDefinitionDirectivePromiseDeferred);
                    });
                    return loadEditorDefinitionDirectivePromiseDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadEditorDefinitionDirective]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

        }

        function buildTabDefinitionFromScope() {
            return {
                TabTitle: $scope.scopeModel.tabTitle,
                ShowTab: $scope.scopeModel.showTab,
                TabSettings: editorDefinitionAPI.getData()
            };
        }

        function insert() {
            var tabDefinition = buildTabDefinitionFromScope();
            if ($scope.onTabContainerAdded != undefined) {
                $scope.onTabContainerAdded(tabDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var tabDefinition = buildTabDefinitionFromScope();
            if ($scope.onTabContainerUpdated != undefined) {
                $scope.onTabContainerUpdated(tabDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }

    appControllers.controller('VR_GenericData_GenericBETabContainerEditorController', GenericBETabContainerEditorController);
})(appControllers);
