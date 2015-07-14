﻿DBReceiveAdapterTemplateController.$inject = ['$scope'];

function DBReceiveAdapterTemplateController($scope) {

    defineScope();
    load();

    function defineScope() {

        $scope.dataSourceAdapter.getData = function () {

            return {
                $type: "Vanrise.Integration.Adapters.BaseDB.DBReceiveAdapter, Vanrise.Integration.Adapters.BaseDB",
                ConnectionString: $scope.connectionString,
                Description: $scope.description,
                Query: $scope.query
            };
        };

        $scope.dataSourceAdapter.loadTemplateData = function () {
            loadForm();
        }
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.dataSourceAdapter.data == undefined || isFormLoaded)
            return;
        var data = $scope.dataSourceAdapter.data;
        if (data != null) {
            $scope.connectionString=ConnectionString;
            $scope.description=Description;
            $scope.query=Query;
        }
        else {
            $scope.connectionString = undefined;
            $scope.description = undefined;
            $scope.query = undefined;
        }
        isFormLoaded = true;
    }

    function load() {

        loadForm();
    }
}
appControllers.controller('Integration_DBReceiveAdapterTemplateController', DBReceiveAdapterTemplateController);
