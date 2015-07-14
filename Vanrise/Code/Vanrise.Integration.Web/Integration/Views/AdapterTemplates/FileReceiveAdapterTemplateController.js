FileReceiveAdapterTemplateController.$inject = ['$scope'];

function FileReceiveAdapterTemplateController($scope) {

    defineScope();
    load();

    function defineScope() {

        $scope.dataSourceAdapter.getData = function () {

            return {
                $type: "Vanrise.Integration.Adapters.FileReceiveAdapter.FileReceiveAdapter, Vanrise.Integration.Adapters.FileReceiveAdapter",
                FolderPath: $scope.folderPath
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
            $scope.folderPath = data.FolderPath;
        }
        else {
            $scope.folderPath = undefined;
        }
        isFormLoaded = true;
    }

    function load() {

        loadForm();
    }
}
appControllers.controller('Integration_FileReceiveAdapterTemplateController', FileReceiveAdapterTemplateController);
