(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePostpaidvoice.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OnNetOperatorDeclarationServicePostpaidvoice(UtilsService, VRUIUtilsService) {
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
                var ctor = new OnNetOperatorDeclarationServicePostpaidvoiceCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/MainExtensions/Templates/OnNetOperatorDeclarationServicePostpaidVoiceTemplate.html"
        };

        function OnNetOperatorDeclarationServicePostpaidvoiceCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var postpaidVoiceEntity;

                    if (payload != undefined) {
                        postpaidVoiceEntity = payload.settings;
                        $scope.scopeModel.calls = postpaidVoiceEntity.Calls;
                        $scope.scopeModel.revenue = postpaidVoiceEntity.Revenue;
                        $scope.scopeModel.duration = postpaidVoiceEntity.Duration;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPostpaidVoiceOperationDeclarationService,Retail.RA.Business",
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
    
    app.directive('retailRaOnnetoperatordeclarationservicePostpaidvoice', OnNetOperatorDeclarationServicePostpaidvoice);

})(app);