"use strict";

app.directive("businessprocessVrCallhttpserviceHeadersGrid", ["UtilsService", "VRUIUtilsService","VRCommon_FieldTypesService",
    function (UtilsService, VRUIUtilsService, VRCommon_FieldTypesService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "="
			},

			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var headersGrid = new HeadersGrid($scope, ctrl, $attrs);
				headersGrid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/CallHttpService/Templates/VRCallHttpServiceHeadersGridTemplate.html'
		};

		function HeadersGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var gridAPI;
            var context;

			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.headers = [];

				$scope.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				$scope.scopeModel.addRow = function (data) {
                    prepareItemToAdd();
                };

				$scope.scopeModel.deleteRow = function (dataItem) {
					var index = UtilsService.getItemIndexByVal($scope.scopeModel.headers, dataItem.Entity.Key, 'Entity.Key');
					if (index > -1) {
						$scope.scopeModel.headers.splice(index, 1);
					}
				};

				$scope.scopeModel.validateColumns = function () {
					//if ($scope.scopeModel.headers == undefined || $scope.scopeModel.headers.length == 0) {
					//	return 'Please, one record must be added at least.';
					//}

					var keys = [];
					for (var i = 0; i < $scope.scopeModel.headers.length; i++) {
						if ($scope.scopeModel.headers[i].Entity.Key != undefined) {
							var key = $scope.scopeModel.headers[i].Entity.Key.toLowerCase();
							if (UtilsService.contains(keys, key))
								return 'Two or more headers have the same Key';
							keys.push(key);
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
                $scope.scopeModel.headers.push(dataItem);
            }
            function perpareItem(headerObject) {

                var dataItem = {
                    Entity: {
                        Key: headerObject.payload.Key
                    }
                };
                
                dataItem.onValueExpressionBuilderDirectiveReady = function (api) {
                    dataItem.valueExpressionBuilderDirectiveAPI = api;
                    headerObject.valueReadyPromiseDeferred.resolve();
                };
                headerObject.valueReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        context: context,
                        value: headerObject.payload.Value
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.valueExpressionBuilderDirectiveAPI, payload, headerObject.valueLoadPromiseDeferred);
                });
                $scope.scopeModel.headers.push(dataItem);
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
                        if (payload.headers != undefined) {
                            for (var i = 0; i < payload.headers.length; i++) {
                                var headerObject = {
                                    payload: payload.headers[i],
                                    valueReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    valueLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };

                                promises.push(headerObject.valueLoadPromiseDeferred.promise);
                                perpareItem(headerObject);
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    var items = [];
                    for (var x = 0; x < $scope.scopeModel.headers.length; x++) {
                        var header = $scope.scopeModel.headers[x];
                        items.push(
                            {
                                Key: header.Entity.Key,
                                Value: header.valueExpressionBuilderDirectiveAPI != undefined ? header.valueExpressionBuilderDirectiveAPI.getData() : undefined
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