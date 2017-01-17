'use strict';

app.directive('retailVoiceAccountidentificationSelector', ['Retail_Voice_AccountIdentificationAPIService', 'UtilsService', 'VRUIUtilsService',
    function (Retail_Voice_AccountIdentificationAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
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
                ctrl.selectedvalues;
                ctrl.datasource = [];
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var ctor = new accountIdentificationCtor(ctrl, $scope, $attrs);
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
                return getAccountIdentificationTemplate(attrs);
            }
        };

        function getAccountIdentificationTemplate(attrs) {
            var multipleselection = "";
            var label = "Account Identification";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
                label = "Account Identifications";
            }

            var hideremoveicon;
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                   + '<vr-select datatextfield="Title" datavaluefield="ExtensionConfigurationId" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" ' + multipleselection + ' on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"' +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                   '</vr-select>'
                   + '</vr-columns>';
        }

        function accountIdentificationCtor(ctrl, $scope, $attrs) {

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
                    
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    
                    return Retail_Voice_AccountIdentificationAPIService.GetAccountIdentificationTemplates().then(function (response) {
                        ctrl.datasource.length = 0;
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'ExtensionConfigurationId', $attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ExtensionConfigurationId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);