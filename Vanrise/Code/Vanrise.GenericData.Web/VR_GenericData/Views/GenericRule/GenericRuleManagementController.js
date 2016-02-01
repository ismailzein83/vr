(function (appControllers) {
    'use strict';

    genericRuleManagementController.$inject = ['$scope', 'VR_GenericData_GenericRule'];

    function genericRuleManagementController($scope, VR_GenericData_GenericRule) {

        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            //$scope.search = function () {
            //    getFilterObject();
            //    return gridAPI.loadGrid(filter);
            //};

            $scope.addGenericRule = function () {
                var onGenericRuleAdded = function (ruleObj) {
                    gridAPI.onGenericRuleAdded(ruleObj);
                };

                VR_GenericData_GenericRule.addGenericRule(onGenericRuleAdded);
            };

            //function getFilterObject() {
            //    filter = {
            //        Name: $scope.name,
            //        Email: $scope.email
            //    };
            //}
        }

        function load() {

        }

       
    }

    appControllers.controller('VR_GenericData_GenericRuleManagementController', genericRuleManagementController);

})(appControllers);
