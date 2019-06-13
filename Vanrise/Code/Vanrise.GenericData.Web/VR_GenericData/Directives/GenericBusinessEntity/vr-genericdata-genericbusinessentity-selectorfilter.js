"use strict";
app.directive("vrGenericdataGenericbusinessentitySelectorfilter", ["UtilsService", "VRUIUtilsService", 'VR_GenericData_GenericBusinessEntityAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntitySelectorFilterCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntitySelectorFilter.html"
        };

        function GenericBusinessEntitySelectorFilterCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                       
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);