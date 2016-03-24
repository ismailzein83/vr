(function (appControllers) {

    'use strict';

    AnalyticService.$inject = ['VRModalService'];

    function AnalyticService(VRModalService) {
        return ({
            OpenAnalyticReport: OpenAnalyticReport
        });

        function OpenAnalyticReport() {
            var parameters = {
            };
            var modalSettings = {
                useModalTemplate: true,
                width: "80%",
                maxHeight: "800px",
                title: entityName
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/Reports/AnalyticReport.html', parameters, modalSettings);
        }
    }

    appControllers.service('Analytic_AnalyticService', AnalyticService);

})(appControllers);
