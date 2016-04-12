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
            return "<excelconversion-fieldmapping-cellfieldmapping on-ready='cellfieldCtrl.onDirectiveReady' " + label + " isrequired='true' ></excelconversion-fieldmapping-cellfieldmapping>"
        }
        function ConcatenatedpartCellfield($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;

            var cellFieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            function initializeController() {
                ctrl.onDirectiveReady = function(api)
                {
                    directiveAPI = api;
                    cellFieldReadyPromiseDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined)
                    {
                        context = payload.context;

                    }

                    var loadCellFieldPromiseDeferred = UtilsService.createPromiseDeferred();
                    cellFieldReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            context: getContext(),
                        };
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
                    if (directiveAPI != undefined)
                    {
                        data = {
                            $type:"ExcelConversion.MainExtensions.ConcatenatedParts.CellFieldConcatenatedPart, ExcelConversion.MainExtensions ",
                            CellFieldMapping: directiveAPI.getData()
                        }
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

    app.directive('excelconversionConcatenatedpartCellfield', concatenatedpartCellfieldDirective);

})(app);