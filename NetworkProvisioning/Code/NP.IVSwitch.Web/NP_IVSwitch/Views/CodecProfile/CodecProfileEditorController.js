﻿(function (appControllers) {

	"use strict";

	CodecProfileEditorController.$inject = ['$scope', 'NP_IVSwitch_CodecProfileAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService'];

	function CodecProfileEditorController($scope, NP_IVSwitch_CodecProfileAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService) {

		var isEditMode;
		var codecProfileId;
		var codecProfileEntity;
		var codecProfileEditorRuntimeEntity;
		var codecDefEntity;
		var codecDefList = new Array();
		var codecDefIds = new Array();
		var context;
		var isViewHistoryMode;

		var gridAPI;
		var gridReadyDefferedPromise = UtilsService.createPromiseDeferred();

		loadParameters();

		defineScope();

		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != undefined && parameters != null) {
				codecProfileId = parameters.CodecProfileId;
				context = parameters.context;
			};
			isViewHistoryMode = (context != undefined && context.historyId != undefined);
			isEditMode = (codecProfileId != undefined);
		}

		function defineScope() {

			$scope.scopeModel = {};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				gridReadyDefferedPromise.resolve();
			};

			$scope.hasSaveCodecProfilePermission = function () {
				if (isEditMode) {
					return NP_IVSwitch_CodecProfileAPIService.HasEditCodecProfilePermission();
				}
				else {
					return NP_IVSwitch_CodecProfileAPIService.HasAddCodecProfilePermission();
				}
			};

			$scope.scopeModel.save = function () {
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
			$scope.scopeModel.isLoading = true;

			if (isEditMode) {
				getCodecProfile().then(function () {
						loadAllControls();

				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
					$scope.scopeModel.isLoading = false;
				});
			}
			else if (isViewHistoryMode) {
				getCodecProfileHistory().then(function () {
					loadAllControls();
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
					$scope.isLoading = false;
				});

			}

			else {
				loadAllControls();
			}
		}

		function getCodecProfileHistory() {
			return NP_IVSwitch_CodecProfileAPIService.GetCodecProfileHistoryDetailbyHistoryId(context.historyId).then(function (response) {
				codecProfileEditorRuntimeEntity = response;
				codecProfileEntity = codecProfileEditorRuntimeEntity.Entity;
				codecDefEntity = codecProfileEditorRuntimeEntity.CodecDefList;
			});
		}

		function getCodecProfile() {
			return NP_IVSwitch_CodecProfileAPIService.GetCodecProfileEditorRuntime(codecProfileId).then(function (response) {
				codecProfileEditorRuntimeEntity = response;
				codecProfileEntity = codecProfileEditorRuntimeEntity.Entity;
				codecDefEntity = codecProfileEditorRuntimeEntity.CodecDefList;
			});
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData,loadGrid]).catch(function (error) {
				VRNotificationService.notifyExceptionWithClose(error, $scope);
			}).finally(function () {

				$scope.scopeModel.isLoading = false;
			});
			function setTitle() {
				if (isEditMode) {
					var codecProfileName = (codecProfileEntity != undefined) ? codecProfileEntity.ProfileName : null;
					$scope.title = UtilsService.buildTitleForUpdateEditor(codecProfileName, 'Codec Profile');
				}

				else if (isViewHistoryMode && codecProfileEntity != undefined)
					$scope.title = "Codec Profile: " + codecProfileEntity.ProfileName;

				else {
					$scope.title = UtilsService.buildTitleForAddEditor('Codec Profile');
				}
			}
			function loadStaticData() {
				if (codecProfileEntity == undefined)
					return;
				$scope.scopeModel.name = codecProfileEntity.ProfileName;
			}
		}

		function insert() {
			$scope.scopeModel.isLoading = true;

			codecDefEntity = gridAPI.getData();

			return NP_IVSwitch_CodecProfileAPIService.AddCodecProfile(buildCodecProfileObjFromScope()).then(function (response) {
				if (VRNotificationService.notifyOnItemAdded('Codec Profile', response, 'Name')) {
					if ($scope.onCodecProfileAdded != undefined)
						$scope.onCodecProfileAdded(response.InsertedObject);
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
			});
		}

		function update() {
			$scope.scopeModel.isLoading = true;

			codecDefEntity = gridAPI.getData();

			return NP_IVSwitch_CodecProfileAPIService.UpdateCodecProfile(buildCodecProfileObjFromScope()).then(function (response) {
				if (VRNotificationService.notifyOnItemUpdated('Codec Profile', response, 'Name')) {

					if ($scope.onCodecProfileUpdated != undefined) {
						$scope.onCodecProfileUpdated(response.UpdatedObject);
					}
					$scope.modalContext.closeModal();
				}
			}).catch(function (error) {
				VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
				$scope.scopeModel.isLoading = false;
				codecProfileEntity = undefined;
				codecProfileEditorRuntimeEntity = undefined;
			});
		}

		function buildCodecProfileObjFromScope() {

			for(var i =0; i<codecDefEntity.length; i++){
				codecDefIds[i] = codecDefEntity[i].CodecId;
			}
			return {
				CodecProfileId: codecProfileEntity != undefined ? codecProfileEntity.CodecProfileId : undefined,
				ProfileName: $scope.scopeModel.name,
				CodecDefId :  codecDefIds
			};
		}

		function loadGrid() {
			var gridDeffered = UtilsService.createPromiseDeferred();
			if (codecDefEntity != undefined) {
				for (var i = 0; i < codecDefEntity.length; i++) {
					codecDefList.push(codecDefEntity[i]);
				}
			}
			gridReadyDefferedPromise.promise.then(function () {
				var gridPayload = { codecDefList: codecDefList };
				VRUIUtilsService.callDirectiveLoad(gridAPI, gridPayload, gridDeffered);
			});
			return gridDeffered.promise;

		}
	}

	appControllers.controller('NP_IVSwitch_CodecProfileEditorController', CodecProfileEditorController);

})(appControllers);