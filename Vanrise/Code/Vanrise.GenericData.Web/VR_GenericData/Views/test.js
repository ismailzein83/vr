(function (appControllers) {

    'use strict';

    GenericRuleDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_BusinessEntityAPIService'];

    function GenericRuleDefinitionManagementController($scope, VR_GenericData_BusinessEntityAPIService) {

        var gridAPI;
        var gridQuery = {};

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModal = {};
            $scope.scopeModal.dataRecordTypes = [];
            $scope.scopeModal.onSettingSelectionChanged = function (item) {
                if (item != undefined)

                console.log(item);

            }
        }

        function load() {
            VR_GenericData_BusinessEntityAPIService.GetDataRecordTypesInfo(3).then(function (response) {
            
                if (response) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModal.dataRecordTypes.push(response[i]);
                    }
                }
                $scope.scopeModal.selectedDataRecordTypes = $scope.scopeModal.dataRecordTypes[0]
            });
        }


    }

    appControllers.controller('VR_GenericData_testController', GenericRuleDefinitionManagementController);

})(appControllers);