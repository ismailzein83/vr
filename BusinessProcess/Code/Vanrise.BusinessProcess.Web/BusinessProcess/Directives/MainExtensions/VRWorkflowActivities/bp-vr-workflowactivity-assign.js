'use strict';

app.directive('businessprocessVrWorkflowactivityAssign', ['UtilsService', 'VRUIUtilsService', 'VRCommon_FieldTypesService',
    function (UtilsService, VRUIUtilsService, VRCommon_FieldTypesService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new workflowAssign(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowAssignTemplate.html'
        };

        function workflowAssign(ctrl, $scope, $attrs) {
            var assignListItems = [];
            var context;
            var textFieldType = VRCommon_FieldTypesService.getTextFieldType();
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = { items: [] };
                $scope.scopeModel.isVRWorkflowActivityDisabled = false;

                $scope.scopeModel.addAssign = function () {
                    prepareItemToAdd();
                };

                $scope.scopeModel.removeAssign = function (item) {
                    $scope.scopeModel.items.splice($scope.scopeModel.items.indexOf(item), 1);
                    assignListItems.splice(assignListItems.indexOf(item), 1);
                };

                defineAPI();
            }
            function prepareItemToAdd() {
                var dataItem = {};
                dataItem.onToExpressionBuilderDirectiveReady = function (api) {
                    dataItem.toExpressionBuilderDirectiveAPI = api;
                    var setLoader = function (value) { dataItem.isToExpressionBuilderDirectiveLoading = value; };
                    var payload = {
                        context: context,
                        fieldType: textFieldType
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.toExpressionBuilderDirectiveAPI, payload, setLoader);
                };
                dataItem.onValueExpressionBuilderDirectiveReady = function (api) {
                    dataItem.valueExpressionBuilderDirectiveAPI = api;
                    var setLoader = function (value) { dataItem.isValueExpressionBuilderDirectiveLoading = value; };
                    var payload = {
                        context: context,
                        fieldType: textFieldType
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.valueExpressionBuilderDirectiveAPI, payload, setLoader);
                };
                $scope.scopeModel.items.push(dataItem);
                assignListItems.push(dataItem);
            }
            function perpareItem(currentAssignObject) {

                var dataItem = {};

                dataItem.onToExpressionBuilderDirectiveReady = function (api) {
                    dataItem.toExpressionBuilderDirectiveAPI = api;
                    currentAssignObject.toReadyPromiseDeferred.resolve();
                };
                currentAssignObject.toReadyPromiseDeferred.promise.then(function () {

                    var payload = {
                        context: context,
                        value: currentAssignObject.payload.To,
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.toExpressionBuilderDirectiveAPI, payload, currentAssignObject.toLoadPromiseDeferred);
                });
                dataItem.onValueExpressionBuilderDirectiveReady = function (api) {
                    dataItem.valueExpressionBuilderDirectiveAPI = api;
                    currentAssignObject.valueReadyPromiseDeferred.resolve();
                };
                currentAssignObject.valueReadyPromiseDeferred.promise.then(function () {
                    var payload = {
                        context: context,
                        value: currentAssignObject.payload.Value,
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.valueExpressionBuilderDirectiveAPI, payload, currentAssignObject.valueLoadPromiseDeferred);
                });
                $scope.scopeModel.items.push(dataItem);
                assignListItems.push(dataItem);
            }
			function defineAPI() {
				var api = {};
				api.load = function (payload) {
					var promises = [];
					if (payload != undefined) {
                        if (payload.Context != null)
                            context = payload.Context; 
                        if (payload.Settings != undefined) {
                            $scope.scopeModel.isVRWorkflowActivityDisabled = payload.Settings.IsDisabled;

                            if (payload.Settings.Items != null && payload.Settings.Items.length > 0) {
                                for (var x = 0; x < payload.Settings.Items.length; x++) {
                                    var currentAssign = payload.Settings.Items[x];
                                    var currentAssignObject = {
                                        payload: currentAssign,
                                        toReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        toLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        valueReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        valueLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    perpareItem(currentAssignObject);
                                }
                            }
                            else {
                                prepareItemToAdd();
                            }
                        }
					}
					return UtilsService.waitMultiplePromises(promises);
				};

                api.getData = function () {
                    var items = [];

                    for (var x = 0; x < assignListItems.length; x++) {
                        var currentAssign = assignListItems[x];
                        items.push(
                            {
                                To: currentAssign.toExpressionBuilderDirectiveAPI != undefined ? currentAssign.toExpressionBuilderDirectiveAPI.getData() : undefined,
                                Value: currentAssign.valueExpressionBuilderDirectiveAPI != undefined ? currentAssign.valueExpressionBuilderDirectiveAPI.getData() : undefined
                            });
                    }

                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.VRWorkflowAssignActivity, Vanrise.BusinessProcess.MainExtensions",
                        Items: items
                    };
                };

				api.changeActivityStatus = function (isVRWorkflowActivityDisabled) {
					$scope.scopeModel.isVRWorkflowActivityDisabled = isVRWorkflowActivityDisabled;
				};

				api.getActivityStatus = function () {
					return $scope.scopeModel.isVRWorkflowActivityDisabled;
				};

				api.isActivityValid = function () {
					if ($scope.scopeModel.items == undefined || $scope.scopeModel.items.length == 0)
						return false;

                    for (var x = 0; x < $scope.scopeModel.items.length; x++) {
                        var currentAssign = $scope.scopeModel.items[x]; 
                        if (currentAssign.toExpressionBuilderDirectiveAPI == undefined || currentAssign.toExpressionBuilderDirectiveAPI.getData() == undefined || currentAssign.valueExpressionBuilderDirectiveAPI == undefined || currentAssign.valueExpressionBuilderDirectiveAPI.getData() == undefined)
							return false;
					}

					return true;
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
		return directiveDefinitionObject;
	}]);