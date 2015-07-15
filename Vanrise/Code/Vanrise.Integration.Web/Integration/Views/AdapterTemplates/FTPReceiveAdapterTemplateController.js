FTPReceiveAdapterTemplateController.$inject = ['$scope'];

function FTPReceiveAdapterTemplateController($scope) {

    defineScope();
    load();

    function defineScope() {

        $scope.selectedAction = '';

        $scope.actionsAfterImport = [{ value: 0, name: 'Rename' }, { value: 1, name: 'Delete' }, { value: 2, name: 'Move' }];



        $scope.dataSourceAdapter.getData = function () {
            console.log('$scope.selectedAction.value')
            return {
                $type: "",
                Extension: $scope.extension,
                Directory: $scope.directory,
                ServerIP: $scope.serverIP,
                UserName: $scope.userName,
                Password: $scope.password,
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
             $scope.extension=Extension;
             $scope.directory=Directory;
             $scope.serverIP=ServerIP;
             $scope.userName=UserName;
             $scope.password=Password;
             $scope.directorytoMoveFile=DirectorytoMoveFile;
             $scope.selectedAction = ActionAfterImport;
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
