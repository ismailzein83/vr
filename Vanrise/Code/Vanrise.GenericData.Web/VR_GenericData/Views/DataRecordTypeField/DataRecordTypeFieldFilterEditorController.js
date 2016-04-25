(function (appControllers) {

    'use strict';

    DataRecordStorageFilterEditorController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_DataRecordFieldAPIService', 'UtilsService'];

    function DataRecordStorageFilterEditorController($scope, VRNavigationService, VR_GenericData_DataRecordFieldAPIService, UtilsService) {

        $scope.dataRecordTypeId;
        loadParameters();
        defineScope();

        var context = { getFields: function () { return this.dataRecordFieldTypeConfigs }, dataRecordFieldTypeConfigs: [] };
        var groupFilterAPI;

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                $scope.dataRecordTypeId = parameters.DataRecordTypeId;
            }
        }

        function defineScope() {
            $scope.onGroupFilterReady = function (api) {
                groupFilterAPI = api;
                var payload = { dataRecordTypeId: $scope.dataRecordTypeId, context: context };
                loadContext().then(function () {
                    groupFilterAPI.load(payload);
                });
            };

            $scope.save = function () {
                var expression = groupFilterAPI.getExpression();
                console.log(expression);
                if ($scope.onDataRecordFieldTypeFilterAdded != undefined) {
                    $scope.onDataRecordFieldTypeFilterAdded(groupFilterAPI.getData(), groupFilterAPI.getExpression());
                }

                $scope.modalContext.closeModal();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function loadContext() {
            var obj = { DataRecordTypeId: $scope.dataRecordTypeId };
            var serializedFilter = UtilsService.serializetoJson(obj);
            return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(serializedFilter).then(function (response) {
                if (response != undefined)
                    angular.forEach(response, function (item) {
                        context.dataRecordFieldTypeConfigs.push(item);
                    });
            });
        }
    }

    appControllers.controller('VR_GenericData_DataRecordTypeFieldFilterEditorController', DataRecordStorageFilterEditorController);

})(appControllers);