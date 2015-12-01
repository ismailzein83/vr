'use strict';
app.directive('vrWhsBeRoutingproductSelector', ['WhS_BE_RoutingProductAPIService', 'UtilsService','VRUIUtilsService',
    function (WhS_BE_RoutingProductAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                selectedvalues: '=',
                hideremoveicon: "@",
                onselectitem: "=",
                ondeselectitem: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new routingProductCtor(ctrl, $scope, $attrs);
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
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };


        function getTemplate(attrs) {
            var multipleselection = "";
            var label = "Routing Product";
            if (attrs.ismultipleselection != undefined) {
                label = "Routing Products";
                multipleselection = "ismultipleselection";
            }
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="RoutingProductId" onselectitem="ctrl.onselectitem"  ondeselectitem="ctrl.ondeselectitem"'
            + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" ' + hideremoveicon + '></vr-select>'
               + '</div>'
        }

        function routingProductCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter = {};
                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return WhS_BE_RoutingProductAPIService.GetRoutingProductInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        ctrl.datasource = []; // clear data source to avoid duplication
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });
                        VRUIUtilsService.setSelectedValues(selectedIds, 'RoutingProductId', $attrs, ctrl);
                    });
                }

                api.getSelectedIds = function()
                {
                    return VRUIUtilsService.getIdSelectedIds('RoutingProductId', $attrs, ctrl);
                }

                api.getSelectedValues = function () {
                    return ctrl.selectedvalues;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);