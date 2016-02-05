(function (appControllers) {

    'use strict';

    SwitchBrandService.$inject = ['VRModalService', 'VRNotificationService', 'CDRAnalysis_PSTN_SwitchBrandAPIService'];

    function SwitchBrandService(VRModalService, VRNotificationService, CDRAnalysis_PSTN_SwitchBrandAPIService) {
        function editSwitchBrand(brandId, onSwitchBrandUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchBrandUpdated = onSwitchBrandUpdated;
            };
            var parameters = {
                BrandId: brandId
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchBrandEditor.html", parameters, settings);
        }

        function addSwitchBrand(onSwitchBrandAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchBrandAdded = onSwitchBrandAdded;
            };
            var parameters = {};

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchBrandEditor.html", parameters, settings);
        }
        
        function deleteSwitchBrand(switchBrandObj, onSwitchDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response == true) {
                        return CDRAnalysis_PSTN_SwitchBrandAPIService.DeleteBrand(switchBrandObj.BrandId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Switch Brand", deletionResponse))
                                onSwitchDeleted(switchBrandObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                    }
            });
        }
      
        return ({
            editSwitchBrand: editSwitchBrand,
            addSwitchBrand: addSwitchBrand,
            deleteSwitchBrand: deleteSwitchBrand
        });
    }

    appControllers.service('CDRAnalysis_PSTN_SwitchBrandService', SwitchBrandService);

})(appControllers);
