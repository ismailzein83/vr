(function (app) {

    'use strict';

    DatabaseJobProcessDirective.$inject = ['UtilsService'];

    function DatabaseJobProcessDirective(UtilsService) {
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
                return "/Client/Modules/BusinessProcess/Directives/ScheduleTask/Templates/DatabaseJobTaskTemplate.html";
            }
        };
        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var connectionStringType;

            function initializeController() {
                $scope.showConnectionString = true;
                $scope.showConnectionStringName = false;
                $scope.showConnectionStringAppSettingName = false;

                connectionStringType = {
                    ConnectionString: { value: 0, description: "Connection String" },
                    ConnectionStringName: { value: 1, description: "Connection String Name" },
                    ConnectionStringAppSettingName: { value: 2, description: "Connection String App Setting Name" }
                };

                $scope.connectionStringType = UtilsService.getArrayEnum(connectionStringType);
                $scope.selectedConnectionStringType = connectionStringType.ConnectionString;

                $scope.onConnectionStringTypeSelectionChanged = function () {
                    if ($scope.selectedConnectionStringType != undefined) {

                        switch ($scope.selectedConnectionStringType.value) {
                            case connectionStringType.ConnectionString.value:
                                $scope.showConnectionString = true;
                                $scope.showConnectionStringName = false;
                                $scope.showConnectionStringAppSettingName = false;
                                break;
                            case connectionStringType.ConnectionStringName.value:
                                $scope.showConnectionString = false;
                                $scope.showConnectionStringName = true;
                                $scope.showConnectionStringAppSettingName = false;
                                break;
                            case connectionStringType.ConnectionStringAppSettingName.value:
                                $scope.showConnectionString = false;
                                $scope.showConnectionStringName = false;
                                $scope.showConnectionStringAppSettingName = true;
                                break;
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
                        $scope.query = payload.data.Query;

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
                        $type: "Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.DatabaseJobProcessInput, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments",
                        ConnectionStringName: $scope.showConnectionStringName ? $scope.connectionStringName : undefined,
                        ConnectionString: $scope.showConnectionString ? $scope.connectionString : undefined,
                        ConnectionStringAppSettingName: $scope.showConnectionStringAppSettingName ? $scope.connectionStringAppSettingName : undefined,
                        Query: $scope.query
                    };
                };

                api.getExpressionsData = function () {
                    return { "ScheduleTime": "ScheduleTime" };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('businessprocessDatabasejobTask', DatabaseJobProcessDirective);
})(app);