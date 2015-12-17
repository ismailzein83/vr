'use strict';
app.directive('vrWhsCdrprocessingDatetimeSelective', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
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
                return '/Client/Modules/WhS_CDRProcessing/Directives/CDRFields/CDRFieldTypeExtensions/Templates/SelectiveDatetimeDirectiveTemplate.html';
            }

        };

        function datetimeTypeCtor(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    //if (payload != undefined) {
                    //    ctrl.date = payload.Date;
                    //}
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.CDRProcessing.Entities.CDRFieldDateTimeType, TOne.WhS.CDRProcessing.Entities",
                      //  Date: ctrl.date
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);