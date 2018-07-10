'use strict';
app.directive('vrWhsBeRoutingproductSelector', ['WhS_BE_RoutingProductAPIService', 'UtilsService', 'VRUIUtilsService','WhS_BE_RoutingProductService',
    function (WhS_BE_RoutingProductAPIService, UtilsService, VRUIUtilsService, WhS_BE_RoutingProductService) {

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
                ondeselectitem: "=",
                hideselectedvaluessection: "@",
                onbeforeselectionchanged:"=",
                label: '@',
                showaddbutton:"@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;


                $scope.addNewRoutingProduct = function () {
                    var onRoutingProductAdded = function (routingProductObj) {
                        ctrl.datasource.push(routingProductObj.Entity);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(routingProductObj.Entity);
                        else
                            ctrl.selectedvalues = routingProductObj.Entity;
                    };
                    WhS_BE_RoutingProductService.addRoutingProduct(onRoutingProductAdded);
                };

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
                };
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };


        function getTemplate(attrs) {
            var multipleselection = (attrs.ismultipleselection != undefined) ? 'ismultipleselection' : '';

            var labelAttribute = '', labelAttributeValue = '';
            if (attrs.label != undefined)
                labelAttributeValue = attrs.label;
            else {
                labelAttributeValue = 'Routing Product';
                if (attrs.ismultipleselection != undefined)
                    labelAttributeValue += 's';
            }
            if (labelAttributeValue != undefined && labelAttributeValue != null && labelAttributeValue != '')
                labelAttribute = "label='" + labelAttributeValue + "'";

            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            var addClicked = '';
            if (attrs.showaddbutton != undefined)
                addClicked = 'onaddclicked="addNewRoutingProduct"';
                

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            var stopreadonly = "";
            if (attrs.stopreadonly != undefined)
                stopreadonly = "stopreadonly";

            return '<div>'
                + '<vr-select ' + multipleselection + ' ' + addClicked+ '  datatextfield="Name" datavaluefield="RoutingProductId" onbeforeselectionchanged="ctrl.onbeforeselectionchanged" onselectitem="ctrl.onselectitem"  ondeselectitem="ctrl.ondeselectitem"'
            + required + ' ' + labelAttribute + ' ' + '"  on-ready="ctrl.onSelectorReady" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="' + labelAttributeValue + '" ' + hideremoveicon + ' ' + hideselectedvaluessection + ' ' + stopreadonly + ' ></vr-select>'
               + '</div>';
        }

        function routingProductCtor(ctrl, $scope, $attrs) {

            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorApi.clearDataSource();

                    var filter = {};
                    var selectedIds;
                    var defaultItems;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        defaultItems = payload.defaultItems;
                    }

                    return WhS_BE_RoutingProductAPIService.GetRoutingProductInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (defaultItems) {
                            for (var i = 0; i < defaultItems.length; i++)
                                ctrl.datasource.push(defaultItems[i]);
                        }

                        for (var i = 0; i < response.length; i++)
                            ctrl.datasource.push(response[i]);

                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'RoutingProductId', $attrs, ctrl);
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('RoutingProductId', $attrs, ctrl);
                };

                api.getSelectedValues = function () {
                    return ctrl.selectedvalues;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);