(<any>window).chat = {
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