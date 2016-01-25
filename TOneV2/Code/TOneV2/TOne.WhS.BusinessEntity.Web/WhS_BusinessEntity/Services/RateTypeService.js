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

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RateType/RateTypeEditor.html', parameters, settings);
        }

        function editRateType(obj, onRateTypeUpdated) {
            var settings = {
                //useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onRateTypeUpdated = onRateTypeUpdated;
            };
            var parameters = {
                RateTypeId: obj.RateTypeId
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RateType/RateTypeEditor.html', parameters, settings);
        }
    }

    appControllers.service('WhS_BE_RateTypeService', RateTypeService);

})(appControllers);
