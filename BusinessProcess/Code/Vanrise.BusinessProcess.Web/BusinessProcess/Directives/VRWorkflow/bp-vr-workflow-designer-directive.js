'use strict';

app.directive('businessprocessVrWorkflowDesignerDirective', ['UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_VRWorkflowAPIService', 'VRDragdropService', 'VRNotificationService',
	function (UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService, BusinessProcess_VRWorkflowAPIService, VRDragdropService, VRNotificationService) {

	    var directiveDefinitionObject = {
	        restrict: 'E',
	        scope: {
	            onReady: '='
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;
	            var ctor = new workflowDesigner(ctrl, $scope, $attrs);
	            ctor.initializeController();
	        },
	        controllerAs: 'ctrl',
	        bindToController: true,
	        compile: function (element, attrs) {

	        },
	        templateUrl: '/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowDesignerDirectiveTemplate.html'
	    };

	    function workflowDesigner(ctrl, $scope, $attrs) {

	        var workflowContainerAPI;
	        var workflowContainerReadyPromiseDeferred = UtilsService.createPromiseDeferred();
	        var rootActivity;
	        var vrWorkflowId;
	        var getWorkflowArguments;
	        var addToList;
	        var removeFromList;
	        var reserveVariableName;
	        var reserveVariableNames;
	        var isVariableNameReserved;
	        var eraseVariableName;
	        var doesActivityhaveErrors;

	        this.initializeController = initializeController;
	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.datasource = [];
	            $scope.scopeModel.activityConfigs = [];

	            $scope.scopeModel.dragdropGroupCorrelation = VRDragdropService.createCorrelationGroup();
	            $scope.scopeModel.dragdropsetting = {
	                groupCorrelation: $scope.scopeModel.dragdropGroupCorrelation,
	                canReceive: true,
	                canSend: true,
	                copyOnSend: true,
	                onItemReceived: function (itemAdded, dataSource, sourceList, itemAddedContext) {
	                    var vRWorkflowActivity = {};
	                    if (itemAdded.directiveAPI != null) {
	                        vRWorkflowActivity.Settings = itemAdded.directiveAPI.getData().Settings;
	                    }
	                    else {
	                        vRWorkflowActivity.Settings = {
	                            Editor: (itemAdded.Editor) ? itemAdded.Editor : itemAdded.Settings.Editor,
	                            Title: (itemAdded.Title) ? itemAdded.Title : itemAdded.Settings.Title,
	                            IsNew: true
	                        };
	                    }
	                    vRWorkflowActivity.VRWorkflowActivityId = UtilsService.guid();

	                    vRWorkflowActivity.onDirectiveReady = function (api) {
	                        if (vRWorkflowActivity.directiveAPI != null)
	                            return;
	                        vRWorkflowActivity.directiveAPI = api;
	                        var setLoader = function (value) { $scope.x = value; };
	                        var context = (itemAddedContext != undefined) ? itemAddedContext : {
	                            vrWorkflowId: vrWorkflowId,
	                            getWorkflowArguments: getWorkflowArguments,
	                            addToList: addToList,
	                            removeFromList: removeFromList,
	                            reserveVariableName: reserveVariableName,
	                            reserveVariableNames: reserveVariableNames,
	                            isVariableNameReserved: isVariableNameReserved,
	                            eraseVariableName: eraseVariableName,
	                            doesActivityhaveErrors: doesActivityhaveErrors
	                        };
	                        var payload = {
	                            Context: context,
	                            VRWorkflowActivityId: vRWorkflowActivity.VRWorkflowActivityId,
	                            Settings: vRWorkflowActivity.Settings
	                        };
	                        VRUIUtilsService.callDirectiveLoad(vRWorkflowActivity.directiveAPI, payload);
	                    };

	                    return vRWorkflowActivity;
	                },

	                enableSorting: true
	            };

	            $scope.scopeModel.onWorkflowContainerReady = function (api) {
	                workflowContainerAPI = api;
	                workflowContainerReadyPromiseDeferred.resolve();
	            };

	            defineAPI();
	        }

	        function defineAPI() {
	            var api = {};

	            api.load = function (payload) {

	                if (payload != undefined) {
	                    rootActivity = payload.rootActivity;
	                    vrWorkflowId = payload.vrWorkflowId;
	                    getWorkflowArguments = payload.getWorkflowArguments;
	                    addToList = payload.addToList;
	                    removeFromList = payload.removeFromList;
	                    reserveVariableName = payload.reserveVariableName;
	                    reserveVariableNames = payload.reserveVariableNames;
	                    isVariableNameReserved = payload.isVariableNameReserved;
	                    eraseVariableName = payload.eraseVariableName;
	                    doesActivityhaveErrors = payload.doesActivityhaveErrors;
	                }
	                return loadAllControls();

	                function loadAllControls() {
	                    return UtilsService.waitMultipleAsyncOperations([loadWorkflowContainer, loadWorkflowActivityExtensionConfigs])
							.catch(function (error) {
							    VRNotificationService.notifyExceptionWithClose(error, $scope);
							});
	                }

	                function getChildContext() {
	                    var childContext = {};
	                    childContext.vrWorkflowId = vrWorkflowId;
	                    childContext.getWorkflowArguments = getWorkflowArguments;
	                    childContext.reserveVariableName = reserveVariableName;
	                    childContext.reserveVariableNames = reserveVariableNames;
	                    childContext.eraseVariableName = eraseVariableName;
	                    childContext.isVariableNameReserved = isVariableNameReserved;
	                    childContext.addToList = addToList;
	                    childContext.removeFromList = removeFromList;
	                    childContext.doesActivityhaveErrors = doesActivityhaveErrors;
	                    return childContext;
	                }

	                function loadWorkflowContainer() {
	                    var workflowContainerLoadDeferred = UtilsService.createPromiseDeferred();
	                    if (rootActivity == undefined) {
	                        rootActivity = {
	                            Settings: {
	                                Editor: "businessprocess-vr-workflowactivity-sequence",
	                                Title: "Sequence"
	                            },
	                            VRWorkflowActivityId: UtilsService.guid()
	                        };
	                    }
	                    workflowContainerReadyPromiseDeferred.promise.then(function () {
	                        var payload = {
	                            getChildContext: getChildContext,
	                            vRWorkflowActivity: rootActivity
	                        };
	                        VRUIUtilsService.callDirectiveLoad(workflowContainerAPI, payload, workflowContainerLoadDeferred);
	                    });
	                    return workflowContainerLoadDeferred.promise;
	                }

	                function loadWorkflowActivityExtensionConfigs() {
	                    return BusinessProcess_VRWorkflowAPIService.GetVRWorkflowActivityExtensionConfigs().then(function (response) {
	                        if (response != null) {
	                            for (var i = 0; i < response.length; i++) {
	                                $scope.scopeModel.activityConfigs.push(response[i]);
	                            }
	                        }
	                    });
	                }
	            };

	            api.getData = function () {
	                if (workflowContainerAPI != null)
	                    return workflowContainerAPI.getData();
	            };

	            api.isActivityValid = function () {
	                if (workflowContainerAPI != null && workflowContainerAPI.isActivityValid != undefined)
	                    return workflowContainerAPI.isActivityValid();

	                return true;
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }
	    }
	    return directiveDefinitionObject;
	}]);