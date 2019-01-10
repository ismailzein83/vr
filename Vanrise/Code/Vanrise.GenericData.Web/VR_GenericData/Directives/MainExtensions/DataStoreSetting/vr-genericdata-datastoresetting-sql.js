(function (app) {

    'use strict';

    DataStoreSettingSqlDirective.$inject = ['UtilsService'];

    function DataStoreSettingSqlDirective(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: function (element, attrs) {
                return "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStoreSetting/Templates/SqlTemplate.html";
            }
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var connectionStringType = {
                ConnectionString: { value: 0, description: "Connection String" },
                ConnectionStringName: { value: 1, description: "Connection String Name" },
                ConnectionStringAppSettingName: { value: 2, description: "Connection String App Setting Name" }
            };

            function initializeController() {
                $scope.showConnectionString = true;
                $scope.showConnectionStringName = false;
                $scope.showConnectionStringAppSettingName = false;
                $scope.connectionStringType = UtilsService.getArrayEnum(connectionStringType);
                $scope.selectedConnectionStringType = connectionStringType.ConnectionString;

                $scope.onConnectionStringTypeSelectionChanged = function () {
                    if ($scope.selectedConnectionStringType != undefined) {

                        switch ($scope.selectedConnectionStringType.value) {
                            case connectionStringType.ConnectionString.value: $scope.showConnectionString = true; $scope.showConnectionStringName = false; $scope.showConnectionStringAppSettingName = false; break;
                            case connectionStringType.ConnectionStringName.value: $scope.showConnectionStringName = true; $scope.showConnectionString = false; $scope.showConnectionStringAppSettingName = false; break;
                            case connectionStringType.ConnectionStringAppSettingName.value: $scope.showConnectionStringAppSettingName = true; $scope.showConnectionString = false; $scope.showConnectionStringName = false; break;
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        $scope.connectionString = payload.data.ConnectionString;
                        $scope.connectionStringName = payload.data.ConnectionStringName;
                        $scope.connectionStringAppSettingName = payload.data.ConnectionStringAppSettingName;

                        if ($scope.connectionString != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                        } else if ($scope.connectionStringName != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionStringName;
                        } else if ($scope.connectionStringAppSettingName != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionStringAppSettingName;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage",
                        ConnectionString: $scope.showConnectionString ? $scope.connectionString : undefined,
                        ConnectionStringName: $scope.showConnectionStringName ? $scope.connectionStringName : undefined,
                        ConnectionStringAppSettingName: $scope.showConnectionStringAppSettingName ? $scope.connectionStringAppSettingName : undefined
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataDatastoresettingSql', DataStoreSettingSqlDirective);
})(app);