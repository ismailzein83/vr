//'use strict';

//app.directive('businessprocessVrWorkflowactivitySubprocess', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowAPIService',
//	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowAPIService) {

//	    var directiveDefinitionObject = {
//	        restrict: 'E',
//	        scope: {
//	            onReady: '='
//	        },
//	        controller: function ($scope, $element, $attrs) {
//	            var ctrl = this;
//	            var ctor = new workflowCtor(ctrl, $scope, $attrs);
//	            ctor.initializeController();
//	        },
//	        controllerAs: 'ctrl',
//	        bindToController: true,
//	        compile: function (element, attrs) {

//	        },
//	        templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowSubProcessTemplate.html'
//	    };

//	    function workflowCtor(ctrl, $scope, $attrs) {

//	        var vrWorkflowSelectorAPI;
//	        var vrWorkflowSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//	        var inArgumentsGridAPI;
//	        var inArgumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

//	        var outArgumentsGridAPI;
//	        var outArgumentsGridReadyDeferred = UtilsService.createPromiseDeferred();

//	        var selectedVRWrkflow;

//	        var loadSelectedVRWorkflowPromise;

//	        this.initializeController = initializeController;
//	        function initializeController() {
//	            $scope.scopeModel = {};
//	            $scope.scopeModel.hasInArguments = true;
//	            $scope.scopeModel.hasOutArguments = false;

//	            $scope.scopeModel.onVRWorkflowSelectorReady = function (api) {
//	                vrWorkflowSelectorAPI = api;
//	                vrWorkflowSelectorReadyDeferred.resolve();
//	            };

//	            $scope.scopeModel.onInArgumentsGridReady = function (api) {
//	                inArgumentsGridAPI = api;
//	                inArgumentsGridReadyDeferred.resolve();
//	            };

//	            $scope.scopeModel.onOutArgumentsGridReady = function (api) {
//	                outArgumentsGridAPI = api;
//	                outArgumentsGridReadyDeferred.resolve();
//	            };

//	            defineAPI();
//	        }

//	        function loadVRWorkflow(vrWorkflowId) {
//	            return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowEditorRuntime(vrWorkflowId).then(function (response) {
//	                selectedVRWrkflow = response.Entity;
//	            });
//	        }

//	        function defineAPI() {
//	            var api = {};

//	            api.load = function (payload) {
//	                var promises = [];
//	                var selectedVRWorkflowId;

//	                if (payload != undefined) {
//	                    if (payload.Settings != undefined) {
//	                        selectedVRWorkflowId = payload.Settings.VRWorkflowId;
//	                    }

//	                    if (payload.Context != null)
//	                        $scope.scopeModel.context = payload.Context;

//	                    if (selectedVRWorkflowId != undefined) {
//	                        loadSelectedVRWorkflowPromise = loadVRWorkflow(selectedVRWorkflowId);
//	                        promises.push(loadSelectedVRWorkflowPromise);

//	                    }

//	                    var loadVRWorkflowSelectorPromise = loadVRWorkflowSelector();
//	                    promises.push(loadVRWorkflowSelectorPromise)
//	                }

//	                function loadVRWorkflowSelector() {
//	                    var vrWorkflowSelectorLoadDeferred = UtilsService.createPromiseDeferred();

//	                    vrWorkflowSelectorReadyDeferred.promise.then(function () {

//	                        var vrWorkflowSelectorPayload;
//	                        if (selectedVRWorkflowId != undefined) {
//	                            vrWorkflowSelectorPayload = { selectedIds: selectedVRWorkflowId };
//	                        }
//	                        VRUIUtilsService.callDirectiveLoad(vrWorkflowSelectorAPI, vrWorkflowSelectorPayload, vrWorkflowSelectorLoadDeferred);
//	                    });

//	                    return vrWorkflowSelectorLoadDeferred.promise;
//	                }

//	                function loadInArgumentsGrid() {
//	                    var inArgumentsGridLoadDeferred = UtilsService.createPromiseDeferred();
//	                    loadSelectedVRWorkflowPromise.promise.then(function () {
//	                    });


//	                    return inArgumentsGridLoadDeferred;
//	                };

//	            };

//	            api.getData = function () {
//	                return {
//	                    $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowSubProcessActivity, Vanrise.BusinessProcess.MainExtensions",
//	                    VRWorkflowId: vrWorkflowSelectorAPI.getSelectedIds(),
//	                };
//	            };

//	            if (ctrl.onReady != null)
//	                ctrl.onReady(api);
//	        };
//	    }
//	    return directiveDefinitionObject;
//	}]);