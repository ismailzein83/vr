(function (appControllers) {

	"use strict";

	GenericRowEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

	function GenericRowEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

		var isEditMode;
		var recordTypeFields;
		var rowEntity;
		var context;

        var genericFieldDefinitionDirectiveAPI;
        var genericFieldDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);
			if (parameters != undefined && parameters != null) {
				recordTypeFields = parameters.recordTypeFields;
				context = parameters.context;
				rowEntity = parameters.rowEntity;
			}
			isEditMode = (rowEntity != undefined);
		}

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.onGenericFieldDefinitionDirectiveReady = function (api) {
                genericFieldDefinitionDirectiveAPI = api;
                genericFieldDefinitionDirectiveReadyPromiseDeferred.resolve();
            };

			$scope.scopeModal.SaveRow = function () {
				if (isEditMode) {
					return updateRow();
				}
				else {
					return insertRow();
				}
			};

			$scope.scopeModal.close = function () {
				$scope.modalContext.closeModal();
			};
		}

		function load() {
			$scope.scopeModal.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([loadScopeDataFromObj, setTitle]).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				}).finally(function () {
					$scope.scopeModal.isLoading = false;
				});

			function setTitle() {
				if (isEditMode)
					$scope.title = UtilsService.buildTitleForUpdateEditor('Row');
				else
					$scope.title = UtilsService.buildTitleForAddEditor('Row');
            }

			function loadScopeDataFromObj() {
                var loadGenericFieldDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                genericFieldDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        recordTypeFields: recordTypeFields,
                        context: context,
                        entity: rowEntity
                    };

                    VRUIUtilsService.callDirectiveLoad(genericFieldDefinitionDirectiveAPI, payload, loadGenericFieldDirectivePromiseDeferred);
                });

                return loadGenericFieldDirectivePromiseDeferred.promise;
			}
        }

		function buildRowObjectFromScope() {
            return genericFieldDefinitionDirectiveAPI.getData();
		}

		function insertRow() {

			var rowObject = buildRowObjectFromScope();
			if ($scope.onRowAdded != undefined)
				$scope.onRowAdded(rowObject);
			$scope.modalContext.closeModal();
		}

		function updateRow() {
			var rowObject = buildRowObjectFromScope();
			if ($scope.onRowUpdated != undefined)
				$scope.onRowUpdated(rowObject);
			$scope.modalContext.closeModal();
		}

	}

	appControllers.controller('VR_GenericData_GenericRowEditorController', GenericRowEditorController);
})(appControllers);