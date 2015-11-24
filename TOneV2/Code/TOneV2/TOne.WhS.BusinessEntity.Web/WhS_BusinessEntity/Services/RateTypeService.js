app.service('WhS_BE_RateTypeService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {

        function editRateType(obj, onRateTypeUpdated) {
            var settings = {
                useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.Name, "Rate Type");
                modalScope.onRateTypeUpdated = onRateTypeUpdated;
            };
            var parameters = {
                RateTypeId: obj.RateTypeId
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RateType/RateTypeEditor.html', parameters, settings);
        }

        function addRateType(onRateTypeAdded) {
            var settings = {
                useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Rate Type");
                modalScope.onRateTypeAdded = onRateTypeAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RateType/RateTypeEditor.html', parameters, settings);
        }

        return ({
            editRateType: editRateType,
            addRateType: addRateType
        });

    }]);