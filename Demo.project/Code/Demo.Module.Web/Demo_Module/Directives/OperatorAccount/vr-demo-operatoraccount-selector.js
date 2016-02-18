'use strict';
app.directive('vrDemoOperatoraccountSelector', ['Demo_OperatorAccountAPIService', 'UtilsService', '$compile','VRUIUtilsService',
function (Demo_OperatorAccountAPIService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "=",
            selectedvalues:'='

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
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
            return getTemplate(attrs);
        }

    };


    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "Operator Account";
        if (attrs.ismultipleselection != undefined) {
            label = "Operator Accounts";
            multipleselection = "ismultipleselection";
        }
   
        return '<div  vr-loader="isLoadingDirective">'
            + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="OperatorAccountId" isrequired="ctrl.isrequired" '
            + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" entityName="Operator Account"  onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
            + '</div>'
    }

    function operatorAccountCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('OperatorAccountId', $attrs, ctrl);
            }
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return Demo_OperatorAccountAPIService.GetOperatorAccountsInfo().then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                        });
                        if (selectedIds!=undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'OperatorAccountId', $attrs, ctrl);

                    });

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);