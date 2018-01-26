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
        templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Attachment/Templates/AttachmentFieldTypeEntitManagementTemplate.html'
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
                        createdTime: new Date()
                    };
                $scope.scopeModel.attachementFieldTypes.push({ Entity: attachementFieldType });
              
            };
            $scope.scopeModel.save = function () {
                console.log(getAttachementFieldTypeData());
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        if(payload != undefined)
                        {
                            if (payload.attachementFieldTypes != undefined) {
                                for (var i = 0; i < payload.attachementFieldTypes.length; i++) {
                                    var attachementFieldType = payload.attachementFieldTypes[i];
                                    $scope.attachementFieldTypes.push({ Entity: attachementFieldType });
                                }
                            }
                        }
                    };
                    directiveAPI.getData = function () {
                        return getAttachementFieldTypeData();
                    };
                    return directiveAPI;
                };

            };
        }
     
        function getAttachementFieldTypeData() {
            var attachementFieldTypes = [];
            if ($scope.scopeModel.attachementFieldTypes != undefined) {
                for (var i = 0; i < $scope.scopeModel.attachementFieldTypes.length; i++) {
                    if ($scope.scopeModel.attachementFieldTypes.Entity != undefined) {
                        var attachementFieldType =
                            {
                                FileId: $scope.scopeModel.attachementFieldTypes[i].Entity.file.fileId,
                                Notes: $scope.scopeModel.attachementFieldTypes[i].Entity.notes,
                                CreatedTime: $scope.scopeModel.attachementFieldTypes[i].Entity.createdTime
                            };
                        attachementFieldTypes.push(attachementFieldType);
                    }
                }
            }
            return attachementFieldTypes;
        }
    }
    return directiveDefinitionObject;
}]);