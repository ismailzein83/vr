'use strict';

app.directive('retailBeAccountConvertorEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBeAccountConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/AccountConvertorEditor.html"
        };

        function retailBeAccountConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            $scope.scopeModel = {};

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                    }
                }

                api.getData = function () {
                    var data = {
                        $type: "Retail.BusinessEntity.RingoExtensions.AccountConvertor, Retail.BusinessEntity.RingoExtensions",
                        Name: "Account Convertor"
                    };
                    return data;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);