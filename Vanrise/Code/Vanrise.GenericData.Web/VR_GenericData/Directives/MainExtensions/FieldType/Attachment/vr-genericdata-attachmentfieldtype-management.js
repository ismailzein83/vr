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
        var gridAPI;
        var context;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.attachementFieldTypes = [];
            var attachementFieldTypeArray;
            $scope.scopeModel.addNewAttachementFieldType = function () {
                var attachementFieldType =
                    {
                        FileId: '',
                        Notes: '',
                        CreatedTime: ''
                    };
                $scope.scopeModel.attachementFieldTypes.push({ Entity: attachementFieldType });
                attachementFieldTypeArray = $scope.scopeModel.attachementFieldTypes;
            };
            $scope.scopeModel.save = function () {
                console.log(getAttachementFieldTypeData());
                console.log("fileId");
                console.log($scope.scopeModel.attachementFieldTypes[0]);
                console.log($scope.scopeModel.file);
                console.log($scope.scopeModel.note);
            }
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
                    var attachementFieldType = $scope.scopeModel.attachementFieldTypes[i];
                    attachementFieldTypes.push(attachementFieldType.Entity);
                }
            }
            return attachementFieldTypes;
        }
    }
    return directiveDefinitionObject;
}]);