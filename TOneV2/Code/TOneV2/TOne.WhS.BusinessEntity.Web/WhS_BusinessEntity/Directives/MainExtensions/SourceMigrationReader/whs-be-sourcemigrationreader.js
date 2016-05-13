"use strict";

app.directive("whsBeSourcemigrationreader", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_SwitchAPIService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SwitchAPIService) {

        var directiveDefinitionObject = {
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
                }
            },
            templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/SourceMigrationReader/Templates/SourceMigrationReader.html"
        };

        function DirectiveConstructor($scope, ctrl) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.getData = function () {
                    var schedulerTaskAction;
                    schedulerTaskAction = {};
                    schedulerTaskAction.$type = "TOne.WhS.DBSync.Business.DBSyncTaskActionArgument, TOne.WhS.DBSync.Business";
                    schedulerTaskAction.ConnectionString = $scope.connectionString;
                    schedulerTaskAction.UseTempTables = ($scope.useTempTables == true) ? true : false;
                    return schedulerTaskAction;
                };

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        $scope.connectionString = payload.data.ConnectionString;
                        $scope.useTempTables = payload.data.UseTempTables;
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
