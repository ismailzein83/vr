(function (appControllers) {

    "use strict";
    RdbAnalyticDataproviderTableSelector.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_AnalyticConfigurationAPIService'];
    function RdbAnalyticDataproviderTableSelector(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticConfigurationAPIService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',

            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var RdbAnalyticDataproviderTableSelector = new RdbAnalyticDataproviderTableSelector($scope, ctrl, $attrs);
                RdbAnalyticDataproviderTableSelector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: 
        };
        function RdbAnalyticDataproviderTableSelector($scope, ctrl, $attrs) {

        }


        return directiveDefinitionObject;
    }
    
    app.directive("vrRdbAnalyticDataproviderTableSelector", RdbAnalyticDataproviderTableSelector);
})(appControllers);