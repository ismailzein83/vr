"use strict";

app.directive("demoModulePageDefinitionDatetimeField", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DatetimeField($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/PageDefinition/MainExtensions/Templates/DatetimeField.html"
        };

        function DatetimeField($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();

              
            }
           

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                   
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.PageDefinition.DatetimeFieldType,Demo.Module.MainExtension",

                    };
                };
                
              if (ctrl.onReady != null)
               ctrl.onReady(api);
                
            }
        }

        return directiveDefinitionObject;

    }
]);