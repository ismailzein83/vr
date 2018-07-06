(function (app) {

    'use strict';

    AftersavehandlerVouchercardsgenerationDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function AftersavehandlerVouchercardsgenerationDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AftersavehandlerVouchercardsgenerationCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_Voucher/Elements/VoucherCardsGeneration/Directives/Templates/AftersavehandlerVouchercardsgenerationTemplate.html'
        };

        function AftersavehandlerVouchercardsgenerationCtor($scope, ctrl) {
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
                    return {
                        $type: "Vanrise.Voucher.Business.AfterSaveVoucherCardsGenerationHandler, Vanrise.Voucher.Business"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrVoucherAftersavehandlerVouchercardsgeneration', AftersavehandlerVouchercardsgenerationDirective);

})(app);