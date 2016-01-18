(function (appControllers) {

    "use strict";
    viewAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function viewAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {

        function AddView(view) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "View", "AddView"), view);
        }

        function UpdateView(view) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "View", "UpdateView"), view);
        }

        function DeleteView(viewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "View", "DeleteView"), {
                viewId: viewId
            });
        }

        function GetView(ViewId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "View", "GetView"),
                {
                    ViewId: ViewId
                });

        }

        function GetFilteredDynamicPages(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "View", "GetFilteredDynamicViews"), filter);
        }

        function UpdateViewsRank(updatedIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "View", "UpdateViewsRank"), updatedIds);
        }


       
        return ({
            UpdateView: UpdateView,
            AddView: AddView,
            DeleteView: DeleteView,
            GetView: GetView,
            GetFilteredDynamicPages: GetFilteredDynamicPages,
            UpdateViewsRank: UpdateViewsRank
        });
    }

    appControllers.service('VR_Sec_ViewAPIService', viewAPIService);

})(appControllers);