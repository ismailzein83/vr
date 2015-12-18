app.service('VRCommon_DataRecordFieldService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_DataRecordFieldAPIService',

    function (VRModalService, VRNotificationService, UtilsService, VRCommon_DataRecordFieldAPIService) {

        return ({
            addDataRecordField: addDataRecordField,
            editDataRecordField: editDataRecordField,
            deleteDataRecordField: deleteDataRecordField
        });

        function addDataRecordField(onDataRecordFieldAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldAdded = onDataRecordFieldAdded;
            };
            var parameters = {};
            VRModalService.showModal('/Client/Modules/Common/Views/GenericDataRecord/DataRecordFieldEditor.html', parameters, settings);
        }

        function editDataRecordField(obj, onDataRecordFieldUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onDataRecordFieldUpdated = onDataRecordFieldUpdated;
            };
            var parameters = {
                ID: obj.ID,
            };
            VRModalService.showModal('/Client/Modules/Common/Views/GenericDataRecord/DataRecordFieldEditor.html', parameters, settings);
        }

        function deleteDataRecordField($scope, obj, onDataRecordFieldDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return VRCommon_DataRecordFieldAPIService.DeleteCDRField(obj.Entity.ID)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Data Record Field", deletionResponse);
                                onDataRecordFieldDeleted(obj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }

    }]);