"use strict";

app.directive("vrRuntimeTaskactionWorkflow", ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/Runtime/Directives/TaskAction/Templates/TaskActionWorkFlow.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        var context;
        var bpDefenitionDirectiveAPI;
        var bpDefenitionDirectiveReadyPromiseDeferred;

        var bpDefenitionSelectorAPI;
        var bpDefenitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.bpDefinitions = [];

            $scope.onBPDefinitionDirectiveReady = function (api) {
                bpDefenitionDirectiveAPI = api;

                var setLoader = function (value) {
                    $scope.isLoadingAction = value;
                };
                var payloadDirective = {
                    context: getContext()
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, bpDefenitionDirectiveAPI, payloadDirective, setLoader, bpDefenitionDirectiveReadyPromiseDeferred);
            };

            $scope.onBPDefinitionSelectorReady = function (api) {
                bpDefenitionSelectorAPI = api;
                bpDefenitionSelectorReadyPromiseDeferred.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            var bpDefinitionId;           
            $scope.disableBPDefinitionColumn = false;
           
            api.getData = function () {

                return {
                    $type: "Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.WFTaskActionArgument, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments",
                    RawExpressions: (bpDefenitionDirectiveAPI != undefined && bpDefenitionDirectiveAPI.getExpressionsData != undefined) ? bpDefenitionDirectiveAPI.getExpressionsData() : null,
                    BPDefinitionID: bpDefenitionSelectorAPI.getSelectedIds(),
                    ProcessInputArguments: (bpDefenitionDirectiveAPI != undefined) ? bpDefenitionDirectiveAPI.getData() : null
                };
            };
            api.setAdditionalParamter = function (additionalParameter) {
                if (additionalParameter != undefined) {
                    bpDefinitionId = additionalParameter.bpDefinitionID;
                }
            };

            api.load = function (payload) {
                var data;

                if (payload != undefined){
                    context = payload.context;
                    if(payload.data != undefined)
                        data = payload.data;
                }
                   
                var promises = [];

                var loadBPDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                bpDefenitionSelectorReadyPromiseDeferred.promise.then(function () {
                    var payloadSelector = {
                        filter: {
                            Filters: [{
                                $type: "Vanrise.BusinessProcess.Business.BPDefinitionScheduleTaskFilter, Vanrise.BusinessProcess.Business",
                            }]
                        }
                    };
                    if (data != undefined) {
                        $scope.disableBPDefinitionColumn = true;
                        payloadSelector.selectedIds =  (data != undefined) ? data.BPDefinitionID : null;
                    }
                    else if (bpDefinitionId != undefined) {
                        $scope.disableBPDefinitionColumn = true;
                        payloadSelector.selectedIds = bpDefinitionId;
                    }
                    VRUIUtilsService.callDirectiveLoad(bpDefenitionSelectorAPI, payloadSelector, loadBPDefinitionSelectorPromiseDeferred);
                });
                promises.push(loadBPDefinitionSelectorPromiseDeferred.promise);

                var loadBPDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
                if (data != undefined && data.ProcessInputArguments) {
                    bpDefenitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    bpDefenitionDirectiveReadyPromiseDeferred.promise.then(function () {
                        bpDefenitionDirectiveReadyPromiseDeferred = undefined;
                        var payloadDirective = {
                            context: getContext()
                        };
                        if (data != undefined) {                           
                            payloadDirective.data = data.ProcessInputArguments;
                            payloadDirective.rawExpressions = data.RawExpressions;
                        }
                        VRUIUtilsService.callDirectiveLoad(bpDefenitionDirectiveAPI, payloadDirective, loadBPDefinitionPromiseDeferred);

                    });
                    promises.push(loadBPDefinitionPromiseDeferred.promise);
                }
                return UtilsService.waitMultiplePromises(promises);

            };

            api.validate = function () {
                if (bpDefenitionDirectiveAPI != undefined && bpDefenitionDirectiveAPI.validate != undefined && typeof (bpDefenitionDirectiveAPI.validate) == "function") {
                    return bpDefenitionDirectiveAPI.validate();
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

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);
