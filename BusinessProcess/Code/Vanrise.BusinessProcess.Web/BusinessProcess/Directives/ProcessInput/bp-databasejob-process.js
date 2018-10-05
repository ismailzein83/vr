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
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return "/Client/Modules/BusinessProcess/Directives/ProcessInput/Templates/DatabaseJobProcessTemplate.html";
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
                            case connectionStringType.ConnectionString.value:
                                $scope.showConnectionString = true;
                                $scope.showConnectionStringName = false;
                                break;
                            case connectionStringType.ConnectionStringName.value:
                                $scope.showConnectionStringName = true;
                                $scope.showConnectionString = false;
                                break;
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.BusinessProcess.Entities.DatabaseJobProcessInput, Vanrise.BusinessProcess.Entities",
                            ConnectionStringName: $scope.showConnectionStringName ? $scope.connectionStringName : undefined,
                            ConnectionString: $scope.showConnectionString ? $scope.connectionString : undefined,
                            CustomCode: $scope.customCode
                        }
                    };
                };


                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {

                        $scope.connectionStringName = payload.data.ConnectionStringName;
                        $scope.connectionString = payload.data.ConnectionString;
                        $scope.customCode = payload.data.CustomCode;

                        if ($scope.connectionStringName != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionStringName;
                        } else if ($scope.connectionString != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                        }
                    }
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

   app.directive('businessprocessDatabasejobProcess', DatabaseJobProcessDirective);

})(app);