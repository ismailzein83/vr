(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePostpaidvoice.$inject = ['UtilsService', 'VRUIUtilsService','Retail_RA_ScopeEnum'];

    function OnNetOperatorDeclarationServicePostpaidvoice(UtilsService, VRUIUtilsService, Retail_RA_ScopeEnum) {
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

                $scope.scopeModel.scopes = UtilsService.getArrayEnum(Retail_RA_ScopeEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var postpaidVoiceEntity;

                    if (payload != undefined) {
                        postpaidVoiceEntity = payload.settings;
                        if (postpaidVoiceEntity != undefined) {
                            $scope.scopeModel.calls = postpaidVoiceEntity.Calls;
                            $scope.scopeModel.revenue = postpaidVoiceEntity.Revenue;
                            $scope.scopeModel.duration = postpaidVoiceEntity.Duration;
                            $scope.scopeModel.durationExcludingBundles = postpaidVoiceEntity.DurationExcludingBundles;
                            $scope.scopeModel.revenueExcludingBundles = postpaidVoiceEntity.RevenueExcludingBundles;
                            $scope.scopeModel.selectedScope = UtilsService.getItemByVal($scope.scopeModel.scopes, postpaidVoiceEntity.Scope, 'value');
                        }
                    }
                    var rootPromiseNode = {
                        promises: promises
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPostpaidVoiceOperationDeclarationService,Retail.RA.Business",
                        Calls: $scope.scopeModel.calls,
                        Revenue: $scope.scopeModel.revenue,
                        Duration: $scope.scopeModel.duration,
                        DurationExcludingBundles: $scope.scopeModel.durationExcludingBundles,
                        RevenueExcludingBundles: $scope.scopeModel.revenueExcludingBundles,
                        Scope: $scope.scopeModel.selectedScope.value
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