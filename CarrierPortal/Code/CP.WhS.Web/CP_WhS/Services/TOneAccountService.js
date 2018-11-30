(function (appControllers) {

	'use stict';

	TOneAccountService.$inject = [];

	function TOneAccountService() {

		function getTOneConnectionId() {
			return "B8058F6A-6545-465A-9DCA-6A4157ECFECB";
		}

		return {
			getTOneConnectionId: getTOneConnectionId
		};
	}

	appControllers.service('CP_WhS_TOneAccountService', TOneAccountService);

})(appControllers);