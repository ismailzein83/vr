//(function (app) {

//    'use strict';

//    SendEmailGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

//    function SendEmailGenericBEDefinitionActionDirective(UtilsService, VRNotificationService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new SendEmailGenericBEDefinitionActionCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/SendEmailGenericBEDefinitionActionTemplate.html'
//        };

//        function SendEmailGenericBEDefinitionActionCtor($scope, ctrl) {

//            var mailMessageTemplateSelectorAPI;
//            var mailMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            this.initializeController = initializeController;

//            function initializeController() {
//                $scope.scopeModel = {};
//                ctrl.datasource = [];

//                $scope.scopeModel.onMailMessageTemplateDirectiveReady = function (api) {console.log("1")
//                    mailMessageTemplateSelectorAPI = api;
//                    mailMessageTemplateSelectorReadyDeferred.resolve();
//                };
//                $scope.scopeModel.addSendEmailObjectInfo = function () {
//                    ctrl.datasource.push({});
//                };

//                defineAPI();

//            }
//            function loadMailMessageTypeSelector(payload) {
//                var mailMessageTemplateSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
//                mailMessageTemplateSelectorReadyDeferred.promise.then(function () {
//                    var mailMessageTemplateSelectorPayload = {
//                        //filter: {
//                        //    VRMailMessageTypeId: '0f64da0b-e2d0-4421-beb9-32c6e749f8f1'
//                        //},
//                        selectedIds: payload.MailMessageTypeId
//                    }; 
//                    VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSelectorAPI, mailMessageTemplateSelectorPayload, mailMessageTemplateSelectorLoadPromiseDeferred);
//                });
//                return mailMessageTemplateSelectorLoadPromiseDeferred.promise;
//            }
    
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    if (payload != undefined) {
//                        var context = payload.context;
//                        if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
//                            context.showSecurityGridCallBack(true);
//                        $scope.scopeModel.entityObjectName = payload.EntityObjectName;
//                        $scope.scopeModel.infoType = payload.InfoType;
//                        if (payload.SendEmailObjectsInfo != undefined && payload.SendEmailObjectsInfo.length > 0) {
//                            for (var i = 0; i < payload.SendEmailObjectsInfo.length; i++) {
//                                var objectInfo = payload.SendEmailObjectsInfo[i];
//                                ctrl.datasource.push(objectInfo);
//                            }
//                        }
//                        promises.push(loadMailMessageTypeSelector(payload)); console.log(promises)
//                    }
                  
//                    return UtilsService.waitPromiseNode({
//                        promises: promises
//                    });
//                };

//                api.getData = function () {
//                    console.log(ctrl.datasource)
//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.SendEmailGenericBEAction, Vanrise.GenericData.MainExtensions",
//                        MailMessageTypeId: mailMessageTemplateSelectorAPI.getSelectedIds(),
//                        EntityObjectName: $scope.scopeModel.entityObjectName,
//                        InfoType: $scope.scopeModel.infoType,
//                        SendEmailObjectsInfo: ctrl.datasource
//                    };
//                };
//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('vrGenericdataGenericbeSendemailactionDefinition', SendEmailGenericBEDefinitionActionDirective);

//})(app);