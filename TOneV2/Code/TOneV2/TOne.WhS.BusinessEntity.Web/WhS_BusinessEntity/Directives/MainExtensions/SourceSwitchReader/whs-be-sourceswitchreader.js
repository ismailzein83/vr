"use strict";

app.directive("whsBeSourceswitchreader", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_SwitchAPIService',
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
            templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/SourceSwitchReader/Templates/SourceSwitchReader.html"
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
                    schedulerTaskAction.$type = "TOne.WhS.DBSync.Business.SwitchSyncTaskActionArgument, TOne.WhS.DBSync.Business",
                    schedulerTaskAction.SourceSwitchReader = {
                        $type: "TOne.WhS.DBSync.Business.SourceSwitchesReaders.SwitchTOneV1Reader, TOne.WhS.DBSync.Business",
                        ConnectionString: $scope.connectionString
                    };
                    console.log('schedulerTaskAction')
                    console.log(schedulerTaskAction)
                    return schedulerTaskAction;
                };

                api.load = function (payload) {
                    console.log('payload')
                    console.log(payload)
                    if (payload != undefined && payload.data != undefined) {
                        $scope.connectionString = payload.data.SourceSwitchReader.ConnectionString;
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
