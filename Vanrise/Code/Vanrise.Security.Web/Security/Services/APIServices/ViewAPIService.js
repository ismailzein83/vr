(function (appControllers) {

    'use strict';

    ViewAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig', 'VR_Sec_SecurityAPIService'];

    function ViewAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, VR_Sec_SecurityAPIService) {
        return ({
            GetView: GetView,
            GetFilteredDynamicPages: GetFilteredDynamicPages,
            AddView: AddView,
            UpdateView: UpdateView,
            UpdateViewsRank: UpdateViewsRank,
            DeleteView: DeleteView,
            HasAddViewPermission: HasAddViewPermission
        });

        function GetView(ViewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'View', 'GetView'), {
                ViewId: ViewId
            });
        }

        function GetFilteredDynamicPages(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'View', 'GetFilteredDynamicViews'), filter);
        }

        function AddView(view) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'View', 'AddView'), view);
        }

        function UpdateView(view) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'View', 'UpdateView'), view);
        }

        function UpdateViewsRank(updatedIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'View', 'UpdateViewsRank'), updatedIds);
        }

        function DeleteView(viewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'View', 'DeleteView'), {
                viewId: viewId
            });
        }

        function HasAddViewPermission() {
            return VR_Sec_SecurityAPIService.IsAllowed(VR_Sec_ModuleConfig.moduleName + '/View/AddView');
        }
    }

    appControllers.service('VR_Sec_ViewAPIService', ViewAPIService);

})(appControllers);