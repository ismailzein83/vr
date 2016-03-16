(function (appControllers) {

    'use strict';

    MenuService.$inject = [ 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function MenuService(UtilsService, VRModalService, VRNotificationService) {
        return ({
            openRankingEditor: openRankingEditor,
        });

        function openRankingEditor(onRankingSuccess) {
            var modalSettings = {
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRankingSuccess = onRankingSuccess;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/Menu/MenuRankingEditor.html', null, modalSettings);
        }
    }

    appControllers.service('VR_Sec_MenuService', MenuService);

})(appControllers);
