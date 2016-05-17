(function (appControllers) {

    'use strict';

    DataRecordStorageFilterEditorController.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_DataRecordFieldAPIService', 'UtilsService'];

    function DataRecordStorageFilterEditorController($scope, VRNavigationService, VR_GenericData_DataRecordFieldAPIService, UtilsService) {

        var fields =[];
        var filterObj;

        loadParameters();
        defineScope();

        var context = { getFields: function () { return fields } };
        var groupFilterAPI;

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                fields = parameters.Fields;
                filterObj= parameters.FilterObj;
            }
        }

        function defineScope() {
            $scope.title = 'Advanced Filter';
            $scope.onGroupFilterReady = function (api) {
                groupFilterAPI = api;
                var payload = {context: context, filterObj: filterObj };
                groupFilterAPI.load(payload);
            };

            $scope.save = function () {
                if ($scope.onDataRecordFieldTypeFilterAdded != undefined) {
                    $scope.onDataRecordFieldTypeFilterAdded(groupFilterAPI.getData(), groupFilterAPI.getExpression());
                } 

                $scope.modalContext.closeModal();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
    }

    appControllers.controller('VR_GenericData_DataRecordTypeFieldFilterEditorController', DataRecordStorageFilterEditorController);

})(appControllers);