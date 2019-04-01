'use strict';

app.directive('bpGenerictasktypeactionfilterconditionFiltergroup', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FilterCondition(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {

                };
            },
            templateUrl: "/Client/Modules/BusinessProcess/Directives/MainExtensions/BPGenericTaskTypeActionFilterCondition/Templates/FilterGroupFilterCondition.html"
        };

        function FilterCondition(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var filterAPI;
            var filterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onfilterReady = function (api) {
                    filterAPI = api;
                    filterReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([filterReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                    }
                    function loadFilterGroup() {
                        var filterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        filterReadyPromiseDeferred.promise.then(function () {
                            var filterGroupPayload = {
                                context: getContext(),
                                FilterGroup: payload != undefined && payload.filter != undefined ? payload.filter.FilterGroup : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(filterAPI, filterGroupPayload, filterLoadPromiseDeferred);
                        });
                        return filterLoadPromiseDeferred.promise;
                    }
                    var rootPromiseNode = {
                        promises:[loadFilterGroup()]
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var filter = filterAPI.getData();
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.FilterGroupBPGenericTaskTypeActionFilterCondition, Vanrise.BusinessProcess.MainExtensions",
                        FilterGroup: filter != undefined ? filter.filterObj : undefined
                    };
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
    }]);