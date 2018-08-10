'use strict';

app.directive('businessprocessVrWorkflowactivitySubprocess', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {

	    var directiveDefinitionObject = {
	        restrict: 'E',
	        scope: {
	            onReady: '='
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;
	            var ctor = new workflowCtor(ctrl, $scope, $attrs);
	            ctor.initializeController();
	        },
	        controllerAs: 'ctrl',
	        bindToController: true,
	        compile: function (element, attrs) {

	        },
	        templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowSubProcessTemplate.html'
	    };

	    function workflowCtor(ctrl, $scope, $attrs) {

	        var vrWorkflowSelectorAPI;
	        var vrWorkflowSelectorReadyDeferred = UtilsService.createPromiseDeferred();
	        var vrWorkflowSelectorSelectionChangedDeferred;

	        var inArgumentsGridAPI;
	        var inArgumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

	        var outArgumentsGridAPI;
	        var outArgumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

	        var selectedVRWrkflow;

	        var loadSelectedVRWorkflowPromise;

	        this.initializeController = initializeController;
	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.hasInArguments = false;
	            $scope.scopeModel.hasOutArguments = false;
	            $scope.scopeModel.inArguments = [];
	            $scope.scopeModel.outArguments = [];

	            $scope.scopeModel.onVRWorkflowSelectorReady = function (api) {
	                vrWorkflowSelectorAPI = api;
	                vrWorkflowSelectorReadyDeferred.resolve();
	            };

	            $scope.scopeModel.onInArgumentsGridReady = function (api) {
	                inArgumentsGridAPI = api;
	                inArgumentsGridReadyDeferred.resolve();
	            };

	            $scope.scopeModel.onOutArgumentsGridReady = function (api) {
	                outArgumentsGridAPI = api;
	                outArgumentsGridReadyDeferred.resolve();
	            };

	            $scope.scopeModel.onVRWorkflowSelectionChanged = function (selectedItem) {
	                if (selectedItem == undefined)
	                    return;
	                else {
	                    if (vrWorkflowSelectorSelectionChangedDeferred != undefined) {
	                        vrWorkflowSelectorSelectionChangedDeferred.resolve();
	                    }
	                    else {
	                        $scope.scopeModel.isLoading = true;
	                        var rootPromiseNode = {
	                            promises: [loadVRWorkflow(selectedItem.VRWorkflowId)],
	                            getChildNode: function () {
	                                var gridPromises = [];
	                                if ($scope.scopeModel.hasInArguments)
	                                    gridPromises.push(loadInArgumentsGrid());

	                                if ($scope.scopeModel.hasOutArguments)
	                                    gridPromises.push(loadOutArgumentsGrid());

	                                return {
	                                    promises: gridPromises
	                                };
	                            }
	                        };
	                        UtilsService.waitPromiseNode(rootPromiseNode).then(function () { $scope.scopeModel.isLoading = false; });
	                    }
	                }
	            };

	            defineAPI();
	        }

	        function loadInArgumentsGrid(selectedArguments) {
	            $scope.scopeModel.inArguments = [];
	            var inArgumentsGridLoadDeferred = UtilsService.createPromiseDeferred();
	            inArgumentsGridReadyDeferred.promise.then(function () {
	                if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
	                    for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
	                        var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
	                        if (currentArgumentDefinition.Direction != 1) {
	                            var argValue = tryGetArgumentValue(selectedArguments, currentArgumentDefinition.Name);
	                            $scope.scopeModel.inArguments.push({ name: currentArgumentDefinition.Name, value: argValue });
	                        }
	                    }
	                }
	                inArgumentsGridLoadDeferred.resolve();
	            });

	            return inArgumentsGridLoadDeferred.promise;
	        };

	        function tryGetArgumentValue(selectedArguments, argumentName) {
	            console.log(selectedArguments);
	            if (selectedArguments == undefined)
	                return;
                
	            return selectedArguments[argumentName];
	        };

	        function loadOutArgumentsGrid(selectedArguments) {
	            $scope.scopeModel.outArguments = [];
	            var outArgumentsGridLoadDeferred = UtilsService.createPromiseDeferred();
	            outArgumentsGridReadyDeferred.promise.then(function () {
	                if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
	                    for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
	                        var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
	                        if (currentArgumentDefinition.Direction != 0) {
	                            var argValue = tryGetArgumentValue(selectedArguments, currentArgumentDefinition.Name);
	                            $scope.scopeModel.outArguments.push({ name: currentArgumentDefinition.Name, value: argValue });
	                        }
	                    }
	                }
	                outArgumentsGridLoadDeferred.resolve();
	            });

	            return outArgumentsGridLoadDeferred.promise;
	        };



	        function loadVRWorkflow(vrWorkflowId) {
	            $scope.scopeModel.hasInArguments = false;
	            $scope.scopeModel.hasOutArguments = false;

	            return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowEditorRuntime(vrWorkflowId).then(function (response) {
	                selectedVRWrkflow = response.Entity;
	                if (selectedVRWrkflow != undefined && selectedVRWrkflow.Settings != undefined && selectedVRWrkflow.Settings.Arguments != undefined) {
	                    for (var i = 0; i < selectedVRWrkflow.Settings.Arguments.length; i++) {
	                        var currentArgumentDefinition = selectedVRWrkflow.Settings.Arguments[i];
	                        switch (currentArgumentDefinition.Direction) {
	                            case 0: $scope.scopeModel.hasInArguments = true; break;
	                            case 1: $scope.scopeModel.hasOutArguments = true; break;
	                            case 2: $scope.scopeModel.hasInArguments = true; $scope.scopeModel.hasOutArguments = true; break;
	                        }

	                    }
	                }
	            });
	        }

	        function defineAPI() {
	            var api = {};

	            api.load = function (payload) {
	                console.log(payload);

	                var promises = [];
	                var selectedVRWorkflowId;

	                if (payload != undefined) {
	                    if (payload.Settings != undefined) {
	                        selectedVRWorkflowId = payload.Settings.VRWorkflowId;
	                    }

	                    if (payload.Context != null)
	                        $scope.scopeModel.context = payload.Context;

	                    if (selectedVRWorkflowId != undefined) {
	                        vrWorkflowSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

	                        var rootPromiseNode = {
	                            promises: [loadVRWorkflow(selectedVRWorkflowId)],
	                            getChildNode: function () {
	                                var gridPromises = [];
	                                if ($scope.scopeModel.hasInArguments)
	                                    gridPromises.push(loadInArgumentsGrid(payload.Settings.InArguments));

	                                if ($scope.scopeModel.hasOutArguments)
	                                    gridPromises.push(loadOutArgumentsGrid(payload.Settings.OutArguments));

	                                return {
	                                    promises: gridPromises
	                                };
	                            }
	                        };
	                        promises.push(UtilsService.waitPromiseNode(rootPromiseNode));
	                    }

	                    var loadVRWorkflowSelectorPromise = loadVRWorkflowSelector();
	                    promises.push(loadVRWorkflowSelectorPromise)
	                }

	                function loadVRWorkflowSelector() {
	                    var vrWorkflowSelectorLoadDeferred = UtilsService.createPromiseDeferred();

	                    vrWorkflowSelectorReadyDeferred.promise.then(function () {

	                        var vrWorkflowSelectorPayload;
	                        if (selectedVRWorkflowId != undefined) {
	                            vrWorkflowSelectorPayload = { selectedIds: selectedVRWorkflowId };
	                        }
	                        VRUIUtilsService.callDirectiveLoad(vrWorkflowSelectorAPI, vrWorkflowSelectorPayload, vrWorkflowSelectorLoadDeferred);
	                    });

	                    return vrWorkflowSelectorLoadDeferred.promise
	                }

	                return UtilsService.waitMultiplePromises(promises).then(function () { vrWorkflowSelectorSelectionChangedDeferred = undefined });
	            };

	            api.getData = function () {
	                var inArguments;
	                if ($scope.scopeModel.hasInArguments) {
	                    inArguments = {};
	                    for (var x = 0; x < $scope.scopeModel.inArguments.length; x++) {
	                        var currentInArgument = $scope.scopeModel.inArguments[x];
	                        if (currentInArgument.value != undefined) {
	                            inArguments[currentInArgument.name] = currentInArgument.value;
	                        }
	                    }
	                }

	                var outArguments;
	                if ($scope.scopeModel.hasOutArguments) {
	                    outArguments = {};
	                    for (var x = 0; x < $scope.scopeModel.outArguments.length; x++) {
	                        var currentOutArgument = $scope.scopeModel.outArguments[x];
	                        if (currentOutArgument.value != undefined) {
	                            outArguments[currentOutArgument.name] = currentOutArgument.value;
	                        }
	                    }
	                }

	                return {
	                    $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowSubProcessActivity, Vanrise.BusinessProcess.MainExtensions",
	                    VRWorkflowId: vrWorkflowSelectorAPI.getSelectedIds(),
	                    InArguments: inArguments,
	                    OutArguments: outArguments
	                };
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        };
	    }
	    return directiveDefinitionObject;
	}]);