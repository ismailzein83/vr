(function (app) {
    'use strict';

    ExecuteDatabaseCommandSMSHandlerDirective.$inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService"];

    function ExecuteDatabaseCommandSMSHandlerDirective(UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ExecuteDatabaseCommandSMSHandlerDirective($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/SMS/Templates/SMSSendHandler.html"

        };

        function ExecuteDatabaseCommandSMSHandlerDirective($scope, ctrl, $attrs) {
        
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
                        $type: "Vanrise.Common.MainExtensions.SMSSendHandler.ExecuteDatabaseCommandSMSHandler,Vanrise.Common.MainExtensions",
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
            
        }

        return directiveDefinitionObject;
    }

    app.directive('vrCommonExecutedatabasecommandSmshandler', SMSSendHandlerDirective);

})(app);