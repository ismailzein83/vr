app.directive('vrGenericdataGenericbeGridactiondefinitionFilterextension', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new filterextension(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };


        function getTemplate(attrs) {
            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }
            var template = '<vr-genericdata-recordfilter  on-ready="scopeModel.onfilterReady" isrequired="true" normal-col-num="4"></vr-genericdata-recordfilter> ';

            return template;
        }


        function filterextension(ctrl, $scope, $attrs) {

            var selectedValues;

            var filterAPI;
            var filterReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onfilterReady = function (api) {
                    filterAPI = api;
                    filterReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([filterReadyPromiseDeferred.promise]).then(function () {
                    defineApi();

                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    filterAPI.load(payload);

                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.ActionFilterCondition.GenericFilterGroupCondition, Vanrise.GenericData.MainExtensions",
                        FilterGroup: filterAPI != undefined ? filterAPI.getData().filterObj : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            

        }
        return directiveDefinitionObject;
    }]);