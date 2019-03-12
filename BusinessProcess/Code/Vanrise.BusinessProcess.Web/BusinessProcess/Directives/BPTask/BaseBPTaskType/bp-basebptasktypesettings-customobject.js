(function (app) {

    'use strict';

    BPTaskTypeCustomObjectSettings.$inject = ['UtilsService', 'VRUIUtilsService'];

    function BPTaskTypeCustomObjectSettings( UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPTask/BaseBPTaskType/Templates/BaseBPTaskTypeSettingsTemplate.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.BusinessProcess.Business.BaseBPTaskTypeSettingsCustomObject,Vanrise.BusinessProcess.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        } 
    }

    app.directive('bpBasebptasktypesettingsCustomobject', BPTaskTypeCustomObjectSettings);

})(app);
