(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePrepaidVoice.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OnNetOperatorDeclarationServicePrepaidVoice(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OnNetOperatorDeclarationServicePrepaidVoiceCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/MainExtensions/Templates/OnNetOperatorDeclarationServicePrepaidVoiceTemplate.html"
        };

        function OnNetOperatorDeclarationServicePrepaidVoiceCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var prepaidVoiceEntity;

                    if (payload != undefined) {
                        prepaidVoiceEntity = payload.settings;
                        $scope.scopeModel.calls = prepaidVoiceEntity.Calls;
                        $scope.scopeModel.revenue = prepaidVoiceEntity.Revenue;
                        $scope.scopeModel.duration = prepaidVoiceEntity.Duration;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPrepaidVoiceOperationDeclarationService,Retail.RA.Business",
                        Calls: $scope.scopeModel.calls,
                        Revenue: $scope.scopeModel.revenue,
                        Duration: $scope.scopeModel.duration
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    
    app.directive('retailRaOnnetoperatordeclarationservicePrepaidvoice', OnNetOperatorDeclarationServicePrepaidVoice);

})(app);