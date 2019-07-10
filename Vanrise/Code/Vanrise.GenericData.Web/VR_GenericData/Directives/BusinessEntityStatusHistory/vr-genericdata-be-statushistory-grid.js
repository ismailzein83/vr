"use strict";

app.directive("vrGenericdataBeStatushistoryGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_BusinessEntityStatusHistoryAPIService", "VRUIUtilsService", "VRCommon_StyleDefinitionAPIService",
	function (UtilsService, VRNotificationService, VR_GenericData_BusinessEntityStatusHistoryAPIService, VRUIUtilsService, VRCommon_StyleDefinitionAPIService) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope:
			{
				onReady: "=",
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var dataRecordTypeGrid = new BEStatusHistoryGrid($scope, ctrl, $attrs);
				dataRecordTypeGrid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityStatusHistory/Templates/BusinessEntityStatusHistoryGrid.html"

		};

		function BEStatusHistoryGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var gridAPI;
			var gridDeferredReady = UtilsService.createPromiseDeferred();
			var styleDefinitions = [];


			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.statusHistories = [];

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					gridDeferredReady.resolve();
				};

				$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return VR_GenericData_BusinessEntityStatusHistoryAPIService.GetFilteredBusinessEntitiesStatusHistory(dataRetrievalInput)
						.then(function (response) {
							onResponseReady(response);
						})
						.catch(function (error) {
							VRNotificationService.notifyException(error, $scope);
						});
				};

				ctrl.getPreviousStatusColor = function (dataItem, colDef) {
					if (dataItem != undefined && dataItem.PreviousStyleDefinitionId != undefined) {
						var style = UtilsService.getItemByVal(styleDefinitions, dataItem.PreviousStyleDefinitionId, 'StyleDefinitionId');
						if (style != undefined)
							return style.StyleDefinitionSettings.StyleFormatingSettings;
					}
				};

				ctrl.getStatusColor = function (dataItem, colDef) {
					if (dataItem != undefined && dataItem.StyleDefinitionId != undefined) {
						var style = UtilsService.getItemByVal(styleDefinitions, dataItem.StyleDefinitionId, 'StyleDefinitionId');
						if (style != undefined)
							return style.StyleDefinitionSettings.StyleFormatingSettings;
					}
				};

				defineAPI();
			}
			function defineAPI() {
				var api = {};

				api.loadGrid = function (payload) {
					var gridQuery;
					var promises = [];
					var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();

					promises.push(loadGridPromiseDeferred.promise);
					promises.push(loadStyleDefinitions());

					if (payload != undefined)
						gridQuery = payload.query;

					gridDeferredReady.promise.then(function () {
						gridAPI.retrieveData(gridQuery).then(function () {
							loadGridPromiseDeferred.resolve();
						});
					});
					return UtilsService.waitMultiplePromises(promises);
				};
				function loadStyleDefinitions() {
					return VRCommon_StyleDefinitionAPIService.GetAllStyleDefinitions().then(function (response) {
						if (response) {
							for (var i = 0; i < response.length; i++) {
								styleDefinitions.push(response[i]);
							}
						}
					});
				}


				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

		}
		return directiveDefinitionObject;
	}
]);