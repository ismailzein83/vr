'use strict';

app.service("MultiTranscludeService", function () {
    return {
        transclude: function (iElem, transcludeFn) {
            transcludeFn(function (clone) {

                angular.forEach(clone, function (cloneEl) {

                    // node type 3 is "text" node
                    if (cloneEl.nodeType === 3) {
                        return;
                    }

                    // get target name from clone
                    var destinationId = cloneEl.attributes["transclude-to"].value;

                    //find target
                    var destination = iElem.find("[transclude-id='" + destinationId + "']");

                    // append target if found
                    if (destination.length) {

                        destination.append(cloneEl);

                    } else {
                        // if target isn't found (missing/invalid transclude), clean up and throw error         
                        cloneEl.remove();

                        //throw new Error(
                        //  'Target not found. Please specify the correct transclude-to attribute.' 
                        //);

                    }
                });
            });
        }
    };
});