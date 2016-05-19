(function (appControllers) {

    'use strict';

    DataRecordFieldChoiceService.$inject = ['VRModalService', 'VRNotificationService'];

    function DataRecordFieldChoiceService(VRModalService, VRNotificationService) {
        function editDataRecordFieldChoice(dataRecordFieldChoiceId, onDataRecordFieldChoiceUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldChoiceUpdated = onDataRecordFieldChoiceUpdated;
            };
            var parameters = {
                DataRecordFieldChoiceId: dataRecordFieldChoiceId
            };

            VRModalService.showModal("/Client/Modules/VR_GenericData/Views/DataRecordFieldChoice/DataRecordFieldChoiceEditor.html", parameters, settings);
        }

        function addDataRecordFieldChoice(onDataRecordFieldChoiceAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldChoiceAdded = onDataRecordFieldChoiceAdded;
            };
            var parameters = {};

            VRModalService.showModal("/Client/Modules/VR_GenericData/Views/DataRecordFieldChoice/DataRecordFieldChoiceEditor.html", parameters, settings);
        }

        return ({
            editDataRecordFieldChoice: editDataRecordFieldChoice,
            addDataRecordFieldChoice: addDataRecordFieldChoice
        })
    }

    appControllers.service('VR_GenericData_DataRecordFieldChoiceService', DataRecordFieldChoiceService);

})(appControllers);
