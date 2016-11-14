(function (app) {

    'use strict';

    AnalyticItemconfigJoinEditor.$inject = ['UtilsService', 'VRUIUtilsService'];

    function AnalyticItemconfigJoinEditor(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var itemconfigJoinEditor = new ItemconfigJoinEditor(ctrl, $scope, $attrs);
                itemconfigJoinEditor.initializeController();
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
                return '/Client/Modules/Analytic/Directives/AnalyticItemConfig/Templates/JoinEditorTemplate.html';
            }
        };

        function ItemconfigJoinEditor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
           
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var tableId;
                    var promises = [];
                    var configEntity;
                    if (payload != undefined) {
                        tableId = payload.tableId;
                        configEntity = payload.ConfigEntity;
                        if (configEntity != undefined) {
                            $scope.scopeModel.joinStatement = configEntity.JoinStatement;
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var join = {
                        $type: "Vanrise.Analytic.Entities.AnalyticJoinConfig ,Vanrise.Analytic.Entities",
                        JoinStatement: $scope.scopeModel.joinStatement
                    };
                    return join;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticItemconfigJoinEditor', AnalyticItemconfigJoinEditor);

})(app);
