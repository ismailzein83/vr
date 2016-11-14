(function (app) {

    'use strict';

    AnalyticItemActionEditorDirective.$inject = ['VR_Analytic_AnalyticItemActionService', 'UtilsService'];

    function AnalyticItemActionEditorDirective(VR_Analytic_AnalyticItemActionService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var analyticItemActionEditor = new AnalyticItemActionEditor($scope, ctrl, $attrs);
                analyticItemActionEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticItemAction/Templates/AnalyticItemActionEditorTemplate.html"
        };

        function AnalyticItemActionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var counter = 0;
            var context;
            function initializeController() {
                ctrl.itemActions = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addItemAction = function () {
                    var onItemActionAdded = function (ItemActionObj) {
                        ctrl.itemActions.push(ItemActionObj);
                    };
                    VR_Analytic_AnalyticItemActionService.addItemAction(onItemActionAdded);
                };

                ctrl.removeItemAction = function (itemAction) {
                    ctrl.itemActions.splice(ctrl.itemActions.indexOf(itemAction), 1);
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        ctrl.itemActions.length = 0;
                        if (payload.itemActions && payload.itemActions.length > 0) {
                            for (var y = 0; y < payload.itemActions.length; y++) {
                                var currentItemAction = payload.itemActions[y];
                                ctrl.itemActions.push(currentItemAction);
                            }
                        }
                    }
                };

                api.getData = function () {
                    var itemActions = [];
                    for (var i = 0; i < ctrl.itemActions.length ; i++) {
                        var itemAction = ctrl.itemActions[i];
                        itemActions.push(itemAction);
                    }
                    return itemActions;
                };

                return api;
            }

            function defineMenuActions() {
                ctrl.itemActionsGridMenuActions = [{
                    name: 'Edit',
                    clicked: editItemAction
                }];
            }

            function editItemAction(itemAction) {
                var onItemActionUpdated = function (itemActionObj) {
                    ctrl.itemActions[ctrl.itemActions.indexOf(itemAction)] = itemActionObj;
                };
                VR_Analytic_AnalyticItemActionService.editItemAction(itemAction, onItemActionUpdated);
            }
        }
    }

    app.directive('vrAnalyticAnalyticitemactionEditor', AnalyticItemActionEditorDirective);

})(app);