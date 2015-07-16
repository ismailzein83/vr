FileReceiveAdapterTemplateController.$inject = ['$scope', 'UtilsService'];

function FileReceiveAdapterTemplateController($scope, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.selectedAction = '';

        $scope.actionsAfterImport = [{ value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];

        $scope.dataSourceAdapter.getData = function () {
            return {
                $type: "Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments.FileAdapterArgument, Vanrise.Integration.Adapters.FileReceiveAdapter.Arguments",
                Extension: $scope.extension,
                Directory: $scope.directory,
                DirectorytoMoveFile: $scope.directorytoMoveFile,
                ActionAfterImport: $scope.selectedAction.value
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
            $scope.extension = data.Extension;
            $scope.directory = data.Directory;
            $scope.directorytoMoveFile = data.DirectorytoMoveFile;
            $scope.selectedAction = UtilsService.getItemByVal($scope.actionsAfterImport, data.ActionAfterImport, "value");
        }
        else {
            $scope.extension = undefined;
            $scope.directory = undefined;
            $scope.directorytoMoveFile = undefined;
            $scope.selectedAction = undefined;
        }
        isFormLoaded = true;
    }

    function load() {

        loadForm();
    }
}
appControllers.controller('Integration_FileReceiveAdapterTemplateController', FileReceiveAdapterTemplateController);
