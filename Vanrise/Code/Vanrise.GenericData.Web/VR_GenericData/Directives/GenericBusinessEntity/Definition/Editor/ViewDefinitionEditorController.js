(function (appControllers) {

    "use strict";

    GenericBEViewDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEViewDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var viewDefinition;
        var context;

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var viewSettingsDirectiveAPI;
        var viewSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var viewConditionDirectiveAPI;
		var viewConditionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var localizationTextResourceSelectorAPI;
		var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewDefinition = parameters.viewDefinition;
                context = parameters.context;
            }
            isEditMode = (viewDefinition != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onViewDefinitionSettingDirectiveReady = function (api) {
                viewSettingsDirectiveAPI = api;
                viewSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onViewDefinitionConditionDirectiveReady = function (api) {
                viewConditionDirectiveAPI = api;
                viewConditionReadyPromiseDeferred.resolve();
            };

			$scope.scopeModel.onLocalizationTextResourceDirectiveReady = function (api) {
				localizationTextResourceSelectorAPI = api;
				localizationTextResourceSelectorReadyPromiseDeferred.resolve();
			};

            $scope.scopeModel.saveViewDefinition = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {

            loadAllControls();

            function loadAllControls() {
                $scope.scopeModel.isLoading = true;
                function setTitle() {
                    if (isEditMode && viewDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(viewDefinition.Name, 'View Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('View Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.name = viewDefinition.Name;
                }

                function loadSettingDirectiveSection() {
                    var loadViewSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
                    viewSettingsReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            parameterEntity: viewDefinition != undefined && viewDefinition.Settings != undefined ? viewDefinition.Settings : undefined,
                            context:context
                        };

                        VRUIUtilsService.callDirectiveLoad(viewSettingsDirectiveAPI, payload, loadViewSettingsPromiseDeferred);
                    });
                    return loadViewSettingsPromiseDeferred.promise;
				}

				function loadLocalizationTextResourceSelector() {
					var loadSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
					localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
						var payload = {
							selectedValue: viewDefinition != undefined ? viewDefinition.TextResourceKey : undefined
						};
						VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, payload, loadSelectorPromiseDeferred);
					});
					return loadSelectorPromiseDeferred.promise;
				}


                function loadConditionDirectiveSection() {
                    var loadViewConditionPromiseDeferred = UtilsService.createPromiseDeferred();
                    viewConditionReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            condition: viewDefinition != undefined && viewDefinition.Condition != undefined ? viewDefinition.Condition : undefined,
                            context: context
                        };
                        VRUIUtilsService.callDirectiveLoad(viewConditionDirectiveAPI, payload, loadViewConditionPromiseDeferred);
                    });
                    return loadViewConditionPromiseDeferred.promise;
                }

				return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadSettingDirectiveSection, loadConditionDirectiveSection, loadLocalizationTextResourceSelector]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildViewDefinitionFromScope() {
            return {
                GenericBEViewDefinitionId: viewDefinition != undefined ? viewDefinition.GenericBEViewDefinitionId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
                Settings: viewSettingsDirectiveAPI.getData(),
				Condition: viewConditionDirectiveAPI.getData(),
				TextResourceKey: localizationTextResourceSelectorAPI.getSelectedValues()
            };
        }

        function insert() {
            var viewDefinition = buildViewDefinitionFromScope();
            if ($scope.onGenericBEViewDefinitionAdded != undefined) {
                $scope.onGenericBEViewDefinitionAdded(viewDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var viewDefinition = buildViewDefinitionFromScope();
            if ($scope.onGenericBEViewDefinitionUpdated != undefined) {
                $scope.onGenericBEViewDefinitionUpdated(viewDefinition);
            }
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VR_GenericData_GenericBEViewDefintionController', GenericBEViewDefintionController);
})(appControllers);
