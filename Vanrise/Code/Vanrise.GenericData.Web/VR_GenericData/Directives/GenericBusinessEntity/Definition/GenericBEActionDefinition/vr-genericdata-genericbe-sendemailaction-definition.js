(function (app) {

    'use strict';

    SendEmailGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function SendEmailGenericBEDefinitionActionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SendEmailGenericBEDefinitionActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/SendEmailGenericBEDefinitionActionTemplate.html'
        };

        function SendEmailGenericBEDefinitionActionCtor($scope, ctrl) {

            var mailMessageTypeSelectorAPI;
            var mailMessageTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
               
                $scope.scopeModel.addSendEmailObjectInfo = function () {
                    ctrl.datasource.push({ data: { SendEmailObjectInfoId: UtilsService.guid()} });
                };

                $scope.scopeModel.onMailMessageTypeDirectiveReady = function (api) {
                    mailMessageTypeSelectorAPI = api;
                    mailMessageTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.isMailMessageTypeRequired = function () {
                    if (($scope.scopeModel.entityObjectName != undefined && $scope.scopeModel.entityObjectName != null && $scope.scopeModel.entityObjectName != '') ||
                        ($scope.scopeModel.infoType != undefined && $scope.scopeModel.infoType != null && $scope.scopeModel.infoType != ''))
                        return false;
                    return true;
                };
                $scope.scopeModel.isObjectInfoRequired = function () {
                    if (mailMessageTypeSelectorAPI.getSelectedIds() != undefined)
                        return false;
                    return true;
                };
                $scope.scopeModel.onDeleteRow = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineAPI();

            }
            function loadMailMessageTypeSelector(payload) {
                var mailMessageTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                mailMessageTypeSelectorReadyDeferred.promise.then(function () {
                    var mailMessageTypeSelectorPayload = {
                        selectedIds: payload.settings != undefined ? payload.settings.MailMessageTypeId : undefined
                    }; 
                    VRUIUtilsService.callDirectiveLoad(mailMessageTypeSelectorAPI, mailMessageTypeSelectorPayload, mailMessageTypeSelectorLoadPromiseDeferred);
                });
                return mailMessageTypeSelectorLoadPromiseDeferred.promise;
            }
    
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        var context = payload.context;
                        if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
                            context.showSecurityGridCallBack(true);
                        if (payload.settings != undefined) {
                            var settings = payload.settings;
                            $scope.scopeModel.entityObjectName = settings.EntityObjectName;
                            $scope.scopeModel.infoType = settings.InfoType;
                            if (settings.SendEmailObjectsInfo != undefined && settings.SendEmailObjectsInfo.length > 0) {
                                for (var i = 0; i < settings.SendEmailObjectsInfo.length; i++) {
                                    var objectInfo = settings.SendEmailObjectsInfo[i];
                                    ctrl.datasource.push({ data: objectInfo });
                                }
                            }
                        }
                        promises.push(loadMailMessageTypeSelector(payload));

                    }
                  
                    return UtilsService.waitPromiseNode({
                        promises: promises
                    });
                };

                api.getData = function () {
              
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEActions.SendEmailGenericBEAction,Vanrise.GenericData.MainExtensions",
                        MailMessageTypeId: mailMessageTypeSelectorAPI.getSelectedIds(),
                        EntityObjectName: $scope.scopeModel.entityObjectName,
                        InfoType: $scope.scopeModel.infoType,
                        SendEmailObjectsInfo: getObjectInfo()
                    };
                };
                function getObjectInfo() {
                    var objectInfo = [];
                    if (ctrl.datasource.length > 0) {
                        for (var j = 0; j < ctrl.datasource.length; j++) {
                            objectInfo.push(ctrl.datasource[j].data);
                        }
                    }
                    return objectInfo;
                }
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeSendemailactionDefinition', SendEmailGenericBEDefinitionActionDirective);

})(app);