(function (appControllers) {

    'use strict';

    ViewAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig', 'SecurityService'];

    function ViewAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, SecurityService) {
        var controllerName = 'View';

        return ({
            GetView: GetView,
            GetFilteredDynamicPages: GetFilteredDynamicPages,
            AddView: AddView,
            UpdateView: UpdateView,
            UpdateViewsRank: UpdateViewsRank,
            DeleteView: DeleteView,
            HasAddViewPermission: HasAddViewPermission,
            HasUpdateViewPermission: HasUpdateViewPermission,
            GetFilteredViews: GetFilteredViews,
            HasUpdateViewsRankPermission: HasUpdateViewsRankPermission
        });

        function GetView(ViewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetView'), {
                ViewId: ViewId
            });
        }
        function GetFilteredViews(input)
        {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetFilteredViews'), input);
        }
        function GetFilteredDynamicPages(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetFilteredDynamicViews'), filter);
        }

        function AddView(view) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'AddView'), view);
        }

        function UpdateView(view) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateView'), view);
        }

        function UpdateViewsRank(updatedIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateViewsRank'), updatedIds);
        }

        function DeleteView(viewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'DeleteView'), {
                viewId: viewId
            });
        }
        function HasUpdateViewsRankPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateViewsRank']));
        }
        function HasAddViewPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddView']));
        }

        function HasUpdateViewPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateViewsRank']));
        }
    }

    appControllers.service('VR_Sec_ViewAPIService', ViewAPIService);

})(appControllers);