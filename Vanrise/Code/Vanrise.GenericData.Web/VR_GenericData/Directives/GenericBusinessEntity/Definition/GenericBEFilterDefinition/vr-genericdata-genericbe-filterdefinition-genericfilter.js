(function (app) {

    'use strict';

    FilterDefinitionGenericFilterDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService'];

	function FilterDefinitionGenericFilterDirective(UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new GenericFilterCtor($scope, ctrl);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/GenericFilterDefinitionTemplate.html'
		};

		function GenericFilterCtor($scope, ctrl) {

			var context;
			var settings;
			var firstLoad = true;
			var dataRecordFieldTypes = [];
			var defaultFieldsValues;
			var fieldType;

			var dataRecordTypeFieldsSelectorAPI;
			var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
			var dataRecordTypePromiseDeferred;

			var localizationTextResourceSelectorAPI;
			var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			// var fieldTypeRuntimeDirectiveAPI;
			// var fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			this.initializeController = initializeController;
			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
					dataRecordTypeFieldsSelectorAPI = api;
					dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
				};

				$scope.scopeModel.onDataRecordTypeFieldsSelectionChanged = function () {
					if (firstLoad) return;
					if (dataRecordTypeFieldsSelectorAPI.getSelectedValue() != undefined) {
						$scope.scopeModel.fieldTitle = dataRecordTypeFieldsSelectorAPI.getSelectedValue().Title;
					}
					else {
						$scope.scopeModel.fieldTitle = undefined;
					}

				};

				$scope.scopeModel.onLocalizationTextResourceDirectiveReady = function (api) {
					localizationTextResourceSelectorAPI = api;
					localizationTextResourceSelectorReadyPromiseDeferred.resolve();
				};

				//$scope.scopeModel.onFieldTypeRumtimeDirectiveReady = function (api) {
				//    fieldTypeRuntimeDirectiveAPI = api;
				//    console.log(api);
				//    fieldTypeRuntimeReadyPromiseDeferred.resolve();
				//};

				//$scope.scopeModel.onDataRecordTypeFieldsSelectionChanged = function () {
				//    var item = dataRecordTypeFieldsSelectorAPI.getSelectedValue();

				//    if (item != undefined) {
				//        if (dataRecordTypePromiseDeferred != undefined) {
				//            dataRecordTypePromiseDeferred.resolve();
				//        }
				//        else {
				//            $scope.scopeModel.fieldTitle = item.Title;
				//            $scope.scopeModel.fieldTypeRuntimeDirective = undefined;
				//            fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

				//            fieldType = context.getFieldType(item.Name);
				//            var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");

				//            if (dataRecordFieldType != undefined && dataRecordFieldType.FilterEditor != undefined) {
				//                $scope.scopeModel.fieldTypeRuntimeDirective = dataRecordFieldType.FilterEditor;

				//                fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
				//                    var setLoader = function (value) { $scope.scopeModel.isFieldTypeRumtimeDirectiveLoading = value; };
				//                    var directivePayload = {
				//                        fieldTitle: "Default value",
				//                        fieldType: fieldType,
				//                        fieldName: item.Name,
				//                    };
				//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldTypeRuntimeDirectiveAPI, directivePayload, setLoader);
				//                });
				//            }
				//        }
				//    }
				//    else {
				//        $scope.scopeModel.fieldTitle = undefined;
				//       // fieldTypeRuntimeDirectiveAPI = undefined;
				//    }
				defineAPI();
			};
			function defineAPI() {
				var api = {};
				api.getData = function () {
					//var filterValues;
					//if (fieldTypeRuntimeDirectiveAPI != undefined) {
					//    filterValues = fieldTypeRuntimeDirectiveAPI.getValuesAsArray();
					//}
					//var finalArray = [];
					//if (filterValues != undefined) {
					//    for (var i = 0; i < filterValues.length; i++) {
					//        var filterValue = filterValues[i];
					//        if (filterValue != undefined) {
					//            finalArray.push(filterValue);
					//        }
					//    }
					//}
					return {
						$type: "Vanrise.GenericData.MainExtensions.GenericFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
						FieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
						FieldTitle: $scope.scopeModel.fieldTitle,
						IsRequired: $scope.scopeModel.isRequired,
						//DefaultFieldValues: (finalArray != undefined && finalArray.length != 0) ? finalArray : undefined
						TextResourceKey: localizationTextResourceSelectorAPI.getSelectedValues()
					};
				};

				api.load = function (payload) {
					var initialPromises = [];
					if (payload != undefined) {
						context = payload.context;
						settings = payload.settings;
					}

					function loadDataRecordTitleFieldsSelector() {
						var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
						dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
							var fieldsPayload = {
								dataRecordTypeId: context.getDataRecordTypeId(),
							};
							if (payload.settings != undefined && payload.settings.FieldName != undefined) {
								fieldsPayload.selectedIds = payload.settings.FieldName;
							}
							VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
						});
						return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
					}

					function loadStaticData() {
						if (payload != undefined && payload.settings != undefined) {
							$scope.scopeModel.fieldTitle = payload.settings.FieldTitle;
							$scope.scopeModel.isRequired = payload.settings.IsRequired;
						}
					}

					function loadLocalizationTextResourceSelector() {
						var loadSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
						localizationTextResourceSelectorReadyPromiseDeferred.promise.then(function () {
							var directivePayload = {
								selectedValue: (payload != undefined && payload.settings != undefined) ? payload.settings.TextResourceKey : undefined

							};
							VRUIUtilsService.callDirectiveLoad(localizationTextResourceSelectorAPI, directivePayload, loadSelectorPromiseDeferred);
						});
						return loadSelectorPromiseDeferred.promise;
					}

					//function getDataRecordFieldTypeConfigs() {
					//    return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
					//        for (var i = 0; i < response.length; i++) {
					//            var dataRecordFieldType = response[i];
					//            dataRecordFieldTypes.push(dataRecordFieldType);
					//        }
					//    });
					//}

					//function getFieldTypeRuntimeDirective() {
					//    getDataRecordFieldTypeConfigs().then(function () {
					//        if (settings != undefined) {
					//            fieldType = context.getFieldType(settings.FieldName);
					//            var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, fieldType.ConfigId, "ExtensionConfigurationId");
					//            if (dataRecordFieldType != undefined && dataRecordFieldType.FilterEditor != undefined) {
					//                $scope.scopeModel.fieldTypeRuntimeDirective = dataRecordFieldType.FilterEditor;
					//            }
					//        }
					//    }).catch(function () {
					//        $scope.scopeModel.fieldTypeRuntimeDirective = undefined;
					//    });
					//}

					//function loadFieldTypeRuntimeEditor() {
					//    var loadRuntimeEditorPromiseDeferred = UtilsService.createPromiseDeferred();

					//    fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
					//        dataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred();
					//        var directivePayload = {
					//            fieldTitle: "Default Value(s)",
					//            fieldType: fieldType,
					//            fieldName: settings != undefined ? settings.FieldName : undefined,
					//            fieldValue: settings != undefined ? settings.DefaultFieldValues : undefined
					//        };
					//        VRUIUtilsService.callDirectiveLoad(fieldTypeRuntimeDirectiveAPI, directivePayload, loadRuntimeEditorPromiseDeferred);
					//    });
					//    return loadRuntimeEditorPromiseDeferred.promise;
					//}

					return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadDataRecordTitleFieldsSelector, loadLocalizationTextResourceSelector]).then(function () {
					}).finally(function () {
						dataRecordTypePromiseDeferred = undefined;
						setTimeout(function () {
							firstLoad = false;
						}, 1);
					});


					//loadStaticData();
					//initialPromises.push(getFieldTypeRuntimeDirective());
					//initialPromises.push(loadDataRecordTitleFieldsSelector());

					//var rootPromiseNode = {
					//    promises: initialPromises,
					//    getChildNode: function () {
					//        var directivePromises = [];

					//        directivePromises.push(loadFieldTypeRuntimeEditor());

					//        return {
					//            promises: directivePromises
					//            }
					//        };
					//    }
					//};

					//return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
					//    dataRecordTypePromiseDeferred = undefined;
					//    setTimeout(function () {
					//        firstLoad = false;
					//    }, 1);
					//});
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}


			function getContext() {
				return context;
			}
		}
	}
    app.directive('vrGenericdataGenericbeFilterdefinitionGenericfilter', FilterDefinitionGenericFilterDirective);
})(app);