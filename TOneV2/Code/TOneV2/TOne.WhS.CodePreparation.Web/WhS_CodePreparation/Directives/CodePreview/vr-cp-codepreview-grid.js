"use strict";

app.directive("vrCpCodepreviewGrid", ["WhS_CP_CodePreparationPreviewAPIService", "WhS_CP_CodeChangeTypeEnum", "VRNotificationService",
	function (WhS_CP_CodePreparationPreviewAPIService, WhS_CP_CodeChangeTypeEnum, VRNotificationService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;

				var codePreviewGrid = new CodePreviewGrid($scope, ctrl, $attrs);
				codePreviewGrid.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: "/Client/Modules/WhS_CodePreparation/Directives/CodePreview/Templates/CodePreparationCodePreviewGridTemplate.html"

		};

		function CodePreviewGrid($scope, ctrl, $attrs) {

			var gridAPI;
			var zoneName;
			this.initializeController = initializeController;

			function initializeController() {

				$scope.showZoneName = true;
				$scope.changedCodes = [];
				$scope.onGridReady = function (api) {
					gridAPI = api;

					if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
						ctrl.onReady(getDirectiveAPI());
					function getDirectiveAPI() {

						var directiveAPI = {};
						directiveAPI.load = function (query) {
							zoneName = query.ZoneName;
							$scope.showZoneName = query.ShowZoneName != undefined ? query.ShowZoneName : true;
							return gridAPI.retrieveData(query);
						};

						return directiveAPI;
					}
				};


				$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return WhS_CP_CodePreparationPreviewAPIService.GetFilteredCodePreview(dataRetrievalInput)
						.then(function (response) {
							if (response && response.Data) {
								for (var i = 0; i < response.Data.length; i++) {
									mapDataNeeded(response.Data[i]);
								}
							}

							onResponseReady(response);
						})
						.catch(function (error) {
							VRNotificationService.notifyException(error, $scope);
						});
				};

			}


			function mapDataNeeded(dataItem) {
				dataItem.showMovedTo = false;
				dataItem.showMovedFrom = false;
				switch (dataItem.Entity.ChangeType) {
					case WhS_CP_CodeChangeTypeEnum.New.value:
						dataItem.codeStatusIconUrl = WhS_CP_CodeChangeTypeEnum.New.icon;
						dataItem.codeStatusIconTooltip = WhS_CP_CodeChangeTypeEnum.New.label;
						break;

					case WhS_CP_CodeChangeTypeEnum.Closed.value:
						dataItem.codeStatusIconUrl = WhS_CP_CodeChangeTypeEnum.Closed.icon;
						dataItem.codeStatusIconTooltip = WhS_CP_CodeChangeTypeEnum.Closed.label;
						break;

					case WhS_CP_CodeChangeTypeEnum.Moved.value:
						dataItem.showMovedTo = true;
						dataItem.showMovedFrom = true;
						if (zoneName != undefined && zoneName != '' && dataItem.Entity.ZoneName != zoneName) {
							dataItem.codeStatusIconUrl = WhS_CP_CodeChangeTypeEnum.MovedFrom.icon;
							dataItem.codeStatusIconTooltip = WhS_CP_CodeChangeTypeEnum.Moved.label + " to " + dataItem.Entity.ZoneName;
						}
						else {
							dataItem.codeStatusIconUrl = WhS_CP_CodeChangeTypeEnum.MovedTo.icon;
							dataItem.codeStatusIconTooltip = WhS_CP_CodeChangeTypeEnum.Moved.label + " from " + dataItem.Entity.RecentZoneName;
							dataItem.Entity.BED = dataItem.Entity.EED;
							dataItem.Entity.EED = "";
						}
						break;
				}

				if (zoneName == dataItem.Entity.ZoneName)
					dataItem.showMovedTo = false;

				if (dataItem.Entity.RecentZoneName == null || zoneName == dataItem.Entity.RecentZoneName)
					dataItem.showMovedFrom = false;
			}

		}

		return directiveDefinitionObject;

	}]);
