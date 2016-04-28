﻿"use strict";

app.directive("whsBeSourceswitchreaderTonev1", [function () {

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
        templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/SourceSwitchReader/Templates/SourceSwitchReaderTOneV1.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "TOne.WhS.DBSync.Business.SourceSwitchesReaders.SwitchTOneV1Reader, TOne.WhS.DBSync.Business",
                    ConnectionString: $scope.connectionString
                };
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.connectionString = payload.connectionString;
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);
