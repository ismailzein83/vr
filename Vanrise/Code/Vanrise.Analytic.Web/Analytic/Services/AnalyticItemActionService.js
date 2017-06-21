(function (appControllers) {

    'use strict';

    AnalyticItemActionService.$inject = ['VRModalService','VR_Analytic_AnalyticItemActionAPIService','UtilsService'];

    function AnalyticItemActionService(VRModalService, VR_Analytic_AnalyticItemActionAPIService, UtilsService) {
        return ({
            addItemAction: addItemAction,
            editItemAction: editItemAction,
            excuteItemAction: excuteItemAction
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

        function excuteItemAction(itemAction,settings)
        {
          return  VR_Analytic_AnalyticItemActionAPIService.GetAnalyticItemActionsTemplateConfigs().then(function (response) {

              var templateConfig = UtilsService.getItemByVal(response, itemAction.ConfigId, "ExtensionConfigurationId");
              if(templateConfig !=undefined)
              {
                  switch(templateConfig.Name)
                  {
                      case "VR_Analytic_Report_ItemAction_OpenRecordSearch": loadOpenRecordSearch(itemAction,settings); break;
                  }
              }
            });
        }
        function loadOpenRecordSearch(itemAction,settings) {
            var modalParameters = {
                analyticReportId: itemAction.ReportId,
                settings: {
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

    };

    appControllers.service('VR_Analytic_AnalyticItemActionService', AnalyticItemActionService);

})(appControllers);
