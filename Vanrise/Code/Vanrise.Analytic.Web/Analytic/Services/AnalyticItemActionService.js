(function (appControllers) {

	'use strict';

	AnalyticItemActionService.$inject = ['VRModalService', 'VR_Analytic_AnalyticItemActionAPIService', 'UtilsService'];

	function AnalyticItemActionService(VRModalService, VR_Analytic_AnalyticItemActionAPIService, UtilsService) {
		var actionTypes = [];

		function registerActionType(actionType) {
			actionTypes.push(actionType);
		}

		function getActionTypeIfExistByName(actionTypeName) {
			for (var i = 0; i < actionTypes.length; i++) {
				var actionType = actionTypes[i];
				if (actionType.ActionTypeName == actionTypeName)
					return actionType;
			}
		}

		function getActionTypeIfExistByConfigId(configId) {
			for (var i = 0; i < actionTypes.length; i++) {
				var actionType = actionTypes[i];
				if (actionType.ConfigId == configId)
					return actionType;
			}
		}

		function excuteItemAction(payload) {
			if (payload == undefined || payload.ItemAction == undefined || payload.Settings == undefined)
				return;

			var actionType = getActionTypeIfExistByConfigId(payload.ItemAction.ConfigId);
			if (actionType != null) {
				actionType.ExecuteAction(payload);
			}
		}

		function registerOpenRecordSearch() {
			var actionType = {
				ActionTypeName: "CDRs",
				ConfigId: "e2a332a2-74fa-4c42-a5d1-33fbda093946",
				ExecuteAction: function (payload) {
					if (payload == undefined || payload.ItemAction == undefined || payload.Settings == undefined)
						return;
					openRecordSearchAction(payload)
				}
			};
			registerActionType(actionType);
		}

		function openRecordSearchAction(payload) {
			var modalParameters = {
				analyticReportId: payload.ItemAction.ReportId,
				analyticFilter: {
					AnalyticReportId: payload.ItemAction.ReportId,
					SourceName: payload.ItemAction.SourceName,
					DimensionFilters: payload.Settings.DimensionFilters,
					FilterGroup: payload.Settings.FilterGroup,
					FromDate: payload.Settings.FromDate,
					ToDate: payload.Settings.ToDate,
					TableId: payload.Settings.TableId,
					Period: payload.Settings.Period
				},
				autoSearch: true
			};
			var modalSettings = {
				useModalTemplate: true,
				width: "80%",
				title: payload.ItemAction.Title
			};
			modalSettings.onScopeReady = function (modalScope) {
			};

			VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Runtime/GenericAnalyticReport.html', modalParameters, modalSettings);
		}

		return ({
			registerActionType: registerActionType,
			getActionTypeIfExistByName: getActionTypeIfExistByName,
			getActionTypeIfExistByConfigId: getActionTypeIfExistByConfigId,
			excuteItemAction: excuteItemAction,
			openRecordSearchAction: openRecordSearchAction,
			registerOpenRecordSearch: registerOpenRecordSearch,
			addItemAction: addItemAction,
			editItemAction: editItemAction,
			openRecordSearch: openRecordSearch
		});

		function addItemAction(onItemActionAdded) {
			var modalSettings = {};

			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onItemActionAdded = onItemActionAdded;
			};
			var modalParameters = {
			};
			VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticItemActionEditor.html', modalParameters, modalSettings);
		}

		function editItemAction(itemAction, onItemActionUpdated) {
			var modalParameters = {
				itemAction: itemAction
			};
			var modalSettings = {};


			modalSettings.onScopeReady = function (modalScope) {
				modalScope.onItemActionUpdated = onItemActionUpdated;
			};

			VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticItemActionEditor.html', modalParameters, modalSettings);
		}

		/*
				function excuteItemAction(itemAction, settings) {
					return VR_Analytic_AnalyticItemActionAPIService.GetAnalyticItemActionsTemplateConfigs().then(function (response) {
		
						var templateConfig = UtilsService.getItemByVal(response, itemAction.ConfigId, "ExtensionConfigurationId");
						if (templateConfig != undefined) {
							switch (templateConfig.Name) {
								case "VR_Analytic_Report_ItemAction_OpenRecordSearch": loadOpenRecordSearch(itemAction, settings); break;
							}
						}
					});
				}
				*/

		function loadOpenRecordSearch(itemAction, settings) {
			var modalParameters = {
				analyticReportId: itemAction.ReportId,
				analyticFilter: {
					AnalyticReportId: itemAction.ReportId,
					SourceName: itemAction.SourceName,
					DimensionFilters: settings.DimensionFilters,
					FilterGroup: settings.FilterGroup,
					FromDate: settings.FromDate,
					ToDate: settings.ToDate,
					TableId: settings.TableId,
					Period: settings.Period
				},
				autoSearch: true
			};
			var modalSettings = {
				useModalTemplate: true,
				width: "80%",
				title: itemAction.Title
			};
			modalSettings.onScopeReady = function (modalScope) {
			};

			VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Runtime/GenericAnalyticReport.html', modalParameters, modalSettings);
		}

		function openRecordSearch(reportId, title, sourceName, fromDate, toDate, period, fieldFilters) {
			var modalParameters = {
				analyticReportId: reportId,
				preDefinedFilter: {
					SourceName: sourceName,
					FieldFilters: fieldFilters,
					FromDate: fromDate,
					ToDate: toDate,
					AnalyticReportId: reportId,
					Period: period
				},
				autoSearch: true
			};
			var modalSettings = {
				useModalTemplate: true,
				width: "80%",
				title: title
			};
			modalSettings.onScopeReady = function (modalScope) {
			};

			VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Runtime/GenericAnalyticReport.html', modalParameters, modalSettings);
		}

	};

	appControllers.service('VR_Analytic_AnalyticItemActionService', AnalyticItemActionService);

})(appControllers);
