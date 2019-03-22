'use strict';

app.directive('bpTasktypeStaticeditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BPTaskTypeStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPTask/Templates/BPTaskTypeStaticEditorTemplate.html"
        };

        function BPTaskTypeStaticEditor(ctrl, $scope, $attrs) {

            var selectedValues;

            var baseBPTaskTypeSettingsSelectiveAPI;
            var baseBPTaskTypeSettingsPromiseReadyDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBaseBPTaskTypeSettingsSelectiveReady = function (api) {
                    baseBPTaskTypeSettingsSelectiveAPI = api;
                    baseBPTaskTypeSettingsPromiseReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([baseBPTaskTypeSettingsPromiseReadyDeferred.promise]).then(function () {
                    defineApi();
                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    selectedValues = payload.selectedValues;
                    var initialPromises = [];

                    function loadBaseBPTaskTypeSettings() {
                        var baseBPTaskTypeSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                        baseBPTaskTypeSettingsPromiseReadyDeferred.promise.then(function () {
                            var baseBPTaskTypeSettingsPayload = {
                                settings: selectedValues != undefined ? selectedValues.Settings : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(baseBPTaskTypeSettingsSelectiveAPI, baseBPTaskTypeSettingsPayload, baseBPTaskTypeSettingsLoadDeferred);
                        });
                        return baseBPTaskTypeSettingsLoadDeferred.promise;
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            directivePromises.push(loadBaseBPTaskTypeSettings());

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.setData = function (bpTaskType) {
                    bpTaskType.Settings = baseBPTaskTypeSettingsSelectiveAPI.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }]);