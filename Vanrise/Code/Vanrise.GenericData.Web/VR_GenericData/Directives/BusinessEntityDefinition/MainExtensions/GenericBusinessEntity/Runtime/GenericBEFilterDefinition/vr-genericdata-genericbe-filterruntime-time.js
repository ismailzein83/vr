(function (app) {

    'use strict';

    TimeFilterRuntimeSettingsDirective.$inject = ['UtilsService'];

    function TimeFilterRuntimeSettingsDirective(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TimeFilterRuntimeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/TimeFilterRuntimeTemplate.html"
        };

      
        function TimeFilterRuntimeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {

                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (filterObject) {
                    filterObject.FromDate = $scope.scopeModel.fromDate;
                    filterObject.ToDate = $scope.scopeModel.toDate;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeTime', TimeFilterRuntimeSettingsDirective);

})(app);