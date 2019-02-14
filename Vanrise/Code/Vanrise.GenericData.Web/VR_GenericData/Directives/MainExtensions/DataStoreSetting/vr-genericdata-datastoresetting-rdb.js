(function (app) {

    'use strict';
    DataStoreSettingRDBDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DataStoreSettingRDBDirective(UtilsService, VRUIUtilsService) {
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
                return "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStoreSetting/Templates/DataStoreSettingsRDBTemplate.html";
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

                api.getData = function () {
                    var connectionString = connectionStringApi.getData();
                    return {
                        $type: "Vanrise.GenericData.RDBDataStorage.RDBDataStoreSettings, Vanrise.GenericData.RDBDataStorage",
                        ConnectionString: connectionString.ConnectionString,
                        ConnectionStringName: connectionString.ConnectionStringName,
                        ConnectionStringAppSettingName: connectionString.ConnectionStringAppSettingName
                    };
                };

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

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }
    app.directive('vrGenericdataDatastoresettingRdb', DataStoreSettingRDBDirective);
})(app);