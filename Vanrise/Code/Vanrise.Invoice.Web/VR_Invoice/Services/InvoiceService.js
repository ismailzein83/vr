
app.service('VR_Invoice_InvoiceService', ['VRModalService','SecurityService',
    function (VRModalService, SecurityService) {

        var actionTypes = [];
        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }

        function getActionTypeIfExist(actionTypeName)
        {
            for(var i=0;i<actionTypes.length;i++)
            {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }

        function generateInvoice(onGenerateInvoice, invoiceTypeId) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenerateInvoice = onGenerateInvoice;
            };
            var parameters = {
                invoiceTypeId: invoiceTypeId
            };

            VRModalService.showModal('/Client/Modules/VR_Invoice/Views/GenerateInvoiceEditor.html', parameters, settings);
        }

        function registerInvoiceRDLCReport() {
            var actionType = {
                ActionTypeName: "Download",
                actionMethod: function (payload) {
                    var paramsurl = "";
                    paramsurl += "invoiceId=" + payload.invoice.Entity.InvoiceId;
                    paramsurl += "&actionTypeName=" + "Download";
                    paramsurl += "&Auth-Token=" + encodeURIComponent(SecurityService.getUserToken());
                    window.open("Client/Modules/VR_Invoice/Reports/InvoiceReport.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1");
                }
            };
            registerActionType(actionType);
        }

        return ({
            generateInvoice: generateInvoice,
            registerActionType: registerActionType,
            getActionTypeIfExist: getActionTypeIfExist,
            registerInvoiceRDLCReport: registerInvoiceRDLCReport
        });
    }]);
