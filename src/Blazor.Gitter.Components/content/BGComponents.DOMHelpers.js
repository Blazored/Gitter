window.customizationJsFunctions = {
    getDOMElementVal : function (srcElem) {
        return srcElem.value;
    },

    setDOMElementVal: function (tarElem, valToSet) {
        tarElem.value = valToSet;
        return "OK";
    },

    setDOMElementBgnColor: function (tarElem, bgnclr) {
        if (tarElem)
            tarElem.style.backgroundColor = bgnclr;
        return "OK";
    },

    // Used by our modal component base class to impose or remove modality
    setModalState : function (stateToSet) {

        //
        //  Apply the appropriate body style based on state. This is in addition to the 
        //  directly applied styles on the body type itself.
        //
        if (stateToSet)
            document.body.className = "opti-body-modal";
        else
            document.body.className = "opti-body";

        return "OK";
    }
};
