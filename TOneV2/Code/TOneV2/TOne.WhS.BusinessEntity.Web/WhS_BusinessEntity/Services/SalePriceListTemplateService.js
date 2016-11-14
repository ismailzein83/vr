(function (appControllers) {

	'use strict';

	SalePriceListTemplateService.$inject = ['VRModalService', 'VRNotificationService'];

	function SalePriceListTemplateService(VRModalService, VRNotificationService) {

		var editorUrl = '/Client/Modules/WhS_BusinessEntity/Views/SalePriceListTemplate/SalePriceListTemplateEditor.html';

		function addSalePriceListTemplate(onSalePriceListTemplateAdded) {

			var parameters;

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onSalePriceListTemplateAdded = onSalePriceListTemplateAdded;
			};

			VRModalService.showModal(editorUrl, parameters, settings);
		}

		function editSalePriceListTemplate(salePriceListTemplateId, onSalePriceListTemplateUpdated) {

			var parameters = {
				salePriceListTemplateId: salePriceListTemplateId
			};

			var settings = {};
			settings.onScopeReady = function (modalScope) {
				modalScope.onSalePriceListTemplateUpdated = onSalePriceListTemplateUpdated;
			};

			VRModalService.showModal(editorUrl, parameters, settings);
		}

		return {
			addSalePriceListTemplate: addSalePriceListTemplate,
			editSalePriceListTemplate: editSalePriceListTemplate
		};
	}

	appControllers.service('WhS_BE_SalePriceListTemplateService', SalePriceListTemplateService);

})(appControllers);