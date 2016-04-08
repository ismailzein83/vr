(function (appControllers) {

    'use strict';

    FileService.$inject = ['VRModalService', 'VRNotificationService'];

    function FileService(VRModalService, VRNotificationService) {

        return {
            viewRecentFiles: viewRecentFiles
        };

        function viewRecentFiles(moduleType, onRecentFileSelected) {

            var parameters = {
                moduleType: moduleType
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onRecentFileSelected = onRecentFileSelected;
            };

            VRModalService.showModal("/Client/Modules/Common/Views/File/RecentFile.html", parameters, settings);
        }
    }

    appControllers.service('VRCommon_FileService', FileService);

})(appControllers);