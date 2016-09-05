(function (appControllers) {

    "use strict";

    volumeCommitmentAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function volumeCommitmentAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        var controllerName = "VolumeCommitment";

        function GetFilteredVolumeCommitments(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredVolumeCommitments"), input);
        }
        function GetVolumeCommitment(volumeCommitmentId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'GetVolumeCommitment'), {
                volumeCommitmentId: volumeCommitmentId
            });
        }
        function AddVolumeCommitment(volumeCommitment) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'AddVolumeCommitment'), volumeCommitment);
        }

        function UpdateVolumeCommitment(volumeCommitment) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, 'UpdateVolumeCommitment'), volumeCommitment);
        }

        return ({
            GetFilteredVolumeCommitments: GetFilteredVolumeCommitments,
            GetVolumeCommitment: GetVolumeCommitment,
            AddVolumeCommitment: AddVolumeCommitment,
            UpdateVolumeCommitment: UpdateVolumeCommitment
        });

    }

    appControllers.service("WhS_BE_VolumeCommitmentAPIService", volumeCommitmentAPIService);

})(appControllers);