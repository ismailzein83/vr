DBReceiveAdapterTemplateController.$inject = ['$scope', 'DataSourceAPIService', 'VRNotificationService'];

function DBReceiveAdapterTemplateController($scope, DataSourceAPIService, VRNotificationService) {

    defineScope();
    load();

    function defineScope() {

        $scope.connectionString = undefined;
        $scope.query = undefined;
        $scope.lastImportedId = 0;

        $scope.dataSourceAdapter.argument.getData = function () {

            return {
                $type: "Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterArgument, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments",
                ConnectionString: $scope.connectionString,
                Query: $scope.query
            };
        };

        $scope.dataSourceAdapter.adapterState.getData = function () {

            return {
                $type: "Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments.DBAdapterState, Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments",
                LastImportedId: $scope.lastImportedId
            };
        };

        $scope.dataSourceAdapter.loadTemplateData = function () {
            loadForm();
        }
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.dataSourceAdapter.argument.data == undefined || isFormLoaded)
            return;

        var argumentData = $scope.dataSourceAdapter.argument.data;
        if (argumentData != null) {
            $scope.connectionString = argumentData.ConnectionString;
            $scope.query = argumentData.Query;
        }

        var adapterState = $scope.dataSourceAdapter.adapterState.data;
        if (adapterState != null)
        {
            $scope.lastImportedId = adapterState.LastImportedId;
        }

        isFormLoaded = true;
    }

    function load() {

        loadForm();
    }


}
appControllers.controller('Integration_DBReceiveAdapterTemplateController', DBReceiveAdapterTemplateController);
