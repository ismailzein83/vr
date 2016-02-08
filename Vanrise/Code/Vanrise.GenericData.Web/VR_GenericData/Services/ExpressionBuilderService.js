(function (appControllers) {

    'use strict';

    ExpressionBuilderService.$inject = ['VRModalService'];

    function ExpressionBuilderService(VRModalService) {
        return ({
            openExpressionBuilder: openExpressionBuilder,
        });

        function openExpressionBuilder(onSetExpressionBuilder, expressionBuilderValue, context) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSetExpressionBuilder = onSetExpressionBuilder;
            };
            var parameter;
            if (expressionBuilderValue != undefined)
                parameter = {
                    ExpressionBuilderValue: expressionBuilderValue,
                    Context: context
                }
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/ExpressionBuilder/ExpressionBuilderEditor.html', parameter, modalSettings);
        }

    };

    appControllers.service('VR_GenericData_ExpressionBuilderService', ExpressionBuilderService);

})(appControllers);
