(function (appControllers) {

    'use stict';

    RateTypeService.$inject = ['UtilsService', 'VRModalService', 'VRCommon_ObjectTrackingService'];

    function RateTypeService(UtilsService, VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addRateType: addRateType,
            editRateType: editRateType,
            registerObjectTrackingDrillDownToRateType: registerObjectTrackingDrillDownToRateType,
            getDrillDownDefinition: getDrillDownDefinition
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

        function getEntityUniqueName() {
            return "VR_Common_RateType";
        }

        function registerObjectTrackingDrillDownToRateType() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, rateTypeItem) {
                rateTypeItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: rateTypeItem.Entity.RateTypeId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return rateTypeItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
    }

    appControllers.service('VRCommon_RateTypeService', RateTypeService);

})(appControllers);