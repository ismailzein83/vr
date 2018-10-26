'use strict';

app.directive('npIvswitchCodecdefGrid', ['UtilsService', 'VRUIUtilsService', 'NP_IVSwitch_CodecDefService', 'VRNotificationService',
	function (UtilsService, VRUIUtilsService, NP_IVSwitch_CodecDefService, VRNotificationService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var codecDefGrid = new CodecDefGrid($scope, ctrl, $attrs);
				codecDefGrid.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/NP_IVSwitch/Directives/CodecDef/Templates/CodecDefGridTemplate.html'
		};

		function CodecDefGrid($scope, ctrl, $attrs) {
			this.initializeController = initializeController;
			var gridAPI;
			var selectorAPI;
			var selectorDirectiveReadyPromiseDeffered = UtilsService.createPromiseDeferred();

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.codecDef = [];

				$scope.scopeModel.menuActions = [];

				$scope.scopeModel.onGridReady = function (api) {
					gridAPI = api;
					defineAPI();
				};

				//$scope.scopeModel.add = function () {
				//	var onCodecDefAdded = function (addedCodecDef) {
				//		$scope.scopeModel.codecDef.push({ Entity: addedCodecDef });

				//	};
				//	NP_IVSwitch_CodecDefService.addCodecDef(onCodecDefAdded);
				//};

				$scope.scopeModel.onSelectorDirectiveReady = function (api) {
					selectorAPI = api;
					selectorDirectiveReadyPromiseDeffered.resolve();
				};

				$scope.scopeModel.onSelectItem = function (option) {

					if (option != undefined) {
						var index = UtilsService.getItemIndexByVal($scope.scopeModel.codecDef, option.CodecId, "Entity.CodecId");
						if (index == -1) {
							var entity = {
								CodecId: option.CodecId,
								Description: option.Description,
								PayloadSize: option.DefaultMsPerPacket,
								SamplingFrequency: option.ClockRate,
								PassThru: option.PassThru

							};

							var gridItem = { Entity: entity };
							$scope.scopeModel.codecDef.push(gridItem);
						}
					}
				};

				$scope.scopeModel.onDeselectItem = function (option) {
					if (option != undefined) {
						var index = UtilsService.getItemIndexByVal($scope.scopeModel.codecDef, option.CodecId, "Entity.CodecId");
						$scope.scopeModel.codecDef.splice(index, 1);
					}
				};

				$scope.scopeModel.onDeselecAllItems = function (option) {
					$scope.scopeModel.codecDef.length = 0;
				};

				ctrl.isGridValid = function () {
					if ($scope.scopeModel.codecDef.length == 0) {
						return 'At least one codec must be added.';
					}
					return null;
				};

				ctrl.deleterow = function (DeletedItem) {
					var index = $scope.scopeModel.codecDef.indexOf(DeletedItem);
					$scope.scopeModel.codecDef.splice(index, 1);
					$scope.scopeModel.selectedValues.splice(index, 1);

				};
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					$scope.scopeModel.codecDef.length = 0;
					var selectedItems = [];
					if (payload != undefined) {
						var codecDefList = payload.codecDefList
						if (codecDefList!=undefined)
							for (var x = 0; x < codecDefList.length; x++) {
								selectedItems.push(codecDefList[x].CodecId);
						}
					}
					loadgrid(payload.codecDefList);

					var promises = [];
					promises.push(loadSelector(selectedItems));

					return UtilsService.waitMultiplePromises(promises);

				};
				api.getData = function () {
					var Data = [];
					var dataFromScope = $scope.scopeModel.codecDef;
					if (dataFromScope!=undefined)
					for (var i = 0; i < dataFromScope.length; i++) {
						Data.push(dataFromScope[i].Entity);
					}
					return Data;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

			function loadSelector(selectedItems) {
				var selectorDeffered = UtilsService.createPromiseDeferred();
				selectorDirectiveReadyPromiseDeffered.promise.then(function () {
					var directivePayload;
					if (selectedItems != undefined)
						directivePayload = {
							selectedIds: selectedItems
						};
					VRUIUtilsService.callDirectiveLoad(selectorAPI, directivePayload, selectorDeffered);
				});
				return selectorDeffered.promise;
			}

			function loadgrid(gridItems) {
				if (gridItems != undefined)
					for (var x = 0; x < gridItems.length; x++) {
						var option = gridItems[x];
						var entity = {
							CodecId: option.CodecId,
							Description: option.Description,
							PayloadSize: option.DefaultMsPerPacket,
							SamplingFrequency: option.ClockRate,
							PassThru : option.PassThru
						};
						var gridItem = {Entity:entity};
						$scope.scopeModel.codecDef.push(gridItem);
					}
			}
		}
	}]);
