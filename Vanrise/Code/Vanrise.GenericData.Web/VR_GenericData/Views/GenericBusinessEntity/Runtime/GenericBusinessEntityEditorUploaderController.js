(function (appControllers) {

	'use strict';
	GenericBusinessEntityEditorUploaderController.$inject = ['$scope', 'VRNavigationService', 'VRUIUtilsService', 'UtilsService', 'VR_GenericData_GenericBusinessEntityAPIService','VRNotificationService'];
	function GenericBusinessEntityEditorUploaderController($scope, VRNavigationService, VRUIUtilsService, UtilsService, VR_GenericData_GenericBusinessEntityAPIService, VRNotificationService) {
		var fieldId;
		var businessEntityDefinitionId;
		var definitionTitle;

		loadParameters();

		defineScope();

		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined) {
				businessEntityDefinitionId = parameters.businessEntityDefinitionId;

			}
		}

		function defineScope() {
			$scope.scopeModel = {};

			$scope.scopeModel.uploadGenericBusinessEntities = function () {
				fieldId = $scope.scopeModel.document.fileId;
				return VR_GenericData_GenericBusinessEntityAPIService.UploadGenericBusinessEntities(businessEntityDefinitionId, fieldId).then(function (response) {
					fieldId = response.FileID;
					$scope.scopeModel.isUploadingComplete = true;
					$scope.scopeModel.addedBusinessEntities = response.NumberOfItemsAdded;
					$scope.scopeModel.existsBusinessEntities = response.NumberOfItemsFailed;
					VRNotificationService.showSuccess(definitionTitle+" Finished Upload");
				});
			};

			$scope.scopeModel.downloadTemplate = function () {
				return VR_GenericData_GenericBusinessEntityAPIService.DownloadGenericBusinessEntityTemplate(businessEntityDefinitionId).then(function (response) {
					UtilsService.downloadFile(response.data, response.headers);
				});
			};

			$scope.scopeModel.downloadLog = function () {
				if (fieldId != undefined) {
					return VR_GenericData_GenericBusinessEntityAPIService.DownloadBusinessEntityLog(fieldId).then(function (response) {
						UtilsService.downloadFile(response.data, response.headers);
					});
				}
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {
	
	    function setTitle() {
				getGenericBusinessEntityEditorRuntime().then(function () {
					$scope.title = UtilsService.buildTitleForUploadEditor(definitionTitle);
				});
			}

		function getGenericBusinessEntityEditorRuntime() {

				return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityEditorRuntime(businessEntityDefinitionId, undefined, undefined).then(function (response) {
					if (response != undefined) {
						definitionTitle = response.DefinitionTitle;
					}
				});
			}

			return UtilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}
	}

	appControllers.controller('VR_GenericData_GenericBusinessEntityEditorUploaderController', GenericBusinessEntityEditorUploaderController);


})(appControllers);