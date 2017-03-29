app.run(['VR_Analytic_AnalyticReportService', 'VR_Analytic_DataAnalysisItemDefinitionService', function (VR_Analytic_AnalyticReportService, VR_Analytic_DataAnalysisItemDefinitionService) {
    VR_Analytic_AnalyticReportService.registerObjectTrackingDrillDownToAnalyticReport();
    VR_Analytic_DataAnalysisItemDefinitionService.registerObjectTrackingDrillDownToDataAnalysisItemDefinition();
}]);