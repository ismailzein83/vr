FTPReceiveAdapterTemplateController.$inject = ['$scope','UtilsService'];

function FTPReceiveAdapterTemplateController($scope, UtilsService) {

    defineScope();
    load();

    function defineScope() {

        $scope.selectedAction = '';

        $scope.actionsAfterImport = [{ value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];


        $scope.dataSourceAdapter.argument.getData = function () {
            return {
                $type: "Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments.FTPAdapterArgument, Vanrise.Integration.Adapters.FTPReceiveAdapter.Arguments",
                Extension: $scope.extension,
                Directory: $scope.directory,
                ServerIP: $scope.serverIP,
                UserName: $scope.userName,
                Password: $scope.password,
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
             $scope.extension=data.Extension;
             $scope.directory = data.Directory;
             $scope.serverIP = data.ServerIP;
             $scope.userName = data.UserName;
             $scope.password = data.Password;
             $scope.directorytoMoveFile = data.DirectorytoMoveFile;
             $scope.selectedAction = UtilsService.getItemByVal($scope.actionsAfterImport, data.ActionAfterImport, "value");
        }
        else {
            $scope.extension = undefined;
            $scope.directory = undefined;
            $scope.serverIP = undefined;
            $scope.userName = undefined;
            $scope.password = undefined;
            $scope.directorytoMoveFile = undefined;
            $scope.selectedAction = undefined;
        }
        isFormLoaded = true;
    }

    function load() {

        loadForm();
    }
}
appControllers.controller('Integration_FTPReceiveAdapterTemplateController', FTPReceiveAdapterTemplateController);
