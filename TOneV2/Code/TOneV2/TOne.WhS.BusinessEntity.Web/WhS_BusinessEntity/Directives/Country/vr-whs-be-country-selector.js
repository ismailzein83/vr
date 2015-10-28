'use strict';
app.directive('vrWhsBeCountrySelector', ['WhS_BE_CountryAPIService', 'WhS_BE_MainService', 'UtilsService', '$compile', function (WhS_BE_CountryAPIService, WhS_BE_MainService,UtilsService, $compile  ) {

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
            showaddbutton: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            $scope.selectedCountryValues;
            if ($attrs.ismultipleselection != undefined)
                $scope.selectedCountryValues = [];

            $scope.AddNewCountry = function () {
                var onCountryAdded = function () {
                    WhS_BE_CountryAPIService.GetAllCountries().then(function (response) {
                        $scope.datasource.length = 0;
                        angular.forEach(response, function (itm) {
                            $scope.datasource.push(itm);
                        });
                    });
                };
                WhS_BE_MainService.addCountry(onCountryAdded);
            }
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
        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="AddNewCountry"';
        if (attrs.ismultipleselection != undefined)
            return  ' <vr-select ismultipleselection datasource="datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="selectedCountryValues" ' + disabled + ' onselectionchanged="onselectionchanged" datatextfield="Name" datavaluefield="CountryId"'
                   + 'entityname="Country" label="Country" '+addCliked+'></vr-select>';
        else
            return '<div vr-loader="isLoadingDirective" style="display:inline-block;width:100%">'
               + ' <vr-select datasource="datasource" selectedvalues="selectedCountryValues" ' + required + ' ' + hideselectedvaluessection + ' onselectionchanged="onselectionchanged"  ' + disabled + ' datatextfield="Name" datavaluefield="CountryId"'
               + 'entityname="Country" label="Country" ' + addCliked + '></vr-select></div>';
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
            api.getIdsData = function () {
                return getIdsList($scope.selectedCountryValues , "CountryId" );
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
            function getIdsList(tab, attname) {
                var list = [];
                for (var i = 0; i < tab.length ; i++)
                    list[list.length] = tab[i][attname];
                return list;

            }
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

