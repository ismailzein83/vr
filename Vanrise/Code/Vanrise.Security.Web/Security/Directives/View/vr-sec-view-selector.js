'use strict';
app.directive('vrSecViewSelector', ['VR_Sec_ViewAPIService', 'VR_Sec_ViewService', 'UtilsService', 'VRUIUtilsService',
   
    function (VR_Sec_ViewAPIService, VR_Sec_ViewService, UtilsService, VRUIUtilsService) {
       
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
                normalColNum:"@",
                customlabel: "@",
                onitemadded: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new ViewCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getViewTemplate(attrs);
            }

        };


        function getViewTemplate(attrs) {

            var multipleselection = "";

            var label = "View";
            if (attrs.ismultipleselection != undefined) {
                label = "Views";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

          

            return  '<vr-columns colnum="{{ctrl.normalColNum}}">'
                        + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="ViewId" isrequired="ctrl.isrequired"'
                        + ' label="' + label + '"  datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="View" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"></vr-select>'
                    + '</vr-columns>';
        }

        function ViewCtor(ctrl, $scope, attrs) {

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
                        selectedIds = payload.selectedIds;
                    }
                    return VR_Sec_ViewAPIService.GetViewsInfo().then(function (response) {
                        ctrl.datasource.length = 0;

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'ViewId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ViewId', attrs, ctrl);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);