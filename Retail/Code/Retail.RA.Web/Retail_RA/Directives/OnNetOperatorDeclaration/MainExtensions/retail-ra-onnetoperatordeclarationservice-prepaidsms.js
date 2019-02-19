(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePrepaidSMS.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OnNetOperatorDeclarationServicePrepaidSMS(UtilsService, VRUIUtilsService) {
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
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var prepaidSMSEntity;

                    if (payload != undefined) {
                        prepaidSMSEntity = payload.settings;
                        $scope.scopeModel.revenue = prepaidSMSEntity.Revenue;
                        $scope.scopeModel.sms = prepaidSMSEntity.SMS;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPrepaidSMSOperationDeclarationService,Retail.RA.Business",
                        Revenue: $scope.scopeModel.revenue,
                        SMS: $scope.scopeModel.sms
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