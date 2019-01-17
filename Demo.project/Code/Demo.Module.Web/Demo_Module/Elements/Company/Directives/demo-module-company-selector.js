
app.directive("demoModuleCompanySelector", ["VRNotificationService", "Demo_Module_CompanyService", "Demo_Module_CompanyAPIService", "VRUIUtilsService", "UtilsService",
    function (VRNotificationService, Demo_Module_CompanyService, Demo_Module_CompanyAPIService, VRUIUtilsService, UtilsService) {

        'use strict';

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
                hideremoveicon: '@',
                normalColNum: '@',
                isdisabled: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var companySelector = new CompanySelector($scope, ctrl, $attrs);
                companySelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getCompanyTemplate(attrs);
            }
        };

        function getCompanyTemplate(attrs) {

            var label = "Company";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Company";
                multipleselection = "ismultipleselection";
            }

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                       + '<span vr-disabled="ctrl.isdisabled">'
                              +'<vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + ' datatextfield="Name" datavaluefield="CompanyId" isrequired="ctrl.isrequired" '
                                      + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Company"'
                                      + 'onselectitem = "ctrl.onselectitem" ondeselectitem = "ctrl.ondeselectitem"' + hideremoveicon + ' >'
                             + '</vr-select>'
                     + '</span>'
                + '</vr-columns >';
        };

        function CompanySelector($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
           
            var selectorApi = {};

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineScope();
                };
            };

            function defineScope() {
                var api = {};

                api.load = function (payload) {
                    selectorApi.clearDataSource();
                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return Demo_Module_CompanyAPIService.GetCompaniesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++)
                                ctrl.datasource.push(response[i]);

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'CompanyId', $attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CompanyId', $attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }]);