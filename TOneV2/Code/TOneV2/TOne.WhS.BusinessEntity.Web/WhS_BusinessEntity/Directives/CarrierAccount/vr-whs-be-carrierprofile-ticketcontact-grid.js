"use strict";

app.directive("vrWhsBeCarrierprofileTicketcontactGrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ticketContactGrid = new TicketContactGrid($scope, ctrl, $attrs);
            ticketContactGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CarrierAccount/Templates/CarrierProfileTickectContactGridTemplate.html"
    };

    function TicketContactGrid($scope, ctrl, $attrs) {


        this.initializeController = initializeController;

        function initializeController() {

            $scope.ticketContacts = [];


            $scope.onGridReady = function (api) {

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.load = function (payload) {
                        if (payload != undefined && payload.ticketContacts != undefined) {
                            for (var i = 0; i < payload.ticketContacts.length; i++) {
                                $scope.ticketContacts.push(payload.ticketContacts[i]);
                            }
                        }
                    };


                    directiveAPI.getData = function () {
                        var ticketContacts = [];
                        for (var i = 0; i < $scope.ticketContacts.length; i++) {
                            ticketContacts.push({
                                CarrierProfileTicketContactId: $scope.ticketContacts[i].CarrierProfileTicketContactId,
                                Name: $scope.ticketContacts[i].Name,
                                PhoneNumber: $scope.ticketContacts[i].PhoneNumber,
                                Emails: $scope.ticketContacts[i].Emails
                            });
                        }
                        return ticketContacts;
                    };

                    directiveAPI.addTicketContact = function () {
                        var ticketContact = {
                            CarrierProfileTicketContactId: UtilsService.guid(),
                            Name: null,
                            PhoneNumber: [],
                            Emails: []
                        };
                        $scope.ticketContacts.push(ticketContact);
                    };
                    return directiveAPI;
                }
            };

            $scope.removeTicketContact = function (dataItem) {
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        var index = $scope.ticketContacts.indexOf(dataItem);
                        $scope.ticketContacts.splice(index, 1);
                    }

                });
            };

        };
    }

    return directiveDefinitionObject;

}]);
