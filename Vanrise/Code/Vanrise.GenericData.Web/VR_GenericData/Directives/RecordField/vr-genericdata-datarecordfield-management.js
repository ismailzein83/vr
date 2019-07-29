"use strict";

app.directive("vrGenericdataDatarecordfieldManagement", ["UtilsService", "VRNotificationService", "VR_GenericData_DataRecordFieldService", "VR_GenericData_DataRecordFieldAPIService", "VRUIUtilsService", "VR_GenericData_DataRecordTypeAPIService",
	function (UtilsService, VRNotificationService, VR_GenericData_DataRecordFieldService, VR_GenericData_DataRecordFieldAPIService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "=",
				validationcontext: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var dataRecordFieldManagement = new DataRecordFieldManagement($scope, ctrl, $attrs);
				dataRecordFieldManagement.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordField/Templates/DataRecordFieldManagement.html"
		};

		function DataRecordFieldManagement($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var gridAPI;
			var gridAPIdeferred = UtilsService.createPromiseDeferred();
			var dataRecordTypeExtraFieldsApi;
			var dataRecordTypeExtraFieldsReadyDeferred;

			function initializeController() {
				ctrl.datasource = [];

				ctrl.fieldTypeConfigs = [];

				ctrl.isValid = function () {
					if (ctrl.hasExtraFields || (ctrl.datasource != undefined && ctrl.datasource.length > 0))
						return null;
					return "You Should Add at least one field";
				};

				ctrl.onDataRecordTypeExtraFieldsReady = function (api) {
					dataRecordTypeExtraFieldsApi = api;
					var setLoader = function (value) {
						ctrl.isDirectiveLoading = value;
					};
					VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordTypeExtraFieldsApi, undefined, setLoader, dataRecordTypeExtraFieldsReadyDeferred);
				};

				ctrl.disableAddButton = function () {
					if (ctrl.isValid() != null)
						return false;

					return ctrl.validationcontext.validate() != null;
				};

				ctrl.removeFilter = function (item) {
					var index = UtilsService.getItemIndexByVal(ctrl.datasource, item.rowIndex, 'rowIndex');
					if (index > -1)
						ctrl.datasource.splice(index, 1);
				};

				$scope.scopeModel = {};

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					gridAPIdeferred.resolve();
				};

				ctrl.addDataRecordField = function () {
					var gridItem = {
						Name: undefined,
						Title: undefined,
						fieldTypeSelectorAPI: undefined,
						fieldTypeSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
					};

					gridItem.edit = function (dataItem) {

						var onDataRecordFieldUpdated = function (dataRecordField) {
							dataItem.Formula = dataRecordField.Formula;
						};
						var allFields = [];
						for (var j = 0; j < ctrl.datasource.length; j++) {
							allFields.push(ctrl.datasource[j]);
						}

						getDataRecordExtraFieldsReturnedPromise(allFields).then(function () {
							var formula = dataItem != undefined ? dataItem.Formula : undefined;
							VR_GenericData_DataRecordFieldService.editDataRecordField(onDataRecordFieldUpdated, formula, allFields, true);
						});
					};

					gridItem.onFieldTypeSelectorReady = function (api) {
						gridItem.fieldTypeSelectorAPI = api;
						gridItem.fieldTypeSelectorReadyDeferred.resolve();
					};

					gridItem.fieldTypeSelectorReadyDeferred.promise.then(function () {
						var dataRecordFieldTypePayload = {};
						dataRecordFieldTypePayload.additionalParameters = { showDependantFieldsGrid: true };
						var setLoader = function (value) { gridItem.isFieldTypeSelectorLoading = value; };
						VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gridItem.fieldTypeSelectorAPI, dataRecordFieldTypePayload, setLoader);
					});
					gridAPI.expandRow(gridItem);
					ctrl.datasource.push(gridItem);
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.getData = function () {
					var fields;
					if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
						fields = [];
						for (var i = 0; i < ctrl.datasource.length; i++) {
							var currentItem = ctrl.datasource[i];
							if (currentItem != undefined) {
								fields.push({
									Name: currentItem.Name,
									Type: (currentItem.fieldTypeSelectorAPI != undefined) ? currentItem.fieldTypeSelectorAPI.getData() : null,
									Title: currentItem.Title,
									Formula: currentItem.Formula,
								});
							}
						}
					}

					var obj = {
						Settings: {
							DateTimeField: ctrl.dateTimeField,
							IdField: ctrl.idField
						},
						Fields: fields,
						HasExtraFields: ctrl.hasExtraFields,
						ExtraFieldsEvaluator: ctrl.hasExtraFields ? dataRecordTypeExtraFieldsApi.getData() : undefined
					};
					return obj;
				};

				api.load = function (payload) {
					var promises = [];
					var settings;
					if (payload != undefined) {
						settings = payload.Settings;

						ctrl.hasExtraFields = payload.ExtraFieldsEvaluator != undefined;
						if (ctrl.hasExtraFields) {
							dataRecordTypeExtraFieldsReadyDeferred = UtilsService.createPromiseDeferred();
							var directiveLoadDeferred = UtilsService.createPromiseDeferred();

							dataRecordTypeExtraFieldsReadyDeferred.promise.then(function () {

								dataRecordTypeExtraFieldsReadyDeferred = undefined;
								var dataRecordTypeExtraFieldsPayload = payload.ExtraFieldsEvaluator;
								VRUIUtilsService.callDirectiveLoad(dataRecordTypeExtraFieldsApi, dataRecordTypeExtraFieldsPayload, directiveLoadDeferred);
							});

							promises.push(directiveLoadDeferred.promise);
						}
					}

					if (settings != undefined) {
						ctrl.dateTimeField = settings.DateTimeField;
						ctrl.idField = settings.IdField;
					}
					var dataRecorddFieldTypePromise = VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
						angular.forEach(response, function (item) {
							ctrl.fieldTypeConfigs.push(item);
						});
					});
					promises.push(dataRecorddFieldTypePromise);

					if (payload != undefined && payload.Fields != undefined) {
						for (var i = 0; i < payload.Fields.length; i++) {
							var field = payload.Fields[i];
							if (field != undefined) {
								var gridItem = {
									Name: field.Name,
									Type: field.Type,
									Title: field.Title,
									Formula: field.Formula,
									fieldTypeSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
									fieldTypeSelectorLoadDeferred: UtilsService.createPromiseDeferred(),
								};
								promises.push(gridItem.fieldTypeSelectorLoadDeferred.promise);

								addColumnOnEdit(gridItem);
								addNeededFields(gridItem);
								ctrl.datasource.push(gridItem);
							}
						}

						gridAPIdeferred.promise.then(function () {
							for (var i = 0; i < ctrl.datasource.length; i++) {
								gridAPI.expandRow(ctrl.datasource[i]);
							};
						});
					}
					return UtilsService.waitMultiplePromises(promises);
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function addColumnOnEdit(gridItem) {

				gridItem.onFieldTypeSelectorReady = function (api) {
					gridItem.fieldTypeSelectorAPI = api;
					gridItem.fieldTypeSelectorReadyDeferred.resolve();
					var dataRecordFieldTypePayload = gridItem.Type;
					dataRecordFieldTypePayload.additionalParameters = { showDependantFieldsGrid: true };
					VRUIUtilsService.callDirectiveLoad(gridItem.fieldTypeSelectorAPI, dataRecordFieldTypePayload, gridItem.fieldTypeSelectorLoadDeferred);
				};

				gridItem.edit = function (dataItem) {
					var isEdit = dataItem.Formula != undefined;
					var onDataRecordFieldAdded = function (dataRecordField) {
						dataItem.Formula = dataRecordField.Formula;
					};
					var onDataRecordFieldUpdated = function (dataRecordField) {
						dataItem.Formula = dataRecordField.Formula;
					};

					var allFields = [];

					for (var j = 0; j < ctrl.datasource.length; j++) {
						allFields.push(ctrl.datasource[j]);
					}
					getDataRecordExtraFieldsReturnedPromise(allFields).then(function () {
						var formula = dataItem != undefined ? dataItem.Formula : undefined;
						VR_GenericData_DataRecordFieldService.editDataRecordField(onDataRecordFieldUpdated, formula, allFields, true);

					});
				}
			}

			function addNeededFields(dataItem) {
				var template = UtilsService.getItemByVal(ctrl.fieldTypeConfigs, dataItem.Type.ConfigId, "ExtensionConfigurationId");
				dataItem.TypeDescription = template != undefined ? template.Name : "";
			}

			function getDataRecordExtraFieldsReturnedPromise(allFields) {
				var addDataRecordFieldReadyDeferred = UtilsService.createPromiseDeferred();

				var extraFieldsEvaluator = (dataRecordTypeExtraFieldsApi != undefined && ctrl.hasExtraFields) ? dataRecordTypeExtraFieldsApi.getData() : undefined;

				if (ctrl.hasExtraFields && extraFieldsEvaluator != undefined) {
					VR_GenericData_DataRecordTypeAPIService.GetDataRecordExtraFields(extraFieldsEvaluator).then(function (response) {
						if (response && response.length > 0) {
							for (var i = 0; i < response.length; i++) {
								allFields.push(response[i]);
							}
						}
						addDataRecordFieldReadyDeferred.resolve();
					});
				}
				else {
					addDataRecordFieldReadyDeferred.resolve();
				}

				return addDataRecordFieldReadyDeferred.promise;
			}

		}

		return directiveDefinitionObject;

	}
]);