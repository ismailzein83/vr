"use strict";

app.directive("bpInstanceOpenbpinstanceviewerAction", ["BusinessProcess_BPInstanceAPIService", "BusinessProcess_BPInstanceService", "BusinessProcess_GridMaxSize", "VRTimerService", "UtilsService", "VRUIUtilsService",
	function (BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, BusinessProcess_GridMaxSize, VRTimerService, UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new OpenBPInstanceViewerActionCtor($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/OpenBPInstanceViewerActionTemplate.html"
		};

		function OpenBPInstanceViewerActionCtor($scope, ctrl) {
			this.initializeController = initializeController;

			var dataRecordTypeTitleFieldsSelectorAPI;
			var dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			function initializeController() {


				$scope.scopeModel = {};

				$scope.scopeModel.onDataRecordTypeTitleFieldsSelectorDirectiveReady = function (api) {
					dataRecordTypeTitleFieldsSelectorAPI = api;
					dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred.resolve();
				};



				defineAPI();


			}
			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var loadAPIDeferred = UtilsService.createPromiseDeferred();
					if (payload != undefined && payload.settings != undefined && payload.settings.ProcessInstanceIdFieldName && payload.context != undefined) {
						dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred.promise.then(function () {
							var typeFieldsPayload = {
								dataRecordTypeId: payload.context.getDataRecordTypeId(),
								selectedIds: payload.settings.ProcessInstanceIdFieldName
							};
							VRUIUtilsService.callDirectiveLoad(dataRecordTypeTitleFieldsSelectorAPI, typeFieldsPayload, loadAPIDeferred);
						});
					}
					else {
						if (payload != undefined && payload.context != undefined) {
							dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred.promise.then(function () {
								var typeFieldsPayload = {
									dataRecordTypeId: payload.context.getDataRecordTypeId(),
								};
								VRUIUtilsService.callDirectiveLoad(dataRecordTypeTitleFieldsSelectorAPI, typeFieldsPayload, loadAPIDeferred);
							});
						}
					}
					var promises = [];
					promises.push(loadAPIDeferred.promise);
					return UtilsService.waitMultiplePromises(promises);

				};

				api.getData = function () {
					return {
						$type: "Vanrise.BusinessProcess.Business.OpenBPInstanceViewerAction,Vanrise.BusinessProcess.Business",
						ProcessInstanceIdFieldName: dataRecordTypeTitleFieldsSelectorAPI.getSelectedValue().Name
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);