'use strict';

app.directive('retailBeDistributorSynchronizerEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBeDistributorSynchronizerEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Distributor/Templates/DistributorSynchronizerEditor.html"
        };

        function retailBeDistributorSynchronizerEditorCtor(ctrl, $scope, $attrs) {
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
                        $type: "Retail.BusinessEntity.Business.DistributorSynchronizer, Retail.BusinessEntity.Business",
                        Name: "Distributor Synchronizer"
                    };
                    return data;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);