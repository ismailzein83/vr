app.directive('vrSecSecurityprovidersettingsStaticeditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new securityProviderStaticEditor(ctrl, $scope, $attrs);
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
            var template = '<vr-sec-securityprovider-settings-selector  on-ready="scopeModel.onSecurityProviderSelectorReady" isrequired="true" normal-col-num="4"></vr-sec-securityprovider-settings-selector> ';

            return template;
        }


        function securityProviderStaticEditor(ctrl, $scope, $attrs) {

            var selectedValues;

            var securityProviderSelectorAPI;
            var securityProviderSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSecurityProviderSelectorReady = function (api) {
                    securityProviderSelectorAPI = api;
                    securityProviderSelectorReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([securityProviderSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineApi();

                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                    }

                    promises.push(loadSecurityProviderSelector());

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.setData = function (securityProviderObject) {

                    if (securityProviderObject != undefined)
                        securityProviderObject.Settings = {
                            $type: "Vanrise.Security.Entities.SecurityProviderSettings,Vanrise.Security.Entities",
                            ExtendedSettings: securityProviderSelectorAPI.getData()
                        };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadSecurityProviderSelector() {
                var selectorPayload;
                if (selectedValues != undefined && selectedValues.Settings != undefined) {
                    selectorPayload = {
                        securityProviderEntity: selectedValues.Settings.ExtendedSettings
                    };
                }
                return securityProviderSelectorAPI.load(selectorPayload);
            }

        }
        return directiveDefinitionObject;
    }]);