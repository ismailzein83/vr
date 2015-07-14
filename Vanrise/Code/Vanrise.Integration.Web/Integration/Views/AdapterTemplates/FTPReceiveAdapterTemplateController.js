FTPReceiveAdapterTemplateController.$inject = ['$scope'];

function FTPReceiveAdapterTemplateController($scope) {

    defineScope();
    load();

    function defineScope() {

        console.log(1111111111111111);
        console.log(1111111111111111);
        console.log(1111111111111111);
        console.log(1111111111111111);
        console.log(1111111111111111);
        console.log(1111111111111111);
        console.log(1111111111111111);
        console.log(1111111111111111);

        $scope.selectedAction = '';

        $scope.actionsAfterImport = [{ value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];



        $scope.dataSourceAdapter.getData = function () {

            return {
                $type: "Vanrise.Integration.Adapters.BaseFTP.TPReceiveAdapter, Vanrise.Integration.Adapters.BaseFTP",
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
appControllers.controller('Integration_FTPReceiveAdapterTemplateController', FTPReceiveAdapterTemplateController);
