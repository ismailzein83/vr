(function (app) {

	'use strict';

	ItemconfigMeasureEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_MeasureUnitEnum'];

	function ItemconfigMeasureEditorDirective(UtilsService, VRUIUtilsService, VR_Analytic_MeasureUnitEnum) {
		return {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var itemconfigMeasureEditor = new ItemconfigMeasureEditor(ctrl, $scope, $attrs);
				itemconfigMeasureEditor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {
				return {
					pre: function ($scope, iElem, iAttrs, ctrl) {

					}
				};
			},

			templateUrl: function (element, attrs) {
				return '/Client/Modules/Analytic/Directives/AnalyticItemConfig/Templates/MeasureEditorTemplate.html';
			}
		};

		function ItemconfigMeasureEditor(ctrl, $scope, attrs) {
			this.initializeController = initializeController;
			var joinSelectorAPI;
			var joinReadyDeferred = UtilsService.createPromiseDeferred();

			var dependentAggregateDimensionSelectorAPI;
			var dependentAggregateDimensionReadyDeferred = UtilsService.createPromiseDeferred();

			var dependentDimensionSelectorAPI;
			var dependentDimensionReadyDeferred = UtilsService.createPromiseDeferred();

			var requiredPermissionAPI;
			var requiredPermissionReadyDeferred = UtilsService.createPromiseDeferred();


			var fieldTypeAPI;
			var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();

			var analyticMeasureSelectorAPI;
			var analyticMeasureReadyDeferred = UtilsService.createPromiseDeferred();

			var selectedAnalyticMeasureDeferred;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.onJoinSelectorDirectiveReady = function (api) {
					joinSelectorAPI = api;
					joinReadyDeferred.resolve();
				};

				$scope.scopeModel.onDependentAggregateSelectorDirectiveReady = function (api) {
					dependentAggregateDimensionSelectorAPI = api;
					dependentAggregateDimensionReadyDeferred.resolve();
				};
				$scope.scopeModel.onDependentDimensionSelectorDirectiveReady = function (api) {
					dependentDimensionSelectorAPI = api;
					dependentDimensionReadyDeferred.resolve();
				};
				$scope.scopeModel.onRequiredPermissionReady = function (api) {
					requiredPermissionAPI = api;
					requiredPermissionReadyDeferred.resolve();
				};
				$scope.scopeModel.onFieldTypeReady = function (api) {
					fieldTypeAPI = api;
					fieldTypeReadyDeferred.resolve();
				};

				$scope.scopeModel.onAnalyticMeasureUnitReady = function (api) {
					analyticMeasureSelectorAPI = api;
					analyticMeasureReadyDeferred.resolve();
				};

				$scope.scopeModel.onAnalyticMeasureUnitSelectionChanged = function (option) {
					if (option != undefined) {
						if (selectedAnalyticMeasureDeferred != undefined) {
							selectedAnalyticMeasureDeferred.resolve();
						}
						else {
							$scope.scopeModel.fixAnalyticMeasure = (option.value == VR_Analytic_MeasureUnitEnum.Fixed.value) ? true : false;
							if ($scope.scopeModel.fixAnalyticMeasure)
							$scope.scopeModel.fixedMeasureUnit = "";
						}
					}
					else
						$scope.scopeModel.fixAnalyticMeasure = false;
				};
				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var tableId;
					var promises = [];
					var configEntity;

					if (payload != undefined) {
						tableId = payload.tableId;
						configEntity = payload.ConfigEntity;
						if (configEntity != undefined) {
							$scope.scopeModel.sqlExpressionMethod = configEntity.GetValueMethod;
							$scope.scopeModel.measureDescription = configEntity.Description;
							$scope.scopeModel.hideInLegend = configEntity.HideDescriptionInLegend;
						}
						var loadJoinDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
						joinReadyDeferred.promise.then(function () {
							var payloadJoinDirective = {
								filter: { TableIds: [tableId] },
								selectedIds: configEntity != undefined ? configEntity.JoinConfigNames : undefined
							};
							VRUIUtilsService.callDirectiveLoad(joinSelectorAPI, payloadJoinDirective, loadJoinDirectivePromiseDeferred);
						});
						promises.push(loadJoinDirectivePromiseDeferred.promise);

						var loadAggregateDependentDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
						dependentAggregateDimensionReadyDeferred.promise.then(function () {
							var payloadParentDirective = {
								filter: { TableIds: [tableId] },
								selectedIds: configEntity != undefined ? configEntity.DependentAggregateNames : undefined
							};

							VRUIUtilsService.callDirectiveLoad(dependentAggregateDimensionSelectorAPI, payloadParentDirective, loadAggregateDependentDirectivePromiseDeferred);
						});
						promises.push(loadAggregateDependentDirectivePromiseDeferred.promise);

						var loadDependentDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
						dependentDimensionReadyDeferred.promise.then(function () {
							var payloadParentDirective = {
								filter: { TableIds: [tableId] },
								selectedIds: configEntity != undefined ? configEntity.DependentDimensions : undefined
							};

							VRUIUtilsService.callDirectiveLoad(dependentDimensionSelectorAPI, payloadParentDirective, loadDependentDirectivePromiseDeferred);
						});
						promises.push(loadDependentDirectivePromiseDeferred.promise);

						var loadFieldTypePromiseDeferred = UtilsService.createPromiseDeferred();
						fieldTypeReadyDeferred.promise.then(function () {
							var payloadFieldTypeDirective = configEntity != undefined ? configEntity.FieldType : undefined;

							VRUIUtilsService.callDirectiveLoad(fieldTypeAPI, payloadFieldTypeDirective, loadFieldTypePromiseDeferred);
						});
						promises.push(loadFieldTypePromiseDeferred.promise);

						var requiredPermissionLoadDeferred = UtilsService.createPromiseDeferred();

						requiredPermissionReadyDeferred.promise.then(function () {
							var payload;

							if (configEntity != undefined && configEntity.RequiredPermission != null) {
								payload = {
									data: configEntity.RequiredPermission
								};
							}

							VRUIUtilsService.callDirectiveLoad(requiredPermissionAPI, payload, requiredPermissionLoadDeferred);
						});
						promises.push(requiredPermissionLoadDeferred.promise);

						var analyticMeasureLoadDeferred = UtilsService.createPromiseDeferred();
						analyticMeasureReadyDeferred.promise.then(function () {
							selectedAnalyticMeasureDeferred = UtilsService.createPromiseDeferred();
							$scope.scopeModel.analyticMeasureUnit = UtilsService.getArrayEnum(VR_Analytic_MeasureUnitEnum);
							if (payload != undefined && payload.ConfigEntity != undefined) {
								$scope.scopeModel.selectedAnalyticMeasureUnit = UtilsService.getItemByVal($scope.scopeModel.analyticMeasureUnit, payload.ConfigEntity.MeasureUnit, 'value');
								$scope.scopeModel.fixAnalyticMeasure = (payload.ConfigEntity.MeasureUnit == VR_Analytic_MeasureUnitEnum.Fixed.value) ? true : false;

								if ($scope.scopeModel.fixAnalyticMeasure)
									$scope.scopeModel.fixedMeasureUnit = payload.ConfigEntity.FixedMeasureUnit;
							}

							analyticMeasureLoadDeferred.resolve();
						});
						promises.push(analyticMeasureLoadDeferred.promise);

						return UtilsService.waitMultiplePromises(promises).then(function () {
							selectedAnalyticMeasureDeferred = undefined;
						});
					}


				};

				api.getData = function () {
					var fieldType = fieldTypeAPI != undefined ? fieldTypeAPI.getData() : undefined;
					var joinConfigNames = joinSelectorAPI != undefined ? joinSelectorAPI.getSelectedIds() : undefined;
					var dependentAggregateNames = dependentAggregateDimensionSelectorAPI != undefined ? dependentAggregateDimensionSelectorAPI.getSelectedIds() : undefined;
					var dependentDimensions = dependentDimensionSelectorAPI != undefined ? dependentDimensionSelectorAPI.getSelectedIds() : undefined;

					var dimension = {
						$type: "Vanrise.Analytic.Entities.AnalyticMeasureConfig ,Vanrise.Analytic.Entities",
						GetValueMethod: $scope.scopeModel.sqlExpressionMethod,
						JoinConfigNames: joinConfigNames,
						DependentAggregateNames: dependentAggregateNames,
						DependentDimensions: dependentDimensions,
						FieldType: fieldType,
						RequiredPermission: requiredPermissionAPI.getData(),
						Description: $scope.scopeModel.measureDescription,
						HideDescriptionInLegend: $scope.scopeModel.hideInLegend,
						MeasureUnit: ($scope.scopeModel.selectedAnalyticMeasureUnit != undefined) ? $scope.scopeModel.selectedAnalyticMeasureUnit.value:null,
						FixedMeasureUnit: ($scope.scopeModel.fixAnalyticMeasure) ? $scope.scopeModel.fixedMeasureUnit : null
					};
					return dimension;
				};

				if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
					ctrl.onReady(api);
				}
			}
		}
	}

	app.directive('vrAnalyticItemconfigMeasureEditor', ItemconfigMeasureEditorDirective);

})(app);
