"use strict";

app.directive("vrGenericdataGenericbeAftersavehandlerSendemail", ["UtilsService", "VRNotificationService",
    function (UtilsService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum:"@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SendEmailHandler($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "sendEmailCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/OnAfterSaveHandler/Templates/SendEmailHandlerTemplate.html"
        };

        function SendEmailHandler($scope, ctrl, $attrs) {

            var context;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {                   
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers.SendEmailAfterSaveHandler, Vanrise.GenericData.MainExtensions",
                        EntityObjectName: $scope.scopeModel.entityObjectName,
                        InfoType: $scope.scopeModel.infoType
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined && payload.settings) {                        
                        $scope.scopeModel.entityObjectName = payload.settings.EntityObjectName;
                        $scope.scopeModel.infoType = payload.settings.InfoType;
                    }
                };


          
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;

    }
]);