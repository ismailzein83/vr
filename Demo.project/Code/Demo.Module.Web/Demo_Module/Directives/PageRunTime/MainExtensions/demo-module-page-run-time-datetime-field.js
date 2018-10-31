"use strict";

app.directive("demoModulePageRunTimeDatetimeField", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
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
            templateUrl: "/Client/Modules/Demo_Module/Directives/PageRunTime/MainExtensions/Templates/DatetimeField.html"
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
                    if (payload != undefined) {
                        if (payload.runTimeEditor != undefined)
                            $scope.scopeModel.DatetimeValue = payload.runTimeEditor;
                        if (payload.label != undefined) $scope.scopeModel.label = payload.label;
                        if (payload.isRequired != undefined) $scope.scopeModel.isRequired = payload.isRequired;

                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return $scope.scopeModel.DatetimeValue;
                };
                
              if (ctrl.onReady != null)
               ctrl.onReady(api);
                
            }
        }

        return directiveDefinitionObject;

    }
]);