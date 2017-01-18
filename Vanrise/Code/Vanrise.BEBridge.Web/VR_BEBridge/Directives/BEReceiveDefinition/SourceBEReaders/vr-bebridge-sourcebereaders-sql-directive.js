'use strict';

app.directive('vrBebridgeSourcebereadersSqlDirective', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var sqlSourceReader = new filepSourceReader($scope, ctrl, $attrs);
                sqlSourceReader.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_BEBridge/Directives/BEReceiveDefinition/SourceBEReaders/Templates/BEReceiveDefinitionSqlSourceReadersTemplate.html'
        };

        function filepSourceReader($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.connectionString = payload.Setting.ConnectionString;
                        $scope.scopeModel.query = payload.Setting.Query;
                        $scope.scopeModel.timeoutInSec = payload.Setting.CommandTimeout;
                        $scope.scopeModel.basedOnId = payload.Setting.BasedOnId;
                        $scope.scopeModel.idField = payload.Setting.IdField;
                    }
                };
                api.getData = function () {
                    var setting =
                    {
                        ConnectionString: $scope.scopeModel.connectionString,
                        Query: $scope.scopeModel.query,
                        CommandTimeout: $scope.scopeModel.timeoutInSec,
                        BasedOnId: $scope.scopeModel.basedOnId,
                        IdField: $scope.scopeModel.idField
                    };
                    return {
                        $type: "Vanrise.BEBridge.MainExtensions.SourceBEReaders.SqlSourceReader, Vanrise.BEBridge.MainExtensions",
                        Setting: setting
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
