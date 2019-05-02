(function (appControllers) {

	'use strict';

	SupplierTargetMatchService.$inject = ['VRModalService', 'VRNotificationService'];

	function SupplierTargetMatchService(VRModalService, VRNotificationService) {
		return ({
			exportSupplierTargetMatch: exportSupplierTargetMatch,
		});

		function exportSupplierTargetMatch(onExportSupplierTargetMatch) {
			var settings = {
			};

			var parameters;

			settings.onScopeReady = function (modalScope) {
				modalScope.onExportSupplierTargetMatch = onExportSupplierTargetMatch;
			};

			VRModalService.showModal('/Client/Modules/WhS_Sales/Views/SupplierTargetMatch/ExportSupplierTargetMatchEditor.html', parameters, settings);
		}
	}

	appControllers.service('WhS_Sales_SupplierTargetMatchService', SupplierTargetMatchService);

})(appControllers);