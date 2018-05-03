(function (app) {

    'use strict';

    FreeRadiusPostgresDataManager.$inject = ["UtilsService"];

    function FreeRadiusPostgresDataManager(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FreeRadiusPostgresDataManagerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlData",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/FreeRadius/FreeRadiusDataManagers/Templates/FreeRadiusPostgresDataManagerTemplate.html"
        };

        function FreeRadiusPostgresDataManagerCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.redundantConnectionStrings = [];
                $scope.scopeModel.isLoading = false;

                $scope.scopeModel.addRedundantConnectionString = function () {
                    $scope.scopeModel.redundantConnectionStrings.push({ ConnectionString: undefined });
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var freeRadiusPostgresDataManager;

                    if (payload != undefined) {
                        freeRadiusPostgresDataManager = payload.freeRadiusDataManager;

                        if (freeRadiusPostgresDataManager != undefined) {
                            $scope.scopeModel.connectionString = freeRadiusPostgresDataManager.ConnectionString.ConnectionString;
                            $scope.scopeModel.schemaName = freeRadiusPostgresDataManager.ConnectionString.SchemaName;
                        }

                        if (freeRadiusPostgresDataManager.RedundantConnectionStrings != undefined)
                            $scope.scopeModel.redundantConnectionStrings = freeRadiusPostgresDataManager.RedundantConnectionStrings;
                    }
                };

                api.getData = function getData() {

                    var data = {
                        $type: "TOne.WhS.RouteSync.FreeRadius.FreeRadiusPostgresDataManager, TOne.WhS.RouteSync.FreeRadius",
                        ConnectionString: { ConnectionString: $scope.scopeModel.connectionString, SchemaName: $scope.scopeModel.schemaName },
                        RedundantConnectionStrings: $scope.scopeModel.redundantConnectionStrings
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncFreeradiuspostgresDatamanager', FreeRadiusPostgresDataManager);

})(app);