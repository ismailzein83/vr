(function (app) {

    'use strict';
    DataStoreSettingSqlDirective.$inject = [ 'UtilsService'];

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
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStoreSetting/Templates/SqlTemplate.html";
            }
        };
        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;
            var connectionStringType;
            function initializeController() {
                connectionStringType = {
                    ConnectionString: { value: 0, description: "Connection String" },
                    ConnectionStringName: { value: 1, description: "Connection String Name" },
                };

                $scope.connectionStringType = UtilsService.getArrayEnum(connectionStringType);
                $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                $scope.showConnectionString = true;
                $scope.showConnectionStringName = false;
                $scope.onConnectionStringTypeSelectionChanged = function () {
                    if ($scope.selectedConnectionStringType != undefined) {

                        switch ($scope.selectedConnectionStringType.value) {
                            case connectionStringType.ConnectionString.value: $scope.showConnectionString = true; $scope.showConnectionStringName = false; break;
                            case connectionStringType.ConnectionStringName.value: $scope.showConnectionStringName = true; $scope.showConnectionString = false; break;
                        }

                    }
                };



                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage",
                        ConnectionStringName: $scope.showConnectionStringName ? $scope.connectionStringName : undefined,
                        ConnectionString: $scope.showConnectionString ? $scope.connectionString : undefined
                    };
                };


                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {

                        $scope.connectionStringName = payload.data.ConnectionStringName;
                        $scope.connectionString = payload.data.ConnectionString;

                        if ($scope.connectionStringName != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionStringName;
                        } else if ($scope.connectionString != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                        }

                        //  $scope.connectionString = payload.data.ConnectionString;
                    }
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataDatastoresettingSql', DataStoreSettingSqlDirective);

})(app);