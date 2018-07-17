//(function (appControllers) {

//    "use strict";

//    VRReportService.$inject = [ 'VRModalService', 'VRUIUtilsService'];

//    function VRReportService( VRModalService, VRUIUtilsService) {

//        function addVRReport(onVRReportAdded) {
//            var settings = {};

//            settings.onScopeReady = function (modalScope) {
//                modalScope.onVRReportAdded = onVRReportAdded
//            };
//            VRModalService.showModal('/Client/Modules/Analytic/Views/VRReport/VRReportEditor.html', null, settings);
//        };

//        function editVRReport(vRReportId, onVRReportUpdated) {
//            var settings = {};

//            var parameters = {
//                vRReportGenerationId: vRReportGenerationId,
//            };

//            settings.onScopeReady = function (modalScope) {
//                modalScope.onVRReportUpdated = onVRReportUpdated;
//            };
//            VRModalService.showModal('/Client/Modules/Analytic/Views/DataAnalysis/VRReport/VRReportGenerationEditor.html', parameters, settings);
//        }       
        

//        return {
//            addVRReportGeneration: addVRReportGeneration,
//            editVRReportGeneration: editVRReportGeneration
//        };
//    }

//    appControllers.service('VR_Analytic_VRReportService', VRReportService);

//})(appControllers);