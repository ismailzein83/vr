(function (appControllers) {

    'use strict';

    CDRFieldService.$inject = ['WhS_CDRProcessing_DefineCDRFieldsAPIService', 'VRModalService', 'VRNotificationService'];

    function CDRFieldService(WhS_CDRProcessing_DefineCDRFieldsAPIService, VRModalService, VRNotificationService) {
        return {
            addNewCDRField: addNewCDRField,
            editCDRField: editCDRField,
            deleteCDRField: deleteCDRField
        };

        function addNewCDRField(onCDRFieldAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCDRFieldAdded = onCDRFieldAdded;
            };
            var parameters = {
            };
            VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/CDRFields/DefineCDRFieldsEditor.html', parameters, settings);
        }

        function editCDRField(obj, onCDRFieldUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCDRFieldUpdated = onCDRFieldUpdated;
            };
            var parameters = {
                ID: obj.ID,
            };
            VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/CDRFields/DefineCDRFieldsEditor.html', parameters, settings);
        }

        function deleteCDRField($scope, obj, onCDRFieldObjDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_CDRProcessing_DefineCDRFieldsAPIService.DeleteCDRField(obj.Entity.ID)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("CDR Field", deletionResponse);
                                onCDRFieldObjDeleted(obj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_CDRProcessing_CDRFieldService', CDRFieldService);

})(appControllers);
