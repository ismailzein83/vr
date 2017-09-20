'use strict';

app.directive('retailTelesSitesSelector', ['Retail_Teles_SiteAPIService', 'UtilsService', 'VRUIUtilsService', 'Retail_Teles_SiteService', function (Retail_Teles_SiteAPIService, UtilsService, VRUIUtilsService, Retail_Teles_SiteService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: '@',
            selectedvalues: '=',
            onselectionchanged: '=',
            onselectitem: '=',
            ondeselectitem: '=',
            isrequired: '=',
            hideremoveicon: '@',
            normalColNum: '@',
            customvalidate: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var enterpriseSelector = new SitesSelector(ctrl, $scope, $attrs);
            enterpriseSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function SitesSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;
        var vrConnectionId;
        var enterpriseId;
        function initializeController() {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
            $scope.addNewSite = function () {
                if (enterpriseId != undefined)
                {
                    var onSiteAdded = function (siteObj) {
                        ctrl.datasource.push(siteObj);
                        if (attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(siteObj);
                        else
                            ctrl.selectedvalues = siteObj;
                    };
                    Retail_Teles_SiteService.addTelesSite(onSiteAdded, enterpriseId, vrConnectionId);
                }
                
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    vrConnectionId = payload.vrConnectionId;
                    enterpriseId = payload.enterpriseId;
                    if (payload.filter != undefined)
                        filter = payload.filter;

                    return Retail_Teles_SiteAPIService.GetEnterpriseSitesInfo(vrConnectionId, enterpriseId, UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'TelesSiteId', attrs, ctrl);
                            }
                        }
                    });
                }
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('TelesSiteId', attrs, ctrl);
            };
            api.clearDataSource = function () {
                selectorAPI.clearDataSource();
                ctrl.datasource.length = 0;
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "Site";

        if (attrs.ismultipleselection != undefined) {
            label = "Sites";
            multipleselection = "ismultipleselection";
        }
        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewSite"';

        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + ' datatextfield="Name" ' + addCliked + ' datavaluefield="TelesSiteId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate"></vr-select></vr-columns>';
    }

}]);