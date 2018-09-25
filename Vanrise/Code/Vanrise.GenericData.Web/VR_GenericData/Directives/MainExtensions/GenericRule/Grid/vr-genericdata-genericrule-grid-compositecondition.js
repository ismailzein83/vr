(function (app) {

    'use strict';

    CompositeConditionGrid.$inject = ['VR_GenericData_GenericRuleAPIService', 'VR_GenericData_GenericRuleDefinitionAPIService', 'VR_GenericData_GenericRule', 'VRUIUtilsService', 'UtilsService', 'VRNotificationService'];

    function CompositeConditionGrid(VR_GenericData_GenericRuleAPIService, VR_GenericData_GenericRuleDefinitionAPIService, VR_GenericData_GenericRule, VRUIUtilsService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var obj = new GenericRuleGrid($scope, ctrl, $attrs);
                obj.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/GenericRule/Grid/Templates/CompositeConditionGridTemplate.html'
        };

        function GenericRuleGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            function initializeController() {
            };
        }
    }
    
    app.directive('vrGenericdataGenericruleGridCompositecondition', CompositeConditionGrid);

})(app);