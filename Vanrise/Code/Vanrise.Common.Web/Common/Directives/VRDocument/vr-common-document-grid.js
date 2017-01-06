"use strict";

app.directive("vrCommonDocumentGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var documentGrid = new DocumentGrid($scope, ctrl, $attrs);
            documentGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/VRDocument/Templates/VRDocumentGridTemplate.html"

    };

    function DocumentGrid($scope, ctrl, $attrs) {
        var documentCategories;
        var documentCategorySelectorAPI;
        var documentsPayload;

        this.initializeController = initializeController;

        function initializeController() {

            $scope.documents = [];
           

            $scope.onGridReady = function (api) {

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.load = function (query) {
                        documentCategories = query.documentCategories;
                        documentsPayload = query.documents;
                        loadGrid(documentsPayload);
                    };


                    directiveAPI.getData = function () {
                        return getDocuments();
                    };

                    directiveAPI.addDocument = function () {
                        var documentItem = getDocumentItem();
                        $scope.documents.push(documentItem);
                    };


                    return directiveAPI;
                }
            };

            $scope.removeDocument = function (dataItem) {
               
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        var index = $scope.documents.indexOf(dataItem);
                        $scope.documents.splice(index, 1);
                    }
                    
                });
            };

        };


        function loadGrid(documents) {
            var promises = [];

            if (documents != undefined) {
                for (var i = 0; i < documents.length; i++) {
                    var documentItem = getDocumentItem(documents[i]);
                    promises.push(documentItem.Entity.directiveLoadDeferred.promise);
                    $scope.documents.push(documentItem);
                }
            }
            return UtilsService.waitMultiplePromises(promises);
        }



        function getDocumentItem(document) {
            var documentItem = {
                Entity : {}
            };

            if (document != undefined) {
                $scope.isLoadingMappedCol = true;

                documentItem.Entity.documentAttachment = {
                    fileId: document.FileId
                };

                documentItem.Entity.documentDescription = document.Description;

                documentItem.Entity.createdOn = document.CreatedOn;
                documentItem.Entity.updatedOn = document.UpdatedOn;

            }

            documentItem.Entity.directiveLoadDeferred = UtilsService.createPromiseDeferred();

            documentItem.Entity.onDocumentCategorySelectorReady = function (api) {
                documentItem.Entity.directiveAPI = api;
                var directivePayload = {
                    documentCategories: documentCategories
                };
                if (document != undefined) 
                    directivePayload.selectedIds = document.CategoryId;

                VRUIUtilsService.callDirectiveLoad(documentItem.Entity.directiveAPI, directivePayload, documentItem.Entity.directiveLoadDeferred);
            };

            UtilsService.waitMultiplePromises([documentItem.Entity.directiveLoadDeferred.promise]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingMappedCol = false;
            });

            return documentItem;
        };


        function getDocuments() {

            if ($scope.documents.length == 0)
                return null;

            var documents = [];

            for (var i = 0; i < $scope.documents.length; i++) {

                var document = $scope.documents[i].Entity;
               
                var documentSetting = {
                    FileId: document.documentAttachment.fileId,
                    CategoryId: document.directiveAPI.getSelectedIds(),
                    Description: document.documentDescription,
                    CreatedOn: document.createdOn
                };

                if (documentsPayload == undefined || document.createdOn == undefined)
                    documentSetting.CreatedOn = new Date();

                documents.push(documentSetting);
            }
            return documents;
        }

    }

    return directiveDefinitionObject;

}]);
