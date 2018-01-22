﻿(function (app) {

    'use strict';

    BasicAdvancedFilterRuntimeSettingsDirective.$inject = ['UtilsService'];

    function BasicAdvancedFilterRuntimeSettingsDirective(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BasicAdvancedFilterRuntimeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/BasicAdvancedFilterRuntimeTemplate.html"
        };


        function BasicAdvancedFilterRuntimeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    var promises = [];
                    if (payload != undefined) {

                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (filterObject) {
                 
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeBasicadvanced', BasicAdvancedFilterRuntimeSettingsDirective);

})(app);