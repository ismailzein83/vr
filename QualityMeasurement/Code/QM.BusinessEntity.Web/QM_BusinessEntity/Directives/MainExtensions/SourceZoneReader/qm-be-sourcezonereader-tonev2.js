"use strict";

app.directive("qmBeSourcezonereaderTonev2", [function () {
    
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
        templateUrl: function (element, attrs) {
         
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
       
        return "/Client/Modules/QM_BusinessEntity/Directives/MainExtensions/SourceZoneReader/Templates/SourceZoneReaderTOneV2.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.connectionString = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                
                return {
                    $type: "QM.BusinessEntity.MainExtensions.SourceZonesReaders.ZoneTOneV2SQLReader, QM.BusinessEntity.MainExtensions",
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
    }

    return directiveDefinitionObject;
}]);
