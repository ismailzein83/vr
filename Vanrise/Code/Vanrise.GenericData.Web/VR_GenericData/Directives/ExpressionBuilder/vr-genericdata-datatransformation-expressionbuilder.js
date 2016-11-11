'use strict';
app.directive('vrGenericdataExpressionbuilder', ['VR_GenericData_ExpressionBuilderService', 'UtilsService', '$compile', 'VRUIUtilsService', function (VR_GenericData_ExpressionBuilderService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            label: '@',
            hidelabel: '@',
            isrequired:'='
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
            };
        },
        template: function (element, attrs) {
            return getTamplate(attrs);
            }
    };

 
    function getTamplate(attrs)
    {
        var withemptyline = 'withemptyline';
        if (attrs.hidelabel != undefined)
            withemptyline = '';
        var template =
            '<vr-row removeline>'
             + '<vr-columns width="3/4row">'
             + '<vr-label ng-if="ctrl.hidelabel ==undefined">{{ctrl.label}}</vr-label>'
             + '<vr-textbox value="expressionBuilderValue" isrequired="ctrl.isrequired"></vr-textbox>'
             + '</vr-columns>'
             + '<vr-columns width="1/4row" ' +withemptyline+ ' > '
             + '   <vr-button type="Edit" data-onclick="openExpressionBuilder" standalone></vr-button>'
             + '</vr-columns>'
            + '</vr-row>';
        return template;
           
    }
    function recordFieldCtor(ctrl, $scope, $attrs) {
        var context;
        function initializeController() {

         
            $scope.openExpressionBuilder = function () {
                var onSetExpressionBuilder = function (expressionBuilderValue) {
                    $scope.expressionBuilderValue = expressionBuilderValue;
                };
                VR_GenericData_ExpressionBuilderService.openExpressionBuilder(onSetExpressionBuilder, context, $scope.expressionBuilderValue)
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return $scope.expressionBuilderValue;
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    context = payload.context;
                    if (payload.selectedRecords != undefined) {
                        $scope.expressionBuilderValue = payload.selectedRecords;
                    }
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

