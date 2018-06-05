
app.directive('demoBestpracticesParentSelector', ['VRNotificationService', 'Demo_BestPractices_ParentAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_BestPractices_ParentAPIService, UtilsService, VRUIUtilsService) {
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
            isdisabled:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var parentSelector = new ParentSelector(ctrl, $scope, $attrs);
            parentSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getCompanyTemplate(attrs);
        }
    };

    function getCompanyTemplate(attrs) {

        var multipleselection = "";
        var label = "Parent";
        if (attrs.ismultipleselection != undefined) {
            label = "Parents";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}">'
             +'<span  vr-disabled="ctrl.isdisabled"><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="ParentId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Parent" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></span></vr-columns>';
    };


    function ParentSelector(ctrl, $scope, attrs) {

        var selectorAPI;

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };

        };

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                }
                return Demo_BestPractices_ParentAPIService.GetParentsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'ParentId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('ParentId', attrs, ctrl);
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);