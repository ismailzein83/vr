"use strict";

app.directive('vrNpCodepreparationCodesgrid', ['VRNotificationService', 'VRUIUtilsService', 'Vr_NP_CodePrepAPIService', 'UtilsService', 'Vr_NP_CodeItemDraftStatusEnum', 'Vr_NP_CodeItemStatusEnum',
	function (VRNotificationService, VRUIUtilsService, Vr_NP_CodePrepAPIService, UtilsService, Vr_NP_CodeItemDraftStatusEnum, Vr_NP_CodeItemStatusEnum) {

		var directiveDefinitionObject = {

			restrict: "E",
			scope: {
				onReady: "=",
				selectedcodes: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new saleCodesGrid($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: "/Client/Modules/VR_NumberingPlan/Directives/CodePreparation/Templates/CodePreparationSaleCodesGridTemplate.html"

		};

		function saleCodesGrid($scope, ctrl, $attrs) {
			var gridAPI;
			this.initializeController = initializeController;

			function initializeController() {
				$scope.salecodes = [];
				$scope.ZoneName;

				$scope.onGridReady = function (api) {
					gridAPI = api;
					if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
						ctrl.onReady(getDirectiveAPI());
					function getDirectiveAPI() {

						var directiveAPI = {};
						directiveAPI.loadGrid = function (query) {

							$scope.ZoneName = query.ZoneName;
							$scope.ShowDraftStatus = query.ShowDraftStatus;
							$scope.ShowSelectCode = query.ShowSelectCode;
							return gridAPI.retrieveData(query);
						};

						directiveAPI.onCodeAdded = function (codeItemObject) {
							gridAPI.itemAdded(codeItemObject);
						};

						directiveAPI.onCodeClosed = function (codeItemObject) {
							gridAPI.itemDeleted(codeItemObject);
						};
						directiveAPI.clearUpdatedItems = gridAPI.clearUpdatedItems;
						directiveAPI.getSelectedCodes = function () {
							return ctrl.selectedcodes;
						};

						directiveAPI.isZoneClosed = function () {
							for (var i = 0; i < $scope.salecodes.length; i++) {
								var saleCode = $scope.salecodes[i];

								if (saleCode.DraftStatus == Vr_NP_CodeItemDraftStatusEnum.ExistingNotChanged.value) {
									if (saleCode.EED == null)
										return false;
								}
								else if (saleCode.DraftStatus != Vr_NP_CodeItemDraftStatusEnum.ExistingClosed.value && saleCode.DraftStatus != Vr_NP_CodeItemDraftStatusEnum.ClosedZoneCode.value)
									return false;
							}
							return true;
						};

						directiveAPI.hideShowRenameZone = function () {
							for (var i = 0; i < $scope.salecodes.length; i++) {
								var saleCode = $scope.salecodes[i];

								if (saleCode.DraftStatus == Vr_NP_CodeItemDraftStatusEnum.ExistingClosed.value || saleCode.DraftStatus == Vr_NP_CodeItemDraftStatusEnum.MovedFrom.value
									|| saleCode.DraftStatus == Vr_NP_CodeItemDraftStatusEnum.MovedTo.value)
									return false;
							}
							return true;
						};

						return directiveAPI;
					}
				};

				$scope.onCodeChecked = function (dataItem) {
					if (ctrl.selectedcodes != undefined) {
						var index = UtilsService.getItemIndexByVal(ctrl.selectedcodes, dataItem.Code, 'Code');
						if (index >= 0 && !dataItem.IsSelected) {
							ctrl.selectedcodes.splice(index, 1);
						}
						else if (index <= 0 && dataItem.IsSelected) {
							ctrl.selectedcodes.push(dataItem);
						}
					}
				};

				$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
					return Vr_NP_CodePrepAPIService.GetCodeItems(dataRetrievalInput)
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


				function mapDataNeeded(dataItem) {
					dataItem.ShowDraftStatusIcon = true;
					dataItem.ShowStatusIcon = dataItem.Status != null;
					dataItem.ShowSelectCode = $scope.ShowSelectCode && dataItem.DraftStatus == Vr_NP_CodeItemDraftStatusEnum.ExistingNotChanged.value && dataItem.Status == null;

					switch (dataItem.DraftStatus) {
						case Vr_NP_CodeItemDraftStatusEnum.ExistingNotChanged.value:
							dataItem.ShowDraftStatusIcon = false;
							break;

						case Vr_NP_CodeItemDraftStatusEnum.MovedFrom.value:
							dataItem.DraftStatusIconUrl = Vr_NP_CodeItemDraftStatusEnum.MovedFrom.icon;
							dataItem.DraftStatusIconTooltip = Vr_NP_CodeItemDraftStatusEnum.MovedTo.label + " " + dataItem.OtherCodeZoneName;
							break;
						case Vr_NP_CodeItemDraftStatusEnum.MovedTo.value:
							dataItem.DraftStatusIconUrl = Vr_NP_CodeItemDraftStatusEnum.MovedTo.icon;
							dataItem.DraftStatusIconTooltip = Vr_NP_CodeItemDraftStatusEnum.MovedFrom.label + " " + dataItem.OtherCodeZoneName;
							break;
						case Vr_NP_CodeItemDraftStatusEnum.ExistingClosed.value:
							dataItem.DraftStatusIconUrl = Vr_NP_CodeItemDraftStatusEnum.ExistingClosed.icon;
							dataItem.DraftStatusIconTooltip = Vr_NP_CodeItemDraftStatusEnum.ExistingClosed.label;
							break;
						case Vr_NP_CodeItemDraftStatusEnum.New.value:
							dataItem.DraftStatusIconUrl = Vr_NP_CodeItemDraftStatusEnum.New.icon;
							dataItem.DraftStatusIconTooltip = Vr_NP_CodeItemDraftStatusEnum.New.label;
							break;
						case Vr_NP_CodeItemDraftStatusEnum.ClosedZoneCode.value:
							dataItem.DraftStatusIconUrl = Vr_NP_CodeItemDraftStatusEnum.ClosedZoneCode.icon;
							dataItem.DraftStatusIconTooltip = Vr_NP_CodeItemDraftStatusEnum.ClosedZoneCode.label;

					}


					if (dataItem.Status == Vr_NP_CodeItemStatusEnum.PendingEffective.value) {
						dataItem.StatusIconUrl = Vr_NP_CodeItemStatusEnum.PendingEffective.icon;
						dataItem.StatusIconTooltip = Vr_NP_CodeItemStatusEnum.PendingEffective.label;
					}
					else if (dataItem.Status == Vr_NP_CodeItemStatusEnum.PendingClosed.value) {
						dataItem.StatusIconUrl = Vr_NP_CodeItemStatusEnum.PendingClosed.icon;
						dataItem.StatusIconTooltip = Vr_NP_CodeItemStatusEnum.PendingClosed.label;
					}

				}
			}
		}

		return directiveDefinitionObject;

	}]);
