(function (app) {

    'use strict';

    CallHttpServiceEventsTracking.$inject = ['UtilsService', 'VRUIUtilsService', 'VisualEventTypeEnum'];

    function CallHttpServiceEventsTracking(UtilsService, VRUIUtilsService, VisualEventTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var callHttpServiceEventsTracking = new CallHttpServiceEventsTrackingController($scope, ctrl, $attrs);
                callHttpServiceEventsTracking.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPVisualItemDefinition/IconsEditors/Templates/EventsTrackingVisualItemIconsCommon.html"
        };

        function CallHttpServiceEventsTrackingController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var events = [];

            var gridAPI;
            var gridPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.bpEventsInstanceTracking = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridPromiseReadyDeffered.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        events = payload.events;
                        if (events != undefined && events.length > 0) {
                            initialPromises.push(loadGrid());
                        }
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadGrid() {
                var loadGridPromiseDeffered = UtilsService.createPromiseDeferred();
                gridPromiseReadyDeffered.promise.then(function () {
                    for (var i = 0; i < events.length; i++) {
                        var event = events[i];
                        var existingEvent = UtilsService.getItemByVal($scope.scopeModel.bpEventsInstanceTracking, event.EventTypeId, 'EventTypeId');
                        if (existingEvent == undefined) {
                            event.EventType = UtilsService.getEnumDescription(VisualEventTypeEnum, event.EventTypeId);
                            $scope.scopeModel.bpEventsInstanceTracking.push(event);
                        }
                    }
                    $scope.scopeModel.bpEventsInstanceTracking.sort(function (a, b) {
                        return b.BPVisualEventId - a.BPVisualEventId;
                    });
                    loadGridPromiseDeffered.resolve();
                });
                return loadGridPromiseDeffered.promise;
            }
        }
    }

    app.directive('bpWorkflowVisualitemdefintionTrackingeventsCommon', CallHttpServiceEventsTracking);

})(app);