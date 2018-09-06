"use strict";

app.directive("vrNpCodepreviewGrid", ["Vr_NP_CodePreparationPreviewAPIService", "Vr_NP_CodeChangeTypeEnum", "VRNotificationService",
	function (Vr_NP_CodePreparationPreviewAPIService, Vr_NP_CodeChangeTypeEnum, VRNotificationService) {

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
			templateUrl: "/Client/Modules/VR_NumberingPlan/Directives/CodePreview/Templates/CodePreparationCodePreviewGridTemplate.html"

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
					return Vr_NP_CodePreparationPreviewAPIService.GetFilteredCodePreview(dataRetrievalInput)
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
					case Vr_NP_CodeChangeTypeEnum.New.value:
						dataItem.codeStatusIconUrl = Vr_NP_CodeChangeTypeEnum.New.icon;
						dataItem.codeStatusIconTooltip = Vr_NP_CodeChangeTypeEnum.New.label;
						break;

					case Vr_NP_CodeChangeTypeEnum.Closed.value:
						dataItem.codeStatusIconUrl = Vr_NP_CodeChangeTypeEnum.Closed.icon;
						dataItem.codeStatusIconTooltip = Vr_NP_CodeChangeTypeEnum.Closed.label;
						break;

					case Vr_NP_CodeChangeTypeEnum.Moved.value:
						dataItem.showMovedTo = true;
						dataItem.showMovedFrom = true;
						if (zoneName != undefined && zoneName != '' && dataItem.Entity.ZoneName != zoneName) {
							dataItem.codeStatusIconUrl = Vr_NP_CodeChangeTypeEnum.MovedFrom.icon;
							dataItem.codeStatusIconTooltip = Vr_NP_CodeChangeTypeEnum.Moved.label + " to " + dataItem.Entity.ZoneName;
						}
						else {
							dataItem.codeStatusIconUrl = Vr_NP_CodeChangeTypeEnum.MovedTo.icon;
							dataItem.codeStatusIconTooltip = Vr_NP_CodeChangeTypeEnum.Moved.label + " from " + dataItem.Entity.RecentZoneName;
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
