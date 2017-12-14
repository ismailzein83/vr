'use strict';
app.directive('vrCommonVrlocalizationtextresourceSelector', ['UtilsService', 'VRUIUtilsService','VRCommon_VRLocalizationTextResourceAPIService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRLocalizationTextResourceAPIService) {

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
                hideremoveicon: '@',
                normalColNum: '@',
                label:'@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new textResourceSelectorCtor(ctrl, $scope, $attrs);
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
                return getTextResourceSelectorTemplate(attrs);
            }

        };


        function getTextResourceSelectorTemplate(attrs) {

            var multipleselection = "";
            var label = "Text Resource";
            if (attrs.ismultipleselection != undefined) {
                label = "Text Resources";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="ResourceKey" datavaluefield="VRLocalizationTextResourceId" label="' + label + '" ' + '  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="TextResource" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired" haspermission="ctrl.haspermission"></vr-select></vr-columns>';
        }

        function textResourceSelectorCtor(ctrl, $scope, attrs) {

            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var filter;
                    var selectedValue;

                    selectorAPI.clearDataSource();
                    if (payload != undefined) {
                        if (payload.selectedIds != undefined)
                            selectedIds = payload.selectedIds;
                        if (payload.selectedValue != undefined)
                            selectedValue = payload.selectedValue;

                        filter = payload.filter;

                    }

                    VRCommon_VRLocalizationTextResourceAPIService.GetVRLocalizationTextResourceInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++)
                                ctrl.datasource.push(response[i]);
                            if (selectedIds != undefined)
                                VRUIUtilsService.setSelectedValues(selectedIds, 'VRLocalizationTextResourceId', attrs, ctrl);

                            if (selectedValue != undefined) {
                                //var item = UtilsService.getItemByVal(ctrl.datasource, selectedValue, "ResourceKey")
                                //var resourceId = item.VRLocalizationTextResourceId;
                                VRUIUtilsService.setSelectedValues(selectedValue, 'ResourceKey', attrs, ctrl);
                            }


                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('VRLocalizationTextResourceId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);