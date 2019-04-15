(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePrepaidSMS.$inject = ['UtilsService', 'VRUIUtilsService','Retail_RA_ScopeEnum'];

    function OnNetOperatorDeclarationServicePrepaidSMS(UtilsService, VRUIUtilsService, Retail_RA_ScopeEnum) {
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
                var ctor = new OnNetOperatorDeclarationServicePrepaidSMSCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/MainExtensions/Templates/OnNetOperatorDeclarationServicePrepaidSMSTemplate.html"
        };

        function OnNetOperatorDeclarationServicePrepaidSMSCtor($scope, ctrl, $attrs) {
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
                    var prepaidSMSEntity;

                    if (payload != undefined) {
                        prepaidSMSEntity = payload.settings;
                        if (prepaidSMSEntity != undefined) {
                            $scope.scopeModel.revenue = prepaidSMSEntity.Revenue;
                            $scope.scopeModel.sms = prepaidSMSEntity.SMS;
                            $scope.scopeModel.revenueExcludingBundles = prepaidSMSEntity.RevenueExcludingBundles;
                            $scope.scopeModel.smsExcludingBundles = prepaidSMSEntity.SMSExcludingBundles;
                            $scope.scopeModel.selectedScope = UtilsService.getItemByVal($scope.scopeModel.scopes, prepaidSMSEntity.Scope, 'value');
                        }
                    }
                    var rootPromiseNode = {
                        promises: promises
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPrepaidSMSOperationDeclarationService,Retail.RA.Business",
                        Revenue: $scope.scopeModel.revenue,
                        SMS: $scope.scopeModel.sms,
                        RevenueExcludingBundles: $scope.scopeModel.revenueExcludingBundles,
                        SMSExcludingBundles: $scope.scopeModel.smsExcludingBundles,
                        Scope: $scope.scopeModel.selectedScope.value
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    
    app.directive('retailRaOnnetoperatordeclarationservicePrepaidsms', OnNetOperatorDeclarationServicePrepaidSMS);

})(app);