"use strict";

app.directive("vrGenericdataGenericbeAftersavehandlerSendemail", ["UtilsService", "VRNotificationService",
    function (UtilsService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum:"@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SendEmailHandler($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "sendEmailCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/OnAfterSaveHandler/Templates/SendEmailHandlerTemplate.html"
        };

        function SendEmailHandler($scope, ctrl, $attrs) {

            var context;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.removeSendEmailObjectsInfo = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity.SendEmailObjectInfoId, 'Entity.SendEmailObjectInfoId');
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                $scope.scopeModel.addSendEmailObjectsInfo = function () {
                    $scope.scopeModel.datasource.push({
                        Entity: {
                            SendEmailObjectInfoId: UtilsService.guid(),
                            InfoType: undefined,
                            ObjectName: undefined
                        }
                    });
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {    
                    var sendEmailObjectsInfo;
                    if ($scope.scopeModel.datasource.length > 0) {
                        sendEmailObjectsInfo = [];
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var item = $scope.scopeModel.datasource[i];
                            sendEmailObjectsInfo.push({
                                SendEmailObjectInfoId:item.Entity.SendEmailObjectInfoId,
                                InfoType: item.Entity.InfoType,
                                ObjectName: item.Entity.ObjectName,
                            });
                        }
                    }
                   
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers.SendEmailAfterSaveHandler, Vanrise.GenericData.MainExtensions",
                        EntityObjectName: $scope.scopeModel.entityObjectName,
                        InfoType: $scope.scopeModel.infoType,
                        SendEmailObjectsInfo: sendEmailObjectsInfo
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined && payload.settings) {                        
                        $scope.scopeModel.entityObjectName = payload.settings.EntityObjectName;
                        $scope.scopeModel.infoType = payload.settings.InfoType;

                        if (payload.settings.SendEmailObjectsInfo != undefined)
                        {
                            for (var i = 0; i < payload.settings.SendEmailObjectsInfo.length; i++) {
                                var sendEmailObjectsInfo = payload.settings.SendEmailObjectsInfo[i];
                                $scope.scopeModel.datasource.push({ Entity: sendEmailObjectsInfo });
                            }
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;

    }
]);