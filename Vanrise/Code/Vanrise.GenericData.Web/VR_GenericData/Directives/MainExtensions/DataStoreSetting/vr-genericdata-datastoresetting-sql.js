(function (app) {

    'use strict';

    DataStoreSettingSqlDirective.$inject = ['UtilsService','VRUIUtilsService'];

    function DataStoreSettingSqlDirective(UtilsService, VRUIUtilsService) {
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

            var connectionStringApi;
            var connectionStringPromiseReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onConnectionStringReady = function (api) {
                    connectionStringApi = api;
                    connectionStringPromiseReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadConnectionStringDirective());

                    function loadConnectionStringDirective() {
                        var connectionStringPromiseLoadDeferred = UtilsService.createPromiseDeferred();
                        connectionStringPromiseReadyDeferred.promise.then(function () {
                            var payloadDirective;
                            if (payload != undefined && payload.data != undefined) {
                                payloadDirective = {
                                    ConnectionString: payload.data.ConnectionString,
                                    ConnectionStringName: payload.data.ConnectionStringName,
                                    ConnectionStringAppSettingName: payload.data.ConnectionStringAppSettingName
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(connectionStringApi, payloadDirective, connectionStringPromiseLoadDeferred);
                        });
                        return connectionStringPromiseLoadDeferred.promise;
                    }
                };

                api.getData = function () {
                    var connectionString = connectionStringApi.getData();
                    return {
                        $type: "Vanrise.GenericData.SQLDataStorage.SQLDataStoreSettings, Vanrise.GenericData.SQLDataStorage",
                        ConnectionString: connectionString.ConnectionString,
                        ConnectionStringName: connectionString.ConnectionStringName,
                        ConnectionStringAppSettingName: connectionString.ConnectionStringAppSettingName
                    };
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataDatastoresettingSql', DataStoreSettingSqlDirective);
})(app);