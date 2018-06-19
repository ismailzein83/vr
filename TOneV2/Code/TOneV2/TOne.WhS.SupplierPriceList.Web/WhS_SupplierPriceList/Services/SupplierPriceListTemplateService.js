(function (appControllers) {

	'use strict';

	SupplierPriceListTemplateService.$inject = ['VRModalService'];

	function SupplierPriceListTemplateService(VRModalService) {

		function saveSupplierPriceListTemplate(onSupplierPriceListTemplatSaved, configDetails, supplierId) {
			var settings = {
			};

			settings.onScopeReady = function (modalScope) {
				modalScope.onSupplierPriceListTemplatSaved = onSupplierPriceListTemplatSaved;
			};
			var parameters = {
				ConfigDetails: configDetails,
				SupplierId: supplierId
			};

			VRModalService.showModal("/Client/Modules/WhS_SupplierPriceList/Views/SupplierPriceListTemplateEditor.html", parameters, settings);
		}

		function showReceivedPricelistErrorDetails(errorDetails) {
			var settings = {};
			var parameters = {
				ErrorDetails: errorDetails,
			};

			VRModalService.showModal("/Client/Modules/WhS_SupplierPriceList/Views/ReceivedPricelistErrorDetails.html", parameters, settings);
		}

		return ({
			saveSupplierPriceListTemplate: saveSupplierPriceListTemplate,
			showReceivedPricelistErrorDetails: showReceivedPricelistErrorDetails
		})
	}

	appControllers.service('WhS_SupPL_SupplierPriceListTemplateService', SupplierPriceListTemplateService);

})(appControllers);
