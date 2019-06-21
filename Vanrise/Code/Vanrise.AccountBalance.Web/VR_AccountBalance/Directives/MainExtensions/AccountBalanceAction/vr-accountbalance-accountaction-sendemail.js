'use strict';
app.directive('vrAccountbalanceAccountactionSendemail', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_ActionDefinitionAPIService',
	function (UtilsService, VRUIUtilsService, Retail_BE_ActionDefinitionAPIService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope:
			{
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new NotificationActionEmailCtor(ctrl, $scope);
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
			templateUrl: function (element, attrs) {
				return '/Client/Modules/VR_AccountBalance/Directives/MainExtensions/AccountBalanceAction/Templates/AccountBalanceActionSendEmail.html';
			}

		};

		function NotificationActionEmailCtor(ctrl, $scope) {

			this.initializeController = initializeController;

			var mailTemplateSelectorReadyAPI;
			var mailTemplateSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			var vrActionEntity;

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onMailTemplateSelectorReady = function (api) {
					mailTemplateSelectorReadyAPI = api;
					mailTemplateSelectorReadyPromiseDeferred.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var mailTemplateId;

					if (payload != undefined && payload.vrActionEntity != undefined) {
						mailTemplateId = payload.vrActionEntity.MailMessageTemplateId;
					}

					var mailMessageTemplatePayload = {
						selectedIds: mailTemplateId,
						filter: { VRMailMessageTypeId: payload.selectedVRActionDefinition.Settings.ExtendedSettings.MailMessageTypeId }
					};

					var promises = [];
					var loadMailTemplatesPromiseDeferred = UtilsService.createPromiseDeferred();
					promises.push(loadMailTemplatesPromiseDeferred.promise);


					mailTemplateSelectorReadyPromiseDeferred.promise.then(function () {
						VRUIUtilsService.callDirectiveLoad(mailTemplateSelectorReadyAPI, mailMessageTemplatePayload, loadMailTemplatesPromiseDeferred);
					});
					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						$type: "Vanrise.AccountBalance.MainExtensions.GenericSendEmailAction,Vanrise.AccountBalance.MainExtensions",
						ActionName: "Email (" + $scope.scopeModel.selectedEmailTemplate.Name + " )",
						MailMessageTemplateId: mailTemplateSelectorReadyAPI.getSelectedIds()
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}

		}
		return directiveDefinitionObject;
	}
]);