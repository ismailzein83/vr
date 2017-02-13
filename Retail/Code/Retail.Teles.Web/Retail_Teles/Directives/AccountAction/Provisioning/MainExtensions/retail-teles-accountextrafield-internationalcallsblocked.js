'use strict';

app.directive('retailTelesAccountextrafieldInternationalcallsblocked', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InternationalCallsBlockedExtraField($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/MainExtensions/Templates/InternationalCallsBlockedExtraFieldTemplate.html'
        };

        function InternationalCallsBlockedExtraField($scope, ctrl, $attrs) {
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
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.Teles.Business.InternationalCallsBlockedFieldDefinition, Retail.Teles.Business',
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);