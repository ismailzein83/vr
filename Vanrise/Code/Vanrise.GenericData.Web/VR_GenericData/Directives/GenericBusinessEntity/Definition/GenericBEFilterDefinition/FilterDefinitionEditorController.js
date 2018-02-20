(function (appControllers) {

    "use strict";

    GenericBEFilterDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEFilterDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var filterDefinition;
        var context;

        var filterDefinitionAPI;
        var filterDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                filterDefinition = parameters.filterDefinition;
                context = parameters.context;
            }
            isEditMode = (filterDefinition != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGenericBEFilterDefinitionDirectiveReady = function (api) {
                filterDefinitionAPI = api;
                filterDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveFilterDefinition = function () {
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
                    if (isEditMode && filterDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(filterDefinition.Name, 'Filter Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Filter Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.name = filterDefinition.Name;
                    $scope.scopeModel.showInBasic = filterDefinition.ShowInBasic;

                }

            

                function loadFilterDefinitionDirective() {
                    var loadFilterDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    filterDefinitionReadyPromiseDeferred.promise.then(function () {
                        var filterPayload = {
                            settings: filterDefinition != undefined && filterDefinition.FilterSettings || undefined,
                            context: getContext()
                        };

                        VRUIUtilsService.callDirectiveLoad(filterDefinitionAPI, filterPayload, loadFilterDefinitionDirectivePromiseDeferred);
                    });
                    return loadFilterDefinitionDirectivePromiseDeferred.promise;
                }


                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadFilterDefinitionDirective]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildFilterDefinitionFromScope() {
            return {
                BasicAdvancedFilterItemId: (isEditMode)?filterDefinition.BasicAdvancedFilterItemId: UtilsService.guid(),
                Name: $scope.scopeModel.name,
                ShowInBasic: $scope.scopeModel.showInBasic,
                FilterSettings: filterDefinitionAPI.getData()
            };
        }

        function insert() {
            var filterDefinition = buildFilterDefinitionFromScope();
            if ($scope.onGenericBEFilterDefinitionAdded != undefined) {
                $scope.onGenericBEFilterDefinitionAdded(filterDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var filterDefinition = buildFilterDefinitionFromScope();
            if ($scope.onGenericBEFilterDefinitionUpdated != undefined) {
                $scope.onGenericBEFilterDefinitionUpdated(filterDefinition);
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

    appControllers.controller('VR_GenericData_GenericBEFilterDefintionController', GenericBEFilterDefintionController);
})(appControllers);
