DBReceiveAdapterTemplateController.$inject = ['$scope','DataSourceAPIService'];

function DBReceiveAdapterTemplateController($scope, DataSourceAPIService) {

    defineScope();
    load();

    function defineScope() {

        $scope.connectionString = undefined;
        $scope.description = undefined;
        $scope.query = undefined;

        $scope.dataSourceAdapter.getData = function () {

            return {
                $type: "Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterArgument, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments",
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
            $scope.connectionString = data.ConnectionString;
            $scope.description = data.Description;
            $scope.query = data.Query;
        }

        isFormLoaded = true;
    }

    function load() {

        loadForm();
    }


}
appControllers.controller('Integration_DBReceiveAdapterTemplateController', DBReceiveAdapterTemplateController);
