(function (app) {

    'use strict';

    OrderDetailService.$inject = ['VRModalService', 'VRNotificationService'];

    function OrderDetailService(VRModalService, VRNotificationService) {
        return {      
            editSection: editSection,
            addSection: addSection
        };


        function addSection(onSectionAdded, exitingSections) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSectionAdded = onSectionAdded;
            };

            var parameters = {
                exitingSections: exitingSections
            };

            VRModalService.showModal('/Client/Modules/Retail_Zajil/Views/OrderDetail/OrderDetailEditor.html', parameters, modalSettings);
        }

        function editSection(onSectionUpdated, sectionEntity) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSectionUpdated = onSectionUpdated;
            };

            var parameters = {
                sectionTitleValue: sectionEntity
            };

            VRModalService.showModal('/Client/Modules/Retail_Zajil/Views/OrderDetail/OrderDetailEditor.html', parameters, modalSettings);
        }

    }

    app.service('Retail_Zajil_OrderDetailService', OrderDetailService);

})(app);