"use strict";

app.directive("vrInvoiceCommentsubsectionGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceTypeAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope:
        {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var subSectionGrid = new CommentSubSectionGrid($scope, ctrl, $attrs);
            subSectionGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/CommentSubSectionGridTemplate.html"

    };

    function CommentSubSectionGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        var gridAPI;
        var commentPayload;

        var vrCommentGridAPI;
        var vrCommentGridReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceTypeId;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.onVRCommentReady = function (api) {
                vrCommentGridAPI = api;
                vrCommentGridReadyDeferred.resolve();
            };
            defineAPI();
        }
        function defineAPI() {

            var api = {};
            api.load = function (payload) {
                var promises = [];
                var query = {};

                if (payload != undefined && payload.query != undefined) {

                    var invoiceTypeId = payload.query.InvoiceTypeId;
                    var objectId = payload.query.InvoiceId;

                    VR_Invoice_InvoiceTypeAPIService.GetInvoiceTypeCommentDefinitionId(invoiceTypeId).then(function (response) {
                        commentPayload = { definitionId: response, objectId: objectId };
                        var loadVRCommentPromise = loadVRComment();
                        promises.push(loadVRCommentPromise);
                        UtilsService.waitMultiplePromises(promises);

                    });

                }
            };


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };

        function loadVRComment() {
            var vrCommentLoadDeferred = UtilsService.createPromiseDeferred();
            vrCommentGridReadyDeferred.promise.then(function () {
                var dataPayload = commentPayload;

                VRUIUtilsService.callDirectiveLoad(vrCommentGridAPI, dataPayload, vrCommentLoadDeferred);
            });
            return vrCommentLoadDeferred.promise;
        }

    }

    return directiveDefinitionObject;

}
]);