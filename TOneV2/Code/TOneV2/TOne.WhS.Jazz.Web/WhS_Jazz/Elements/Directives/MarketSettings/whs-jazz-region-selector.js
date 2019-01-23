(function (app) {

    'use strict';

    WhSJazzRegionSelectorDirective.$inject = ['UtilsService', 'VRUIUtilsService','WhS_Jazz_RegionCodeAPIService'];

    function WhSJazzRegionSelectorDirective(UtilsService, VRUIUtilsService, WhS_Jazz_RegionCodeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                isdisabled: "=",
                customlabel: "@",
                hidelabel:"="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var marketSelector = new RegionSelector(ctrl, $scope, $attrs);
                marketSelector.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function RegionSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var specificTypeName;
            function initializeController() {



                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter;
                    var selectedIds;

                    if (payload != undefined) {
                        if (payload.showaddbutton)
                            ctrl.onAddRegion = onAddRegion;
                        specificTypeName = payload.specificTypeName;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return WhS_Jazz_RegionCodeAPIService.GetRegionCodesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'ID', attrs, ctrl);
                        }
                     
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ID', attrs, ctrl);
                };
                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };
                api.getDatasource = function () {
                    return ctrl.datasource;
                };

                return api;
            }
            function onAddRegion() {
              
            }
     
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Region';
            if (attrs.ismultipleselection != undefined) {
                label = 'Regions';
                multipleselection = 'ismultipleselection';
            }
            var datatextfield = "Name";
            if (attrs.showtitle != undefined)
                datatextfield = "Name";
            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }
            //var addCliked = '';
            //if (attrs.onaddclicked != undefined)
            //    addCliked = ' ';
            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<div>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                + ' datasource="ctrl.datasource" hidelabel="ctrl.hidelabel"'
                + ' selectedvalues="ctrl.selectedvalues"'
                + ' onselectionchanged="ctrl.onselectionchanged"'
                + 'onaddclicked="ctrl.onAddRegion"'
                + ' onselectitem="ctrl.onselectitem"'
                + ' ondeselectitem="ctrl.ondeselectitem"'
                + ' datavaluefield="ID"'
                + ' datatextfield="' + datatextfield + '"'
                + ' ' + multipleselection
                + ' ' + hideselectedvaluessection
                + ' isrequired="ctrl.isrequired"'
                + ' ' + hideremoveicon
                + ' label="' + label + '"'
                + ' entityName="' + label + '"'
                + '</vr-select>'
                + '</div>';
        }
    }

    app.directive('whsJazzRegionSelector', WhSJazzRegionSelectorDirective);

})(app);