(function (appControllers) {

    "use strict";

    GenericBEActionDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEActionDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var actionDefinition;
        var context;


        var actionSettingsDirectiveAPI;
		var actionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var securityActionAPI;
		var securityActionReadyPromiseDeffered = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                actionDefinition = parameters.actionDefinition;
            }
            isEditMode = (actionDefinition != undefined);
        }
        function defineScope() {
			$scope.scopeModel = {};
			$scope.scopeModel.showSecurityGrid = true;

            $scope.scopeModel.onActionDefinitionSettingDirectiveReady = function (api) {
                actionSettingsDirectiveAPI = api;
                actionSettingsReadyPromiseDeferred.resolve();
            };

			$scope.scopeModel.onSecurityActionRequiredPermissionReady = function (api) {
				securityActionAPI = api;
				securityActionReadyPromiseDeffered.resolve();
			};


			$scope.scopeModel.saveActionDefinition = function () {
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
                    if (isEditMode && actionDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(actionDefinition.Name, 'Action Definition Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Action Definition Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.name = actionDefinition.Name;
                }

                function loadSettingDirectiveSection() {
                    var loadActionSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
					actionSettingsReadyPromiseDeferred.promise.then(function () {
						var payload = { context: getContext() };

						payload.settings = actionDefinition != undefined && actionDefinition.Settings != undefined ? actionDefinition.Settings : undefined;


                        VRUIUtilsService.callDirectiveLoad(actionSettingsDirectiveAPI, payload, loadActionSettingsPromiseDeferred);
                    });
                    return loadActionSettingsPromiseDeferred.promise;
				}

				function loadSecurityActionSection() {
					var loadSecurityActionPromiseDeferred = UtilsService.createPromiseDeferred();
					securityActionReadyPromiseDeffered.promise.then(function () {
						var payload;
						if (actionDefinition != undefined ) {
							payload = {
								data: actionDefinition.RequiredPermission
							};
						}
						VRUIUtilsService.callDirectiveLoad(securityActionAPI, payload, loadSecurityActionPromiseDeferred);

					});
					return loadSecurityActionPromiseDeferred.promise;
				}

				return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadSettingDirectiveSection, loadSecurityActionSection]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildActionDefinitionFromScope() {
            return {
                GenericBEActionId: actionDefinition != undefined ? actionDefinition.GenericBEActionId : UtilsService.guid(),
                Name: $scope.scopeModel.name,
				Settings: actionSettingsDirectiveAPI.getData(),
				RequiredPermission: !$scope.scopeModel.showSecurityGrid ? null : securityActionAPI.getData()
            };
        }

        function insert() {
            var actionDefinition = buildActionDefinitionFromScope();
            if ($scope.onGenericBEActionDefinitionAdded != undefined) {
                $scope.onGenericBEActionDefinitionAdded(actionDefinition);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
			var actionDefinition = buildActionDefinitionFromScope();
            if ($scope.onGenericBEActionDefinitionUpdated != undefined) {
                $scope.onGenericBEActionDefinitionUpdated(actionDefinition);
            }
            $scope.modalContext.closeModal();
		}

		function getContext() {
			var currentContext = context;
			if (currentContext == undefined)
				currentContext = {};

			currentContext.showSecurityGridCallBack = function (showGrid) {
				if (securityActionAPI != undefined && $scope.scopeModel.showSecurityGrid != showGrid) {
					securityActionAPI.load({ data: null });
				}
				$scope.scopeModel.showSecurityGrid = showGrid;
			};
			return currentContext;
		}

    }

    appControllers.controller('VR_GenericData_GenericBEActionDefintionController', GenericBEActionDefintionController);
})(appControllers);
