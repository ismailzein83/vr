﻿'use strict';
app.directive('vrWhsCdrprocessingNumberSelective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new numberTypeCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_CDRProcessing/Directives/CDRFields/CDRFieldTypeExtensions/Templates/SelectiveNumberDirectiveTemplate.html';
            }

        };

        function numberTypeCtor(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.typeValue = payload.Value;
                    }
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.CDRProcessing.Entities.CDRFieldNumberType, TOne.WhS.CDRProcessing.Entities",
                        Value: ctrl.typeValue
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);