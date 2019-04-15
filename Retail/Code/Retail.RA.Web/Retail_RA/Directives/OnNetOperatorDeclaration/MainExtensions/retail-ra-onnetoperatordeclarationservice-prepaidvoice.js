(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePrepaidVoice.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_RA_ScopeEnum'];

    function OnNetOperatorDeclarationServicePrepaidVoice(UtilsService, VRUIUtilsService, Retail_RA_ScopeEnum) {
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

                $scope.scopeModel.scopes = UtilsService.getArrayEnum(Retail_RA_ScopeEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var prepaidVoiceEntity;

                    if (payload != undefined) {
                        prepaidVoiceEntity = payload.settings;
                        if (prepaidVoiceEntity != undefined) {
                            $scope.scopeModel.calls = prepaidVoiceEntity.Calls;
                            $scope.scopeModel.revenue = prepaidVoiceEntity.Revenue;
                            $scope.scopeModel.duration = prepaidVoiceEntity.Duration;
                            $scope.scopeModel.durationExcludingBundles = prepaidVoiceEntity.DurationExcludingBundles;
                            $scope.scopeModel.revenueExcludingBundles = prepaidVoiceEntity.RevenueExcludingBundles;
                            $scope.scopeModel.selectedScope = UtilsService.getItemByVal($scope.scopeModel.scopes, prepaidVoiceEntity.Scope, 'value');
                        }
                    }

                    var rootPromiseNode = {
                        promises: promises
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPrepaidVoiceOperationDeclarationService,Retail.RA.Business",
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
    
    app.directive('retailRaOnnetoperatordeclarationservicePrepaidvoice', OnNetOperatorDeclarationServicePrepaidVoice);

})(app);