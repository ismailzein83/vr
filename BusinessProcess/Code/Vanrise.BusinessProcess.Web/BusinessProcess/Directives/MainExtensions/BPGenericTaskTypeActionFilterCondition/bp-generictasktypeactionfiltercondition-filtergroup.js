'use strict';

app.directive('bpGenerictasktypeactionfilterconditionFiltergroup', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FilterCondition(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {

                };
            },
            templateUrl: "/Client/Modules/BusinessProcess/Directives/MainExtensions/BPGenericTaskTypeActionFilterCondition/Templates/FilterGroupFilterCondition.html"
        };

        function FilterCondition(ctrl, $scope, attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.FilterGroupBPGenericTaskTypeActionFilterCondition, Vanrise.BusinessProcess.MainExtensions"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);