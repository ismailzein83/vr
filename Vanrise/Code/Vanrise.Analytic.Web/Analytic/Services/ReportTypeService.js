(function (appControllers) {
    'use stict';

    ReportTypeService.$inject = ['VRModalService'];

    function ReportTypeService(VRModalService) {

        function addAttachement(onAttachementAdded, context) {
            var modalParameters = {
                context: context
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAttachementAdded = onAttachementAdded;
            };
           
            VRModalService.showModal('/Client/Modules/Analytic/Views/ReportType/AttachementReportTypeDefinitionEditor.html', modalParameters, modalSettings);
        }

        function editAttachement(attachementEntity, context, onAttachementUpdated) {
            var modalParameters = {
                context: context,
                attachementEntity: attachementEntity
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAttachementUpdated = onAttachementUpdated;
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/ReportType/AttachementReportTypeDefinitionEditor.html', modalParameters, modalSettings);
        }

        return {
            addAttachement: addAttachement,
            editAttachement: editAttachement
        };
    }
    appControllers.service('VR_Analytic_ReportTypeService', ReportTypeService);
})(appControllers);
