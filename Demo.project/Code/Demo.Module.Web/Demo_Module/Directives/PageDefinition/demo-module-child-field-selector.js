
app.directive('demoModuleChildFieldSelector', ['VRNotificationService', 'Demo_Module_PageDefinitionAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_PageDefinitionAPIService, UtilsService, VRUIUtilsService) {
    'use strict';
    
    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            onFieldReady: '=',
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

            var fieldSelector = new ChildSelector(ctrl, $scope, $attrs);
            fieldSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getChildTemplate(attrs);
        }
    };

    function getChildTemplate(attrs) {

        var multipleselection = "";
        var label = "Child Field";
        if (attrs.ismultipleselection != undefined) {
            label = "Child Field";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns vr-disabled="ctrl.isdisabled" colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Title" datavaluefield="Name" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  entityName="ChildField" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    };


    function ChildSelector(ctrl, $scope, attrs) {
        var selectorAPI;
        var pageDefinitionEntity;


        function initializeController() {
            var promises = [];

            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
            
        };


        function defineAPI() {

            function getPageDefinition(payload) {
                var pageDefintionId = payload.pageDefinitionId;
                return Demo_Module_PageDefinitionAPIService.GetPageDefinitionById(pageDefintionId).then(function (response) {
                    pageDefinitionEntity = response;

                });
            }
        
            var api = {};

            api.load = function (payload) {
                var selectedIds;

                if (payload != undefined && payload.subviewSettingsEntity != undefined) {

                    if (payload.subviewSettingsEntity.FieldName != undefined) {
                        selectedIds = [];
                        selectedIds.push(payload.subviewSettingsEntity.FieldName);
                    }
                }

                selectorAPI.clearDataSource();
                if (payload != undefined && payload.pageDefinitionId != undefined) {
                    return getPageDefinition(payload).then(function (response) {
                        if (pageDefinitionEntity.Details != undefined && pageDefinitionEntity.Details.Fields != undefined) {
                            for (var i = 0; i < pageDefinitionEntity.Details.Fields.length; i++) {
                                var field = pageDefinitionEntity.Details.Fields[i];
                                ctrl.datasource.push(field);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'Name', attrs, ctrl);
                            }

                        }
                    });
                }

            }

            api.getData = function () {
            return VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);