(function (app) {

    'use strict';

    VRTaxDefinitionDirective.$inject = ['UtilsService'];

    function VRTaxDefinitionDirective(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VRTaxDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/VRTax/VRTaxDefinition/Templates/TaxDefinitionTemplate.html"
        };


        function VRTaxDefinition($scope, ctrl, $attrs) {

           

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.taxesDefinitions = [];

                $scope.scopeModel.disabledAddTitle = false;

                $scope.scopeModel.addTaxDefinition = function () {
                    $scope.scopeModel.taxesDefinitions.push({
                        itemId: UtilsService.guid(),
                        title: undefined
                    });
                };

                $scope.scopeModel.validateTaxesDefinitions = function () {
                    for (var i = 0; i < $scope.scopeModel.taxesDefinitions.length; i++) {
                        var taxDefinition = $scope.scopeModel.taxesDefinitions[i];
                        if (UtilsService.getItemIndexByVal($scope.scopeModel.taxesDefinitions, taxDefinition.title, "title") != i) {
                            return "There exists a duplicated record.";
                        }
                    }
                    return null;
                };

                $scope.scopeModel.removeTaxDefinition = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.taxesDefinitions, dataItem.title, "title");
                    $scope.scopeModel.taxesDefinitions.splice(index, 1);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        if (payload.TaxesDefinition != undefined && payload.TaxesDefinition.ItemDefinitions != undefined)
                            for (var i = 0; i < payload.TaxesDefinition.ItemDefinitions.length; i++) {
                                var itemDefinition = payload.TaxesDefinition.ItemDefinitions[i];
                                $scope.scopeModel.taxesDefinitions.push({
                                    itemId: itemDefinition.ItemId,
                                    title: itemDefinition.Title
                                });
                            }
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var itemDefinitions;
                    if ($scope.scopeModel.taxesDefinitions.length > 0) {
                        itemDefinitions = [];
                        for (var i = 0; i < $scope.scopeModel.taxesDefinitions.length; i++) {
                            var taxeDefinition = $scope.scopeModel.taxesDefinitions[i];
                            itemDefinitions.push({
                                ItemId: taxeDefinition.itemId,
                                Title: taxeDefinition.title
                            });
                        }
                    }
                    return {
                        $type: "Vanrise.Entities.VRTaxesDefinition,Vanrise.Entities",
                        ItemDefinitions: itemDefinitions
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

app.directive('vrCommonTaxdefinition', VRTaxDefinitionDirective);
 })(app);