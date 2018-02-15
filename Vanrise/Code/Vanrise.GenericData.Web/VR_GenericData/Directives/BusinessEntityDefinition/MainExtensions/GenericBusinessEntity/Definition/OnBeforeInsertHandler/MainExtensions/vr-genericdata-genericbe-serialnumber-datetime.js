"use strict";

app.directive("vrGenericdataGenericbeSerialnumberDatetime", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DateTimeCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/OnBeforeInsertHandler/MainExtensions/Templates/SerialNumberDateTimeTemplate.html"
        };

        function DateTimeCtor($scope, ctrl, $attrs) {

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
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers.DateTimeSerialNumberPart, Vanrise.GenericData.MainExtensions",
                        DateTimeFormat: $scope.scopeModel.dateTimeFormat
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.settings != undefined)
                            $scope.scopeModel.dateTimeFormat = payload.settings.DateTimeFormat;
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

        }

        return directiveDefinitionObject;

    }
]);