(function (appControllers) {

	'use strict';

	BulkActionUtilsService.$inject = ['UtilsService'];

	function BulkActionUtilsService(UtilsService) {
		return {
			onBulkActionChanged: onBulkActionChanged
		};

		function onBulkActionChanged(bulkActionContext) {
			if (bulkActionContext != undefined) {
				if (bulkActionContext.onBulkActionChanged != undefined) {
					bulkActionContext.onBulkActionChanged();
				}
				if (bulkActionContext.requireEvaluation != undefined)
					bulkActionContext.requireEvaluation();
			}
		}
	}

	appControllers.service('WhS_Sales_BulkActionUtilsService', BulkActionUtilsService);

})(appControllers);