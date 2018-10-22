//"use strict";

//app.directive("vrGenericdataGenericbeUploaddefinitionGrid", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService", "VR_GenericData_DataRecordFieldAPIService",
//	function (UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService, VR_GenericData_DataRecordFieldAPIService) {

//		var directiveDefinitionObject = {

//			restrict: "E",
//			scope:
//			{
//				onReady: "=",
//				isrequired: '=',

//			},
//			controller: function ($scope, $element, $attrs) {
//				var ctrl = this;

//				var ctor = new UploadDefinitionGrid($scope, ctrl, $attrs);
//				ctor.initializeController();
//			},
//			controllerAs: "uploadDefGridCtrl",
//			bindToController: true,
//			compile: function (element, attrs) {

//			},
//			templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/UploadDefinitionGridTemplate.html"

//		};

//		function UploadDefinitionGrid($scope, ctrl, $attrs) {

//			var dataRecordTypeId;
//			var uploadedSelectorDirectiveAPI;
//			var uploadedSelectorReadyDeffered = UtilsService.createPromiseDeferred();

//			this.initializeController = initializeController;
//			function initializeController() {
//				$scope.scopeModel = {};
//				$scope.scopeModel.uploadedFields = [];
//				$scope.scopeModel.onDataRecordTypeUploadedFieldsSelectorDirectiveReady = function (api) {
//					uploadedSelectorDirectiveAPI = api;
//					uploadedSelectorReadyDeffered.resolve();

//				};

//				$scope.scopeModel.onSelectUploadFieldItem = function (option) {
//					var index = UtilsService.getItemIndexByVal($scope.scopeModel.uploadedFields, option.Name, "Name");
//					if (option != undefined && index == -1) {
//						var item = {};
//						item.Name = option.Name;
//						item.TitleField = option.Title;
//						item.IsRequired = false;
//						$scope.scopeModel.uploadedFields.push(item);
//					}
//				};

//				$scope.scopeModel.onDeselectUploadFieldItem = function (option) {
//					console.log(option)
//					if (option != undefined) {
//						var itemToRemove = UtilsService.getItemByVal($scope.scopeModel.uploadedFields, option.Title, "TitleField");
//						var index = $scope.scopeModel.uploadedFields.indexOf(itemToRemove);
//						$scope.scopeModel.uploadedFields.splice(index, 1);
//					}
//				};

//				$scope.scopeModel.onDeselecAlltUploadFieldItems = function (option) {
//					$scope.scopeModel.uploadedFields.length = 0;

//				};

//				defineAPI();
//			}

//			function defineAPI() {
//				var api = {};

//				api.load = function (payload) {
//					$scope.scopeModel.uploadedFields = [];
//					var promises = [];


//					if (payload != undefined) {
//						promises.push(loadSelector(payload));
//						if (payload.UploadFields != undefined && payload.UploadFields.length > 0)
//							promises.push(loadGrid(payload));
//					}


//					return UtilsService.waitMultiplePromises(promises);

//				};

//				api.getData = function () {
//					var uploadFields = [];
//					var dataFromScope = $scope.scopeModel.uploadedFields;
//					if (dataFromScope != undefined && dataFromScope.length > 0)
//						for (var x = 0; x < dataFromScope.length; x++)
//							uploadFields.push({ FieldName: dataFromScope[x].Name, IsRequired: dataFromScope[x].IsRequired });
//					return uploadFields;
//				};

//				function loadSelector(payload) {
//					var defferedSelector = UtilsService.createPromiseDeferred();
//					dataRecordTypeId = payload.DataRecordTypeId;
//					var selectedItems = [];
//					if (payload.UploadFields != undefined)
//						for (var x = 0; x < payload.UploadFields.length; x++) {
//							selectedItems.push(payload.UploadFields[x].FieldName);
//						}
//					uploadedSelectorReadyDeffered.promise.then(function () {
//						var recordTypeTitlePayload = {
//							dataRecordTypeId: dataRecordTypeId
//						};
//						if (selectedItems.length > 0)
//							recordTypeTitlePayload.selectedIds = selectedItems;
//						VRUIUtilsService.callDirectiveLoad(uploadedSelectorDirectiveAPI, recordTypeTitlePayload, defferedSelector);
//					});

//					return defferedSelector.promise;
//				}

//				function loadGrid(payload) {
//					var defferedGrid = UtilsService.createPromiseDeferred();
//					var filter = [];
//					for (var x = 0; x < payload.UploadFields.length; x++)
//						filter.push(payload.UploadFields[x].FieldName);
//					var serializedFilter = UtilsService.serializetoJson({ IncludedFieldNames: filter });
//					VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(payload.DataRecordTypeId, serializedFilter).then(function (response) {
//						for (var x = 0; x < payload.UploadFields.length; x++) {
//							for (var i = 0; i < response.length; i++) {
//								if (response[i].Entity.Name == payload.UploadFields[x].FieldName)
//									$scope.scopeModel.uploadedFields.push({ TitleField: response[i].Entity.Title, IsRequired: payload.UploadFields[x].IsRequired, Name: response[i].Entity.Name });
//							}
//							defferedGrid.resolve();
//						}
//					});

//					return defferedGrid.promise;
//				}

//				api.clearDataSource = function () {
//					ctrl.datasource.length = 0;
//				};

//				if (ctrl.onReady != null)
//					ctrl.onReady(api);
//			}



//		}

//		return directiveDefinitionObject;

//	}
//]);