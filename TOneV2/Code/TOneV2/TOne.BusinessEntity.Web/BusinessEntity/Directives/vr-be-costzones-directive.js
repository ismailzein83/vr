(function (angular, app) {

    "use strict";

    app.directive('vrBeCostzones', ['ZoneAPIService', '$q', function (zoneApiService, $q) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                selectedvalues: "=",
                carrierid: "="
            },
            controller: function ($q) {

                var ctrl = this;

                function zonesDatasource(text) {

                    if (ctrl.carrierid) {
                        return zoneApiService.GetSupplierZones(text, ctrl.carrierid);
                    }
                    else
                        return function () { };
                        
                }

                angular.extend(this, {
                    zonesDatasource: zonesDatasource
                });

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                if (attrs.ismultipleselection !== undefined) {
                    return '<vr-select ismultipleselection datasource="ctrl.zonesDatasource" label="Zones" selectedvalues="ctrl.selectedvalues" datatextfield="Name" datavaluefield="ZoneId" entityname="Zones"></vr-select>';
                }
                return '<vr-select datasource="ctrl.zonesDatasource" label="Zone" selectedvalues="ctrl.selectedvalues" datatextfield="Name" datavaluefield="ZoneId" entityname="Zones"></vr-select>';
            }

        };

        return directiveDefinitionObject;
    }]);

})(angular, app);