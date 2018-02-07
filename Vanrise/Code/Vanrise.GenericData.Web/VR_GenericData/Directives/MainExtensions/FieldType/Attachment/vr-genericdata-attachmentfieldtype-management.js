"use strict";

app.directive("vrGenericdataAttachmentfieldtypeManagement", ["UtilsService", "VRNotificationService", "VRUIUtilsService", 
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var attachmentfieldtype = new Attachmentfieldtype($scope, ctrl, $attrs);
            attachmentfieldtype.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Attachment/Templates/AttachmentFieldTypeEntityManagementTemplate.html'
    };

    function Attachmentfieldtype($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.attachementFieldTypes = [];
            $scope.scopeModel.addNewAttachementFieldType = function () {
                var attachementFieldType =
                    {
                        file:undefined,
                        notes: '',
                       // createdTime:new Date()
                    };
                $scope.scopeModel.attachementFieldTypes.push({ Entity: attachementFieldType });
              
            };
            $scope.removerow = function (dataItem) {
                var index = $scope.scopeModel.attachementFieldTypes.indexOf(dataItem);
                $scope.scopeModel.attachementFieldTypes.splice(index, 1);
            };
            $scope.onGridReady = function (api) {
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        var promises = [];
                        if (payload != undefined) {
                            if (payload.attachementFieldTypes != undefined) {
                                for (var i = 0; i < payload.attachementFieldTypes.length; i++) {
                                    var attachmentFieldType = payload.attachementFieldTypes[i];
                                    var attachementFieldType = {
                                        Entity: {
                                            file: { fileId: attachmentFieldType.FileId },
                                            notes: attachmentFieldType.Notes,
                                            createdTime: attachmentFieldType.CreatedTime
                                        }
                                    };
                                    $scope.scopeModel.attachementFieldTypes.push(attachementFieldType);
                                }
                            }
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    };
                    directiveAPI.getData = function () {
                        var obj = {
                            $type: "Vanrise.GenericData.Entities.AttachmentFieldTypeEntityCollection,Vanrise.GenericData.Entities",
                            $values: getAttachementFieldTypeData()
                        };
                        return obj;
                    };
                    return directiveAPI;
                };

            };
        }
     
        function getAttachementFieldTypeData() {
            var attachementFieldTypes = [];
            if ($scope.scopeModel.attachementFieldTypes != undefined) {
                for (var i = 0; i < $scope.scopeModel.attachementFieldTypes.length; i++) {
                    var attachmentFieldType = $scope.scopeModel.attachementFieldTypes[i];
                        var attachementFieldType =
                            {
                                $type: "Vanrise.GenericData.Entities.AttachmentFieldTypeEntity, Vanrise.GenericData.Entities",
                                FileId: attachmentFieldType.Entity.file.fileId,
                                Notes: attachmentFieldType.Entity.notes,
                                CreatedTime: attachmentFieldType.Entity.createdTime
                            };
                        attachementFieldTypes.push(attachementFieldType);
                }
            }
            return attachementFieldTypes;
        }
    }
    return directiveDefinitionObject;
}]);