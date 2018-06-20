'use strict';

app.directive('vrSecSecurityproviderRemoteselector', ['UtilsService', 'VRUIUtilsService', 'VR_Sec_SecurityProviderAPIService',
    function (UtilsService, VRUIUtilsService, VR_Sec_SecurityProviderAPIService) {
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

                var ctor = new RemoteSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function RemoteSelectorCtor(ctrl, $scope, attrs) {
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
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var connectionId;
                    var selectfirstitem;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        connectionId = payload.connectionId;
                        selectfirstitem = payload.selectfirstitem;
                    }
                    return VR_Sec_SecurityProviderAPIService.GetRemoteSecurityProvidersInfo(connectionId).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'SecurityProviderId', attrs, ctrl);
                            }

                            else if (selectfirstitem) {
                                var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].SecurityProviderId] : ctrl.datasource[0].SecurityProviderId;
                                VRUIUtilsService.setSelectedValues(defaultValue, 'SecurityProviderId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SecurityProviderId', attrs, ctrl);
                };


                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";

            var label = "Security Provider";
            if (attrs.ismultipleselection != undefined) {
                label = "Security Providers";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = ' hideremoveicon ';

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                        '<vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="SecurityProviderId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                        '</vr-select>' +
                    '</vr-columns>';
        }
    }]);