(function (app) {

	'use strict';

    ListFilterDefinitionGenericFilterDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService','VRLocalizationService'];

    function ListFilterDefinitionGenericFilterDirective(UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, VRLocalizationService) {
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
			templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/GenericListFilterDefinitionTemplate.html'
		};

		function GenericFilterCtor($scope, ctrl) {
			this.initializeController = initializeController;

			var gridAPI;
            var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();


			var context;
			var settings;
			var dataRecordFieldTypes = [];
			var previousFieldTypeRuntimeDirective;

			var dataRecordTypeFieldsSelectorAPI;
			var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
			var dataRecordTypePromiseDeferredSelectionChanged;

			var fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var isLocalizationEnabled;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.datasource = [];

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					gridReadyPromiseDeferred.resolve();
                };

				$scope.scopeModel.removeGenericFilter = function (item) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, item.rowIndex, 'rowIndex');
					var recordTypeFieldindex = UtilsService.getItemIndexByVal(dataRecordTypeFieldsSelectorAPI.getSelectedValue(), item.name, 'Name');

					if (index > -1)
						$scope.scopeModel.datasource.splice(index, 1);

					if (recordTypeFieldindex > -1)
						dataRecordTypeFieldsSelectorAPI.getSelectedValue().splice(index, 1);
                };
                isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();
                $scope.scopeModel.isLocalizationEnabled = isLocalizationEnabled;

                $scope.scopeModel.onDataRecordTypeFieldsSelected = function (item) {
					if (item != undefined) {
						if (dataRecordTypePromiseDeferredSelectionChanged != undefined) {
							dataRecordTypePromiseDeferredSelectionChanged.resolve();
						} else {
							var dataItem = {
								title: item.Title,
								localizationTextResourceDirectiveReady: UtilsService.createPromiseDeferred(),
								localizationTextResourceAPI: undefined,
								fieldTypeRuntimeReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
								fieldTypeRuntimeDirectiveAPI: undefined,
								name: item.Name
							};

                            dataItem.onLocalizationTextResourceDirectiveReady = function (api) {
                                dataItem.localizationTextResourceAPI = api;
                                dataItem.localizationTextResourceDirectiveReady.resolve();
                            };

							dataItem.localizationTextResourceDirectiveReady.promise.then(function () {
								var modifiedByFieldSetLoader = function (value) { dataItem.localizationTextResourceLoader = value; };
								VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.localizationTextResourceAPI, {}, modifiedByFieldSetLoader, undefined);
							});

                            dataItem.onFieldTypeRumtimeDirectiveReady = function (api) {
								dataItem.fieldTypeRuntimeDirectiveAPI = api;
								dataItem.fieldTypeRuntimeReadyPromiseDeferred.resolve();
							};

                            dataItem.fieldType = context.getFieldType(item.Name);
                            var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, dataItem.fieldType.ConfigId, "ExtensionConfigurationId");

							if (dataRecordFieldType != undefined) {
								dataItem.fieldTypeRuntimeDirective = dataRecordFieldType.FilterEditor;
								if (previousFieldTypeRuntimeDirective != dataItem.fieldTypeRuntimeDirective) {
									fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
									previousFieldTypeRuntimeDirective = dataItem.fieldTypeRuntimeDirective;
								}

								dataItem.fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
									var setLoader = function (value) { dataItem.isFieldTypeRumtimeDirectiveLoading = value; };
									var directivePayload = {
										fieldTitle: "Default value",
                                        fieldType: dataItem.fieldType,
										fieldName: item.Name,
										fieldValue: undefined
									};
									VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.fieldTypeRuntimeDirectiveAPI, directivePayload, setLoader, undefined);
								});
                            }
                            gridReadyPromiseDeferred.promise.then(function () {
                                gridAPI.expandRow(dataItem);
                            });
							$scope.scopeModel.datasource.push(dataItem);
						}
					}
					else {
						$scope.scopeModel.fieldTitle = undefined;
						$scope.scopeModel.fieldTypeRuntimeDirective = undefined;
					}
				};

                $scope.scopeModel.onDataRecordTypeFieldsDeselected = function (item) {
					if (item != undefined) {
						var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, item.Name, 'name');
						if (index > -1)
							$scope.scopeModel.datasource.splice(index, 1);
					}
				};

				$scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
					dataRecordTypeFieldsSelectorAPI = api;
					dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var initialPromises = [];

					if (payload != undefined) {
						context = payload.context;
						settings = payload.settings;
					}

					function loadDataRecordTitleFieldsSelector() {
						if (payload.settings != undefined && payload.settings.FieldName != undefined) {
							dataRecordTypePromiseDeferredSelectionChanged = UtilsService.createPromiseDeferred();
						}

						var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
						dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
							var fieldsPayload = {
								dataRecordTypeId: context.getDataRecordTypeId(),
							};
                            if (payload.settings != undefined && payload.settings.Filters != undefined) {
								var selectedIds = [];
                                for (var i = 0; i < payload.settings.Filters.length; i++) {
                                    selectedIds.push(payload.settings.Filters[i].FieldName);
								}
								fieldsPayload.selectedIds = selectedIds;
							}
							VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
						});
						return loadDataRecordTypeFieldsSelectorPromiseDeferred.promise;
					}

                    function loadGrid(settings) {
                        var promises = [];
                        if (settings.Filters != undefined) {
                            var genericFilters = settings.Filters;
                            for (var i = 0; i < genericFilters.length; i++) {
                                var genericFilter = genericFilters[i];
                                var dataItem = {
                                    title: genericFilter.FieldTitle,
                                    isRequired: genericFilter.IsRequired,
                                    triggerSearch: genericFilter.TriggerSearch,
                                    name: genericFilter.FieldName,
                                    localizationTextResourceDirectiveReady: UtilsService.createPromiseDeferred(),
                                    localizationTextResourceAPI: undefined,
                                    localizationTextResourceLoad: UtilsService.createPromiseDeferred(),
                                    textResourceKey: genericFilter.TextResourceKey,
                                    fieldTypeRuntimeReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    fieldTypeRuntimeDirectiveAPI: undefined,
                                    fieldtypeRuntimeLoad: UtilsService.createPromiseDeferred(),
                                    defaultValues: genericFilter.DefaultFieldValues
                                };
                                addItemToGrid(dataItem);
                                if (isLocalizationEnabled)
                                promises.push(dataItem.localizationTextResourceLoad.promise);
                            }
                        }
                        return promises;
                    }

                    function addItemToGrid(dataItem) {
                        dataItem.onLocalizationTextResourceDirectiveReady = function (api) {
                            dataItem.localizationTextResourceAPI = api;
                            dataItem.localizationTextResourceDirectiveReady.resolve();
                        };

                        dataItem.localizationTextResourceDirectiveReady.promise.then(function () {
                            var directivePayload = {
                                selectedValue: dataItem.textResourceKey
                            };
                            VRUIUtilsService.callDirectiveLoad(dataItem.localizationTextResourceAPI,directivePayload, dataItem.localizationTextResourceLoad);
                        });

                        dataItem.onFieldTypeRumtimeDirectiveReady = function (api) {
                            dataItem.fieldTypeRuntimeDirectiveAPI = api;
                            dataItem.fieldTypeRuntimeReadyPromiseDeferred.resolve();
                        };

                        dataItem.fieldType = context.getFieldType(dataItem.name);
                        var dataRecordFieldType = UtilsService.getItemByVal(dataRecordFieldTypes, dataItem.fieldType.ConfigId, "ExtensionConfigurationId");

                        if (dataRecordFieldType != undefined) {
                            dataItem.fieldTypeRuntimeDirective = dataRecordFieldType.FilterEditor;
                            if (dataItem.previousFieldTypeRuntimeDirective != dataItem.fieldTypeRuntimeDirective) {
                                dataItem.fieldTypeRuntimeReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                dataItem.previousFieldTypeRuntimeDirective = dataItem.fieldTypeRuntimeDirective;
                            }

                            dataItem.fieldTypeRuntimeReadyPromiseDeferred.promise.then(function () {
                                dataItem.fieldtypeRuntimeLoad = UtilsService.createPromiseDeferred();
                                var directivePayload = {
                                    fieldTitle: "Default value",
                                    fieldType: dataItem.fieldType,
                                    fieldName: dataItem.name,
                                    fieldValue: dataItem.defaultValues
                                };
                                VRUIUtilsService.callDirectiveLoad(dataItem.fieldTypeRuntimeDirectiveAPI, directivePayload, dataItem.fieldtypeRuntimeLoad);
                            });
                        }
                        gridAPI.expandRow(dataItem);
                        $scope.scopeModel.datasource.push(dataItem);
                    }

					function getDataRecordFieldTypeConfigs() {
						return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
							for (var i = 0; i < response.length; i++) {
								var dataRecordFieldType = response[i];
								dataRecordFieldTypes.push(dataRecordFieldType);
							}
						});
					}

                    initialPromises.push(gridReadyPromiseDeferred.promise);
					initialPromises.push(getDataRecordFieldTypeConfigs());
                    initialPromises.push(loadDataRecordTitleFieldsSelector());


					var rootPromiseNode = {
						promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            if (settings != undefined) {
                                directivePromises=loadGrid(settings);
							}
							return {
								promises: directivePromises,
							};
						}
					};

					return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
						if (payload != undefined && payload.settings != undefined) {
							$scope.scopeModel.fieldTitle = payload.settings.FieldTitle;
						}
						dataRecordTypePromiseDeferredSelectionChanged = undefined;
					});
				};

				api.getData = function () {
					var dataItems = [];
                    var datasource = $scope.scopeModel.datasource;
					if (datasource != undefined) {
						for (var j = 0; j < datasource.length; j++) {
							var gridItem = datasource[j];
							var filterValues;
							if (gridItem.fieldTypeRuntimeDirectiveAPI != undefined) {
								filterValues = gridItem.fieldTypeRuntimeDirectiveAPI.getValuesAsArray();
							}

							var finalArray = [];
							if (filterValues != undefined) {
								for (var i = 0; i < filterValues.length; i++) {
									var filterValue = filterValues[i];
									if (filterValue != undefined) {
										finalArray.push(filterValue);
									}
								}
							}
							var dataItem = {
								FieldName: gridItem.name,
								FieldTitle: gridItem.title,
								IsRequired: gridItem.isRequired,
								TriggerSearch: gridItem.triggerSearch,
                                TextResourceKey: (gridItem.localizationTextResourceAPI != undefined) ? gridItem.localizationTextResourceAPI.getSelectedValues():undefined,
								DefaultFieldValues: (finalArray != undefined && finalArray.length != 0) ? finalArray : undefined
                            };
                            dataItems.push(dataItem);
                        }
                    }
					return {
						$type: "Vanrise.GenericData.MainExtensions.ListGenericFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        Filters: dataItems
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function getContext() {
				return context;
			}
		}
	}

	app.directive('vrGenericdataGenericbeListfilterdefinitionGenericfilter', ListFilterDefinitionGenericFilterDirective);
})(app);