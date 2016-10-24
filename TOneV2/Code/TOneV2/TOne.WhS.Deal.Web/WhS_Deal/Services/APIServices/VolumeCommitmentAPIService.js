(function (appControllers) {

    "use strict";

    volumeCommitmentAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Deal_ModuleConfig"];

    function volumeCommitmentAPIService(BaseAPIService, UtilsService, WhS_Deal_ModuleConfig) {
        var controllerName = "VolumeCommitment";

        function GetFilteredVolumeCommitments(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, "GetFilteredVolumeCommitments"), input);
        }
        function GetVolumeCommitment(volumeCommitmentId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'GetVolumeCommitment'), {
                volumeCommitmentId: volumeCommitmentId
            });
        }
        function AddVolumeCommitment(volumeCommitment) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'AddVolumeCommitment'), volumeCommitment);
        }

        function UpdateVolumeCommitment(volumeCommitment) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Deal_ModuleConfig.moduleName, controllerName, 'UpdateVolumeCommitment'), volumeCommitment);
        }

        return ({
            GetFilteredVolumeCommitments: GetFilteredVolumeCommitments,
            GetVolumeCommitment: GetVolumeCommitment,
            AddVolumeCommitment: AddVolumeCommitment,
            UpdateVolumeCommitment: UpdateVolumeCommitment
        });

    }

    appControllers.service("WhS_Deal_VolumeCommitmentAPIService", volumeCommitmentAPIService);

})(appControllers);