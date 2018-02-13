(function (appControllers) {

    "use strict";

    connectionEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function connectionEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var connectionStringType;
        var connectionEntity;
        var editMode;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                connectionEntity = parameters.Connection;
            }
            editMode = connectionEntity != undefined;
        }

        function defineScope() {
            $scope.saveConnection = function () {
                if (editMode)
                    return updateConnection();
                else
                    return insertConnection();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.onConnectionStringTypeSelectionChanged = function () {
                if ($scope.selectedConnectionStringType != undefined) {
                    switch ($scope.selectedConnectionStringType.value) {
                        case connectionStringType.ConnectionString.value: $scope.showConnectionString = true; $scope.showConnectionStringName = false; break;
                        case connectionStringType.ConnectionStringName.value: $scope.showConnectionStringName = true; $scope.showConnectionString = false; break;
                    }

                }
            };
        }
        function load() {
            loadAllControls();
            $scope.title = "Connection";

        }
        function loadAllControls() {
            connectionStringType = {
                ConnectionString: { value: 0, description: "Connection String" },
                ConnectionStringName: { value: 1, description: "Connection String Name" }
            };

            $scope.connectionStringType = UtilsService.getArrayEnum(connectionStringType);
            $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
            $scope.showConnectionString = true;
            $scope.showConnectionStringName = false;

            if (connectionEntity != undefined) {


                $scope.connectionString = connectionEntity.ConnectionString;
                $scope.connectionStringName = connectionEntity.ConnectionStringName;
                if ($scope.connectionStringName != undefined) {
                    $scope.selectedConnectionStringType = connectionStringType.ConnectionStringName;
                } else if ($scope.connectionString != undefined) {
                    $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                }
            }

        }
        function buildConnectionObj() {
            var obj = {};
            if ($scope.showConnectionString == true)
                obj.ConnectionString = $scope.connectionString;
            if ($scope.showConnectionStringName == true)
                obj.ConnectionStringName = $scope.connectionStringName;
            return obj;

        }

        function updateConnection() {
            var connectionObj = buildConnectionObj();
            if ($scope.onConnectionUpdated != undefined && typeof ($scope.onConnectionUpdated) == 'function') {
                $scope.onConnectionUpdated(connectionObj);
            }
            $scope.modalContext.closeModal();
        }
        function insertConnection() {
            var connectionObj = buildConnectionObj();
            if ($scope.onConnectionAdded != undefined && typeof ($scope.onConnectionAdded) == 'function') {
                $scope.onConnectionAdded(connectionObj);
            }
            $scope.modalContext.closeModal();
        }


    }

    appControllers.controller('VRCommon_ConnectionEditorController', connectionEditorController);
})(appControllers);
