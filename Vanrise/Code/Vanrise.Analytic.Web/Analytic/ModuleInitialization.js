﻿app.run(['VR_Analytic_AnalyticReportService', 'VR_Analytic_DataAnalysisItemDefinitionService', 'VR_Analytic_AnalyticItemConfigService', 'VR_Analytic_AnalyticItemActionService', 'VR_Analytic_ReportGenerationActionService', function (VR_Analytic_AnalyticReportService, VR_Analytic_DataAnalysisItemDefinitionService, VR_Analytic_AnalyticItemConfigService, VR_Analytic_AnalyticItemActionService, VR_Analytic_ReportGenerationActionService) {
	VR_Analytic_AnalyticReportService.registerObjectTrackingDrillDownToAnalyticReport();
	VR_Analytic_DataAnalysisItemDefinitionService.registerObjectTrackingDrillDownToDataAnalysisItemDefinition();
	VR_Analytic_AnalyticItemConfigService.registerObjectTrackingDrillDownToAnalyticItemConfig();
	VR_Analytic_AnalyticItemActionService.registerOpenRecordSearch();
	VR_Analytic_ReportGenerationActionService.registerDownloadFileAction();
	VR_Analytic_ReportGenerationActionService.registerHistoryViewAction();
}]);