(<any>window).chat = {
    repositionRoomSearchResults: function () {
        const input = document.querySelector("#message-send-input") as HTMLInputElement;

        // TODO: get the @ that's nearest to getSelectionStart
        const resultsIndex = input.value.indexOf("@") + 1;

        const resultsPopup = document.querySelector(".chat-room__roomusersearchresults") as HTMLElement;

        // CHECK: doesn't work right yet
        if (resultsPopup == null)
            return;

        resultsPopup.style.left = resultsIndex.toString();
        resultsPopup.style.bottom = input.style.height;
    },
    getSelectionStart: function (id: string) {
        const el = document.getElementById(id) as HTMLInputElement;
        try {
            if (el) {
                return el.selectionStart;
            }
        }
        catch { }

        return -1;
    },
    getScrollTop: function (id: string) {
        const el = document.getElementById(id);
        try {
            if (el) {
                return el.scrollTop;
            } else {
                return -1;
            }
        } catch (e) {
            return -1;
        }
    },
    isScrolledToBottom: function (id: string) {
        const el = document.getElementById(id);
        try {
            if (el) {
                return el.scrollTop >= el.scrollHeight - el.offsetHeight;
            } else {
                return false;
            }
        } catch (e) {
            return false;
        }
    },
    scrollIntoView: function (id: string) {
        const el = document.getElementById(id);
        try {
            if (el) {
                el.scrollIntoView();
                return true;
            }
            return false;
        } catch (e) {
            return false;
        }
    },
    setFocus: function (control: HTMLElement) {
        if (control) {
            if (control.focus) {
                control.focus();
                return true;
            }
        }
        return false;
    },
    setFocusById: function (id: string) {
        const control = document.getElementById(id);
        console.log("setFocusById: " + id + control);
        if (control) {
            if (control.focus) {
                control.focus();
                return true;
            }
        }
        return false;
    }
}