'use strict';
app.directive('vrWhsBeCountrySelector', ['WhS_BE_CountryAPIService', 'UtilsService', '$compile', function (WhS_BE_CountryAPIService, UtilsService, $compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            onReady: '=',
            label: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '@',
            isdisabled: "=",
            selectedvalues: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            $scope.selectedCountryValues;
            if ($attrs.ismultipleselection != undefined)
                $scope.selectedCountryValues = [];
            $scope.datasource = [];
            var beCountry = new BeCountry(ctrl, $scope, WhS_BE_CountryAPIService, $attrs);
            beCountry.initializeController();
            $scope.onselectionchanged = function () {
                ctrl.selectedvalues = $scope.selectedCountryValues;
                if (ctrl.onselectionchanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval(ctrl.onselectionchanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }

            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            $scope.$watch('ctrl.isdisabled', function () {
                var template = getBeCountryTemplate(iAttrs, ctrl);
                iElement.html(template);
                $compile(iElement.contents())($scope);
            });
        }

    };
    function getBeCountryTemplate(attrs, ctrl) {
        var label;
        var disabled = "";
        if (ctrl.isdisabled)
            disabled = "vr-disabled='true'"

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";


        if (attrs.ismultipleselection != undefined)
            return '<div style="display:inline-block;width: calc(100% - 18px);" vr-loader="isLoadingDirective">'
                   + ' <vr-select ismultipleselection datasource="datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="selectedCountryValues" ' + disabled + ' onselectionchanged="onselectionchanged" datatextfield="Name" datavaluefield="CountryId"'
                   + 'entityname="Country" label="Country"></vr-select></div>';
        else
            return '<div vr-loader="isLoadingDirective">'
               + ' <vr-select datasource="datasource" selectedvalues="selectedCountryValues" ' + required + ' ' + hideselectedvaluessection + ' onselectionchanged="onselectionchanged"  ' + disabled + ' datatextfield="Name" datavaluefield="CountryId"'
               + 'entityname="Country" label="Country"></vr-select></div>';
    }
    function BeCountry(ctrl, $scope, WhS_BE_CarrierAccountAPIService, $attrs) {
        
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function () {
                return WhS_BE_CountryAPIService.GetAllCountries().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.datasource.push(itm);
                    });
                });
            }

            api.getData = function ()
            {
                return $scope.selectedCountryValues;
            }

            api.setData = function (selectedIds) {
                if ($attrs.ismultipleselection!=undefined) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedCountryValue = UtilsService.getItemByVal($scope.datasource, selectedIds[i], "CountryId");
                        if (selectedCountryValue != null)
                            $scope.selectedCountryValues.push(selectedCountryValue);
                    }
                } else {
                    var selectedCountryValue = UtilsService.getItemByVal($scope.datasource, selectedIds, "CountryId");
                    if (selectedCountryValue != null)
                        $scope.selectedCountryValues = selectedCountryValue;
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

