"use strict";

app.directive("vrRuntimeTaskactionWorkflow", ['UtilsService', 'VRUIUtilsService', 'BusinessProcessAPIService',
function (UtilsService, VRUIUtilsService, BusinessProcessAPIService) {

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
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/Runtime/Directives/TaskAction/Templates/TaskActionWorkFlow.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            $scope.bpDefinitions = [];
            $scope.selectedBPDefintion = undefined;
            var bpDefenitionDirectiveAPI;
            var bpDefenitionDirectiveReadyPromiseDeferred //= UtilsService.createPromiseDeferred();

            var bpDefenitionSelectorAPI;
            var bpDefenitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.onBPDefinitionDirectiveReady = function (api) {
                bpDefenitionDirectiveAPI = api;

                var setLoader = function (value) {
                    $scope.isLoadingAction = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, bpDefenitionDirectiveAPI, undefined, setLoader, bpDefenitionDirectiveReadyPromiseDeferred);
            }
            $scope.onBPDefinitionSelectorReady = function (api) {
                bpDefenitionSelectorAPI = api;
                bpDefenitionSelectorReadyPromiseDeferred.resolve();
            }
            
            $scope.onBPDefenitionSelctionChanged = function () {
                if(bpDefenitionDirectiveAPI!=undefined){
                    bpDefenitionDirectiveAPI.load(undefined);
                }
            }
            var api = {};
           
            api.getData = function () {

                return {
                    $type: "Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.WFTaskActionArgument, Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments",
                    RawExpressions: (bpDefenitionDirectiveAPI!=undefined)?bpDefenitionDirectiveAPI.getExpressionsData():null,
                    BPDefinitionID: bpDefenitionSelectorAPI.getSelectedIds(),
                    ProcessInputArguments:(bpDefenitionDirectiveAPI!=undefined)? bpDefenitionDirectiveAPI.getData() : null
                };
            };


            api.load = function (payload) {
                var data;
                if (payload != undefined && payload.data != undefined)
                    data = payload.data;

                var promises = [];

                var loadBPDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();  
                bpDefenitionSelectorReadyPromiseDeferred.promise.then(function () {
                    var payloadSelector;
                    if (data != undefined)
                        payloadSelector = {
                            selectedIds: (data != undefined) ? data.BPDefinitionID: null 
                        };
                    VRUIUtilsService.callDirectiveLoad(bpDefenitionSelectorAPI, payloadSelector, loadBPDefinitionSelectorPromiseDeferred);
                });
                promises.push(loadBPDefinitionSelectorPromiseDeferred.promise);

                var loadBPDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
                if (data != undefined && data.ProcessInputArguments) {
                    bpDefenitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    bpDefenitionDirectiveReadyPromiseDeferred.promise.then(function () {
                        bpDefenitionDirectiveReadyPromiseDeferred = undefined;
                        var payloadDirective;
                        if (data != undefined) {
                            payloadDirective = {
                                data: (data != undefined && data.ProcessInputArguments) ? data.ProcessInputArguments : null,
                                selectedDateOption: (data != undefined && data.RawExpressions != null) ? 0 : 1
                            };

                        }
                        VRUIUtilsService.callDirectiveLoad(bpDefenitionDirectiveAPI, payloadDirective, loadBPDefinitionPromiseDeferred);

                    });
                    promises.push(loadBPDefinitionPromiseDeferred.promise);
                }
                return UtilsService.waitMultiplePromises(promises);
               
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
