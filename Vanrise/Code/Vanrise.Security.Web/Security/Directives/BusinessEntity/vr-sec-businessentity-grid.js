"use strict";

app.directive("vrSecBusinessentityGrid", ['VRNotificationService', 'VR_Sec_BusinessEntityAPIService', 'VR_Sec_BusinessEntityDefinitionService', function (VRNotificationService, VR_Sec_BusinessEntityAPIService, VR_Sec_BusinessEntityDefinitionService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var entitiesGrid = new EntitiesGrid($scope, ctrl, $attrs);
            entitiesGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Security/Directives/BusinessEntity/Templates/BusinessEntityGridTemplate.html"

    };

    function EntitiesGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.entities = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onBusinessEntityAdded = function (businessEntity) {
                        gridAPI.itemAdded(businessEntity);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Sec_BusinessEntityAPIService.GetFilteredBusinessEntities(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editBusinessEntity,
                haspermission: hasUpdateBusinessEntityPermission // System Entities:Assign Permissions
            }];
        }
        function hasUpdateBusinessEntityPermission() {
            return VR_Sec_BusinessEntityAPIService.HasUpdateBusinessEntityPermission();
        }
        function editBusinessEntity(businessEntityObj) {
            var onBusinessEntityUpdated = function (businessEntity) {
                gridAPI.itemUpdated(businessEntity);
            };

            VR_Sec_BusinessEntityDefinitionService.updateBusinessEntityDefinition(businessEntityObj.Entity.EntityId, onBusinessEntityUpdated);
        }
    }

    return directiveDefinitionObject;

}]);



(function (app) {

    'use strict';

    vrSecSecurityProvidersettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Sec_SecurityProviderSettingsAPIService'];

    function vrSecSecurityProvidersettings(UtilsService, VRUIUtilsService, VR_Sec_SecurityProviderSettingsAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SecurityProvider($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function SecurityProvider($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var SecurityProviderEntity;

                    if (payload != undefined) {
                        SecurityProviderEntity = payload.SecurityProviderEntity;
                    }

                    if (SecurityProviderEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getSecurityProviderConfigsPromise = getSecurityProviderConfigs();
                    promises.push(getSecurityProviderConfigsPromise);

                    function getSecurityProviderConfigs() {
                        return VR_Sec_SecurityProviderSettingsAPIService.GetSecurityProviderConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (SecurityProviderEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, SecurityProviderEntity.ConfigId, 'ExtensionConfigurationId');

                                }

                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;

                            var directivePayload = {
                                SecurityProviderEntity: SecurityProviderEntity
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                        data= directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTemplate(attrs) {
            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined) {
                hideremoveicon = 'hideremoveicon';
            }
            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + 'label="Security Provider" '
                            + ' ' + hideremoveicon + ' '
                             + 'isrequired ="ctrl.isrequired"'
                           + ' >'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'

                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                        + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate">'
                + '</vr-directivewrapper>';

            return template;
        }
    }

    app.directive('vrSecSecurityProvidersettings', vrSecSecurityProvidersettings);

})(app);



app.directive("vrSecProvidersettingsLocalsecurityprovider", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new LocalSecurityProvider($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: ""
        };

        function LocalSecurityProvider($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.securityProviderEntity != undefined) {
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.MainExtensions.SecurityProvider.LocalSecurityProvider,Vanrise.Security.MainExtensions"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);



app.directive("vrSecProvidersettingsRemotesecurityprovider", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RemoteSecurityProvider($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: ""
        };

        function RemoteSecurityProvider($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.securityProviderEntity != undefined) {
                        $scope.scopeModel.VRConnectionId = payload.securityProviderEntity.VRConnectionId;
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.MainExtensions.SecurityProvider.RemoteSecurityProvider,Vanrise.Security.MainExtensions",
                        VRConnectionId: $scope.scopeModel.VRConnectionId
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);

app.directive('vrSecSecurityprovidersettingsStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService', 'VR_Sec_SecurityProviderSettingsAPIService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService, VR_Sec_SecurityProviderSettingsAPIService) {
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
            var template =  '<vr-row>'
            + '<vr-sec-security-providersettings  on-ready="scopeModel.onSecurityProviderSelectorReady" normal-col-num="3"></vr-sec-security-providersettings> '
        + '</vr-row>'

            return template;
        }


        function securityProviderStaticEditor(ctrl, $scope, $attrs) {

            var selectedValues;

            var securityProviderSelectorAPI;
            var securityProviderSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedSecurityProviderPromiseDeferred;


            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                               
                $scope.scopeModel.onSecurityProviderSelectorReady = function (api) {
                    securityProviderSelectorAPI = api;
                    securityProviderSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.issecurityProviderSelectorRequired = true;
                           
                          
                                           

                UtilsService.waitMultiplePromises([securityProviderSelectorReadyPromiseDeferred.promise]).then(function () {
                    defineApi();

                });
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    var SecurityProviderEntity;
                    var promises = [];
                    if (payload != undefined) {
                        SecurityProviderEntity = payload.SecurityProviderEntity;
                        if (selectedValues != undefined) {

                            selectedSecurityProviderPromiseDeferred = UtilsService.createPromiseDeferred();
                            if (selectedValues.EscalationLevelId != undefined) {
                                selectedSecurityProviderPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(selectedSecurityProviderPromiseDeferred.promise);
                            }



                        }
                    }

                    promises.push(loadSecurityProviderSelector());


                  
                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                        selectedSecurityProviderPromiseDeferred = undefined;
                    });

                };

                api.setData = function (securityProviderObject) {
                   
                        if(securityProviderObject!=undefined)
                            securityProviderObject.Settings = {
                            ExtendedSettings: securityProviderSelectorAPI.getData()
                            }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadSecurityProviderSelector() {
                var selectorPayload;
                if (selectedValues != undefined) {
                    selectorPayload = {
                        Settings: selectedValues.Settings
                    };
                }
                return securityProviderSelectorAPI.load(selectorPayload);
            }

        }
        return directiveDefinitionObject;
    }]);



(function (appControllers) {

    'use strict';

    SecurityProviderSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function SecurityProviderSettingsAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        var controllerName = 'SecurityProvider';

        return ({
            GetSecurityProviderConfigs: GetSecurityProviderConfigs
        });

       
        function GetSecurityProviderConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, "GetSecurityProviderConfigs"));
}
    }

    appControllers.service('VR_Sec_SecurityProviderSettingsAPIService', SecurityProviderSettingsAPIService);

})(appControllers);





(function (app) {

    'use strict';

    SecurityProviderType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function SecurityProviderType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SecurityProviderCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: ""

        };
        function SecurityProviderCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Security.Business.SecurityProviderCustomObjectTypeSettings, Vanrise.Security.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrSecProvidersettingsCustomobjectsettings', SecurityProviderType);

})(app);






