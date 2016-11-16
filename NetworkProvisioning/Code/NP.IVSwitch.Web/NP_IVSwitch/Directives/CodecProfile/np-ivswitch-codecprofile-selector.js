'use strict';

app.directive('npIvswitchCodecprofileSelector', ['NP_IVSwitch_CodecProfileAPIService', 'UtilsService', 'VRUIUtilsService',

    function (NP_IVSwitch_CodecProfileAPIService, UtilsService, VRUIUtilsService) {
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

                var codecProfileSelector = new CodecProfileSelector(ctrl, $scope, $attrs);
                codecProfileSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function CodecProfileSelector(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

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

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }


                    return NP_IVSwitch_CodecProfileAPIService.GetCodecProfilesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                         if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }


                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'CodecProfileId', attrs, ctrl);
                            }
                        }
                    });


                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CodecProfileId', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Available Codec Profiles";

            if (attrs.ismultipleselection != undefined) {
                label = "Available Codec Profiles";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                   '<vr-select ' + multipleselection + ' datatextfield="ProfileName" datavaluefield="CodecProfileId" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                   '</vr-select>' +
                   '</vr-columns>';
        }

    }]);