'use strict';
app.directive('vrCommonVrlocalizationlanguageSelector', ['VR_AccountManager_AccountManagerAPIService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRLocalizationLanguageAPIService',
    function (VR_AccountManager_AccountManagerAPIService, UtilsService, VRUIUtilsService, VRCommon_VRLocalizationLanguageAPIService) {

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
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new languageSelectorCtor(ctrl, $scope, $attrs);
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
                return getLanguageSelectorTemplate(attrs);
            }

        };


        function getLanguageSelectorTemplate(attrs) {

            var multipleselection = "";
            var label = "Languages";
            if (attrs.ismultipleselection != undefined) {
                label = "Languages";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Name" datavaluefield="LocalizationLanguageId" label="' + label + '" ' + '  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Language" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired" haspermission="ctrl.haspermission"></vr-select></vr-columns>';
        }

        function languageSelectorCtor(ctrl, $scope, attrs) {

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

                    selectorAPI.clearDataSource();
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;

                    }
                    VRCommon_VRLocalizationLanguageAPIService.GetVRLocalizationLanguageInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                       
                        if (response != null) {
                            for (var i = 0; i < response.length; i++)
                                ctrl.datasource.push(response[i]);
                            if (selectedIds != undefined)
                                VRUIUtilsService.setSelectedValues(selectedIds, 'LocalizationLanguageId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('LocalizationLanguageId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);