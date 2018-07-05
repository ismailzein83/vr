(function (appControllers) {

	"use strict";

	WorkflowMainController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VRDragdropService'];

	function WorkflowMainController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VRDragdropService) {

		var workflowMainId;
		var isEditMode;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);
			if (parameters != undefined && parameters != null) {
				workflowMainId = parameters.workflowMainId;
			}
			isEditMode = (workflowMainId != undefined);
		}

		function defineScope() {
			$scope.scopeModal = {};
			$scope.scopeModal.datasource = [];
			$scope.scopeModal.activitesTemplates = [];
			$scope.scopeModal.activitesTemplates.push({
				id: UtilsService.guid(),
				configId: "123",
				editor: "vr-workflow-sequence",
				Title: "Sequence"
			});
			$scope.scopeModal.activitesTemplates.push({
				id: UtilsService.guid(),
				configId: "124",
				editor: "vr-workflow-assign",
				Title: "Assign"
			});
			$scope.scopeModal.activitesTemplates.push({
				id: UtilsService.guid(),
				configId: "543",
				editor: "vr-workflow-customcode",
				Title: "Custom Code"
			});

			$scope.scopeModal.close = function () {
				$scope.modalContext.closeModal();
			};

			$scope.scopeModal.dragdropGroupCorrelation = VRDragdropService.createCorrelationGroup();
			$scope.scopeModal.dragdropsetting = {
				groupCorrelation: $scope.scopeModal.dragdropGroupCorrelation,
				canReceive: true,
				canSend: true,
				copyOnSend: false,
				onItemReceived: function (itemAdded, dataSource, sourceList) {

					var dataItem = {
						id: UtilsService.guid(),
						configId: itemAdded.configId,
						editor: itemAdded.editor,
						name: itemAdded.name || itemAdded.Title
					};
					var data;
					if (itemAdded.directiveAPI != null)
						dataItem.data = itemAdded.directiveAPI.getData();

					dataItem.onDirectiveReady = function (api) {
						dataItem.directiveAPI = api;
						var setLoader = function (value) { $scope.scopeModal.isLoadingDirective = value; };
						var payload = {
							data: dataItem.data
						};
						VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, payload, setLoader);
					};
					return dataItem;
				},
				enableSorting: true
			};
		}

		function load() {
			$scope.scopeModal.isLoading = true;

			if (isEditMode) {
			}
			else {
				loadAllControls();
			}

			function loadAllControls() {
				//return UtilsService.waitMultipleAsyncOperations([loadDefinitionSection, setTitle, loadDataRecordType, loadAllStepts]).then(function () {
				//	loadSequenceStepSection().catch(function (error) {
				//		VRNotificationService.notifyExceptionWithClose(error, $scope);
				//	}).finally(function () {
				//		$scope.scopeModal.isLoading = false;
				//	});
				//}).catch(function (error) {
				//	VRNotificationService.notifyExceptionWithClose(error, $scope);
				//});
			}

			function setTitle() {
				if (isEditMode && dataTransformationDefinitionEntity != undefined)
					$scope.title = UtilsService.buildTitleForUpdateEditor(dataTransformationDefinitionEntity.Name, 'Workflow');
				else
					$scope.title = UtilsService.buildTitleForAddEditor('Workflow');
			}
		}

	}
	appControllers.controller('VR_Workflow_MainController', WorkflowMainController);
})(appControllers);
