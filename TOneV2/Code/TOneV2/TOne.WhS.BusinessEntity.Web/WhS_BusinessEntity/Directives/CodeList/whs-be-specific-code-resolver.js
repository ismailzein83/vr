'use strict';

app.directive('whsBeSpecificCodeResolver', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {

		var directiveDefinitionObject = {
			restrict: "E",
			scope: {
				onReady: "="
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new codeCriteriaCtor($scope, ctrl);
				ctor.initializeController();
			},
			controllerAs: "ctrl",
			bindToController: true,
			compile: function (element, attrs) {

			},
			templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CodeList/templates/SpecificCodeResolverTemplate.html"
		};

		function codeCriteriaCtor($scope, ctrl) {
			this.initializeController = initializeController;

			function initializeController() {
				$scope.scopeModel = {};

				$scope.codeCriteriaArray = [];

				$scope.scopeModel.addCode = function () {

					var codeCriteria = {
						Code: $scope.scopeModel.addedCode,
						WithSubCodes: false
					};

					var codeIsValid = true;

					if ($scope.scopeModel.addedCode == undefined || $scope.scopeModel.addedCode.length == 0) {
						codeIsValid = false;
					}
					else {
						angular.forEach($scope.codeCriteriaArray, function (item) {
							if ($scope.scopeModel.addedCode === item.Code) {
								codeIsValid = false;
							}
						});
					}

					if (codeIsValid) {
						$scope.codeCriteriaArray.push(codeCriteria);
						$scope.scopeModel.addedCode = undefined;
					}
				};

				$scope.isValid = function () {
					if ($scope.codeCriteriaArray.length == 0)
						return "at least choose one code";
					return null;
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined) {
						$scope.scopeModel.isBNumber = payload.context.isBNumber;
						angular.forEach(payload.Codes, function (item) {
							$scope.codeCriteriaArray.push(item);
						});
					}

				};

				api.getData = function () {
					return {
						$type: "TOne.WhS.BusinessEntity.MainExtensions.CodeList.SpecificCodeResolver,TOne.WhS.BusinessEntity.MainExtensions",
						Codes: $scope.codeCriteriaArray
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}

		return directiveDefinitionObject;
	}]);