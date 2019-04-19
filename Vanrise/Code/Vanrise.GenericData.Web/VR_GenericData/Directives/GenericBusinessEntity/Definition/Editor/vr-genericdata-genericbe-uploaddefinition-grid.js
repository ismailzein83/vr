"use strict";

app.directive("vrGenericdataGenericbeUploaddefinitionGrid", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService", "VR_GenericData_DataRecordFieldAPIService",
	function (UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService, VR_GenericData_DataRecordFieldAPIService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope:
			{
				onReady: "=",
				isrequired: '=',

			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var ctor = new UploadDefinitionGrid($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "uploadDefGridCtrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/UploadDefinitionGridTemplate.html"

		};

		function UploadDefinitionGrid($scope, ctrl, $attrs) {

			var dataRecordTypeId;
			var uploadedSelectorDirectiveAPI;
			var uploadedSelectorReadyDeffered = UtilsService.createPromiseDeferred();

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.uploadedFields = [];

				$scope.scopeModel.onDataRecordTypeUploadedFieldsSelectorDirectiveReady = function (api) {
					uploadedSelectorDirectiveAPI = api;
					uploadedSelectorReadyDeffered.resolve();
				};

				$scope.scopeModel.onSelectUploadFieldItem = function (option) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.uploadedFields, option.Name, "Name");
					if (option != undefined && index == -1) {
						var item = {

							Name: option.Name,
							TitleField: option.Title,
							IsRequired: false,
							localizationTextResourceSelectorReadyPromiseDeferred: UtilsService.createPromiseDeferred()
						};
						item.onLocalizationTextResourceDirectiveReady = function (api) {
							item.localizationTextResourceSelectorAPI = api;
							item.localizationTextResourceSelectorReadyPromiseDeferred.resolve();
						};
						item.localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
							var payload = {};
							var setLoader = function (value) { $scope.scopeModel.isLocalizationTextResourceSelectorLoading = value; };
							VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, item.localizationTextResourceSelectorAPI, payload, setLoader);
						});
						$scope.scopeModel.uploadedFields.push(item);
					}
				};

				$scope.scopeModel.onDeselectUploadFieldItem = function (option) {
					if (option != undefined) {
						var itemToRemove = UtilsService.getItemByVal($scope.scopeModel.uploadedFields, option.Name, "Name");
						var index = $scope.scopeModel.uploadedFields.indexOf(itemToRemove);
						$scope.scopeModel.uploadedFields.splice(index, 1);
					}
				};

				$scope.scopeModel.onDeselecAlltUploadFieldItems = function (option) {
					$scope.scopeModel.uploadedFields.length = 0;
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					$scope.scopeModel.uploadedFields.length = 0;
					var promises = [];

					if (payload != undefined) {
						promises.push(loadSelector(payload));
						if (payload.UploadFields != undefined && payload.UploadFields.length > 0) {
							var uploadFields = payload.UploadFields;
							var genericBEAddedValues = payload.genericBEAddedValues;
							if (uploadFields != undefined) {
								for (var x = 0; x < uploadFields.length; x++) {
									var uploadField = uploadFields[x];
									var genericBEAddedValue = genericBEAddedValues[x];
									var loadedItem = {
										TitleField: genericBEAddedValue.FieldTitle,
										IsRequired: uploadField.IsRequired,
										Name: uploadField.FieldName,
										TextResourceKey: uploadField.TextResourceKey,
										localizationTextResourceSelectorLoadDeferred: UtilsService.createPromiseDeferred(),
										localizationTextResourceSelectorReadyPromiseDeferred: UtilsService.createPromiseDeferred()
									};
									promises.push(loadedItem.localizationTextResourceSelectorLoadDeferred.promise);
									addColumnOnEdit(loadedItem);
								}
							}
						}
					}
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					var uploadFields = [];
					var dataFromScope = $scope.scopeModel.uploadedFields;
					if (dataFromScope != undefined && dataFromScope.length > 0)
						for (var x = 0; x < dataFromScope.length; x++) {
							var item = dataFromScope[x];
							var uploadField = {
								FieldName: item.Name,
								IsRequired: item.IsRequired,
								TextResourceKey: item.localizationTextResourceSelectorAPI != undefined ? item.localizationTextResourceSelectorAPI.getSelectedValues() : undefined
							};
							uploadFields.push(uploadField);
						}
					return uploadFields;
				};

				function loadSelector(payload) {
					var defferedSelector = UtilsService.createPromiseDeferred();
					dataRecordTypeId = payload.DataRecordTypeId;
					var selectedItems = [];

					var items = payload.UploadFields;
					if (items != undefined)
						for (var x = 0; x < items.length; x++) {
							selectedItems.push(items[x].FieldName);
						}

					uploadedSelectorReadyDeffered.promise.then(function () {
						var recordTypeTitlePayload = {
							dataRecordTypeId: dataRecordTypeId
						};
						if (selectedItems.length > 0)
							recordTypeTitlePayload.selectedIds = selectedItems;
						VRUIUtilsService.callDirectiveLoad(uploadedSelectorDirectiveAPI, recordTypeTitlePayload, defferedSelector);
					});

					return defferedSelector.promise;
				}

				function addColumnOnEdit(loadedItem) {
					loadedItem.onLocalizationTextResourceDirectiveReady = function (api) {
						loadedItem.localizationTextResourceSelectorAPI = api;
						loadedItem.localizationTextResourceSelectorReadyPromiseDeferred.resolve();
					};
					loadedItem.localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
						var payload = { selectedValue: loadedItem.TextResourceKey };
						VRUIUtilsService.callDirectiveLoad(loadedItem.localizationTextResourceSelectorAPI, payload, loadedItem.localizationTextResourceSelectorLoadDeferred);
					});
					$scope.scopeModel.uploadedFields.push(loadedItem);
				}

				api.clearDataSource = function () {
					ctrl.datasource.length = 0;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}



		}

		return directiveDefinitionObject;

	}
]);