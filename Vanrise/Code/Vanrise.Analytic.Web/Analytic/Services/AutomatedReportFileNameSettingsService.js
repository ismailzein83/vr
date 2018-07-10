
app.service('VR_Analytic_AutomatedReportFileNameSettingsService', ['VRModalService',
    function (VRModalService) {

        function addFileNamePart(onFileNamePartAdded, context) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onFileNamePartAdded = onFileNamePartAdded;
            };
            var parameters = {
                context: context
            };
            VRModalService.showModal('/Client/Modules/Analytic/Directives/AutomatedReport/Handler/Templates/AutomatedReportFileNamePartEditor.html', parameters, settings);
        }

        function editFileNamePart(fileNamePartEntity, onFileNamePartUpdated, context) {
            var settings = { 

            }; 
            settings.onScopeReady = function (modalScope) {
                modalScope.onFileNamePartUpdated = onFileNamePartUpdated;
            };
            var parameters = {
                fileNamePartEntity: fileNamePartEntity,
                context: context
            };

            VRModalService.showModal('/Client/Modules/Analytic/Directives/AutomatedReport/Handler/Templates/AutomatedReportFileNamePartEditor.html', parameters, settings);
        }

        return ({
            addFileNamePart: addFileNamePart,
            editFileNamePart: editFileNamePart

        });
    }]);
