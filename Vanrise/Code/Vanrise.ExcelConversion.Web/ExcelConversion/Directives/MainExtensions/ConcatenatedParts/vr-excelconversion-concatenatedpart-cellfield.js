(function (app) {

    'use strict';

    concatenatedpartCellfieldDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function concatenatedpartCellfieldDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var concatenatedpartCellfield = new ConcatenatedpartCellfield($scope, ctrl, $attrs);
                concatenatedpartCellfield.initializeController();
            },
            controllerAs: "cellfieldCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var label = "";
            if (label != undefined)
                label = attrs.label;
            return "<vr-excelconversion-fieldmapping-cellfieldmapping on-ready='cellfieldCtrl.onDirectiveReady' " + label + " isrequired='true' ></vr-excelconversion-fieldmapping-cellfieldmapping>";
        }
        function ConcatenatedpartCellfield($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;

            var cellFieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            function initializeController() {
                ctrl.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    cellFieldReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var cellFieldMapping;
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.concatenatedPart != undefined)
                        {
                            cellFieldMapping = payload.concatenatedPart.CellFieldMapping;
                        }
                    }

                    var loadCellFieldPromiseDeferred = UtilsService.createPromiseDeferred();
                    cellFieldReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            context: getContext(),
                        };
                        if (cellFieldMapping != undefined)
                        {
                            payload.fieldMapping = cellFieldMapping;
                        }
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, loadCellFieldPromiseDeferred);
                    });

                    return loadCellFieldPromiseDeferred.promise;

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if (directiveAPI != undefined) {
                        data = {
                            $type: "Vanrise.ExcelConversion.MainExtensions.ConcatenatedParts.CellFieldConcatenatedPart, Vanrise.ExcelConversion.MainExtensions ",
                            CellFieldMapping: directiveAPI.getData()
                        };
                        return data;
                    }
                }
                function getContext() {

                    if (context != undefined) {
                        var currentContext = UtilsService.cloneObject(context);
                        return currentContext;
                    }
                }
            }
        }
    }

    app.directive('vrExcelconversionConcatenatedpartCellfield', concatenatedpartCellfieldDirective);

})(app);