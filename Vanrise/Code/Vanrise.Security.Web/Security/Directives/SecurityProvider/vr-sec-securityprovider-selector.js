'use strict';
app.directive('vrSecSecurityproviderSelector', ['VR_Sec_SecurityProviderAPIService', 'VR_Sec_UserService', 'UtilsService', 'VRUIUtilsService',

    function (VR_Sec_SecurityProviderAPIService, VR_Sec_UserService, UtilsService, VRUIUtilsService) {

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
                customlabel: "@",
                hideremoveicon: "@",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new securityProviderCtor(ctrl, $scope, $attrs);
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
                return getSecurityProviderTemplate(attrs);
            }

        };


        function getSecurityProviderTemplate(attrs) {

            var multipleselection = "";

            var label = "Authentication Method";
            if (attrs.ismultipleselection != undefined) {
                label = "Authentication Methods";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = "hideremoveicon";
            }
            var vrcolumnTemplateStart = '';
            var vrcolumnTemplateEnd = '';

            if (attrs.normalColNum != undefined) {
                vrcolumnTemplateStart = '<vr-columns colnum="{{ctrl.normalColNum}}"> ';
                vrcolumnTemplateEnd = ' </<vr-columns>';
            }

            return vrcolumnTemplateStart
                + '<vr-select ' + multipleselection + ' ' + hideremoveicon + '  datatextfield="Name" datavaluefield="SecurityProviderId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Authentication Method" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"></vr-select>'
                + vrcolumnTemplateEnd;
        }

        function securityProviderCtor(ctrl, $scope, attrs) {

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
                    selectorApi.clearDataSource();
                    var selectedIds;
                    var filter;
                    var selectfirstitem = false;
                    var promises = [];
                    var defaultSecurityProviderId;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        selectfirstitem = payload.selectfirstitem == true;
                    }

                    if (!selectedIds) {
                        var getDefaultSecurityProviderIdPromise = GetDefaultSecurityProviderId();
                        promises.push(getDefaultSecurityProviderIdPromise);
                    }

                    function GetDefaultSecurityProviderId() {
                        return VR_Sec_SecurityProviderAPIService.GetDefaultSecurityProviderId().then(function (response) {
                            if (response != undefined && !selectedIds) {
                                defaultSecurityProviderId = response;
                            }

                        });
                    }

                    var getSecurityProvidersInfoPromise = GetSecurityProvidersInfo();
                    promises.push(getSecurityProvidersInfoPromise);

                    function GetSecurityProvidersInfo(){
                        return VR_Sec_SecurityProviderAPIService.GetSecurityProvidersInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        ctrl.datasource.length = 0;
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }
                      
                 
                    });
                    }
            
                    return UtilsService.waitMultiplePromises(promises).then(function(response){
                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SecurityProviderId', attrs, ctrl);
                        } else {
                            VRUIUtilsService.setSelectedValues(defaultSecurityProviderId, 'SecurityProviderId', attrs, ctrl);
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

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);