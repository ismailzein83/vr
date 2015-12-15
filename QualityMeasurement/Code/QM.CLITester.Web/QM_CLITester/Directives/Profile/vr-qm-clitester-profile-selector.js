'use strict';
app.directive('vrQmClitesterProfileSelector', ['QM_CLITester_ProfileAPIService', 'UtilsService', '$compile', 'VRUIUtilsService',
function (QM_CLITester_ProfileAPIService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "@",
            selectedvalues: '='

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new profileCtor(ctrl, $scope, $attrs);
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
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection"
        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";
        var disabled = "";
        return '<div  vr-loader="isLoadingDirective">'
            + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="ProfileId" '
        + required + ' label="Profile" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
           + '</div>'
    }

    function profileCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('ProfileId', $attrs, ctrl);
            }
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return QM_CLITester_ProfileAPIService.GetProfilesInfo().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);

                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'ProfileId', $attrs, ctrl);

                });

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);

        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);