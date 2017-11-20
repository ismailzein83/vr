'use strict';

app.directive("vrCommonLocalizationlanguageSelector", ['VRCommon_VRLocalizationLanguageAPIService', 'UtilsService', 'VRUIUtilsService',

    function (VRCommon_VRLocalizationLanguageAPIService, UtilsService, VRUIUtilsService) {

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
                var localizationLanguageSelector = new LocalizationLanguageSelector(ctrl, $scope, $attrs);
                localizationLanguageSelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };
        function LocalizationLanguageSelector(ctrl, $scope, attrs) {
            var selectorAPI;

            this.initializeController = function () {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        console.log(payload);

                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }
                    return VRCommon_VRLocalizationLanguageAPIService.GetVRLocalizationLanguageInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'LocalizationLanguageId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('LocalizationLanguageId', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Language";

            if (attrs.ismultipleselection != undefined) {
                label = "Languages";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                       '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="LocalizationLanguageId" isrequired="ctrl.isrequired" label="' + label +
                           '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                           '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" customvalidate="ctrl.customvalidate">' +
                       '</vr-select>' +
                   '</vr-columns>';
        }
    }

]);