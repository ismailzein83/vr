
app.service('VRCommon_EmailTemplateService', ['VRModalService',
    function (VRModalService) {


        function editEmailTemplate(emailTemplateId, onEmailTemplateUpdated) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onEmailTemplateUpdated = onEmailTemplateUpdated;
            };
            var parameters = {
                emailTemplateId: emailTemplateId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/EmailTemplate/EmailTemplateEditor.html', parameters, settings);
        }

        return ({
            editEmailTemplate: editEmailTemplate
        });
    }]);
