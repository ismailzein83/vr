
app.service('Retail_Teles_SiteService', ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService) {

        function addTelesSite(onSiteAdded, enterpriseId, vrConnectionId) {
            var parameters = {
                enterpriseId: enterpriseId,
                vrConnectionId: vrConnectionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSiteAdded = onSiteAdded;
            };

            VRModalService.showModal('/Client/Modules/Retail_Teles/Views/TelesSiteEditor.html', parameters, modalSettings);
        }

        return ({
            addTelesSite: addTelesSite,
        });
    }]);
