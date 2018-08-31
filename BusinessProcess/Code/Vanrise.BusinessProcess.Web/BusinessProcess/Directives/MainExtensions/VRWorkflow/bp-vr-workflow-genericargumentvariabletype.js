'use strict';

app.directive('businessprocessVrWorkflowGenericargumentvariabletype', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
				normalColNum: '@'
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new GenericVariableTypeDirectiveCtor(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {
				return {
					pre: function ($scope, iElem, iAttrs, ctrl) {

					}
				};
			},
			templateUrl: "/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflow/Templates/VRWorkflowGenericArgumentVariableTypeTemplate.html"
		};

		function GenericVariableTypeDirectiveCtor(ctrl, $scope, attrs) {
			this.initializeController = initializeController;

			var fieldTypeSelectorAPI;
			var fieldTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onFieldTypeSelectorReady = function (api) {
					fieldTypeSelectorAPI = api;
					fieldTypeSelectorReadyDeferred.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];

					var fieldTypeSelectorLoadPromise = getFieldTypeSelectorLoadPromise();
					promises.push(fieldTypeSelectorLoadPromise);

					function getFieldTypeSelectorLoadPromise() {
						var fieldTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

						fieldTypeSelectorReadyDeferred.promise.then(function () {
							var fieldTypeSelectorPayload;
							if (payload != undefined) {
								fieldTypeSelectorPayload = payload.FieldType;
							}
							VRUIUtilsService.callDirectiveLoad(fieldTypeSelectorAPI, fieldTypeSelectorPayload, fieldTypeSelectorLoadDeferred);
						});

						return fieldTypeSelectorLoadDeferred.promise;
					}

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowVariableTypes.VRWorkflowGenericVariableType, Vanrise.BusinessProcess.MainExtensions",
						FieldType: fieldTypeSelectorAPI.getData()
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);