(function (appControllers) {

	'use strict';

	SalePriceListTemplateService.$inject = ['VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

	function SalePriceListTemplateService(VRModalService, VRNotificationService,VRCommon_ObjectTrackingService) {
	    var drillDownDefinitions = [];
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

		function getEntityUniqueName() {
		    return "WhS_BusinessEntity_SalePriceListTemplate";
		}

		function registerObjectTrackingDrillDownToSalePriceListTemplate() {
		    var drillDownDefinition = {};

		    drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
		    drillDownDefinition.directive = "vr-common-objecttracking-grid";


		    drillDownDefinition.loadDirective = function (directiveAPI, salePriceListTemplateItem) {
		        salePriceListTemplateItem.objectTrackingGridAPI = directiveAPI;

		        var query = {
		            ObjectId: salePriceListTemplateItem.Entity.SalePriceListTemplateId,
		            EntityUniqueName: getEntityUniqueName(),

		        };
		        return salePriceListTemplateItem.objectTrackingGridAPI.load(query);
		    };

		    addDrillDownDefinition(drillDownDefinition);

		}
		function addDrillDownDefinition(drillDownDefinition) {
		    drillDownDefinitions.push(drillDownDefinition);
		}

		function getDrillDownDefinition() {
		    return drillDownDefinitions;
		}

		return {
			addSalePriceListTemplate: addSalePriceListTemplate,
			editSalePriceListTemplate: editSalePriceListTemplate,
			registerObjectTrackingDrillDownToSalePriceListTemplate: registerObjectTrackingDrillDownToSalePriceListTemplate,
			getDrillDownDefinition: getDrillDownDefinition
		};
	}

	appControllers.service('WhS_BE_SalePriceListTemplateService', SalePriceListTemplateService);

})(appControllers);