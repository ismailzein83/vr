
app.directive('demoModuleChildSubviewSelector', ['VRNotificationService', 'Demo_Module_PageDefinitionAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_PageDefinitionAPIService, UtilsService, VRUIUtilsService) {
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

            var childSubviewSelector = new ChildSubviewSelector(ctrl, $scope, $attrs);
            childSubviewSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getChildSubviewTemplate(attrs);
        }
    };
    
    function getChildSubviewTemplate(attrs) {

        var multipleselection = "";
        var label = "Child Subview";
        if (attrs.ismultipleselection != undefined) {
            label = "Child Subview";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns vr-disabled="ctrl.isdisabled" colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="PageDefinitionId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="PageDefinition" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    };


    function ChildSubviewSelector(ctrl, $scope, attrs) {

        var selectorAPI;
        var pageDefinitionEntity;


        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }


        function defineAPI() {
            var api = {};

            api.load = function (payload) { //payload is an object that has selectedids and filter
                selectorAPI.clearDataSource();
                var selectedIds;
                var filter;
                if (payload != undefined && payload.subviewSettingsEntity != undefined) {
                    if (payload.subviewSettingsEntity.PageDefinitionId != undefined) {
                        selectedIds = [];
                        selectedIds.push(payload.subviewSettingsEntity.PageDefinitionId);

                    }
                    filter = payload.filter;

                }
                return Demo_Module_PageDefinitionAPIService.GetPageDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'PageDefinitionId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getData = function () {
                return {
                    $type: "Demo.Module.MainExtension.PageDefinition.SubViews.ChildSubView, Demo.Module.MainExtension",
                    PageDefinitionId: VRUIUtilsService.getIdSelectedIds('PageDefinitionId', attrs, ctrl)
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);
