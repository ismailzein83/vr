'use strict';
app.directive('vrSecTenantSelector', ['VR_Sec_TenantAPIService', 'VR_Sec_TenantService', 'UtilsService', 'VRUIUtilsService',

    function (VR_Sec_TenantAPIService, VR_Sec_TenantService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "=",
                customlabel: "@",
                onitemadded: "=",
                hideifnotneeded: '=',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new tenantCtor(ctrl, $scope, $attrs);
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
                return getTenantTemplate(attrs);
            }

        };


        function getTenantTemplate(attrs) {

            var multipleselection = "";

            var label = "Tenant";
            if (attrs.ismultipleselection != undefined) {
                label = "Tenants";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="TenantId" isrequired="ctrl.isrequired" hideremoveicon = "ctrl.hideremoveicon"'
                + ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Tenant" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        function tenantCtor(ctrl, $scope, attrs) {

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

                    var selectedIds;
                    ctrl.filter;
                    if (payload != undefined) {
                        ctrl.filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }
                    return VR_Sec_TenantAPIService.GetTenantsInfo(UtilsService.serializetoJson(ctrl.filter)).then(function (response) {
                        ctrl.datasource.length = 0;

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'TenantId', attrs, ctrl);
                        } else {
                            if (ctrl.datasource.length == 1 && ctrl.hideifnotneeded != null) {
                                VRUIUtilsService.setSelectedValues(ctrl.datasource[0].TenantId, 'TenantId', attrs, ctrl);
                                ctrl.hideifnotneeded();
                            }
                        }
                    });
                };



                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('TenantId', attrs, ctrl);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);