(function (app) {

    'use strict';

    VisibilityViewManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_VisibilityAccountDefinitionService'];

    function VisibilityViewManagementDirective(UtilsService, VRNotificationService, Retail_BE_VisibilityAccountDefinitionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityViewsCtor($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionViews/Templates/VisibilityViewManagementTemplate.html'
        };

        function VisibilityViewsCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var columnDefinitions;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.views = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onAddView = function () {
                    var onViewAdded = function (addedView) {
                        $scope.scopeModel.views.push({ Entity: addedView });
                    };

                    Retail_BE_VisibilityAccountDefinitionService.addVisibilityView(columnDefinitions, onViewAdded);
                };
                $scope.scopeModel.onDeleteView = function (view) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal($scope.scopeModel.views, view.Entity.ViewTitle, 'Entity.ViewTitle');
                            $scope.scopeModel.views.splice(index, 1);
                        }
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var views;

                    console.log(payload);

                    if (payload != undefined) {
                        views = payload.views;
                        columnDefinitions = payload.columnDefinitions;
                    }

                    //Loading Views Grid
                    if (views != undefined) {
                        for (var index = 0 ; index < views.length; index++) {
                            if (index != "$type") {
                                var view = views[index];
                                extendViewObj(view);
                                $scope.scopeModel.views.push({ Entity: view });
                            }
                        }
                    }

                    function extendViewObj(view) {
                        if (columnDefinitions == undefined || view.Name != undefined)
                            return;

                        for (var index = 0; index < columnDefinitions.length; index++) {
                            var currentColumnDefinition = columnDefinitions[index];
                            if (currentColumnDefinition.FieldName == view.FieldName)
                                view.Name = currentColumnDefinition.Name;
                        }
                    }
                };

                api.getData = function () {

                    var views;
                    if ($scope.scopeModel.views.length > 0) {
                        views = [];
                        for (var i = 0; i < $scope.scopeModel.views.length; i++) {
                            var view = $scope.scopeModel.views[i].Entity;
                            views.push(view);
                        }
                    }

                    return views;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = [{
                    name: 'Edit',
                    clicked: editView
                }];
            }
            function editView(view) {
                var onViewUpdated = function (updatedView) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.views, view.Entity.ViewTitle, 'Entity.ViewTitle');
                    $scope.scopeModel.views[index] = { Entity: updatedView };
                };

                Retail_BE_VisibilityAccountDefinitionService.editVisibilityView(view.Entity, columnDefinitions, onViewUpdated);
            }


        }
    }

    app.directive('retailBeVisibilitygridcolumnManagement', VisibilityViewManagementDirective);

})(app);