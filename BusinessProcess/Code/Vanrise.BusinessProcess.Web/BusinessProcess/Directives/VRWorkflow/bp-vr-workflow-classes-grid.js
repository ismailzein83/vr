'use strict';

app.directive('businessprocessVrWorkflowClassesGrid', ['BusinessProcess_VRWorkflowService', 'UtilsService', 'VRUIUtilsService',
	function (BusinessProcess_VRWorkflowService, UtilsService, VRUIUtilsService) {

	    var directiveDefinitionObject = {
	        restrict: 'E',
	        scope: {
	            onReady: '=',
	        },
	        controller: function ($scope, $element, $attrs) {
	            var ctrl = this;
	            var ctor = new VrWorkflowClassesGridDirectiveCtor(ctrl, $scope, $attrs);
	            ctor.initializeController();
	        },
	        controllerAs: 'ctrl',
	        bindToController: true,
	        compile: function (element, attrs) {
	            return {
	                pre: function ($scope, iElem, iAttrs, ctrl) {

	                }
	            };
	        },
	        templateUrl: "/Client/Modules/BusinessProcess/Directives/VRWorkflow/Templates/VRWorkflowClassesGridTemplate.html"
	    };

	    function VrWorkflowClassesGridDirectiveCtor(ctrl, $scope, attrs) {
	        this.initializeController = initializeController;

	        var gridClassItem;

	        var gridAPI;

	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.datasource = [];

	            $scope.scopeModel.onGridReady = function (api) {
	                gridAPI = api;
	            };

	            $scope.scopeModel.addVRWorkflowClass = function () {
	                var onVRWorkflowClassAdded = function (addedClass) {
	                    $scope.scopeModel.datasource.push({ Entity: addedClass });
	                };

	                var vrWorkflowClassNamespaces = [];
	                angular.forEach($scope.scopeModel.datasource, function (val) {
	                    vrWorkflowClassNamespaces.push(val.Entity.Namespace);
	                });
	                BusinessProcess_VRWorkflowService.addVRWorkflowClass(onVRWorkflowClassAdded, vrWorkflowClassNamespaces);
	            };

	            $scope.scopeModel.removeVRWorkflowClass = function (dataItem) {
	                var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity.VRWorkflowClassId, 'Entity.VRWorkflowClassId');
	                $scope.scopeModel.datasource.splice(index, 1);
	            };

	            defineMenuActions();
	            defineAPI();
	        }

	        function defineAPI() {
	            var api = {};

	            api.load = function (payload) {
	                var promises = [];

	                var vrWorkflowClasses;

	                if (payload != undefined) {
	                    vrWorkflowClasses = payload.vrWorkflowClasses;
	                }

	                if (vrWorkflowClasses != undefined) {
	                    for (var i = 0; i < vrWorkflowClasses.length; i++) {
	                        gridClassItem = vrWorkflowClasses[i];
	                        $scope.scopeModel.datasource.push({ Entity: gridClassItem });
	                    }
	                }

	                return UtilsService.waitMultiplePromises(promises);
	            };

	            api.getData = function () {
	                var vrWorkflowClasses;
	                if ($scope.scopeModel.datasource != undefined) {
	                    vrWorkflowClasses = [];
	                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
	                        vrWorkflowClasses.push($scope.scopeModel.datasource[i].Entity);
	                    }
	                }
	                return vrWorkflowClasses;
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }

	        function defineMenuActions() {
	            var defaultMenuActions = [{
	                name: "Edit",
	                clicked: editVRWorkflowClass
	            }];

	            $scope.scopeModel.gridMenuActions = function (dataItem) {
	                return defaultMenuActions;
	            };
	        }

	        function editVRWorkflowClass(classObj) {
	            var onVRWorkflowClassUpdated = function (updatedClass) {
	                var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, classObj.VRWorkflowClassId, 'Entity.VRWorkflowClassId');
	                $scope.scopeModel.datasource[index] = { Entity: updatedClass };
	            };

	            var vrWorkflowClassNamespaces = [];
	            angular.forEach($scope.scopeModel.datasource, function (val) {
	                vrWorkflowClassNamespaces.push(val.Entity.Namespace);
	            });

	            BusinessProcess_VRWorkflowService.editVRWorkflowClass(onVRWorkflowClassUpdated, classObj.Entity, vrWorkflowClassNamespaces);
	        }

	    }

	    return directiveDefinitionObject;
	}]);