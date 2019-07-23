'use strict';

restrict: 'E',
    app.directive('vrCommonSqlconnectionEditor', ['UtilsService', 'VRUIUtilsService',
        function (UtilsService, VRUIUtilsService) {
            return {
                restrict: 'E',
                scope: {
                    onReady: '=',
                    normalColNum: '@'
                },
                controller: function ($scope, $element, $attrs) {
                    var ctrl = this;
                    var editor = new SQLConnectionEditor($scope, ctrl, $attrs);
                    editor.initializeController();
                },
                controllerAs: 'sqlCtrl',
                bindToController: true,
                templateUrl: '/Client/Modules/Common/Directives/MainExtensions/ConnectionTypes/Templates/SQLConnectionEditorTemplate.html'
            };

            function SQLConnectionEditor($scope, ctrl, $attrs) {
                this.initializeController = initializeController;

                var connectionStringApi;
                var connectionStringPromiseReadyDeferred = UtilsService.createPromiseDeferred();

                function initializeController() {
                    $scope.scopeModel = {};

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
                            $type: "Vanrise.Common.Business.SQLConnection, Vanrise.Common.Business",
                            ConnectionString: connectionString.ConnectionString,
                            ConnectionStringName: connectionString.ConnectionStringName,
                            ConnectionStringAppSettingName: connectionString.ConnectionStringAppSettingName
                        };
                    };

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                        ctrl.onReady(api);
                }
            }
        }]);