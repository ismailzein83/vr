(function (appControllers) {
    'use strict';

    DataRecordFieldChoiceManagementController.$inject = ['$scope', 'VR_GenericData_DataRecordFieldChoiceService', 'VR_GenericData_DataRecordFieldChoiceAPIService'];

    function DataRecordFieldChoiceManagementController($scope, VR_GenericData_DataRecordFieldChoiceService, VR_GenericData_DataRecordFieldChoiceAPIService) {

        var dataRecordFieldChoiceGridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                dataRecordFieldChoiceGridAPI = api;
                var filter = {};
                dataRecordFieldChoiceGridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                return dataRecordFieldChoiceGridAPI.loadGrid(getFilterObject());
            };
            $scope.hasAddDataRecordFieldChoice = function () {
                return VR_GenericData_DataRecordFieldChoiceAPIService.HasAddDataRecordFieldChoice();
            };
            $scope.addDataRecordFieldChoice = function () {
                var onDataRecordFieldChoiceAdded = function (dataRecordFieldChoiceObj) {
                    dataRecordFieldChoiceGridAPI.onDataRecordFieldChoiceAdded(dataRecordFieldChoiceObj);
                };

                VR_GenericData_DataRecordFieldChoiceService.addDataRecordFieldChoice(onDataRecordFieldChoiceAdded);
            };
        }

        function load() {

        }

        function getFilterObject() {
          var  filter = {
                Name: $scope.name,
            };
            return filter;
        }
    }

    appControllers.controller('VR_GenericData_DataRecordFieldChoiceManagementController', DataRecordFieldChoiceManagementController);

})(appControllers);
