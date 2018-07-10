'use strict';
app.directive('businessprocessBpBusinessRuleSetSelector', ['BusinessProcess_BPBusinessRuleSetAPIService', 'UtilsService', 'VRUIUtilsService',
    function (BusinessProcess_BPBusinessRuleSetAPIService, UtilsService, VRUIUtilsService) {

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
                showaddbutton: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new businessRuleSetCtor(ctrl, $scope, $attrs);
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
                return getTemplate(attrs);
            }

        };


        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Parent Rule Set";
            if (attrs.ismultipleselection != undefined) {
                label = "Business Rule Sets";
                multipleselection = "ismultipleselection";
            }
            if (attrs.label != undefined)
                label = attrs.label;
            var addCliked = '';

            //vr-disabled="ctrl.isdisabled" is removed temporary
            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="BPBusinessRuleSetId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Business Rule Set" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        function businessRuleSetCtor(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var serializedFilter="";
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;

                        if (payload.filter != undefined) {
                            serializedFilter = UtilsService.serializetoJson(payload.filter);
                        }
                    }
                    return getBPBusinessRuleSetsInfo(attrs, ctrl, selectedIds, serializedFilter);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BPBusinessRuleSetId', attrs, ctrl);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getBPBusinessRuleSetsInfo(attrs, ctrl, selectedIds, serializedFilter) {
            return BusinessProcess_BPBusinessRuleSetAPIService.GetBusinessRuleSetsInfo(serializedFilter).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'BPBusinessRuleSetId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);