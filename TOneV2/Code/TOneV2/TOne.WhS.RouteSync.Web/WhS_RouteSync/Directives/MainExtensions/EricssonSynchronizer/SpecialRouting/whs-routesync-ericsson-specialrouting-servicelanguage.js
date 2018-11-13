(function (app) {

    'use strict';

    SpecialRoutingServicelanguage.$inject = ["UtilsService", 'VRUIUtilsService'];

    function SpecialRoutingServicelanguage(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SpecialRoutingServicelanguageCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "serviceLanguageCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/SpecialRouting/Templates/EricssonSpecialroutingServicelanguageTemplate.html"
        };

        function SpecialRoutingServicelanguageCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var codeGroupSuffixes;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.codeGroupSuffixes = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    $scope.scopeModel.codeGroupSuffixes = [];
                    if (payload != undefined && payload.Settings != undefined) {

                        codeGroupSuffixes = payload.Settings.CodeGroupSuffixes;
                        if (codeGroupSuffixes != undefined) {
                            var codeGroupSuffixesLength = codeGroupSuffixes.length;
                            for (var i = 0; i < codeGroupSuffixesLength; i++) {
                                $scope.scopeModel.codeGroupSuffixes.push(codeGroupSuffixes[i].Suffix);
                            }
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var codes = [];
                    if ($scope.scopeModel.codeGroupSuffixes != undefined && $scope.scopeModel.codeGroupSuffixes.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.codeGroupSuffixes.length; i++) {
                            codes.push({ Suffix: $scope.scopeModel.codeGroupSuffixes[i] });
                        }
                    }
                    var data = {
                        $type: "TOne.WhS.RouteSync.Ericsson.Entities.EricssonSpecialRoutingServiceLanguage,TOne.WhS.RouteSync.Ericsson",
                        CodeGroupSuffixes: codes
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncEricssonSpecialroutingServicelanguage', SpecialRoutingServicelanguage);

})(app);