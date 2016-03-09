'use strict';
app.directive('vrInterconnectBeOperatoraccountSelector', ['InterConnect_BE_OperatorAccountAPIService', 'UtilsService', 'VRUIUtilsService',
    function (InterConnect_BE_OperatorAccountAPIService, UtilsService, VRUIUtilsService) {

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
                isdisabled: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new operatorAccountCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getOperatorAccountTemplate(attrs);
            }

        };


        function getOperatorAccountTemplate(attrs) {

            var multipleselection = "";
            var label = "Operator Account";
            if (attrs.ismultipleselection != undefined) {
                label = "Operator Accounts";
                multipleselection = "ismultipleselection";
            }



            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="OperatorAccountId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="OperatorAccount" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>'
        }

        function operatorAccountCtor(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var serializedFilter = {};
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        if (payload.filter != undefined) {
                            serializedFilter = UtilsService.serializetoJson(payload.filter);
                        }
                    }

                    return getOperatorAccountsInfo(attrs, ctrl, selectedIds, serializedFilter);
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('OperatorAccountId', attrs, ctrl);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getOperatorAccountsInfo(attrs, ctrl, selectedIds, serializedFilter) {
            return InterConnect_BE_OperatorAccountAPIService.GetOperatorAccountsInfo(serializedFilter).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'OperatorAccountId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);