﻿"use strict";

app.directive("demoModulePageRunTimeDatetimeFilter", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DatetimeFilter($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/PageRunTime/MainExtensions/Templates/DatetimeFilter.html"
        };

        function DatetimeFilter($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();

              
            }
           

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
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