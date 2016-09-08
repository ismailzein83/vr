'use strict';
app.directive('vrQmBeZoneSelector', ['QM_BE_ZoneAPIService', 'UtilsService', '$compile', 'VRUIUtilsService',
function (QM_BE_ZoneAPIService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "@",
            selectedvalues: '='

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new zoneCtor(ctrl, $scope, $attrs);
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
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection";
        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        return '<div  vr-loader="isLoadingDirective">'
            + '<vr-select ' + multipleselection + '  datatextfield="LongName" datadisabledfield="IsDisabled" datavaluefield="ZoneId" '
        + required + ' label="Zones" datasource="ctrl.datasource" on-ready="ctrl.onBaseSelectorReady" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
           + '</div>'
    }

    function zoneCtor(ctrl, $scope, $attrs) {
        var baseApi;
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            ctrl.onBaseSelectorReady = function (api) {
                baseApi = api;
            }
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('ZoneId', $attrs, ctrl);
            }
            api.load = function (payload) {
                
                var filter;
                var selectedIds;
                if (payload != undefined) {
                    filter = payload.filter;
                    selectedIds = payload.selectedIds;
                }
                var serializedFilter = {};
                ctrl.filter = undefined
                if (filter != undefined) {
                    ctrl.filter = filter;
                    serializedFilter = UtilsService.serializetoJson(filter);
                }

                return getZonesInfo($attrs, ctrl, selectedIds, serializedFilter, baseApi);

            }
            
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }

    function getZonesInfo(attrs, ctrl, selectedIds, serializedFilter, baseApi) {
        if (baseApi != undefined)
         baseApi.clearDataSource();
        return QM_BE_ZoneAPIService.GetZonesInfo(serializedFilter).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });

            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'ZoneId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);