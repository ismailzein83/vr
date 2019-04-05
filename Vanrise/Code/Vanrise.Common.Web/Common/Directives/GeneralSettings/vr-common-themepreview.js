'use strict';
app.directive('vrCommonThemepreview', ['UtilsService', 'VRUIUtilsService', 'VRLocalizationService',
    function (UtilsService, VRUIUtilsService, VRLocalizationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ThemePreviewCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return '<div style="height: 150px;width: 100%;overflow: hidden; position: relative;" vr-loader="scopeModel.isLoadingTheme">'
                            + '<iframe ng-src="/Client/Modules/Common/Directives/GeneralSettings/Templates/TemplateTheme.html" style="transform: scale(0.2 ,0.2);transform-origin: 0 0;zoom: 5; pointer-events:none;  height: 200px; width: 500%; position: absolute;left: 0px;" tabindex="-1" id="content">'
                            +' </iframe>'
                        + ' </div>';
            }

        };      
        function ThemePreviewCtor(ctrl, $scope, $attrs) {
            $scope.scopeModel = {};
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};
               
                api.load = function (payload) {
                    var theme;
                    if (payload != undefined)
                        theme = payload.theme;
                    $scope.scopeModel.isLoadingTheme = true;
                    if (theme != undefined) {
                        setTimeout(function () {
                            var iframe = document.getElementById("content");
                            var elmnt = iframe.contentWindow.document.getElementsByTagName("head")[0];                          
                            $(elmnt).find('#themeContent').remove();
                            $(elmnt).append($("<link />", { rel: "stylesheet", id: "themeContent", href: theme, type: "text/css" }));
                            var elementBody = iframe.contentWindow.document.getElementsByTagName("body")[0];
                            if (VRLocalizationService.isLocalizationRTL()) {
                                $(elementBody).addClass("rtl");
                            }
                            $scope.scopeModel.isLoadingTheme = false;
                            $scope.$apply();
                        }, 1000);
                    }
                    else {
                        setTimeout(function () {
                            var iframe = document.getElementById("content");
                            var elmnt = iframe.contentWindow.document.getElementsByTagName("head")[0];
                            var elementBody = iframe.contentWindow.document.getElementsByTagName("body")[0];
                            if (VRLocalizationService.isLocalizationRTL()) {
                                $(elementBody).addClass("rtl");
                            }
                            $(elmnt).find('#themeContent').remove();
                            $scope.scopeModel.isLoadingTheme = false;
                            $scope.$apply();
                        }, 1000);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);

