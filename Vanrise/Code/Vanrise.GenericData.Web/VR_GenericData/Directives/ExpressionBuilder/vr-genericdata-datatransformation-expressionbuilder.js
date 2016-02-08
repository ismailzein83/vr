'use strict';
app.directive('vrGenericdataExpressionbuilder', ['VR_GenericData_ExpressionBuilderService', 'UtilsService', '$compile', 'VRUIUtilsService', function (VR_GenericData_ExpressionBuilderService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            label: '@',
            hidelabel:'@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new recordFieldCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/VR_GenericData/Directives/ExpressionBuilder/Templates/ExpressionBuilderTemplate.html"
    };

 

    function recordFieldCtor(ctrl, $scope, $attrs) {
        var context;
        function initializeController() {

         
            $scope.openExpressionBuilder = function()
            {
                var onSetExpressionBuilder = function(expressionBuilderValue)
                {
                    $scope.expressionBuilderValue = expressionBuilderValue;
                }
                VR_GenericData_ExpressionBuilderService.openExpressionBuilder(onSetExpressionBuilder,context, $scope.expressionBuilderValue)
            }
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return $scope.expressionBuilderValue;
            }

            api.load = function (payload) {
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.selectedRecords != undefined)
                    {
                        $scope.expressionBuilderValue = payload.selectedRecords;
                    }
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

