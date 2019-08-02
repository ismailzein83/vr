"use strict";

app.directive("businessprocessVrCallhttpserviceUrlparametersGrid", ["UtilsService", "VRUIUtilsService","VRCommon_FieldTypesService",
    function (UtilsService, VRUIUtilsService, VRCommon_FieldTypesService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},

			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var urlParametersGrid = new URLParametersGrid($scope, ctrl, $attrs);
				urlParametersGrid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/CallHttpService/Templates/VRCallHttpServiceURLParameterGridTemplate.html'
		};

		function URLParametersGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var gridAPI;
            var context;
            var fieldTextType = VRCommon_FieldTypesService.getTextFieldType();

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.urlParameters = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.addRow = function (data) {
                    prepareItemToAdd();
                };

				$scope.scopeModel.deleteRow = function (dataItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.urlParameters, dataItem.Entity.Name, 'Entity.Name');
					if (index > -1) {
						$scope.scopeModel.urlParameters.splice(index, 1);
					}
				};

				$scope.scopeModel.validateColumns = function () {
					//if ($scope.scopeModel.urlParameters == undefined || $scope.scopeModel.urlParameters.length == 0) {
					//	return 'Please, one record must be added at least.';
					//}

					var names = [];
					for (var i = 0; i < $scope.scopeModel.urlParameters.length; i++) {
						if ($scope.scopeModel.urlParameters[i].Name != undefined) {
							var name = $scope.scopeModel.urlParameters[i].Name.toLowerCase();
							if (UtilsService.contains(names, name))
								return 'Two or more URL parameters have the same name';
							names.push(name);
						}
					}
					return null;
				};
			}
            function prepareItemToAdd() {
                var dataItem = {
                    Entity: {}
                };
                dataItem.onValueExpressionBuilderDirectiveReady = function (api) {
                    dataItem.valueExpressionBuilderDirectiveAPI = api;
                    var setLoader = function (value) { dataItem.isValueExpressionBuilderDirectiveLoading = value; };
                    var payload = {
                        context: context
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.valueExpressionBuilderDirectiveAPI, payload, setLoader);
                };
                $scope.scopeModel.urlParameters.push(dataItem);
            }
            function perpareItem(parameterObject) {

                var dataItem = {
                    Entity: {
                        Name: parameterObject.payload.Name
                    }
                };

                dataItem.onValueExpressionBuilderDirectiveReady = function (api) {
                    dataItem.valueExpressionBuilderDirectiveAPI = api;
                    parameterObject.valueReadyPromiseDeferred.resolve();
                };
                parameterObject.valueReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        context: context,
                        value: parameterObject.payload.Value
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.valueExpressionBuilderDirectiveAPI, payload, parameterObject.valueLoadPromiseDeferred);
                });
                $scope.scopeModel.urlParameters.push(dataItem);
            }
			function defineAPI() {
				var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = {
                            getWorkflowArguments: payload.getWorkflowArguments,
                            getParentVariables: payload.getParentVariables
                        };

                        if (payload.urlParameters != undefined) {
                            for (var i = 0; i < payload.urlParameters.length; i++) {
                                var urlParameterObject = {
                                    payload: payload.urlParameters[i],
                                    valueReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    valueLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(urlParameterObject.valueLoadPromiseDeferred.promise);
                                perpareItem(urlParameterObject);
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
				};

                api.getData = function () {
                    var items = [];
                    for (var x = 0; x < $scope.scopeModel.urlParameters.length; x++) {
                        var urlParameter = $scope.scopeModel.urlParameters[x];
                        items.push(
                            {
                                Name: urlParameter.Entity.Name,
                                Value: urlParameter.valueExpressionBuilderDirectiveAPI != undefined ? urlParameter.valueExpressionBuilderDirectiveAPI.getData() : undefined
                            });
                    }
                    return items;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);