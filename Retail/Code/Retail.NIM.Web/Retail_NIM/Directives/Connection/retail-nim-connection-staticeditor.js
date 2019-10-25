"use strict";

app.directive("retailNimConnectionStaticeditor", ["VRUIUtilsService", "UtilsService",
    function (VRUIUtilsService, UtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                isrequired: "=",
                normalColNum: '@',
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ConnectionStaticEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_NIM/Directives/Connection/Templates/ConnectionStaticEditorTemplate.html'
        };

        function ConnectionStaticEditorCtor(ctrl, $scope, attrs) {
            var connectionDirectiveAPI;
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onConnectionDirectiveReady = function (api) {
                    connectionDirectiveAPI = api;
                    defineAPI();
                };

            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {

                    var connectionDirectivePayload = payload != undefined ? payload.selectedValues : undefined;

                    var connectionDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    VRUIUtilsService.callDirectiveLoad(connectionDirectiveAPI, connectionDirectivePayload, connectionDirectiveLoadPromiseDeferred);

                    return connectionDirectiveLoadPromiseDeferred.promise;

                };

                api.setData = function (Connection) {
                    var object = connectionDirectiveAPI.getData();
                    Connection.Type = object.Type;
                    Connection.Port1 = object.Port1;
                    Connection.Port2 = object.Port2;
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);