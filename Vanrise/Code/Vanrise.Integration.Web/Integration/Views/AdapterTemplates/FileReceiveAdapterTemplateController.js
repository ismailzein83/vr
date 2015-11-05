FileReceiveAdapterTemplateController.$inject = ['$scope', 'UtilsService'];

function FileReceiveAdapterTemplateController($scope, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.extension = undefined;
        $scope.directory = undefined;
        $scope.directorytoMoveFile = undefined;
        $scope.selectedAction = undefined;

        $scope.actionsAfterImport = [{ value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];

        $scope.dataSourceAdapter.argument.getData = function () {

            var extension;

            if ($scope.extension.indexOf(".") == 0)
                extension = $scope.extension;
            else
                extension = "." + $scope.extension;


            return {
                $type: "Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments.FileAdapterArgument, Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments",
                Extension: extension,
                Directory: $scope.directory,
                DirectorytoMoveFile: $scope.directorytoMoveFile,
                ActionAfterImport: $scope.selectedAction.value
            };
        };

        $scope.dataSourceAdapter.adapterState.getData = function () {
            return null;
        };

        $scope.dataSourceAdapter.loadTemplateData = function () {
            loadForm();
        }
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.dataSourceAdapter.argument.data == undefined || isFormLoaded)
            return;

        var data = $scope.dataSourceAdapter.argument.data;

        if (data != null) {
            $scope.extension = data.Extension;
            $scope.directory = data.Directory;
            $scope.directorytoMoveFile = data.DirectorytoMoveFile;
            $scope.selectedAction = UtilsService.getItemByVal($scope.actionsAfterImport, data.ActionAfterImport, "value");
        }

        isFormLoaded = true;
    }

    function load() {

        loadForm();
    }
}
appControllers.controller('Integration_FileReceiveAdapterTemplateController', FileReceiveAdapterTemplateController);
