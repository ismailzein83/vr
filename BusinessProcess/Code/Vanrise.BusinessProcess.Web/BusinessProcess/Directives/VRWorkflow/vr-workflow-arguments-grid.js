'use strict';
app.directive('businessprocessVrWorkflowArgumentsGrid', ['BusinessProcess_VRWorkflowService', 'UtilsService', 'VRUIUtilsService',
function (BusinessProcess_VRWorkflowService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var vrWorkflowArgumentsGridDirective = new VrWorkflowArgumentsGridDirective(ctrl, $scope, $attrs);
                vrWorkflowArgumentsGridDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowArgumentsGridTemplate.html"
        };

        function VrWorkflowArgumentsGridDirective(ctrl, $scope, attrs) {

            var context;
            var gridArgumentItem;

            var gridAPI;

            var directionSelectorAPI;
            var directionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                ctrl.addWorkflowArgument = function () {
                        var dataItem;
                        dataItem = {
                            VRWorkflowArgumentId: UtilsService.guid()
                        };
                       
                        dataItem.onDirectionSelectorReady = function (api) {
                            dataItem.directionSelectorAPI = api;
                            var setLoader = function (value) { ctrl.isLoadingDirective = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directionSelectorAPI, undefined, setLoader);
                        };

                        ctrl.datasource.push(dataItem);

                        dataItem.saveDataItem = function () {
                            var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.VRWorkflowArgumentId, 'VRWorkflowArgumentId');
                            ctrl.datasource[index].Direction = dataItem.directionSelectorAPI.getSelectedIds();
                        };
                };

                ctrl.removeArgument = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.VRWorkflowArgumentId, 'VRWorkflowArgumentId');
                    ctrl.datasource.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.workflowArguments != undefined) {
                            for (var i = 0; i < payload.workflowArguments.length; i++) {
                                gridArgumentItem = payload.workflowArguments[i];
                                gridArgumentItem.readyArgumentDirectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                                gridArgumentItem.loadArgumentDirectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                                promises.push(gridArgumentItem.loadArgumentDirectionSelectorPromiseDeferred.promise);
                                addItemToGrid(gridArgumentItem);
                            }
                        }
                    }

                    function addItemToGrid(gridArgumentItem) {
                        var dataItem = {
                            VRWorkflowArgumentId: gridArgumentItem.VRWorkflowArgumentId,
                            Name: gridArgumentItem.Name,
                            //     Type: gridArgumentItem.Type,
                            Direction: gridArgumentItem.Direction
                        };
                        
                        var dataItemPayload = { selectedIds:  gridArgumentItem.Direction};
                       
                        dataItem.onDirectionSelectorReady = function (api) {
                            dataItem.directionSelectorAPI = api;
                            gridArgumentItem.readyArgumentDirectionSelectorPromiseDeferred.resolve();
                        };

                        gridArgumentItem.readyArgumentDirectionSelectorPromiseDeferred.promise
                            .then(function () {
                                VRUIUtilsService.callDirectiveLoad(dataItem.directionSelectorAPI, dataItemPayload, gridArgumentItem.loadArgumentDirectionSelectorPromiseDeferred);
                            });

                        dataItem.saveDataItem = function () {
                            var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.VRWorkflowArgumentId, 'VRWorkflowArgumentId');
                            ctrl.datasource[index].Direction = dataItem.directionSelectorAPI.getSelectedIds();
                        };

                        ctrl.datasource.push(dataItem);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var workflowArguments;
                    if (ctrl.datasource != undefined) {
                        workflowArguments = [];

                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            workflowArguments.push({
                                VRWorkflowArgumentId: currentItem.VRWorkflowArgumentId,
                                Name: currentItem.Name,
                                Type: "",
                                Direction: currentItem.directionSelectorAPI.getSelectedIds()
                            });
                        }
                    }
                    return workflowArguments;
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