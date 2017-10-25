(function (app) {

    'use strict';

    companyDefinitionPricelistSettingsDirective.$inject = [];

    function companyDefinitionPricelistSettingsDirective() {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var companyDefinitionPricelistSettings = new CompanyDefinitionPricelistSettings($scope, ctrl, $attrs);
                companyDefinitionPricelistSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var label = "";
            if (attrs.label != undefined)
                label = "label='" + attrs.label + "'";
            return '';
        }

        function CompanyDefinitionPricelistSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CompanyDifinitionPricelistSettings, TOne.WhS.BusinessEntity.MainExtensions",
                    };
                    return data;
                }
            }
        }
    }

    app.directive('vrWhsBeCompanydefinitionPricelistsettings', companyDefinitionPricelistSettingsDirective);

})(app);