﻿'use strict';
app.directive('vrGenericdataDatetimeSelective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new datetimeTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Common/Directives/MainExtensions/GenericDataRecord/Templates/SelectiveDatetimeDirectiveTemplate.html';
            }

        };

        function datetimeTypeCtor(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.MainExtensions.DataRecordFieldDateTimeType,Vanrise.Common.MainExtensions",
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);