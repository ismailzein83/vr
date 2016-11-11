﻿(function (app) {

    'use strict';

    CustomerObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function CustomerObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var customerObjectType = new CustomerDierctiveObjectType($scope, ctrl, $attrs);
                customerObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl:  '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/CustomerObjectTypeTemplate.html'


        };
        function CustomerDierctiveObjectType($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CustomerObjectType, TOne.WhS.BusinessEntity.MainExtensions",
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsBeCustomerobjecttype', CustomerObjectType);

})(app);