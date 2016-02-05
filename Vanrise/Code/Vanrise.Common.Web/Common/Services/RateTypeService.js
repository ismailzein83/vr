(function (appControllers) {

    'use stict';

    RateTypeService.$inject = ['UtilsService', 'VRModalService'];

    function RateTypeService(UtilsService, VRModalService) {
        return ({
            addRateType: addRateType,
            editRateType: editRateType
        });

        function addRateType(onRateTypeAdded) {
            var settings = {
                //useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRateTypeAdded = onRateTypeAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/RateType/RateTypeEditor.html', parameters, settings);
        }

        function editRateType(rateTypeId, onRateTypeUpdated) {
            var parameters = {
                RateTypeId: rateTypeId
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onRateTypeUpdated = onRateTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/RateType/RateTypeEditor.html', parameters, settings);
        }
    }

    appControllers.service('VRCommon_RateTypeService', RateTypeService);

})(appControllers);