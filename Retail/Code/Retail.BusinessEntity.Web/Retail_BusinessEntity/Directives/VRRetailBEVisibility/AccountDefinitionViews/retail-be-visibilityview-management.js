'use strict';

app.directive('retailBeVisibilityviewManagement', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityView(ctrl, $scope);
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
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionViews/Templates/VisibilityViewManagementTemplate.html';
            }
        };

        function VisibilityView(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.viewDefinitions = [];
                $scope.scopeModel.selectedViewDefinitions = [];
                $scope.scopeModel.views = [];

                $scope.scopeModel.onSelectViewDefinition = function (selectedItem) {

                    $scope.scopeModel.views.push({
                        AccountViewDefinitionId: selectedItem.AccountViewDefinitionId,
                        Name: selectedItem.Name
                    });
                };
                $scope.scopeModel.onDeselectViewDefinition = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.views, deselectedItem.Name, 'Name');
                    $scope.scopeModel.views.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedViewDefinitions, deletedItem.Name, 'Name');
                    $scope.scopeModel.selectedViewDefinitions.splice(index, 1);
                    $scope.scopeModel.onDeselectViewDefinition(deletedItem);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var views;
                    var viewDefinitions;

                    if (payload != undefined) {
                        views = payload.views;
                        viewDefinitions = payload.viewDefinitions;
                    }

                    //Loading Selector
                    if (viewDefinitions != undefined) {
                        for (var i = 0; i < viewDefinitions.length; i++) {
                            $scope.scopeModel.viewDefinitions.push(viewDefinitions[i]);
                        }
                        if (views != undefined) {
                            for (var i = 0; i < views.length; i++) {
                                var currentView = views[i];
                                for (var j = 0; j < viewDefinitions.length; j++) {
                                    var currentViewDefinition = viewDefinitions[j];
                                    if (currentViewDefinition.AccountViewDefinitionId == currentView.ViewId)
                                        $scope.scopeModel.selectedViewDefinitions.push(currentViewDefinition);
                                }
                            }
                        }
                    }

                    //Loading Grid
                    if ($scope.scopeModel.selectedViewDefinitions != undefined) {
                        for (var i = 0; i < $scope.scopeModel.selectedViewDefinitions.length; i++) {
                            var viewDefinition = $scope.scopeModel.selectedViewDefinitions[i];

                            $scope.scopeModel.views.push({
                                AccountViewDefinitionId: viewDefinition.AccountViewDefinitionId,
                                Name: viewDefinition.Name
                            });
                        }
                    }
                };

                api.getData = function () {

                    var _views;
                    if ($scope.scopeModel.views.length > 0) {
                        _views = [];
                        for (var i = 0; i < $scope.scopeModel.views.length; i++) {
                            var currentView = $scope.scopeModel.views[i];
                            _views.push({
                                ViewId: currentView.AccountViewDefinitionId
                            });
                        }
                    }
                    return _views;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);