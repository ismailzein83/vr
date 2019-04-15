(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePostpaidSMS.$inject = ['UtilsService', 'VRUIUtilsService', 'Retail_RA_ScopeEnum'];

    function OnNetOperatorDeclarationServicePostpaidSMS(UtilsService, VRUIUtilsService, Retail_RA_ScopeEnum) {
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
                var ctor = new OnNetOperatorDeclarationServicePostpaidSMSCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/MainExtensions/Templates/OnNetOperatorDeclarationServicePostpaidSMSTemplate.html"
        };

        function OnNetOperatorDeclarationServicePostpaidSMSCtor($scope, ctrl, $attrs) {
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
                    var postpaidSMSEntity;

                    if (payload != undefined) {
                        postpaidSMSEntity = payload.settings;
                        if (postpaidSMSEntity != undefined) {
                            $scope.scopeModel.revenue = postpaidSMSEntity.Revenue;
                            $scope.scopeModel.sms = postpaidSMSEntity.SMS;
                            $scope.scopeModel.revenueExcludingBundles = postpaidSMSEntity.RevenueExcludingBundles;
                            $scope.scopeModel.smsExcludingBundles = postpaidSMSEntity.SMSExcludingBundles;
                            $scope.scopeModel.selectedScope = UtilsService.getItemByVal($scope.scopeModel.scopes, postpaidSMSEntity.Scope, 'value');
                        }
                    }
                    var rootPromiseNode = {
                        promises: promises
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPostpaidSMSOperationDeclarationService,Retail.RA.Business",
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
    
    app.directive('retailRaOnnetoperatordeclarationservicePostpaidsms', OnNetOperatorDeclarationServicePostpaidSMS);

})(app);