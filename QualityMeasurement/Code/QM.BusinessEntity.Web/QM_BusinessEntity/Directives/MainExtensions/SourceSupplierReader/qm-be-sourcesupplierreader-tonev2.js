﻿"use strict";

app.directive("demoServicesubtypeSms", [function () {
    
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
        templateUrl: "/Client/Modules/QM_BusinessEntity/Directives/MainExtensions/SourceSupplierReader/Templates/SourceSupplierReaderTOneV2.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "QM.BusinessEntity.MainExtensions.SourceSuppliersReaders.SupplierTOneV2SQLReader, QM.BusinessEntity.MainExtensions",
                    ConnectionString: $scope.connectionString
                };
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.connectionString = payload.connectionString;
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);
