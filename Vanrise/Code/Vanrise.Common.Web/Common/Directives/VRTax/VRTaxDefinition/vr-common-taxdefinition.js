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

            ctrl.titles = [];

            this.initializeController = initializeController;

            function initializeController() {             
                ctrl.disabledAddTitle = false;

                ctrl.addTitle = function () {
                    ctrl.titles.push({ title: ctrl.titlevalue });
                    ctrl.titlevalue = undefined;
                    ctrl.disabledAddTitle = true;
                };

                ctrl.onTitleValueChange = function (value) {
                    ctrl.disabledAddTitle = (value == undefined && ctrl.titlevalue.length - 1 < 1) || UtilsService.getItemIndexByVal(ctrl.titles, value, "title") != -1;
                };

                ctrl.validateAddTitle = function () {
                    if (ctrl.title != undefined && ctrl.title.length == 0)
                        return "Enter at least one keyword.";
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    
                    if (payload != undefined) {
                        if (payload.TaxesDefinition != undefined && payload.TaxesDefinition.ItemDefinitions != undefined)
                            angular.forEach(payload.TaxesDefinition.ItemDefinitions, function (val) {
                                ctrl.titles.push({ title: val.Title });
                            });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var itemDefinitions = [];

                    for (var i = 0; i < ctrl.titles.length; i++) {
                        var taxItemDefinition = {};
                        taxItemDefinition.ItemId = UtilsService.guid();
                        taxItemDefinition.Title = ctrl.titles[i].title;
                        itemDefinitions.push(taxItemDefinition);
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