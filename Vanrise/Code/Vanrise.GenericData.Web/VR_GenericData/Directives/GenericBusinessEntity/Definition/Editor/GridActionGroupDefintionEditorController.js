(function (appControllers) {

    "use strict";

    GenericBEGridActionGroupDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEGridActionGroupDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var gridActionGroupDefinition;
        var context;
		var localizationTextResourceSelectorAPI;
		var localizationTextResourceSlectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                gridActionGroupDefinition = parameters.gridActionGroupDefinition;
                context = parameters.context;
            }
            isEditMode = (gridActionGroupDefinition != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveGridActionGroupDefinition = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
			$scope.scopeModel.onLocalizationTextResourceDirectiveReady = function (api) {
				localizationTextResourceSelectorAPI = api;
				localizationTextResourceSlectorReadyPromiseDeferred.resolve();
			};
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {

            loadAllControls();

            function loadAllControls() {
            

                var initialPromises = [];

                $scope.scopeModel.isLoading = true;
                function setTitle() {
                    if (isEditMode && gridActionGroupDefinition != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(gridActionGroupDefinition.Title, 'Grid Action Group');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Grid Action Group');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;

                    $scope.scopeModel.title = gridActionGroupDefinition.Title;
				}
				function loadLocalizationTextResourceSelecor() {
					var loadLocalizationTextResourceSelectorDeferred = UtilsService.createPromiseDeferred();
					localizationTextResourceSlectorReadyPromiseDeferred.promise.then(function () {
						var payload = {
							selectedValue: gridActionGroupDefinition != undefined ? gridActionGroupDefinition.TextResourceKey : undefined
						};
						VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, payload, loadLocalizationTextResourceSelectorDeferred);
					});
					return loadLocalizationTextResourceSelectorDeferred.promise;
				}


                var rootPromiseNode = {
                    promises: initialPromises,
                    getChildNode: function () {
                        var directivePromises = [];
						directivePromises.push(UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadLocalizationTextResourceSelecor]));

                        return {
                            promises: directivePromises
                        };
                    }
                };

                return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildGridActionDefinitionFromScope() {
            return {
                GenericBEGridActionGroupId: gridActionGroupDefinition != undefined ? gridActionGroupDefinition.GenericBEGridActionGroupId : UtilsService.guid(),
				Title: $scope.scopeModel.title,
				TextResourceKey: localizationTextResourceSelectorAPI.getSelectedValues()
            };
        }

        function insert() {
            var gridActionGroupObj = buildGridActionDefinitionFromScope();
            if ($scope.onGridActionGroupAdded != undefined) {
                $scope.onGridActionGroupAdded(gridActionGroupObj);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var gridActionGroupObj = buildGridActionDefinitionFromScope();
            if ($scope.onGridActionGroupUpdated != undefined) {
                $scope.onGridActionGroupUpdated(gridActionGroupObj);
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

    appControllers.controller('VR_GenericData_GenericBEGridActionGroupDefintionEditorController', GenericBEGridActionGroupDefintionController);
})(appControllers);
