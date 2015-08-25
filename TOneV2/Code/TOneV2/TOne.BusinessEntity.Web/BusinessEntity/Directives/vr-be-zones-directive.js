(function (angular,app) {

    "use strict";

    app.directive('vrBeZones', ['ZoneAPIService', function (zoneApiService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                selectedvalues: "=",
                carrierid: "="
            },
            controller: function () {

                var ctrl = this;

                function zonesDatasource(text) {

                    console.log(ctrl.carrierid);
                    if (ctrl.carrierid) {
                        return zoneApiService.GetZones(text, ctrl.carrierid);
                    }

                    return zoneApiService.GetOwnZones(text);
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
                return '<vr-select datasource="ctrl.zonesDatasource" label="Zones" selectedvalues="ctrl.selectedvalues" datatextfield="Name" datavaluefield="ZoneId" entityname="Zones"></vr-select>';
            }

        };
        
        return directiveDefinitionObject;
    }]);

})(angular,app);