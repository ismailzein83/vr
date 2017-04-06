'use strict';

app.directive('retailZajilDidConvertorEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailZajilDidConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Zajil/Directives/MainExtensions/Payment/Templates/DIDConvertorEditor.html"
        };

        function retailZajilDidConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            $scope.scopeModel = {};

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        $scope.scopeModel.bedColumn = payload.BEDColumn;
                        $scope.scopeModel.accountColumn = payload.SourceAccountIdColumn;
                        $scope.scopeModel.sourceIdColumn = payload.SourceIdColumn;
                    }

                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.Zajil.MainExtensions.Convertors.DIDConvertor, Retail.Zajil.MainExtensions",
                        Name: "Zajil DID Convertor",
                        SourceAccountIdColumn: $scope.scopeModel.accountColumn,
                        SourceIdColumn: $scope.scopeModel.sourceIdColumn,
                        BEDColumn: $scope.scopeModel.bedColumn
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);